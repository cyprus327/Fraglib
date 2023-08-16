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
    }
    
    public bool Accumulate { get; set; } = false;

    private readonly Func<int, int, Uniforms, uint> _perPixel;
    private readonly Action _perFrame;
    private Uniforms uniforms = new();

    public override void Update(FrameEventArgs args) {
        uniforms.Time += (float)args.Time;
        uniforms.DeltaTime = (float)args.Time;
        
        _perFrame();
        
        const float T = 0.01f;
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
                        Screen[i] = Accumulate ? 
                            FL.LerpColors(Screen[i], _perPixel(i % width, i / width, uniforms), T) : 
                            _perPixel(i % width, i / width, uniforms);
                    }
                });
            }

            Task.WaitAll(tasks);

            // unsafe {
            //     int vectorSize = Vector<uint>.Count;
            //     int iterations = width * height;
            //     int iterationsPerVector = iterations / vectorSize;

            //     Vector<uint>[] resultVectors = new Vector<uint>[iterationsPerVector];

            //     Parallel.For(0, iterationsPerVector, _options, i => {
            //         int baseIndex = i * vectorSize;
            //         resultVectors[i] = new Vector<uint>(_perPixel(baseIndex % width, baseIndex / width, uniforms));
            //     });

            //     fixed (Vector<uint>* resultPtr = resultVectors)
            //     fixed (uint* screenPtr = &Screen[0]) {
            //         for (int i = 0; i < iterationsPerVector; i++) {
            //             Vector<uint> result = resultPtr[i];

            //             for (int j = 1; j < vectorSize; j++) {
            //                 result = Vector.ConditionalSelect(
            //                     Vector.GreaterThan(Vector<uint>.Zero, result),
            //                     new Vector<uint>(_perPixel((i * vectorSize + j) % width, (i * vectorSize + j) / width, uniforms)),
            //                     result
            //                 );
            //             }

            //             uint* resultElementPtr = (uint*)&result;
            //             for (int j = 0; j < vectorSize; j++) {
            //                 screenPtr[i * vectorSize + j] = resultElementPtr[j];
            //             }
            //         }
            //     }

            //     for (int i = iterationsPerVector * vectorSize; i < iterations; i++) {
            //         Screen[i] = _perPixel(i % width, i / width, uniforms);
            //     }
            // }
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
                    int x = startX + px;
                    if (x >= width || y >= height) continue;
                    int ind = y * width + x;
                    Screen[ind] = Accumulate ? 
                        FL.LerpColors(Screen[ind], fragColor, T) : 
                        fragColor;
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