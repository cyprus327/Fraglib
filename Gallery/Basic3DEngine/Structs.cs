using System.Collections.Generic;

namespace Fraglib.Gallery;

public struct Vec3 {
    public Vec3() {
        x = y = z = 0f;
    }

    public Vec3(float n) {
        x = y = z = n;
    }

    public Vec3(float x, float y, float z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float x, y, z;
}

public struct Triangle {
    public Triangle() {
        p = new Vec3[3];
    }

    public Triangle(Vec3 a, Vec3 b, Vec3 c) {
        p = new Vec3[3] { a, b, c };
    }

    public Triangle(float ax, float ay, float az,
                    float bx, float by, float bz,
                    float cx, float cy, float cz) {
        p = new Vec3[3] {
            new Vec3(ax, ay, az),
            new Vec3(bx, by, bz),
            new Vec3(cx, by, cz)
        };
    }

    public Triangle(Triangle t) {
        p = new Vec3[3];
        p[0] = t.p[0];
        p[1] = t.p[1];
        p[2] = t.p[2];
    }

    public Vec3[] p;
}

public struct Mesh {
    public Mesh() {
        Tris = new List<Triangle>();
    }

    public List<Triangle> Tris;
}

public struct Mat4x4 {
    public Mat4x4() {
        m = new float[][] {
            new float[] { 0f, 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f, 0f },
            new float[] { 0f, 0f, 0f, 0f }
        };
    }

    public float[][] m;

    public void Multiply(Vec3 i, out Vec3 o) {
        o = new Vec3(
            i.x * m[0][0] + i.y * m[1][0] + i.z * m[2][0] + m[3][0],
            i.x * m[0][1] + i.y * m[1][1] + i.z * m[2][1] + m[3][1],
            i.x * m[0][2] + i.y * m[1][2] + i.z * m[2][2] + m[3][2]
        );
        float w = i.x * m[0][3] + i.y * m[1][3] + i.z * m[2][3] + m[3][3];

        if (w != 0f) {
            o.x /= w;
            o.y /= w;
            o.z /= w;
        }
    }

    public static Vec3 Multiply(Mat4x4 mat, Vec3 v) {
        Vec3 o = new(
            v.x * mat.m[0][0] + v.y * mat.m[1][0] + v.z * mat.m[2][0] + mat.m[3][0],
            v.x * mat.m[0][1] + v.y * mat.m[1][1] + v.z * mat.m[2][1] + mat.m[3][1],
            v.x * mat.m[0][2] + v.y * mat.m[1][2] + v.z * mat.m[2][2] + mat.m[3][2]);
        float w = v.x * mat.m[0][3] + v.y * mat.m[1][3] + v.z * mat.m[2][3] + mat.m[3][3];

        if (w != 0f) {
            o.x /= w;
            o.y /= w;
            o.z /= w;
        }

        return o;
    }

    public static Vec3 operator *(Mat4x4 mat, Vec3 v) {
        Vec3 o = new(
            v.x * mat.m[0][0] + v.y * mat.m[1][0] + v.z * mat.m[2][0] + mat.m[3][0],
            v.x * mat.m[0][1] + v.y * mat.m[1][1] + v.z * mat.m[2][1] + mat.m[3][1],
            v.x * mat.m[0][2] + v.y * mat.m[1][2] + v.z * mat.m[2][2] + mat.m[3][2]);
        float w = v.x * mat.m[0][3] + v.y * mat.m[1][3] + v.z * mat.m[2][3] + mat.m[3][3];

        if (w != 0f) {
            o.x /= w;
            o.y /= w;
            o.z /= w;
        }

        return o;
    }
}