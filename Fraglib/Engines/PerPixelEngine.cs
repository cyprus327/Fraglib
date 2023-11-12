using OpenTK.Windowing.Common;
using System.Numerics;
using System.Runtime.CompilerServices;

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
    public override void Update(FrameEventArgs args) {
        float dt = (float)args.Time;
        uniforms.Time += dt;
        uniforms.DeltaTime = dt;
        
        _perFrame();

        int length = Screen.Length;
        int width = WindowWidth;
        int ps = PixelSize, cw = width / ps;
        if (accumulate) {
            if (frameInd++ == 0) {
                Array.Clear(_accumulationData, 0, _accumulationData.Length);
            }
            
            if (ps == 1) {
                Parallel.For(0, length, i => {
                    _accumulationData[i] += _perPixel(i % width, i / width, uniforms).ToVec4();
                    Vector4 accumulatedCol = _accumulationData[i] / frameInd;
                    uint color = FL.NewColor(accumulatedCol);
                    color.SetA((byte)((color.GetA() + Screen[i].GetA()) / 2));
                    Screen[i] = color;
                });

                return;
            }

            Parallel.For(0, WindowHeight / ps, cy => {
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

                            uint newColor = chunkCol;
                            newColor.SetA((byte)((newColor.GetA() + Screen[ind].GetA()) / 2));
                            Screen[ind] = newColor;
                        }
                    }
                }
            });

            return;
        }

        if (ps == 1) {
            Parallel.For(0, length, i => {
                uint color = _perPixel(i % width, i / width, uniforms);
                color.SetA((byte)((color.GetA() + Screen[i].GetA()) / 2));
                Screen[i] = color;
            });

            return;
        }

        Parallel.For(0, WindowHeight / ps, cy => {
            for (int cx = 0; cx < cw; cx++) {
                int ci = cy * ps * width + cx * ps;
                uint chunkCol = _perPixel(ci % width, ci / width, uniforms);
                for (int y = 0; y < ps; y++) {
                    for (int x = 0; x < ps; x++) {
                        int ind = ci + x + y * width;
                        if (ind >= length) {
                            break;
                        }

                        uint newColor = chunkCol;
                        newColor.SetA((byte)((newColor.GetA() + Screen[ind].GetA()) / 2));
                        Screen[ind] = newColor;
                    }
                }
            }
        });
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