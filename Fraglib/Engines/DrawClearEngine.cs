using OpenTK.Windowing.Common;

namespace Fraglib;

internal sealed class DrawClearEngine : Engine {
    public DrawClearEngine(int w, int h, string t, Action program) : base(w, h, t) {
        _program = program;
    }
    
    private readonly Action _program;

    public override void Update(FrameEventArgs args) {
        _program();
    }

    public override void OnWindowClose() {
    }
}