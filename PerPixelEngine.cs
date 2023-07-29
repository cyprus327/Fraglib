using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class PerPixelEngine : Engine {
    public PerPixelEngine(int w, int h, string t, Func<int, int, Uniforms, uint> perPixel, Action perFrame) : base(w, h, t) {
        _perPixel = perPixel;
        _perFrame = perFrame;
        uniforms.Time = uniforms.DeltaTime = 0f;
        uniforms.Width = w;
        uniforms.Height = h;
    }

    private readonly Func<int, int, Uniforms, uint> _perPixel;
    private readonly Action _perFrame;
    private Uniforms uniforms = new();

    public override void Update(FrameEventArgs args) {
        uniforms.Time += (float)args.Time;
        uniforms.DeltaTime = (float)args.Time;
        
        _perFrame();
        
        int width = ScaledWidth, height = ScaledHeight;
        if (PixelSize == 1) {
            Parallel.For(0, width * height, i => {
                Screen[i] = _perPixel(i % width, i / width, uniforms);
            });
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
                    Screen[y * width + x] = fragColor;
                }
            }
        });
    }
}

public struct Uniforms {
    public float DeltaTime;
    public float Time;
    public int Height;
    public int Width;
}