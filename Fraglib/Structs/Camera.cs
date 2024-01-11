using System.Drawing;
using System.Numerics;
using System.Xml;

namespace Fraglib;

public static unsafe partial class FL {
    /// <region>Camera</region>
    public struct Camera {
        /// <name>ctor</name>
        /// <returns>Camera</returns>
        /// <summary>Initializes a camera targeting the main window in projection mode at position (0, 0, 0).</summary>
        public Camera() {
            if (!initialized) {
                throw new Exception(@"You must either use the ctor with parameters or use this ctor after calling Init.");
            }

            Pos = Vector3.Zero;
            this = new Camera(windowWidth, windowHeight);
        }

        /// <name>ctor</name>
        /// <returns>Camera</returns>
        /// <summary>Initializes a new instantce of the Camera struct with the specified properties.</summary>
        /// <param name="targetWidth">The target width for the camera to render to.</param>
        /// <param name="targetHeight">The target height for the camera to render to.</param>
        public Camera(int targetWidth, int targetHeight) {
            TargetWidth = Math.Clamp(targetWidth, 1, 10000);
            TargetHeight = Math.Clamp(targetHeight, 1, 10000);

            _planes = new (Vector2 point, Vector2 dir)[] { 
                (Vector2.Zero, Vector2.UnitX),
                (Vector2.Zero, Vector2.UnitY),
                (new(TargetWidth, TargetHeight), -Vector2.UnitX),
                (new(TargetWidth, TargetHeight), -Vector2.UnitY)
            };

            UpdateMatrices();
        }

        /// <name>Yaw</name>
        /// <returns>float</returns>
        /// <summary>The camera's current yaw in radians. Can be set by using the LookBy or LookAt methods.</summary>
        public float Yaw { get; private set; } = 0f;
        const float MAX_YAW = MathF.PI * 1.99999f;
        
        /// <name>Pitch</name>
        /// <returns>float</returns>
        /// <summary>The camera's current pitch in radians. Can be set by using the LookBy or LookAt methods.</summary>
        public float Pitch { get; private set; } = 0f;
        const float MAX_PITCH = MathF.PI * 0.49999f;
        
        /// <name>Pos</name>
        /// <returns>Vector3</returns>
        /// <summary>Gets or sets camera's position in world space.</summary>
        public Vector3 Pos { get; set; } = Vector3.Zero;

        public Vector3 Forward { get; set; } = -Vector3.UnitZ;

        /// <name>FOV</name>
        /// <returns>float</returns>
        /// <summary>Gets or sets the camera's field of view, only applies to projection mode.</summary>
        public float FOV { 
            readonly get => fov;
            set => fov = Math.Clamp(value, 1f, 179f);
        }
        private float fov = 90f;

        /// <name>Zoom</name>
        /// <returns>float</returns>
        /// <summary>Gets or sets the camera's zoom, only applies to orthographic mode.</summary>
        public float Zoom {
            readonly get => zoom;
            set => zoom = Math.Clamp(value, float.Epsilon, float.MaxValue);
        }
        private float zoom = 1f;

        /// <name>NearPlane</name>
        /// <returns>float</returns>
        /// <summary>Gets or sets the camera's near plane.</summary>
        public float NearPlane {
            readonly get => nearPlane;
            set => nearPlane = Math.Clamp(value, 0.01f, farPlane - 0.01f);
        }
        private float nearPlane = 0.1f;

        /// <name>FarPlane</name>
        /// <returns>float</returns>
        /// <summary>Gets or sets the camera's far plane.</summary>
        public float FarPlane {
            readonly get => farPlane;
            set => farPlane = Math.Clamp(value, nearPlane + 0.01f, 10000f);
        }
        private float farPlane = 1000f;

        /// <name>TargetWidth</name>
        /// <returns>int</returns>
        /// <summary>The width of what the camera is rendering to (e.g. the main window or a texture).</summary>
        public int TargetWidth { get; init; }

