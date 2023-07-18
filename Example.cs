using System.Numerics;
using Fraglib;

internal sealed class Tutorial {
    private static float zoom = 0.07f;
    private static Vector2 center = new(-1.555466652f, 0f);
    private static readonly float _log2 = MathF.Log(2f);

    private static void Main() {
        FL.Init(800, 500, "Mandelbrot Renderer", PerPixel, PerFrame);
        FL.Run();
    }

    private static void PerFrame() {
        zoom *= MathF.Pow(2f, FL.DeltaTime);
    }

    private static uint PerPixel(int x, int y, Uniforms u) {
        Vector2 uv = (new Vector2(x, y) - new Vector2(u.Width / 2f, u.Height / 2f)) / u.Height / zoom + center;
        Vector2 z = new(0f);

        const int MAX_ITER = 200;
        int iter = 0;
        while (iter < MAX_ITER && Vector2.Dot(z, z) < 4.0) {
            z = new Vector2(z.X * z.X - z.Y * z.Y, 2f * z.X * z.Y) + uv;
            iter++;
        }

        if (iter == MAX_ITER) {
            return FL.Black;
        }

        float t = (float)iter / MAX_ITER;
        float r = 9f * (1f - t) * t * t * t;
        float g = 15f * (1f - t) * (1f - t) * t * t;
        float b = 8.5f * (1f - t) * (1f - t) * (1f - t) * t;
        return FL.NewColor(r, g, b);
    }
}
