using Fraglib;
using System.Numerics;

namespace Gallery;

internal sealed class RotatingCube {
    private static Vector3[] cubeVertices = {
            new(-0.5f, -0.5f, -0.5f),
            new(0.5f, -0.5f, -0.5f),
            new(0.5f, 0.5f, -0.5f),
            new(-0.5f, 0.5f, -0.5f),
            new(-0.5f, -0.5f, 0.5f),
            new(0.5f, -0.5f, 0.5f),
            new(0.5f, 0.5f, 0.5f),
            new(-0.5f, 0.5f, 0.5f),
    };

    private static int[][] cubeEdges = {
            new int[] { 0, 1 }, new int[] { 1, 2 }, new int[] { 2, 3 }, new int[] { 3, 0 },
            new int[] { 4, 5 }, new int[] { 5, 6 }, new int[] { 6, 7 }, new int[] { 7, 4 },
            new int[] { 0, 4 }, new int[] { 1, 5 }, new int[] { 2, 6 }, new int[] { 3, 7 }
    };
    
    private Vector3 camPos = new(0f, 0f, -8f);

    public void Program() {
        FL.Clear();

        float angleX = FL.ElapsedTime * 0.5f;
        float angleY = FL.ElapsedTime * 0.3f;

        Matrix4x4 rotX = Matrix4x4.CreateRotationX(angleX);
        Matrix4x4 rotY = Matrix4x4.CreateRotationY(angleY);
        Matrix4x4 rot = rotX * rotY;

        Matrix4x4 view = Matrix4x4.CreateLookAt(camPos, Vector3.Zero, Vector3.UnitY);

        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4f, FL.Width / FL.Height, 0.1f, 100f);

        foreach (int[] edge in cubeEdges) {
            Vector3 p1 = Vector3.Transform(cubeVertices[edge[0]], rot);
            Vector3 p2 = Vector3.Transform(cubeVertices[edge[1]], rot);

            Vector4 projP1 = Vector4.Transform(new Vector4(p1, 1f), view * projection);
            Vector4 projP2 = Vector4.Transform(new Vector4(p2, 1f), view * projection);

            if (projP1.W > 0 && projP2.W > 0) {
                Vector2 screenP1 = new Vector2((projP1.X / projP1.W + 1f) * 0.5f * FL.Width,
                                                (1f - projP1.Y / projP1.W) * 0.5f * FL.Height);
                Vector2 screenP2 = new Vector2((projP2.X / projP2.W + 1f) * 0.5f * FL.Width,
                                                (1f - projP2.Y / projP2.W) * 0.5f * FL.Height);

                FL.DrawLine((int)screenP1.X, (int)screenP1.Y, (int)screenP2.X, (int)screenP2.Y, FL.Rand());
            }
        }

        camPos.Z = -8f + MathF.Sin(FL.ElapsedTime) * 3f;
    }
}