        /// <name>TargetHeight</name>
        /// <returns>int</returns>
        /// <summary>The height of what the camera is rendering to (e.g. the main window or a texture).</summary>
        public int TargetHeight { get; init; }

        /// <name>OrthographicMode</name>
        /// <returns>bool</returns>
        /// <summary>Controls whether or not the camera will render in projection or orthographic.</summary>
        public bool OrthographicMode { get; set; } = false;

        private Matrix4x4 yawPitchMat, viewMat, renderMat, viewRenderMat;

        private readonly (Vector2 point, Vector2 dir)[] _planes;

        /// <name>HandleInputDefault</name>
        /// <returns>void</returns>
        /// <summary>Purely for convenience, handles moving the camera with WASD, Q (up), and E (down), and turning with the mouse when the right mouse button is held.</summary>
        /// <param name="moveSpeedMult">The multiplier for how fast the camera will move.</param>
        /// <param name="lookSpeedMult">The multiplier for how much the mouse will turn the camera.</param>
        public void HandleInputDefault(float moveSpeedMult = 2f, float lookSpeedMult = 0.005f) {
            float amount;
            if (OrthographicMode) {
                amount = moveSpeedMult / MathF.Max(1f, Zoom) * DeltaTime;
                lookSpeedMult /= MathF.Min(1f, Zoom);

                if (GetKeyDown('W')) {
                    MoveUp(amount);
                } else if (GetKeyDown('S')) {
                    MoveDown(amount);
                }
                if (GetKeyDown('E')) {
                    Zoom *= MathF.Pow(2f, DeltaTime);
                } else if (GetKeyDown('Q')) {
                    Zoom *= MathF.Pow(0.5f, DeltaTime);
                }
            } else {
                amount = moveSpeedMult * DeltaTime;

                if (GetKeyDown('W')) {
                    MoveForward(amount);
                } else if (GetKeyDown('S')) {
                    MoveBackward(amount);
                }
                if (GetKeyDown('E')) {
                    MoveUp(amount);
                } else if (GetKeyDown('Q')) {
                    MoveDown(amount);
                }
            }

            if (GetKeyDown('A')) {
                MoveLeft(amount);
            } else if (GetKeyDown('D')) {
                MoveRight(amount);
            }

            if (RMBDown()) {
                MouseGrabType = MouseGrabType.Grabbed;
                LookBy(lookSpeedMult * MouseDelta);
            } else {
                MouseGrabType = MouseGrabType.None;
            }

            UpdateMatrices();
        }

        /// <name>MoveForward</name>
        /// <returns>void</returns>
        /// <summary>Moves the camera forward by the specified amount.</summary>
        /// <param name="amount">The amount by which to move the camera.</param>
        public void MoveForward(float amount) {
            Pos += Vector3.Transform(-Vector3.UnitZ, yawPitchMat) * amount;
        }

        /// <name>MoveBackward</name>
        /// <returns>void</returns>
        /// <summary>Moves the camera backward by the specified amount.</summary>
        /// <param name="amount">The amount by which to move the camera.</param>
        public void MoveBackward(float amount) {
            Pos += Vector3.Transform(Vector3.UnitZ, yawPitchMat) * amount;
        }

        /// <name>MoveRight</name>
        /// <returns>void</returns>
        /// <summary>Moves the camera right by the specified amount.</summary>
        /// <param name="amount">The amount by which to move the camera.</param>
        public void MoveRight(float amount) {
            Pos += Vector3.Transform(Vector3.UnitX, yawPitchMat) * amount;
        }

        /// <name>MoveLeft</name>
        /// <returns>void</returns>
        /// <summary>Moves the camera left by the specified amount.</summary>
        /// <param name="amount">The amount by which to move the camera.</param>
        public void MoveLeft(float amount) {
            Pos += Vector3.Transform(-Vector3.UnitX, yawPitchMat) * amount;
        }

