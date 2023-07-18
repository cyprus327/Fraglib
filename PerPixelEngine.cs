using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class PerPixelEngine : Engine {
    public PerPixelEngine(int s, int w, int h, string t, Func<int, int, Uniforms, uint> pp, Action pf) : base(s, w, h, t) {
        _perPixel = pp;
        _perFrame = pf;
        uniforms.Time = 0f;
        uniforms.Width = w;
        uniforms.Height = h;
    }

    private readonly Func<int, int, Uniforms, uint> _perPixel;
    private readonly Action _perFrame;
    private Uniforms uniforms = new Uniforms();

    public override void Update(FrameEventArgs args) {
        uniforms.Time += (float)args.Time;
        
        _perFrame();
        
        if (PixelSize == 1) {
            Parallel.For(0, WindowWidth, x => {
                for (int y = 0; y < WindowHeight; y++) {
                    Screen[y * WindowWidth + x] = _perPixel(x, y, uniforms);
                }
            });
            return;
        }

        Parallel.For(0, ScaledHeight, sy => {
            int y = sy * PixelSize; // original y
            for (int sx = 0; sx < ScaledWidth; sx++) {
                uint fragColor = _perPixel(sx, sy, uniforms);
                int x = sx * PixelSize; // original x
                for (int py = y; py < y + PixelSize; py++) {
                    for (int px = x; px < x + PixelSize; px++) {
                        if (px >= WindowWidth || py >= WindowHeight) {
                            continue;
                        }
                        Screen[py * WindowWidth + px] = fragColor;
                    }
                }
            }
        });
    }
}

public struct Uniforms {
    public float Time;
    public int Height;
    public int Width;
}