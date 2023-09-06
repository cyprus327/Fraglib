using OpenTK.Windowing.Common;
using System.Numerics;

namespace Fraglib;

internal sealed class PerPixelEngine : Engine {
    public PerPixelEngine(int w, int h, string t, Func<int, int, Uniforms, uint> perPixel, Action perFrame) : base(w, h, t) {
        _perPixel = perPixel;
        _perFrame = perFrame;
        uniforms.Time = uniforms.DeltaTime = 0f;
        uniforms.Width = w;
        uniforms.Height = h;

        _accumulationData = new Vector4[w * h];
    }
    
    private bool accumulate = false;
    public bool Accumulate { 
        get {
            return accumulate;
        } set {
            accumulate = value;
            if (!value) {
                frameInd = 0;
            }
        }
    }

    private readonly Func<int, int, Uniforms, uint> _perPixel;
    private readonly Action _perFrame;
    private Uniforms uniforms = new();

    private readonly Vector4[] _accumulationData;
    private uint frameInd = 0;

    public override void Update(FrameEventArgs args) {
        float dt = (float)args.Time;
        uniforms.Time += dt;
        uniforms.DeltaTime = dt;
        
        _perFrame();

        if (accumulate) {
            if (frameInd++ == 0) {
                Array.Clear(_accumulationData, 0, _accumulationData.Length);
            }

            int length = Screen.Length;
            Parallel.For(0, length, i => {
                _accumulationData[i] += _perPixel(i % length, i / length, uniforms).ToVec4();
                Vector4 accumulatedCol = _accumulationData[i] / frameInd;
                Screen[i] = FL.NewColor(accumulatedCol);
            });
        } else {
            int length = Screen.Length;
            Parallel.For(0, length, i => {
                Screen[i] = _perPixel(i % length, i / length, uniforms);
            });
        }
    }

    public override void OnWindowClose() {
        
    }
}

public struct Uniforms {
    public float DeltaTime;
    public float Time;
    public int Height;
    public int Width;
}