        /// <name>MoveUp</name>
        /// <returns>void</returns>
        /// <summary>Moves the camera up by the specified amount.</summary>
        /// <param name="amount">The amount by which to move the camera.</param>
        public void MoveUp(float amount) {
            Pos = new(Pos.X, Pos.Y - amount, Pos.Z);
        }

        /// <name>MoveDown</name>
        /// <returns>void</returns>
        /// <summary>Moves the camera down by the specified amount.</summary>
        /// <param name="amount">The amount by which to move the camera.</param>
        public void MoveDown(float amount) {
            Pos = new(Pos.X, Pos.Y + amount, Pos.Z);
        }

        /// <name>LookBy</name>
        /// <returns>void</returns>
        /// <summary>Turns the camera by the specified vector.</summary>
        /// <param name="vec">The amount by which to learn, where vec.X represents the yaw delta and vec.Y the pitch delta.</param>
        public void LookBy(Vector2 vec) {
            Yaw -= vec.X;
            Yaw %= MAX_YAW;
            Pitch = Math.Clamp(Pitch + vec.Y, -MAX_PITCH, MAX_PITCH);
        }

        /// <name>LookBy</name>
        /// <returns>void</returns>
        /// <summary>Turns the camera by the specified yaw and pitch values.</summary>
        /// <param name="yaw">The amount by which the camera's yaw will change.</param>
        /// <param name="pitch">The amount by which the camera's pitch will change.</param>
        public void LookBy(float yaw, float pitch) {
            Yaw -= yaw;
            Yaw %= MAX_YAW;
            Pitch = Math.Clamp(Pitch + pitch, -MAX_PITCH, MAX_PITCH);
        }

        /// <name>LookAt</name>
        /// <returns>void</returns>
        /// <summary>Turns the camera to look at the specified point.</summary>
        /// <param name="pos">The world space position that the camera will turn towards.</param>
        public void LookAt(Vector3 pos) {
            Vector3 dir = pos - Pos;
            float dist = dir.Length();

            Yaw = MathF.Atan2(-dir.X, dir.Z) % MAX_YAW;
            Pitch = Math.Clamp(MathF.Asin(dir.Y / dist), -MAX_PITCH, MAX_PITCH);
        }

        /// <name>UpdateMatrices</name>
        /// <returns>void</returns>
        /// <summary>Updates the camera's matrices. Should be called after changing one of the camera's properties' values.</summary>
        public void UpdateMatrices() {
            Forward = Vector3.Transform(-Vector3.UnitZ, yawPitchMat);
            yawPitchMat = Matrix4x4.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            viewMat = Matrix4x4.CreateLookAt(Pos, Pos + Forward, Vector3.Transform(Vector3.UnitY, yawPitchMat));
            renderMat = OrthographicMode ? 
                Matrix4x4.CreateOrthographic(TargetWidth / Zoom, TargetHeight / Zoom, nearPlane, farPlane) : 
                Matrix4x4.CreatePerspectiveFieldOfView(DegToRad(fov), (float)TargetWidth / TargetHeight, nearPlane, farPlane);
            viewRenderMat = viewMat * renderMat;
        }

        /// <name>CanSeePoint</name>
        /// <returns>bool</returns>
        /// <summary>Returns whether or not the camera can see the point specified.</summary>
        /// <param name="point">The point to check.</param>
        public readonly bool CanSeePoint(Vector3 point) {
            Vector4 clipSpace = Vector4.Transform(new Vector4(point, 1f), viewRenderMat);
            clipSpace /= clipSpace.W == 0f ? float.Epsilon : clipSpace.W;
            return Math.Abs(clipSpace.X) <= 1f &&
                Math.Abs(clipSpace.Y) <= 1f &&
                clipSpace.Z >= -1f &&
                clipSpace.Z <= 1f;
        }

