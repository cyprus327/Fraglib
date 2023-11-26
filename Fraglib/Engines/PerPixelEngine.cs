using OpenTK.Windowing.Common;
using System.Numerics;

namespace Fraglib;

internal sealed class PerPixelEngine : Engine {
    public PerPixelEngine(int w, int h, string t, Func<int, int, Uniforms, uint> perPixel, Action perFrame) : base(w, h, t) {
        _perPixel = perPixel;
        _perFrame = perFrame;
        uniforms.Time = uniforms.DeltaTime = 0f;
        uniforms.Width = w;
        uniforms.Height = h;

        _accumulationData = new Vector4[w * h];
    }
    
    private bool accumulate = false;
    public bool Accumulate { 
        get {
            return accumulate;
        } set {
            accumulate = value;
            if (!value) {
                frameInd = 0;
            }
        }
    }

    private readonly Func<int, int, Uniforms, uint> _perPixel;
    private readonly Action _perFrame;
    private Uniforms uniforms = new();

    private readonly Vector4[] _accumulationData;
    private uint frameInd = 0;

    // pretty gross method but idk a good way to reduce it so it's staying
    public override void Update(float dt) {
        uniforms.Time += dt;
        uniforms.DeltaTime = dt;

        int length = Screen.Length;
        int width = WindowWidth, height = WindowHeight;
        int ps = PixelSize, cw = width / ps;
        if (accumulate) {
            if (frameInd++ == 0) {
                Array.Clear(_accumulationData, 0, _accumulationData.Length);
            }
            
            if (ps == 1) {
                Parallel.For(0, height, y => {
                    int yOffset = y * width;
                    for (int x = 0; x < width; x++) {
                        int ind = x + yOffset;
                        _accumulationData[ind] += _perPixel(x, y, uniforms).ToVec4();
                        Vector4 accumulatedCol = _accumulationData[ind] / frameInd;
                        Screen[ind] = FL.NewColor(accumulatedCol);
                    }
                });

                goto END;
            }

            Parallel.For(0, height / ps, cy => {
                for (int cx = 0; cx < cw; cx++) {
                    int ci = cy * ps * width + cx * ps;
                    _accumulationData[ci] += _perPixel(ci % width, ci / width, uniforms).ToVec4();
                    Vector4 accumulatedCol = _accumulationData[ci] / frameInd;
                    uint chunkCol = FL.NewColor(accumulatedCol);
                    for (int y = 0; y < ps; y++) {
                        for (int x = 0; x < ps; x++) {
                            int ind = ci + x + y * width;
                            if (ind >= length) {
                                break;
                            }

                            Screen[ind] = chunkCol;
                        }
                    }
                }
            });

            goto END;
        }

        if (ps == 1) {
            Parallel.For(0, height, y => {
                int yOffset = y * width;
                for (int x = 0; x < width; x++) {
                    uint color = _perPixel(x, y, uniforms);
                    Screen[x + yOffset] = color;
                }
            });

            goto END;
        }

        Parallel.For(0, height / ps, cy => {
            for (int cx = 0; cx < cw; cx++) {
                int ci = cy * ps * width + cx * ps;
                uint chunkCol = _perPixel(ci % width, ci / width, uniforms);
                for (int y = 0; y < ps; y++) {
                    for (int x = 0; x < ps; x++) {
                        int ind = ci + x + y * width;
                        if (ind >= length) {
                            break;
                        }

                        Screen[ind] = chunkCol;
                    }
                }
            }
        });

        END:
        _perFrame();
    }

    public override void OnWindowClose() {
    }
}

public struct Uniforms {
    public float DeltaTime;
    public float Time;
    public int Height;
    public int Width;
}