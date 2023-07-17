using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class PerPixelEngine : Engine {
    public PerPixelEngine(int s, int w, int h, string t, Func<int, int, PerPixelVars, uint> p) : base(s, w, h, t) {
        _perPixel = p;
    }

    private readonly Func<int, int, PerPixelVars, uint> _perPixel;
    private PerPixelVars ppvs = new PerPixelVars();

    public override void Update(FrameEventArgs args) {
        ppvs.Time += (float)args.Time;
        
        if (PixelSize == 1) {
            Parallel.For(0, WindowWidth, x => {
                for (int y = 0; y < WindowHeight; y++) {
                    Screen[y * WindowWidth + x] = _perPixel(x, y, ppvs);
                }
            });
            return;
        }

        Parallel.For(0, ScaledHeight, sy => {
            int y = sy * PixelSize; // original y
            for (int sx = 0; sx < ScaledWidth; sx++) {
                uint fragColor = _perPixel(sx, sy, ppvs);
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

public struct PerPixelVars {
    public float Time;
}