        /// <name>CanSeeCircle</name>
        /// <returns>bool</returns>
        /// <summary>Returns whether or not the camera can see the circle specified.</summary>
        /// <param name="circleCenter">The center of the circle to check.</param>
        /// <param name="circleRadius">The radius of the circle to check.</param>
        public readonly bool CanSeeCircle(Vector3 circleCenter, float circleRadius) {
            Vector4 clipSpace = Vector4.Transform(new Vector4(circleCenter, 1f), viewRenderMat);
            return clipSpace.X + circleRadius >= -clipSpace.W && clipSpace.X - circleRadius <= clipSpace.W &&
                clipSpace.Y + circleRadius >= -clipSpace.W && clipSpace.Y - circleRadius <= clipSpace.W &&
                clipSpace.Z >= 0f && clipSpace.Z - circleRadius <= clipSpace.W;
        }

        /// <name>CanSeeTri</name>
        /// <returns>bool</returns>
        /// <summary>Returns whether or not the camera can see the triangle specified.</summary>
        /// <param name="tri">The triangle to check.</param>
        public readonly bool CanSeeTri(Triangle tri) {
            return Vector3.Dot(tri.GetNormal(), tri.GetCenter() - Pos) > 0f;
        }

        /// <name>ProjectPointToScreen</name>
        /// <returns>void</returns>
        /// <summary>Projects a 3D point to screen coordinates.</summary>
        /// <param name="point">The point to project, of type (int x, int y, int z).</param>
        /// <param name="screenCoords">An out (int x, int y, int z) representing the screen coordinates of the projected point.</param>
        /// <param name="isInCamView">An out bool representing whether or not the point can be seen by the camera.</param>
        public readonly void ProjectPointToScreen((int x, int y, int z) point, out (int x, int y) screenCoords, out bool isInCamView) {
            ProjectPointToScreen(new Vector3(point.x, point.y, point.z), out Vector2 sc, out isInCamView);
            screenCoords = ((int)sc.X, (int)sc.Y);
        }

        /// <name>ProjectPointToScreen</name>
        /// <returns>void</returns>
        /// <summary>Projects a 3D point to screen coordinates.</summary>
        /// <param name="point">The point to project, of type Vector3.</param>
        /// <param name="screenCoords">An out Vector2 representing the screen coordinates of the projected point.</param>
        /// <param name="isInCamView">An out bool representing whether or not the point can be seen by the camera.</param>
        public readonly void ProjectPointToScreen(Vector3 point, out Vector2 screenCoords, out bool isInCamView) {
            Vector4 clipSpace = Vector4.Transform(new Vector4(point, 1f), viewRenderMat);

            clipSpace /= clipSpace.W;

            screenCoords = NormalizeCoordsToScreen(clipSpace);

            isInCamView = Math.Abs(clipSpace.X) <= 1f &&
                Math.Abs(clipSpace.Y) <= 1f &&
                clipSpace.Z >= -1f &&
                clipSpace.Z <= 1f;
        }


        /// <name>ProjectCircleToScreen</name>
        /// <returns>void</returns>
        /// <summary>Projects and scales a circle in world space to the screen.</summary>
        /// <param name="circleCenter">The center of the circle to project.</param>
        /// <param name="radius">The radius of the circle to project.</param>
        /// <param name="screenCoords">An out Vector2 representing the screen coordinates of the projected circle.</param>
        /// <param name="screenRadius">An out float representing the radius of the projected circle.</param>
        /// <param name="isInCamView">An out bool representing whether or not the point can be seen by the camera.</param>
        public readonly void ProjectCircleToScreen(Vector3 circleCenter, float radius, out Vector2 screenCoords, out float screenRadius, out bool isInCamView) {
            Vector4 clipSpace = Vector4.Transform(new Vector4(circleCenter, 1f), viewRenderMat);

            screenCoords = NormalizeCoordsToScreen(clipSpace);

            screenRadius = radius * TargetWidth / (2f * clipSpace.W);

            isInCamView = clipSpace.X + radius >= -clipSpace.W && clipSpace.X - radius <= clipSpace.W &&
                clipSpace.Y + radius >= -clipSpace.W && clipSpace.Y - radius <= clipSpace.W &&
                clipSpace.Z >= 0f && clipSpace.Z - radius <= clipSpace.W;
        }

