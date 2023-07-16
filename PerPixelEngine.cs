using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class PerPixelEngine : Engine {
    public PerPixelEngine(int w, int h, string t, Func<int, int, PerPixelVars, uint> p) : base(w, h, t) {
        _perPixel = p;
    }

    private readonly Func<int, int, PerPixelVars, uint> _perPixel;
    private PerPixelVars ppvs = new PerPixelVars();

    public override void Update(FrameEventArgs args) {
        ppvs.Time += (float)args.Time;
        
        Parallel.For(0, Width, x => {
            for (int y = 0; y < Height; y++) {
                Screen[y * Width + x] = _perPixel?.Invoke(x, y, ppvs) ?? 255;
            }
        });
    }
}

public struct PerPixelVars {
    public float Time;
}