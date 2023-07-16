using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class SetClearEngine : Engine {
    public SetClearEngine(int w, int h, string t, Action gameLoop) : base(w, h, t) {
        _count = w * h * sizeof(uint);
        _gameLoop = gameLoop;
    }

    private readonly int _count;
    private readonly Action _gameLoop;
    
    public void SetPixel(int x, int y, uint c) {
        Screen[y * Width + x] = c;
    }

    public uint GetPixel(int x, int y) {
        return Screen[y * Width + x];
    }

    public void Clear(uint c = 255) {
        Array.Fill(Screen, c);
    }

    public override void Update(FrameEventArgs args) {
        _gameLoop();
    }
}