        /// <name>ProjectRectToScreen</name>
        /// <returns>void</returns>
        /// <summary>Projects and scales a rectangle in world space to the screen.</summary>
        /// <param name="rectCenter">The center of the circle to project.</param>
        /// <param name="width">The width of the rectangle to project.</param>
        /// <param name="height">The height of the rectangle to project.</param>
        /// <param name="screenCoords">An out Vector2 representing the screen coordinates of the projected circle.</param>
        /// <param name="screenWidth">An out float representing the width of the projected circle.</param>
        /// <param name="screenHeight">An out float representing the height of the projected circle.</param>
        /// <param name="isInCamView">An out bool representing whether or not the point can be seen by the camera.</param>
        public readonly void ProjectRectToScreen(Vector3 rectCenter, float width, float height, out Vector2 screenCoords, out float screenWidth, out float screenHeight, out bool isInCamView) {
            Vector4 clipSpace = Vector4.Transform(new Vector4(rectCenter, 1f), viewRenderMat);

            screenCoords = NormalizeCoordsToScreen(clipSpace);

            screenWidth = width * TargetWidth / (2f * clipSpace.W);
            screenHeight = height * TargetHeight / (2f * clipSpace.W) * ((float)TargetWidth / TargetHeight); 

            clipSpace /= clipSpace.W == 0f ? float.Epsilon : clipSpace.W;
            isInCamView = Math.Abs(clipSpace.X) <= 1f &&
                Math.Abs(clipSpace.Y) <= 1f &&
                clipSpace.Z >= -1f &&
                clipSpace.Z <= 1f;
        }

        /// <name>ProjectTriToScreen</name>
        /// <returns>void</returns>
        /// <summary>Projects, scales, and clips a triangle to the screen. This method isn't yet perfect.</summary>
        /// <param name="tri">The triangle to project.</param>
        /// <param name="screenTris">An out List<Triangle> containing the result of projecting the triangle to the screen.</param>
        /// <param name="isInCamView">An out bool representing whether or not the triangle is in the camera's frustum.</param>
        public readonly void ProjectTriToScreen(Triangle tri, out List<Triangle> screenTris, out bool isInCamView) {
            Triangle test = new() { Color = tri.Color };
            int insidePointCount = 0;
            for (int i = 0; i < 3; i++) {
                ProjectPointToScreen(tri.Verts[i], out Vector2 sc, out bool inCamView);
                test.Verts[i] = new(sc, 0f);

                if (inCamView) {
                    insidePointCount++;
                }
            }

            screenTris = new(1) { test };

            if (insidePointCount == 3) {
                isInCamView = true;
                return;
            }
            
            if (insidePointCount == 0) {
                // const byte BOTTOM = 1, TOP = 2, LEFT = 4, RIGHT = 8;
                // HashSet<byte> p = new(3);
                // for (int i = 0; i < 3; i++) {
                //     byte curr = 0;
                //     if (test.Verts[i].X < 0) {
                //         curr |= LEFT;
                //     } else if (test.Verts[i].X >= TargetWidth) {
                //         curr |= RIGHT;
                //     }
                //     if (test.Verts[i].Y < 0) {
                //         curr |= BOTTOM;
                //     } else if (test.Verts[i].Y >= TargetHeight) {
                //         curr |= TOP;
                //     }
                //     if (curr != 0) {
                //         p.Add(curr);
                //     }
                // }

                // if (p.Count <= 1 || (p.Count == 2 && (
                //     (p.Contains(TOP) && (p.Contains(TOP | RIGHT) || p.Contains(TOP | LEFT) || p.Contains(RIGHT) || p.Contains(LEFT))) ||
                //     (p.Contains(BOTTOM) && (p.Contains(BOTTOM | RIGHT) || p.Contains(BOTTOM | LEFT) || p.Contains(RIGHT) || p.Contains(LEFT))) ||
                //     (p.Contains(RIGHT) && (p.Contains(RIGHT | TOP) || p.Contains(RIGHT | BOTTOM) || p.Contains(TOP) || p.Contains(BOTTOM))) ||
                //     (p.Contains(LEFT) && (p.Contains(LEFT | TOP) || p.Contains(LEFT | BOTTOM) || p.Contains(TOP) || p.Contains(BOTTOM)))))) {
                //     isInCamView = false;
                //     return;
                // }
                isInCamView = false;
                return;
            }

            screenTris = ClipTriangle(test);
            isInCamView = screenTris.Capacity > 0;
            return;
        }

