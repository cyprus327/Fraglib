using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class SetClearEngine : Engine {
    public SetClearEngine(int s, int w, int h, string t, Action gameLoop) : base(s, w, h, t) {
        _count = w * h * sizeof(uint);
        _gameLoop = gameLoop;
    }

    private readonly int _count;
    private readonly Action _gameLoop;
    
    public void SetPixel(int x, int y, uint c) {
        if (PixelSize == 1) {
            Screen[y * WindowWidth + x] = c;
            return;
        }

        for (int py = y; py < y + PixelSize; py++) {
            for (int px = x; px < x + PixelSize; px++) {
                Screen[py * WindowWidth + px] = c;
            }
        }
    }

    public void SetVerticalSection(int x, int y0, int y1, uint c) {
        if (y0 >= y1) {
            return;
        }

        if (y1 >= WindowHeight) {
            y1 = WindowHeight;
        }

        while (y0 < y1) {
            SetPixel(x, y0++, c);
        }
    }

    public uint GetPixel(int x, int y) {
        return Screen[y * WindowWidth + x];
    }

    public void Clear(uint c = 255) {
        Array.Fill(Screen, c);
    }

    public override void Update(FrameEventArgs args) {
        _gameLoop();
    }
}