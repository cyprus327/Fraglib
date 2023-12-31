using System.Numerics;

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
            TargetWidth = windowWidth;
            TargetHeight = windowHeight;
        }

        /// <name>ctor</name>
        /// <returns>Camera</returns>
        /// <summary>Initializes a new instantce of the Camera struct with the specified properties.</summary>
        /// <param name="targetWidth">The target width for the camera to render to.</param>
        /// <param name="targetHeight">The target height for the camera to render to.</param>
        public Camera(int targetWidth, int targetHeight) {
            TargetWidth = Math.Clamp(targetWidth, 1, 10000);
            TargetHeight = Math.Clamp(targetHeight, 1, 10000);

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
            yawPitchMat = Matrix4x4.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            viewMat = Matrix4x4.CreateLookAt(Pos, Pos + Vector3.Transform(-Vector3.UnitZ, yawPitchMat), Vector3.UnitY);
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
            Vector4 clipSpace = Vector4.Transform(new Vector4(point.x, point.y, point.z, 1f), viewRenderMat);

            Vector2 sc = NormalizeCoordsToScreen(clipSpace);
            screenCoords.x = (int)sc.X;
            screenCoords.y = (int)sc.Y;

            clipSpace /= clipSpace.W == 0f ? float.Epsilon : clipSpace.W;
            isInCamView = Math.Abs(clipSpace.X) <= 1f &&
                Math.Abs(clipSpace.Y) <= 1f &&
                clipSpace.Z >= -1f &&
                clipSpace.Z <= 1f;
        }

        /// <name>ProjectPointToScreen</name>
        /// <returns>void</returns>
        /// <summary>Projects a 3D point to screen coordinates.</summary>
        /// <param name="point">The point to project, of type Vector3.</param>
        /// <param name="screenCoords">An out Vector2 representing the screen coordinates of the projected point.</param>
        /// <param name="isInCamView">An out bool representing whether or not the point can be seen by the camera.</param>
        public readonly void ProjectPointToScreen(Vector3 point, out Vector2 screenCoords, out bool isInCamView) {
            Vector4 clipSpace = Vector4.Transform(new Vector4(point, 1f), viewRenderMat);

            screenCoords = NormalizeCoordsToScreen(clipSpace);

            clipSpace /= clipSpace.W == 0f ? float.Epsilon : clipSpace.W;
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
            List<byte> outsidePointInds = new(3);
            Triangle test = new() { Color = tri.Color };
            for (byte i = 0; i < 3; i++) {
                ProjectPointToScreen(tri.Verts[i], out var sc, out bool inCamView);
                if (!inCamView) {
                    outsidePointInds.Add(i);
                }
                test.Verts[i] = new(sc, 0f);
            }

            if (outsidePointInds.Count == 0) {
                isInCamView = true;
                screenTris = new(1) { test };
                return;
            }

            screenTris = ClipTriangle(test, outsidePointInds);
            isInCamView = screenTris.Capacity != 0;
            if (!isInCamView) {
                screenTris = new(1) { test };
            }
        }

        private readonly List<Triangle> ClipTriangle(Triangle tri, List<byte> outsidePointInds) {
            static Vector2 Clip(Vector2 ls, Vector2 op, int w, int h) {
                float slope = (op.Y - ls.Y) / (op.X - ls.X);

                if (op.X < 0f) {
                    op.X = 0f;
                    op.Y = ls.Y + slope * (0f - ls.X);
                } else if (op.X >= w) {
                    op.X = w;
                    op.Y = ls.Y + slope * (w - ls.X);
                }

                if (op.Y < 0f) {
                    op.Y = 0f;
                    op.X = ls.X + 1f / slope * (0f - ls.Y);
                } else if (op.Y >= h) {
                    op.Y = h;
                    op.X = ls.X + 1f / slope * (h - ls.Y);
                }

                return op;
            }

            static bool Intersect(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2, out Vector2 intersection) {
                float denom = (e2.Y - s2.Y) * (e1.X - s1.X) - (e2.X - s2.X) * (e1.Y - s1.Y);

                if (denom == 0) {
                    goto END;
                }

                float ua = ((e2.X - s2.X) * (s1.Y - s2.Y) - (e2.Y - s2.Y) * (s1.X - s2.X)) / denom;
                float ub = ((e1.X - s1.X) * (s1.Y - s2.Y) - (e1.Y - s1.Y) * (s1.X - s2.X)) / denom;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1) {
                    float intersectionX = s1.X + ua * (e1.X - s1.X);
                    float intersectionY = s1.Y + ua * (e1.Y - s1.Y);
                    intersection = new(intersectionX, intersectionY);
                    return true;
                }

                END:
                intersection = Vector2.Zero;
                return false;
            }

            const byte BOTTOM = 1, LEFT = 2, TOP = 4, RIGHT = 8;

            Vector2[] verts2D = new Vector2[3];
            for (byte i = 0; i < 3; i++) {
                verts2D[i] = new(tri.Verts[i].X, tri.Verts[i].Y);
            }

            List<Triangle> output;
            Vector2 a, b, c, a_, b_, c_;

            HashSet<byte> d = new(3);
            for (byte i = 0; i < 3; i++) {
                byte q = 0;

                if (verts2D[i].X < 0f) {
                    q |= LEFT;
                } else if (verts2D[i].X >= TargetWidth) {
                    q |= RIGHT;
                }
                if (verts2D[i].Y < 0f) {
                    q |= BOTTOM;
                } else if (verts2D[i].Y >= TargetHeight) {
                    q |= TOP;
                }

                d.Add(q);
            }

            if (outsidePointInds.Count == 3) {
                if (d.Count == 1) {
                    return new(0);
                }

                if (d.Count == 2) {
                    return new(2);
                }

                if (d.Count == 3) {
                    return new(5);
                }

                return new(1);
            }

            // https://gabrielgambetta.com/computer-graphics-from-scratch/images/r14-clip-triangle1.png
            if (outsidePointInds.Count == 2) {
                a = verts2D.Where((v, i) => !outsidePointInds.Contains((byte)i)).First();
                b = verts2D[outsidePointInds[0]];
                c = verts2D[outsidePointInds[1]];

                b_ = Clip(a, b, TargetWidth, TargetHeight);
                c_ = Clip(a, c, TargetWidth, TargetHeight);

                bool sameX = b_.X == c_.X, sameY = b_.Y == c_.Y;
                if ((sameX && !sameY) || (!sameX && sameY)) {
                    return new(1) { new(a, b_, c_) { Color = Red } };
                }

                output = new(2) { new(a, b_, c_) { Color = Red } };

                if (d.Contains(BOTTOM | LEFT)) {
                    output.Add(new(b_, Vector2.Zero, c_) { Color = Magenta });
                } else if (d.Contains(BOTTOM | RIGHT)) {
                    output.Add(new(c_, b_, new(TargetWidth, 0f)) { Color = Magenta });
                } else if (d.Contains(TOP | LEFT)) {
                    output.Add(new(b_, new(0f, TargetHeight), c_) { Color = Magenta });
                } else if (d.Contains(TOP | RIGHT)) {
                    output.Add(new(b_, c_, new(TargetWidth, TargetHeight)) { Color = Magenta });
                } else if (d.Contains(TOP) && d.Contains(BOTTOM)) {
                    if (Intersect(b, c, new(0f, TargetHeight), new(TargetHeight, TargetWidth), out Vector2 b__) &&
                        Intersect(c, b, new(0f, 0f), new(0f, TargetWidth), out Vector2 c__)) {
                        output.Add(new(b__, c_, c__) { Color = Rand() });
                        output.Add(new(b__, b_, c_) { Color = Rand() });
                    }
                }

                return output;
            }

            // https://gabrielgambetta.com/computer-graphics-from-scratch/images/r14-clip-triangle2.png
            var insidePoints = verts2D.Where((v, i) => !outsidePointInds.Contains((byte)i));
            a = insidePoints.First();
            b = insidePoints.Last();
            c = verts2D[outsidePointInds[0]];

            a_ = Clip(a, c, TargetWidth, TargetHeight);
            b_ = Clip(b, c, TargetWidth, TargetHeight);

            if (a_.X == b_.X || a_.Y == b_.Y) {
                return new(2) { 
                    new(a, b, a_) { Color = Blue },
                    new(b, a_, b_) { Color = Green }
                };
            }

            output = new(3) { 
                new(a, b, a_) { Color = Blue },
                new(b, a_, b_) { Color = Green }
            };

            if (d.Contains(BOTTOM | LEFT)) {
                output.Add(new(b_, a_, Vector2.Zero) { Color = Magenta });
            } else if (d.Contains(BOTTOM | RIGHT)) {
                output.Add(new(b_, a_, new(TargetWidth, 0f)) { Color = Magenta });
            } else if (d.Contains(TOP | LEFT)) {
                output.Add(new(a_, b_, new(0f, TargetHeight)) { Color = Magenta });
            } else if (d.Contains(TOP | RIGHT)) {
                output.Add(new(a_, b_, new(TargetWidth, TargetHeight)) { Color = Magenta });
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
}