        private readonly List<Triangle> ClipTriangle(Triangle tri) {
            List<Triangle> clippedTris = new() { tri };

            foreach (var plane in _planes) {
                clippedTris = ClipTrisAgainstPlane(clippedTris, plane);
            }

            return clippedTris;
        }

        private static List<Triangle> ClipTrisAgainstPlane(List<Triangle> tris, (Vector2 point, Vector2 dir) plane) {
            List<Triangle> output = new();

            foreach (var tri in tris) {
                foreach (var clippedTri in ClipTriAgainstPlane(tri, plane)) {
                    output.Add(clippedTri);
                }
            }

            return output;
        }

        private static List<Triangle> ClipTriAgainstPlane(Triangle tri, (Vector2 point, Vector2 dir) plane) {
            static Vector2 FindSplitVert(Vector2 p1, float d1, Vector2 p2, float d2) {
                float t = d1 / (d1 - d2);
                return p1 + t * (p2 - p1);
            }

            Vector2 a = tri.Verts[0].XY();
            Vector2 b = tri.Verts[1].XY();
            Vector2 c = tri.Verts[2].XY();

            float nDotA = Vector2.Dot(plane.dir, a - plane.point);
            float nDotB = Vector2.Dot(plane.dir, b - plane.point);
            float nDotC = Vector2.Dot(plane.dir, c - plane.point);
        
            bool aInside = nDotA > 0f;
            bool bInside = nDotB > 0f;
            bool cInside = nDotC > 0f;
        
            if (aInside && bInside && cInside) {
                return new(1) { tri };
            }

            if (!aInside && !bInside && !cInside) {
                return new(0);
            }

            List<Triangle> output = new(2);
            List<Vector2> insidePoints = new(4);

            if (aInside) {
                insidePoints.Add(a);
            }

            if (aInside != bInside) {
                insidePoints.Add(FindSplitVert(a, nDotA, b, nDotB));
            }

            if (bInside) {
                insidePoints.Add(b);
            }

            if (bInside != cInside) {
                insidePoints.Add(FindSplitVert(b, nDotB, c, nDotC));
            }

            if (cInside) {
                insidePoints.Add(c);
            }

            if (cInside != aInside) {
                insidePoints.Add(FindSplitVert(c, nDotC, a, nDotA));
            }

            if (insidePoints.Count >= 3) {
                output.Add(new(insidePoints[0], insidePoints[1], insidePoints[2]) { Color = tri.Color });
            } 
            if (insidePoints.Count == 4) {
                output.Add(new(insidePoints[0], insidePoints[2], insidePoints[3]) { Color = tri.Color });
            }
            return output;
        }

        private readonly Vector2 NormalizeCoordsToScreen(Vector4 normalCoords) {
            return new(
                (1f + normalCoords.X / normalCoords.W) * 0.5f * TargetWidth,
                (1f - normalCoords.Y / normalCoords.W) * 0.5f * TargetHeight
            );
        }
    }

    public static Vector2 XY(this Vector3 vec) {
        return new(vec.X, vec.Y);
    }
}