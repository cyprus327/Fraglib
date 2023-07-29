
using System.Numerics;
using Fraglib;

internal static class RaycastingExample {
    private static void Main() {
        Raycaster r = new(800, 450, 4);
        
        FL.PixelSize = 4;
        FL.Init(800, 450, "Raycaster", r.Program);
        FL.Run();
    }
}

internal sealed class Raycaster {
    public Raycaster(int width, int height, int pixelSize) {
        _width = width / pixelSize;
        _height = height / pixelSize;
        _pixelSize = pixelSize;
    }

    private readonly int _width, _height;
    private readonly int _pixelSize;

    private readonly int[,] _world = {
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 0, 0, 0, 0, 3, 0, 3, 0, 3, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 2, 2, 0, 2, 2, 0, 0, 0, 0, 3, 0, 3, 0, 3, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 0, 4, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 0, 0, 0, 0, 5, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 0, 4, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 0, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
    };

    private Vector2 pos = new(10f, 10f);
    private Vector2 dir = new(-1f, 0f);
    private Vector2 plane = new(0f, 0.66f);
    private float jumpHeight = 0f, jumpVel = 0f;
    private bool jumping = false;

    public void Program() {
        FL.Clear();

        int camHeight = (int)(_height / 2f - jumpHeight);
        
        FL.FillRect(0, 0, _width, camHeight, FL.Gray);

        for (int x = 0; x < _width; x++) {
            float camX = 2f * (float)x / _width - 1;
            Vector2 rayDir = new(dir.X + plane.X * camX, dir.Y + plane.Y * camX);

            Vector2 deltaDist = new(
                MathF.Sqrt(1f + (rayDir.Y * rayDir.Y) / (rayDir.X * rayDir.X)),
                MathF.Sqrt(1f + (rayDir.X * rayDir.X) / (rayDir.Y * rayDir.Y))
            );

            Vector2 sideDist;
            int stepX, stepY;
            if (rayDir.X < 0) {
                stepX = -1;
                sideDist.X = (pos.X - (int)pos.X) * deltaDist.X;
            } else {
                stepX = 1;
                sideDist.X = ((int)pos.X + 1f - pos.X) * deltaDist.X;
            }

            if (rayDir.Y < 0) {
                stepY = -1;
                sideDist.Y = (pos.Y - (int)pos.Y) * deltaDist.Y;
            } else {
                stepY = 1;
                sideDist.Y = ((int)pos.Y + 1f - pos.Y) * deltaDist.Y;
            }

            int hit = 0, worldX = (int)pos.X, worldY = (int)pos.Y, side = 0;
            while (hit == 0) {
                if (sideDist.X < sideDist.Y) {
                    sideDist.X += deltaDist.X;
                    worldX += stepX;
                    side = 0;
                } else {
                    sideDist.Y += deltaDist.Y;
                    worldY += stepY;
                    side = 1;
                }

                if (_world[worldY, worldX] > 0) {
                    hit = 1;
                }
            }

            float perpWallDist = side == 0 ?
                (worldX - pos.X + (1 - stepX) / 2f) / rayDir.X :
                (worldY - pos.Y + (1 - stepY) / 2f) / rayDir.Y;


            int lineHeight = (int)(_height / perpWallDist);
            int drawStart = (int)Math.Max(0, -lineHeight / 2 + camHeight);
            int drawEnd = (int)Math.Min(_height - 1, lineHeight / 2 + camHeight);
        
            uint col = GetColor(worldX, worldY);

            if (side == 1) {
                col.SetR((byte)(col.GetR() >> 1));
                col.SetG((byte)(col.GetG() >> 1));
                col.SetB((byte)(col.GetB() >> 1));
            }

            FL.DrawLine(x, drawStart, x, drawEnd, col);
        }

        for (int y = 0; y < _world.GetLength(0); y++) {
            for (int x = 0; x < _world.GetLength(1); x++) {
                if (x == (int)pos.X && y == (int)pos.Y) {
                    FL.SetPixel(x, y, FL.Black);
                } else {
                    FL.SetPixel(x, y, GetColor(x, y));
                }
            }
        }

        if (jumping) {
            jumpVel -= 4f * FL.DeltaTime;
            jumpHeight += jumpVel;

            if (jumpHeight <= 0f) {
                jumpHeight = 0f;
                jumpVel = 0f;
                jumping = false;
            }
        }

        HandleInput(10f * FL.DeltaTime);
    }

    private void HandleInput(float s) {
        if (FL.GetKeyDown('W')) {
            if (_world[(int)pos.Y, (int)(pos.X + dir.X * s)] == 0) {
                pos.X += dir.X * s;
            }
            if (_world[(int)(pos.Y + dir.Y * s), (int)pos.X] == 0) {
                pos.Y += dir.Y * s;
            }
        } else if (FL.GetKeyDown('S')) {
            if (_world[(int)pos.Y, (int)(pos.X - dir.X * s)] == 0) {
                pos.X -= dir.X * s;
            }
            if (_world[(int)(pos.Y - dir.Y * s), (int)pos.X] == 0) {
                pos.Y -= dir.Y * s;
            }
        }
        if (FL.GetKeyDown('A')) {
            if (_world[(int)pos.Y, (int)(pos.X - plane.X * s)] == 0) {
                pos.X -= plane.X * s;
            }
            if (_world[(int)(pos.Y - plane.Y * s), (int)pos.X] == 0) {
                pos.Y -= plane.Y * s;
            }
        } else if (FL.GetKeyDown('D')) {
            if (_world[(int)pos.Y, (int)(pos.X + plane.X * s)] == 0) {
                pos.X += plane.X * s;
            }
            if (_world[(int)(pos.Y + plane.Y * s), (int)pos.X] == 0) {
                pos.Y += plane.Y * s;
            }
        }

        s /= 2f;
        if (FL.GetKeyDown('Q')) {
            float sin = MathF.Sin(s), cos = MathF.Cos(s);
            
            dir.Y = dir.X * sin + dir.Y * cos;
            dir.X = dir.X * cos - dir.Y * sin;

            plane.Y = plane.X * sin + plane.Y * cos;
            plane.X = plane.X * cos - plane.Y * sin;
        } else if (FL.GetKeyDown('E')) {
            float sin = MathF.Sin(-s), cos = MathF.Cos(-s);
            
            dir.Y = dir.X * sin + dir.Y * cos;
            dir.X = dir.X * cos - dir.Y * sin;

            plane.Y = plane.X * sin + plane.Y * cos;
            plane.X = plane.X * cos - plane.Y * sin;
        }

        if (!jumping && FL.GetKeyDown(' ')) {
            jumping = true;
            jumpVel = 1f;
        }    
    }

    private uint GetColor(int x, int y) {
        return _world[y, x] switch {
            0 => FL.Gray,
            1 => FL.Crimson,
            2 => FL.Lavender,
            3 => FL.Magenta,
            4 => FL.Blue,
            5 => FL.Rainbow(),
            _ => FL.Black 
        };
    }
}