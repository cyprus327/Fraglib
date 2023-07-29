using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class DrawClearEngine : Engine {
    public DrawClearEngine(int w, int h, string t, Action program) : base(w, h, t) {
        _program = program;
    }

    private readonly Action _program;
    
    public void SetPixel(int x, int y, uint col) {
        if (PixelSize == 1) {
            Screen[y * WindowWidth + x] = col;
            return;
        }

        int xBounds = Math.Min(x + PixelSize, WindowWidth);
        int yBounds = Math.Min(y + PixelSize, WindowHeight);
        for (int py = y; py < yBounds; py++) {
            int yo = py * WindowWidth;
            for (int px = x; px < xBounds; px++) {
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
        _program();
    }
}