using Fraglib;

internal sealed class Example {
    private static void Main() {
        // initialize with additional parameter, pixelSize
        FL.Init(1280, 720, "Example Window", PerPixel, pixelSize: 20);
        FL.Run();
    }

    private static uint PerPixel(int x, int y, PerPixelVars u) {
        float uvx = (float)x / FL.ScaledWidth, uvy = (float)y / FL.ScaledHeight;
        return FL.NewColor((byte)(uvx * 255), (byte)(uvy * 255), (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255));
    }
}
