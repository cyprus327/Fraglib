using Fraglib;

namespace Fraglib.Gallery; 

public sealed class Basic3DEngine {
    public Basic3DEngine(int scaledWidth, int scaledHeight) {
        _meshCube.Tris.Add(new Triangle(0f, 0f, 0f,   0f, 1f, 0f,   1f, 1f, 0f));
        _meshCube.Tris.Add(new Triangle(0f, 0f, 0f,   1f, 1f, 0f,   1f, 0f, 0f));
 
        _meshCube.Tris.Add(new Triangle(1f, 0f, 0f,   1f, 1f, 0f,   1f, 1f, 1f));
        _meshCube.Tris.Add(new Triangle(1f, 0f, 0f,   1f, 1f, 1f,   1f, 0f, 1f));
     
        _meshCube.Tris.Add(new Triangle(1f, 0f, 1f,   1f, 1f, 1f,   0f, 1f, 1f));
        _meshCube.Tris.Add(new Triangle(1f, 0f, 1f,   0f, 1f, 1f,   0f, 0f, 1f));
     
        _meshCube.Tris.Add(new Triangle(0f, 0f, 1f,   0f, 1f, 1f,   0f, 1f, 0f));
        _meshCube.Tris.Add(new Triangle(0f, 0f, 1f,   0f, 1f, 0f,   0f, 0f, 0f));
     
        _meshCube.Tris.Add(new Triangle(0f, 1f, 0f,   0f, 1f, 1f,   1f, 1f, 1f));
        _meshCube.Tris.Add(new Triangle(0f, 1f, 0f,   1f, 1f, 1f,   1f, 1f, 0f));

        _meshCube.Tris.Add(new Triangle(1f, 0f, 1f,   0f, 0f, 1f,   0f, 0f, 0f));
        _meshCube.Tris.Add(new Triangle(1f, 0f, 1f,   0f, 0f, 0f,   1f, 0f, 0f));
        
        float near = 0.1f, far = 1000f, fov = 90f;
        float aspectRatio = (float)scaledHeight / (float)scaledWidth;
        float fovRad = 1f / MathF.Tan(fov * 0.5f / 180f * MathF.PI);
        
        _matProj.m[0][0] = aspectRatio * fovRad;
        _matProj.m[1][1] = fovRad;
        _matProj.m[2][2] = far / (far - near);
        _matProj.m[3][2] = (-far * near) / (far - near);
        _matProj.m[2][3] = 1f;
        _matProj.m[3][3] = 0f;
    }

    private readonly Mesh _meshCube = new();
    private readonly Mat4x4 _matProj = new();
    private readonly Mat4x4 _matRotZ = new(), _matRotX = new();

    public void Program() {
        FL.Clear(FL.Black);

        float time = FL.ElapsedTime;
        
        _matRotZ.m[0][0] = MathF.Cos(time);
		_matRotZ.m[0][1] = MathF.Sin(time);
		_matRotZ.m[1][0] = -MathF.Sin(time);
		_matRotZ.m[1][1] = MathF.Cos(time);
		_matRotZ.m[2][2] = 1;
		_matRotZ.m[3][3] = 1;

		_matRotX.m[0][0] = 1;
		_matRotX.m[1][1] = MathF.Cos(time * 0.5f);
		_matRotX.m[1][2] = MathF.Sin(time * 0.5f);
		_matRotX.m[2][1] = -MathF.Sin(time * 0.5f);
		_matRotX.m[2][2] = MathF.Cos(time * 0.5f);
		_matRotX.m[3][3] = 1;

        foreach (var tri in _meshCube.Tris) {
            Triangle triRotZ = new(), triRotZX = new();
            triRotZ.p[0] = _matRotZ * tri.p[0];
            triRotZ.p[1] = _matRotZ * tri.p[1];
            triRotZ.p[2] = _matRotZ * tri.p[2];
            triRotZX.p[0] = _matRotX * triRotZ.p[0];
            triRotZX.p[2] = _matRotX * triRotZ.p[1];
            triRotZX.p[1] = _matRotX * triRotZ.p[2];

            Triangle triTrans = new(triRotZX);
            triTrans.p[0].z += 3f;
            triTrans.p[1].z += 3f;
            triTrans.p[2].z += 3f;

            Triangle triProj = new();
            triProj.p[0] = _matProj * triTrans.p[0];
            triProj.p[1] = _matProj * triTrans.p[1];
            triProj.p[2] = _matProj * triTrans.p[2];
            
            triProj.p[0].x += 1f;
            triProj.p[0].y += 1f;
            triProj.p[1].x += 1f;
            triProj.p[1].y += 1f;
            triProj.p[2].x += 1f;
            triProj.p[2].y += 1f;

            triProj.p[0].x *= 0.5f * (float)FL.Width;
            triProj.p[0].y *= 0.5f * (float)FL.Height;
            triProj.p[1].x *= 0.5f * (float)FL.Width;
            triProj.p[1].y *= 0.5f * (float)FL.Height;
            triProj.p[2].x *= 0.5f * (float)FL.Width;
            triProj.p[2].y *= 0.5f * (float)FL.Height;

            FL.DrawTriangle((int)triProj.p[0].x, (int)triProj.p[0].y,
                            (int)triProj.p[1].x, (int)triProj.p[1].y,
                            (int)triProj.p[2].x, (int)triProj.p[2].y,
                            FL.Rand());
        }

        HandleInput();
    }

    private void HandleInput() {
    }
}