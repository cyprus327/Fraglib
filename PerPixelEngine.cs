using OpenTK.Windowing.Common;
using System.Collections.Concurrent;
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

    public override void Update(FrameEventArgs args) {
        uniforms.Time += (float)args.Time;
        uniforms.DeltaTime = (float)args.Time;
        
        _perFrame();
        frameInd++;

        if (frameInd == 1) {
            Array.Fill(_accumulationData, Vector4.Zero);
        }

        int width = ScaledWidth, height = ScaledHeight;
        if (PixelSize == 1) {
            const int batchSize = 128;
            int length = width * height;

            int numTasks = (length + batchSize - 1) / batchSize;
            Task[] tasks = new Task[numTasks];

            for (int batchIndex = 0; batchIndex < numTasks; batchIndex++) {
                int start = batchIndex * batchSize;
                int end = Math.Min(start + batchSize, length);

                tasks[batchIndex] = Task.Run(() => {
                    for (int i = start; i < end; i++) {
                        _accumulationData[i] += _perPixel(i % width, i / width, uniforms).ToVec4();
                        Vector4 accumulatedCol = _accumulationData[i] / frameInd;
                        Screen[i] = FL.NewColor(accumulatedCol);
                    }
                });
            }

            Task.WaitAll(tasks);
            return;
        }

        Parallel.For(0, width * height, i => {
            int sx = i % width;
            int sy = i / width;

            uint fragColor = _perPixel(sx, sy, uniforms);

            int startX = sx * PixelSize;
            int startY = sy * PixelSize;

            for (int py = 0; py < PixelSize; py++) {
                int y = startY + py;
                for (int px = 0; px < PixelSize; px++) {
                    if (startX + px >= width || y >= height) continue;
                    int ind = y * width + startX + px;
                    _accumulationData[ind] += _perPixel(i % width, i / width, uniforms).ToVec4();
                    Vector4 accumulatedCol = _accumulationData[ind] / frameInd;
                    Screen[ind] = FL.NewColor(accumulatedCol);
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