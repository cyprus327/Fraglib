using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class SetClearEngine : Engine {
    public SetClearEngine(int s, int w, int h, string t, Action gameLoop) : base(s, w, h, t) {
        _count = w * h * sizeof(uint);
        _gameLoop = gameLoop;
    }

    private readonly int _count;
    private readonly Action _gameLoop;
    
    public void SetPixel(int x, int y, uint col) {
        if (PixelSize == 1) {
            Screen[y * WindowWidth + x] = col;
            return;
        }

        for (int py = y; py < y + PixelSize; py++) {
            int yo = py * WindowWidth;
            for (int px = x; px < x + PixelSize; px++) {
                Screen[yo + px] = col;
            }
        }
    }

    public void FillRect(int x, int y, int w, int h, uint col) {
        for (int r = y; r < y + h; r++) {
            int ro = r * WindowWidth;
            for (int c = x; c < x + w; c++) {
                Screen[ro + c] = col;
            }
        }
    }

    public uint GetPixel(int x, int y) {
        return Screen[y * WindowWidth + x];
    }

    public void Clear(uint col) {
        Array.Fill(Screen, col);
    }

    public override void Update(FrameEventArgs args) {
        _gameLoop();
    }
}