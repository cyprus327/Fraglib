using System;
using System.Numerics;

namespace Fraglib;

public static class FL {
    private static Engine? e = null;

/// <region>Setup</region>
#region setup
    private static bool isDrawClear = false;

    /// <name>Init</name>
    /// <returns>void</returns>
    /// <summary>Initializes the window with the specified width, height, and title.</summary>
    /// <summary>This method must be called before using any other FL methods.</summary>
    /// <param name="width">The width of the window in pixels.</param>
    /// <param name="height">The height of the window in pixels.</param>
    /// <param name="program">The delegate which will get invoked once per frame until the window is closed.</param>
    public static void Init(int width, int height, string title, Action? program = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        windowWidth = width;
        windowHeight = height;
        program ??= () => {};
        e = new DrawClearEngine(width, height, title, program);
        isDrawClear = true;
    }

    /// <name>Init</name>
    /// <returns>void</returns>
    /// <summary>Initializes the window with the specified width, height, title, perPixel function, and optional perFrame function.</summary>
    /// <summary>This method must be called before using any other FL methods.</summary>
    /// <param name="width">The width of the window in pixels.</param>
    /// <param name="height">The height of the window in pixels.</param>
    /// <param name="perPixel">The function that gets invoked for every pixel on the window until the window is closed.</summary>
    /// <param name="perFrame">Optional function that gets invoked once per frame until the window is closed.</summary>
    public static void Init(int width, int height, string title, Func<int, int, Uniforms, uint> perPixel, Action? perFrame = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        windowWidth = width;
        windowHeight = height;
        perFrame ??= () => {};
        e = new PerPixelEngine(width, height, title, perPixel, perFrame);
        ((PerPixelEngine)e).Accumulate = renderSettings.Accumulate;
    }

    /// <name>Run</name>
    /// <returns>void</returns>
    /// <summary>Starts the main loop of the engine.</summary>
    /// <summary>Must be called after Init for a window to appear.</summary>
    /// <summary>Any settings changed after this (PixelSize, VSync, etc.) won't affect anything.</summary>
    public static void Run() {
        if (e is null) {
            return;
        }

        e.VSyncEnabled = renderSettings.VSync;
        e.PixelSize = renderSettings.PixelSize;
        e.ScaleType = renderSettings.ScaleType;
        
        if (isDrawClear) {
            ((DrawClearEngine)e).Multithreaded = renderSettings.Multithreaded;
            if (renderSettings.Multithreaded) {
                ((DrawClearEngine)e).StartThreadPool();
            }
        }

        e.Run();
    }
#endregion setup

/// <region>DrawClear Methods</region>
#region drawclear methods
    /// <name>SetPixel</name>
    /// <returns>void</returns>
    /// <summary>Sets the pixel at the specified position to the given color.</summary>
    /// <param name="x">The x coordinate of the pixel.</param>
    /// <param name="y">The y coordinate of the pixel.</param>
    /// <param name="color">The color of the pixel.</param>
    public static void SetPixel(int x, int y, uint color) {
        if (!isDrawClear || x < 0 || x >= windowWidth || y < 0 || y >= windowHeight) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => {
            e!.Screen[y * windowWidth + x] = color;
        });
    }

    /// <name>GetPixel</name>
    /// <returns>uint</returns>
    /// <summary>Gets the pixel's color at the specified position.</summary>
    /// <param name="x">The x coordinate of the pixel.</param>
    /// <param name="y">The y coordinate of the pixel.</param>
    public static uint GetPixel(int x, int y) {
        if (!isDrawClear) {
            return 0; 
        }

        if (x < 0 || x >= windowWidth || y < 0 || y >= windowHeight) {
            return 0;
        }

        return e!.Screen[y * windowWidth + x];
    }

    /// <name>Clear</name>
    /// <returns>void</returns>
    /// <summary>Clears the window to black.</summary>
    public static void Clear() {
        Clear(Black);
    }

    /// <name>Clear</name>
    /// <returns>void</returns>
    /// <summary>Clears the window to the specified color.</summary>
    /// <param name="color">The color the window will get cleared to.</param>
    public static void Clear(uint color) {
        if (!isDrawClear) {
            return; 
        }

        ((DrawClearEngine)e!).AddAction(() => {
            Array.Fill(e.Screen, color);
        });
    }

    /// <name>DrawRect</name>
    /// <returns>void</returns>
    /// <summary>Draws a rectangle of specified size and color at the specified coordinates.</summary>
    /// <param name="x">The rectangle's x coordinate.</param>
    /// <param name="y">The rectangle's y coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public static void DrawRect(int x, int y, int width, int height, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawRectAction(x, y, width, height, color));
    }

    private static void DrawRectAction(int x, int y, int width, int height, uint color) {
        int xw = x + width, yh = y + height;
        DrawVerticalLineAction(x, y, yh, color);
        DrawVerticalLineAction(xw, y, yh, color);
        DrawHorizontalLineAction(x, xw, y, color);
        DrawHorizontalLineAction(x, xw, yh, color);
    }

    /// <name>FillRect</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid rectangle of specified size and color at the specified coordinates.</summary>
    /// <param name="x">The rectangle's x coordinate.</param>
    /// <param name="y">The rectangle's y coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public static void FillRect(int x, int y, int width, int height, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => FillRectAction(x, y, width, height, color));
    }

    private static unsafe void FillRectAction(int x, int y, int width, int height, uint color) {
        if (width <= 0 || height <= 0) {
            return;
        }

        int xClipped = Math.Max(x, 0);
        int yClipped = Math.Max(y, 0);
        int widthClipped = Math.Min(width + x, windowWidth) - xClipped;
        int heightClipped = Math.Min(height + y, windowHeight) - yClipped;

        if (widthClipped <= 0 || heightClipped <= 0) {
            return;
        }

        fixed (uint* screenPtr = e!.Screen) {
            uint* rowStartPtr = screenPtr + yClipped * windowWidth + xClipped;

            for (int sy = 0; sy < heightClipped; sy++) {
                uint* rowPtr = rowStartPtr + sy * windowWidth;

                for (int sx = 0; sx < widthClipped; sx++) {
                    *(rowPtr + sx) = color;
                }
            }
        }
    }

    /// <name>DrawCircle</name>
    /// <returns>void</returns>
    /// <summary>Draws the outline of a circle of specified size and color at the specified coordinates.</summary>
    /// <param name="centerX">The center coordinate of the circle along the x-axis.</param>
    /// <param name="centerY">The center coordinate of the cirlce along the y-axis.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public static void DrawCircle(float centerX, float centerY, float radius, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawCircleAction(centerX, centerY, radius, color));
    }

    private static unsafe void DrawCircleAction(float centerX, float centerY, float radius, uint color) {
        int x = (int)radius, y = 0;
        int decisionOver2 = 1 - x;

        fixed (uint* screenPtr = e!.Screen) {
            while (y <= x) {
                int i1 = (int)(centerY + y) * windowWidth + (int)(centerX + x);
                int i2 = (int)(centerY - y) * windowWidth + (int)(centerX + x);
                int i3 = (int)(centerY + y) * windowWidth + (int)(centerX - x);
                int i4 = (int)(centerY - y) * windowWidth + (int)(centerX - x);
                int i5 = (int)(centerY + x) * windowWidth + (int)(centerX + y);
                int i6 = (int)(centerY - x) * windowWidth + (int)(centerX + y);
                int i7 = (int)(centerY + x) * windowWidth + (int)(centerX - y);
                int i8 = (int)(centerY - x) * windowWidth + (int)(centerX - y);

                if (i1 >= 0 && i1 < windowWidth * windowHeight) {
                    screenPtr[i1] = color;
                }
                if (i2 >= 0 && i2 < windowWidth * windowHeight) {
                    screenPtr[i2] = color;
                }
                if (i3 >= 0 && i3 < windowWidth * windowHeight) {
                    screenPtr[i3] = color;
                }
                if (i4 >= 0 && i4 < windowWidth * windowHeight) {
                    screenPtr[i4] = color;
                }
                if (i5 >= 0 && i5 < windowWidth * windowHeight) {
                    screenPtr[i5] = color;
                }
                if (i6 >= 0 && i6 < windowWidth * windowHeight) {
                    screenPtr[i6] = color;
                }
                if (i7 >= 0 && i7 < windowWidth * windowHeight) {
                    screenPtr[i7] = color;
                }
                if (i8 >= 0 && i8 < windowWidth * windowHeight) {
                    screenPtr[i8] = color;
                }

                if (decisionOver2 <= 0) {
                    decisionOver2 += 2 * y + 1;
                } else {
                    x--;
                    decisionOver2 += 2 * (y - x) + 1;
                }

                y++;
            }
        }
    }

    /// <name>FillCircle</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid circle of specified size and color at the specified coordinates.</summary>
    /// <param name="centerX">The center coordinate of the circle along the x-axis.</param>
    /// <param name="centerY">The center coordinate of the cirlce along the y-axis.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public static void FillCircle(float centerX, float centerY, float radius, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => FillCircleAction(centerX, centerY, radius, color));
    }

    private static unsafe void FillCircleAction(float centerX, float centerY, float radius, uint color) {
        if (radius == 0) {
            return;
        }

        int xStart = (int)(centerX - radius);
        int xEnd = (int)(centerX + radius);
        int yStart = (int)(centerY - radius);
        int yEnd = (int)(centerY + radius);
        float radiusSquared = radius * radius;

        fixed (uint* screenPtr = e!.Screen) {
            for (int x = xStart; x <= xEnd; x++) {
                for (int y = yStart; y <= yEnd; y++) {
                    float dx = x - centerX;
                    float dy = y - centerY;
                    if (x >= 0 && x < windowWidth && y >= 0 && y < windowHeight && (dx * dx + dy * dy) <= radiusSquared) {
                        screenPtr[y * windowWidth + x] = color;
                    }
                }
            }
        }
    }

    /// <name>DrawLine</name>
    /// <returns>void</returns>
    /// <summary>Draws a line of specified color along the specified path.</summary>
    /// <param name="x0">The starting x coordinate of the line.</param>
    /// <param name="y0">The starting y coordinate of the line.</param>
    /// <param name="x1">The ending x coordinate of the line.</param>
    /// <param name="y1">The ending y coordinate of the line.</param>
    /// <param name="color">The color of the line.</param>
    public static void DrawLine(int x0, int y0, int x1, int y1, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawLineAction(x0, y0, x1, y1, color));
    }

    private static unsafe void DrawLineAction(int x0, int y0, int x1, int y1, uint color) {
        if (x0 < 0 || x0 >= windowWidth || y0 < 0 || y0 >= windowHeight ||
            x1 < 0 || x1 >= windowWidth || y1 < 0 || y1 >= windowHeight) {

            if (x0 != x1) {
                float m = (float)(y1 - y0) / (x1 - x0);

                if (x0 < 0) {
                    y0 += (int)(-x0 * m);
                    x0 = 0;
                } else if (x0 >= windowWidth) {
                    y0 += (int)((windowWidth - 1 - x0) * m);
                    x0 = windowWidth - 1;
                }

                if (x1 < 0) {
                    y1 += (int)(-x1 * m);
                    x1 = 0;
                } else if (x1 >= windowWidth) {
                    y1 += (int)((windowWidth - 1 - x1) * m);
                    x1 = windowWidth - 1;
                }
            }

            if (y0 < 0) {
                x0 += (int)(-y0 / (float)(y1 - y0) * (x1 - x0));
                y0 = 0;
            } else if (y0 >= windowHeight) {
                x0 += (int)((windowHeight - 1 - y0) / (float)(y1 - y0) * (x1 - x0));
                y0 = windowHeight - 1;
            }

            if (y1 < 0) {
                x1 += (int)(-y1 / (float)(y0 - y1) * (x0 - x1));
                y1 = 0;
            } else if (y1 >= windowHeight) {
                x1 += (int)((windowHeight - 1 - y1) / (float)(y0 - y1) * (x0 - x1));
                y1 = windowHeight - 1;
            }
        }

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int syw = sy * windowWidth;

        int er = dx - dy;

        fixed (uint* screenPtr = e!.Screen) {
            uint* ptr = screenPtr + y0 * windowWidth + x0;

            while (true) {
                *ptr = color;

                if (x0 == x1 && y0 == y1) {
                    break;
                }

                int e2 = 2 * er;

                if (e2 > -dy) {
                    er -= dy;
                    x0 += sx;
                    ptr += sx;
                }

                if (e2 < dx) {
                    er += dx;
                    y0 += sy;
                    ptr += syw;
                }
            }
        }
    }

    /// <name>DrawVerticalLine</name>
    /// <returns>void</returns>
    /// <summary>Draws a vertical line of specified color along the specified path.</summary>
    /// <summary>Should be used over DrawLine if the line is vertical.</summary>
    /// <param name="x">The x coordinate of the line.</param>
    /// <param name="y0">The starting y coordinate of the line.</param>
    /// <param name="y1">The ending y coordinate of the line.</param>
    /// <param name="color">The color of the line.</param>
    public static void DrawVerticalLine(int x, int y0, int y1, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawVerticalLineAction(x, y0, y1, color));
    }

    private static unsafe void DrawVerticalLineAction(int x, int y0, int y1, uint color) {
        if (x < 0 || x >= windowWidth || y0 >= windowHeight || y1 < 0) {
            return;
        }

        if (y0 < 0) {
            y0 = 0;
        }
        if (y1 >= windowHeight) {
            y1 = windowHeight - 1;
        }

        fixed (uint* screenPtr = e!.Screen) {
            int stride = Width;
            uint* rowPtr = screenPtr + y0 * stride + x;
            for (int y = y0; y <= y1; y++) {
                *rowPtr = color;
                rowPtr += stride;
            }
        }
    }

    /// <name>DrawHorizontalLine</name>
    /// <returns>void</returns>
    /// <summary>Draws a horizontal line of specified color along the specified path.</summary>
    /// <summary>Should be used over DrawLine if the line is horizontal.</summary>
    /// <param name="x0">The starting x coordinate of the line.</param>
    /// <param name="x1">The ending x coordinate of the line.</param>
    /// <param name="y">The y coordinate of the line.</param>
    /// <param name="color">The color of the line.</param>
    public static void DrawHorizontalLine(int x0, int x1, int y, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawHorizontalLineAction(x0, x1, y, color));
    }

    private static unsafe void DrawHorizontalLineAction(int x0, int x1, int y, uint color) {
        if (y < 0 || y >= windowHeight) {
            return;
        }

        if (x0 < 0) {
            x0 = 0;
        }

        if (x1 >= windowWidth) {
            x1 = windowWidth - 1;
        }

        if (x0 > x1) {
            return;
        }

        fixed (uint* screenPtr = e!.Screen) {
            int startInd = y * windowWidth + x0;
            uint* startPtr = screenPtr + startInd;
            uint* endPtr = startPtr + (x1 - x0);
            for (uint* currentPtr = startPtr; currentPtr <= endPtr; currentPtr++) {
                *currentPtr = color;
            }
        }
    }

    /// <name>DrawTriangle</name>
    /// <returns>void</returns>
    /// <summary>Draws the outline of a triangle of specified color with specified vertices. Should be used over DrawPolygon if the polygon is a triangle.</summary>
    /// <param name="x0">The x coordinate of the 1st vertex.</param>
    /// <param name="y0">The y coordinate of the 1st vertex.</param>
    /// <param name="x1">The x coordinate of the 2nd vertex.</param>
    /// <param name="y1">The y coordinate of the 2nd vertex.</param>
    /// <param name="x2">The x coordinate of the 3rd vertex.</param>
    /// <param name="y2">The y coordinate of the 3rd vertex.</param>
    /// <param name="color">The color of the triangle.</param>
    public static void DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawTriangleAction(x0, y0, x1, y1, x2, y2, color));
    }

    /// <name>DrawTriangle</name>
    /// <returns>void</returns>
    /// <summary>Draws the outline of a triangle of specified color with specified vertices. Should be used over DrawPolygon if the polygon is a triangle.</summary>
    /// <param name="v0">The 1st vertex.</param>
    /// <param name="v1">The 2nd vertex.</param>
    /// <param name="v2">The 3rd vertex.</param>
    /// <param name="color">The color of the triangle.</param>
    public static void DrawTriangle(Vector2 v0, Vector2 v1, Vector2 v2, uint color) {
        DrawTriangle((int)v0.X, (int)v0.Y, (int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y, color);
    }

    private static void DrawTriangleAction(int x0, int y0, int x1, int y1, int x2, int y2, uint color) {
        DrawLineAction(x0, y0, x1, y1, color);
        DrawLineAction(x1, y1, x2, y2, color);
        DrawLineAction(x2, y2, x0, y0, color);
    }

    /// <name>FillTriangle</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid triangle of specified color with specified vertices. Should be used over FillPolygon if the polygon is a triangle.</summary>
    /// <param name="x0">The x coordinate of the 1st vertex.</param>
    /// <param name="y0">The y coordinate of the 1st vertex.</param>
    /// <param name="x1">The x coordinate of the 2nd vertex.</param>
    /// <param name="y1">The y coordinate of the 2nd vertex.</param>
    /// <param name="x2">The x coordinate of the 3rd vertex.</param>
    /// <param name="y2">The y coordinate of the 3rd vertex.</param>
    /// <param name="color">The color of the triangle.</param>
    public static void FillTriangle(int x0, int y0, int x1, int y1, int x2, int y2, uint color) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => FillTriangleAction(x0, y0, x1, y1, x2, y2, color));
    }

    /// <name>FillTriangle</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid triangle of specified color with specified vertices. Should be used over FillPolygon if the polygon is a triangle.</summary>
    /// <param name="v0">The 1st vertex.</param>
    /// <param name="v1">The 2nd vertex.</param>
    /// <param name="v2">The 3rd vertex.</param>
    /// <param name="color">The color of the triangle.</param>
    public static void FillTriangle(Vector2 v0, Vector2 v1, Vector2 v2, uint color) {
        FillTriangle((int)v0.X, (int)v0.Y, (int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y, color);
    }

    private static void FillTriangleAction(int x0, int y0, int x1, int y1, int x2, int y2, uint color) {
        if (y1 < y0) {
            (x0, y0, x1, y1) = (x1, y1, x0, y0);
        }
        if (y2 < y0) {
            (x0, y0, x2, y2) = (x2, y2, x0, y0);
        }
        if (y2 < y1) {
            (x1, y1, x2, y2) = (x2, y2, x1, y1);
        }

        float s1 = (float)(x1 - x0) / (y1 - y0);
        float s2 = (float)(x2 - x0) / (y2 - y0);
        float s3 = (float)(x2 - x1) / (y2 - y1);

        for (int scanlineY = y0; scanlineY <= y1; scanlineY++) {
            int startX = (int)(x0 + (scanlineY - y0) * s1);
            int endX = (int)(x0 + (scanlineY - y0) * s2);

            if (endX < startX) {
                (startX, endX) = (endX, startX);
            }

            DrawHorizontalLineAction(startX, endX, scanlineY, color);
        }

        for (int y = y1; y <= y2; y++) {
            int startX = (int)(x1 + (y - y1) * s3);
            int endX = (int)(x0 + (y - y0) * s2);

            if (endX < startX) {
                (startX, endX) = (endX, startX);
            }

            DrawHorizontalLineAction(startX, endX, y, color);
        }
    }

    /// <name>DrawPolygon</name>
    /// <returns>void</returns>
    /// <summary>Draws the outline of a polygon of specified color with specified vertices.</summary>
    /// <param name="color">The color of the polygon.</param>
    /// <param name="vertices">The vertices of the polygon to draw. Must have a length >= 3.</param>
    public static void DrawPolygon(uint color, params Vector2[] vertices) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawPolygonAction(color, vertices));
    }

    private static void DrawPolygonAction(uint color, params Vector2[] vertices) {
        if (vertices.Length < 3) {
            return;
        }

        int vertexCount = vertices.Length;
        for (int i = 0; i < vertexCount; i++) {
            int next = (i + 1) % vertexCount;
            DrawLineAction((int)vertices[i].X, (int)vertices[i].Y, (int)vertices[next].X, (int)vertices[next].Y, color);
        }
    }

    /// <name>FillPolygon</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid polygon of specified color with specified vertices.</summary>
    /// <param name="color">The color of the polygon.</param>
    /// <param name="vertices">The vertices of the polygon to draw. Must have a length >= 3.</param>
    public static void FillPolygon(uint color, params Vector2[] vertices) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => FillPolygonAction(color, vertices));
    }

    private static unsafe void FillPolygonAction(uint color, params Vector2[] vertices) {
        if (vertices.Length < 3) {
            return;
        }

        int minY = (int)vertices[0].Y;
        int maxY = (int)vertices[0].Y;
        for (int i = 1; i < vertices.Length; i++) {
            int y = (int)vertices[i].Y;
            minY = Math.Min(minY, y);
            maxY = Math.Max(maxY, y);
        }

        int minYBound = Math.Max(minY, 0);
        int maxYBound = Math.Min(maxY, windowHeight - 1);
        int maxXBound = windowWidth - 1;

        fixed (uint* screenPtr = e!.Screen) {
            int length = vertices.Length;
            for (int y = minYBound; y <= maxYBound; y++) {
                List<int> intersections = new();

                int j = length - 1;
                for (int i = 0; i < length; i++) {
                    Vector2 currentVertex = vertices[i];
                    Vector2 nextVertex = vertices[j];

                    if ((currentVertex.Y < y && nextVertex.Y >= y) || (nextVertex.Y < y && currentVertex.Y >= y)) {
                        int intersectionX = (int)(currentVertex.X + (y - currentVertex.Y) * (nextVertex.X - currentVertex.X) / (nextVertex.Y - currentVertex.Y));
                        intersections.Add(intersectionX);
                    }

                    j = i;
                }

                intersections.Sort();

                int count = intersections.Count;
                uint* rowPtr = screenPtr + y * windowWidth;
                for (int i = 0; i < count; i += 2) {
                    uint* end = rowPtr + Math.Min(intersections[Math.Min(i + 1, count - 1)], maxXBound);
                    for (uint* start = rowPtr + Math.Max(intersections[i], 0); start <= end; start++) {
                        *start = color;
                    }
                }
            }
        }
    }

    /// <name>DrawTexture</name>
    /// <returns>void</returns>
    /// <summary>Draws a texture to the window at the specified coordinates.</summary>
    /// <param name="x">The x coordinate to draw the texture at.</param>
    /// <param name="y">The y coordinate to draw the texture at.</param>
    /// <param name="texture">The Texture to draw.</param>
    public static void DrawTexture(int x, int y, Texture texture) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawTextureAction(x, y, texture));
    }

    private static unsafe void DrawTextureAction(int x, int y, Texture texture) {
        fixed (uint* screenPtr = e!.Screen, texturePtr = texture.GetPixels) {
            int textureWidth = texture.Width, textureHeight = texture.Height;

            int startX = Math.Max(0, -x);
            int startY = Math.Max(0, -y);
            int endX = Math.Min(textureWidth, windowWidth - x);
            int endY = Math.Min(textureHeight, windowHeight - y);
            int numBytes = (endX - startX) * sizeof(uint);

            for (int sy = startY; sy < endY; sy++) {
                uint* screenRowPtr = screenPtr + (y + sy) * windowWidth + x;
                uint* textureRowPtr = texturePtr + sy * textureWidth;

                Buffer.MemoryCopy(textureRowPtr + startX, screenRowPtr + startX, numBytes, numBytes);
            }
        }
    }

    public static void DrawTransparentTexture(int x, int y, Texture texture) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawTransparentTextureAction(x, y, texture));
    }

    private static unsafe void DrawTransparentTextureAction(int x, int y, Texture texture) {
        fixed (uint* screenPtr = e!.Screen, texturePtr = texture.GetPixels) {
            int textureWidth = texture.Width, textureHeight = texture.Height;

            int startX = Math.Max(0, -x);
            int startY = Math.Max(0, -y);
            int endX = Math.Min(textureWidth, windowWidth - x);
            int endY = Math.Min(textureHeight, windowHeight - y);

            for (int sy = startY; sy < endY; sy++) {
                uint* screenRowPtr = screenPtr + (y + sy) * windowWidth + x;
                uint* textureRowPtr = texturePtr + sy * textureWidth;

                for (int sx = startX; sx < endX; sx++) {
                    uint srcPixel = textureRowPtr[sx];
                    uint destPixel = screenRowPtr[sx];

                    byte srcAlpha = srcPixel.GetA();
                    byte destAlpha = destPixel.GetA();

                    byte src255 = (byte)((255 - srcAlpha) / 255);
                    byte alpha = (byte)(srcAlpha + destAlpha * src255);
                    byte red = (byte)((srcPixel.GetR() * srcAlpha + destPixel.GetR() * destAlpha * src255) / alpha);
                    byte green = (byte)((srcPixel.GetG() * srcAlpha + destPixel.GetG() * destAlpha * src255) / alpha);
                    byte blue = (byte)((srcPixel.GetB() * srcAlpha + destPixel.GetB() * destAlpha * src255) / alpha);

                    *(screenRowPtr + sx) = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
                }
            }
        }
    }


    /// <name>DrawTextureScaled</name>
    /// <returns>void</returns>
    /// <summary>Draws a texture to the window at the specified coordinates with the specified scale.</summary>
    /// <param name="x">The x coordinate to draw the texture at.</param>
    /// <param name="y">The y coordinate to draw the texture at.</param>
    /// <param name="scaleX">The amount by which to scale the texture horizontally.</param>
    /// <param name="scaleY">The amount by which to scale the texture vertically.</param>
    /// <param name="texture">The Texture to draw.</param>
    public static void DrawTextureScaled(int x, int y, float scaleX, float scaleY, Texture texture) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawTextureScaledAction(x, y, scaleX, scaleY, texture));
    }

    public static void DrawTextureScaled(int x, int y, int scaledX, int scaledY, Texture texture) {
        if (!isDrawClear) {
            return;
        }

        ((DrawClearEngine)e!).AddAction(() => DrawTextureScaledAction(x, y, (float)scaledX / texture.Width, (float)scaledY / texture.Height, texture));
    }

    private static unsafe void DrawTextureScaledAction(int x, int y, float scaleX, float scaleY, Texture texture) {
        if (scaleX <= 0 || scaleY <= 0) {
            return;
        }

        int scaledWidth = (int)(texture.Width * scaleX);
        int scaledHeight = (int)(texture.Height * scaleY);

        int startX = Math.Max(0, -x);
        int startY = Math.Max(0, -y);
        int endX = Math.Min(scaledWidth, windowWidth - x);
        int endY = Math.Min(scaledHeight, windowHeight - y);

        if (endX <= 0 || endY <= 0) {
            return;
        }

        fixed (uint* screenPtr = e!.Screen, texturePtr = texture.GetPixels) {
            int textureWidth = texture.Width, textureHeight = texture.Height;

            for (int sy = startY; sy < endY; sy++) {
                int textureY = (int)(sy / scaleY);

                uint* screenRowPtr = screenPtr + (y + sy) * windowWidth + x;

                for (int sx = startX; sx < endX; sx++) {
                    int textureX = (int)(sx / scaleX);

                    uint* texturePixelPtr = texturePtr + textureY * textureWidth + textureX;
                    uint* screenPixelPtr = screenRowPtr + sx;

                    *screenPixelPtr = *texturePixelPtr;
                }
            }
        }
    }
#endregion drawclear methods

/// <region>Textures</region>
#region textures
    public readonly struct Texture {
        /// <name>Texture</name>
        /// <returns>Texture</returns>
        /// <summary>Creates a Texture from a Bitmap image. The alpha channel of the bitmap isn't taken into account for now.</summary>
        /// <param name="bmpImagePath">The path to a Bitmap image to create the texture from.</param>
        public Texture(string bmpImagePath) {
            if (!File.Exists(bmpImagePath)) {
                throw new FileNotFoundException("Specified image not found.");
            }

            using FileStream fs = new(bmpImagePath, FileMode.Open);

            byte[] header = new byte[54];
            fs.Read(header, 0, 54);
            
            if (header[0] != 'B' && header[1] != 'M') {
                throw new FileLoadException("File specified is not a bitmap (.bmp).");
            }

            Width = BitConverter.ToInt32(header, 18);
            Height = BitConverter.ToInt32(header, 22);

            int bpp = BitConverter.ToInt16(header, 28);
            if (bpp != 32) {
                throw new NotSupportedException($"Only 32 bit bitmaps are supported. Your bitmap is {bpp} bit. Here's a simple converter: https://online-converting.com/image/convert2bmp/");
            }

            int bmpSize = Height * Width;
            pixels = new uint[bmpSize];

            int pixelDataOffset = BitConverter.ToInt32(header, 10);
            fs.Seek(pixelDataOffset, SeekOrigin.Begin);

            byte[] buffer = new byte[4];
            for (int i = 0; i < bmpSize; i++) {
                fs.Read(buffer, 0, buffer.Length);
                pixels[i] = NewColor(buffer[2], buffer[1], buffer[0], buffer[3]);
            }
        }

        /// <name>Texture</name>
        /// <returns>Texture</returns>
        /// <summary>Clones a Texture.</summary>
        /// <param name="texture">The texture to create a copy of.</param>
        public Texture(Texture texture) {
            Width = texture.Width;
            Height = texture.Height;

            pixels = new uint[Width * Height];
            Array.Copy(texture.GetPixels, pixels, texture.GetPixels.Length);
        }

        /// <name>Texture</name>
        /// <returns>Texture</returns>
        /// <summary>Creates an empty Texture of specified width and height.</summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public Texture(int width, int height) {
            Width = width;
            Height = height;

            pixels = new uint[width * height];
        }

        /// <name>Width</name>
        /// <returns>int</returns>
        /// <summary>The texture's width.</summary>
        public int Width { get; }
        /// <name>Height</name>
        /// <returns>int</returns>
        /// <summary>The texture's height.</summary>
        public int Height { get; }

        private readonly uint[] pixels;

        /// <name>GetPixels</name>
        /// <returns>uint[]</returns>
        /// <summary>Gets the pixels of the texture.</summary>
        public readonly uint[] GetPixels => pixels;

        /// <name>SetPixel</name>
        /// <returns>void</returns>
        /// <summary>Sets a pixel in the texture at specified coordinates to specified color.</summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <param name="color">The color to set the pixel.</param>
        public void SetPixel(int x, int y, uint color) {
            int ind = y * Width + x;

            if (ind < 0 || ind >= pixels.Length) {
                return;
            }

            pixels[ind] = color;
        }

        /// <name>GetPixel</name>
        /// <returns>uint</returns>
        /// <summary>Gets a pixel in the texture at specified coordinates.</summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        public uint GetPixel(int x, int y) {
            int ind = y * Width + x;

            if (ind < 0 || ind >= pixels.Length) {
                return Black;
            }

            return pixels[ind];
        }

        /// <name>ScaleTo</name>
        /// <returns>Texture</returns>
        /// <summary>Returns the parent texture scaled by the factors scaleX and scaleY.</summary>
        /// <param name="scaleX">The amount to scale the texture by horizontally.</param>
        /// <param name="scaleY">The amount to scale the texture by vertically.</param>
        public Texture ScaleTo(float scaleX, float scaleY) {
            return ScaleTo((int)(scaleX * Width), (int)(scaleY * Height));
        }

        /// <name>ScaleTo</name>
        /// <returns>Texture</returns>
        /// <summary>Returns the parent texture scaled by the factors scaleX and scaleY.</summary>
        /// <param name="scaledX">The width to scale the texture to in pixels.</param>
        /// <param name="scaledY">The height to scale the texture to in pixels.</param>
        public Texture ScaleTo(int scaledX, int scaledY) {
            float scaleX = (float)scaledX / Width;
            float scaleY = (float)scaledY / Height;

            Texture scaledTexture = new(scaledX, scaledY);

            for (int y = 0; y < scaledY; y++) {
                for (int x = 0; x < scaledX; x++) {
                    uint pixel = GetPixel((int)(x / scaleX), (int)(y / scaleY));
                    scaledTexture.SetPixel(x, y, pixel);
                }
            }

            return scaledTexture;
        }

        /// <name>CropTo</name>
        /// <returns>Texture</returns>
        /// <summary>Returns the parent texture cropped to the resolution specified.</summary>
        /// <param name="texStartX">The x coordinate within the texture where cropping starts.</param>
        /// <param name="texStartY">The y coordinate within the texture where cropping starts.</param>
        /// <param name="texWidth">The width of the cropped texture section.</param>
        /// <param name="texHeight">The height of the cropped texture section.</param>
        public Texture CropTo(int texStartX, int texStartY, int texWidth, int texHeight) {
            if (texStartX < 0 || texStartY < 0 || texStartX >= Width || texStartY >= Height) {
                throw new ArgumentOutOfRangeException("Cropping region is out of bounds.");
            }

            texWidth = Math.Min(texWidth, Width - texStartX);
            texHeight = Math.Min(texHeight, Height - texStartY);

            Texture croppedTexture = new(texWidth, texHeight);

            for (int y = 0; y < texHeight; y++) {
                for (int x = 0; x < texWidth; x++) {
                    uint pixel = GetPixel(texStartX + x, texStartY + y);
                    croppedTexture.SetPixel(x, y, pixel);
                }
            }

            return croppedTexture;
        }
    }
#endregion textures

/// <region>States</region>
#region states
    private static readonly List<uint[]> _states = new();

    /// <name>SaveState</name>
    /// <returns>void</returns>
    /// <summary>Saves the current state of the window to a buffer.</summary>
    /// <param name="state">An int that corresponds to the saved buffer and can be passed into LoadScreen.</param>
    public static void SaveState(out int state) {
        if (e is null) {
            state = -1;
            return;
        }

        int index = _states.Count;
        _states.Add((uint[])e.Screen.Clone());
        state = index;
    }

    /// <name>LoadState</name>
    /// <returns>void</returns>
    /// <summary>Sets the window to a previously saved state.</summary>
    /// <param name="state">An int that corresponds to the previously saved buffer, generated from SaveScreen.</param>
    public static unsafe void LoadState(int state) {
        if (e is null) {
            return;
        }

        if (state < 0 || state >= _states.Count) {
            return;
        }

        unsafe {
            fixed (uint* screenPtr = e.Screen, statePtr = _states[state]) {
                long numBytes = e.Screen.Length * sizeof(uint);
                Buffer.MemoryCopy(statePtr, screenPtr, numBytes, numBytes);
            }
        }
    }

    /// <name>ClearStates</name>
    /// <returns>void</returns>
    /// <summary>Clears any previously saved states.</summary>
    public static void ClearStates() {
        _states.Clear();
    }
#endregion states

/// <region>Colors</region>
#region colors
    private static readonly bool _isLittleEndian = BitConverter.IsLittleEndian;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color black, 4278190080.</summary>
    public static uint Black => 4278190080;
    
    /// <name>Gray</name>
    /// <returns>uint</returns>
    /// <summary>The color gray, 4286611584.</summary>
    public static uint Gray => 4286611584;
    
    /// <name>White</name>
    /// <returns>uint</returns>
    /// <summary>The color white, 4294967295.</summary>
    public static uint White => 4294967295;
    
    /// <name>Red</name>
    /// <returns>uint</returns>
    /// <summary>The color red, 4278190335.</summary>
    public static uint Red => 4278190335;

    /// <name>Green</name>
    /// <returns>uint</returns>
    /// <summary>The color green, 4278255360.</summary>
    public static uint Green => 4278255360;

    /// <name>Blue</name>
    /// <returns>uint</returns>
    /// <summary>The color blue, 4294901760.</summary>
    public static uint Blue => 4294901760;

    /// <name>Yellow</name>
    /// <returns>uint</returns>
    /// <summary>The color yellow, either 4278255615.</summary>
    public static uint Yellow => 4278255615;

    /// <name>Orange</name>
    /// <returns>uint</returns>
    /// <summary>The color orange, either 4278232575.</summary>
    public static uint Orange => 4278232575;

    /// <name>Cyan</name>
    /// <returns>uint</returns>
    /// <summary>The color cyan, either 4294967040.</summary>
    public static uint Cyan => 4294967040;

    /// <name>Magenta</name>
    /// <returns>uint</returns>
    /// <summary>The color magenta, either 4294902015.</summary>
    public static uint Magenta => 4294902015;

    /// <name>Turquoise</name>
    /// <returns>uint</returns>
    /// <summary>The color turquoise, either 4291878976.</summary>
    public static uint Turquoise => 4291878976;

    /// <name>Lavender</name>
    /// <returns>uint</returns>
    /// <summary>The color lavender, either 4294633190.</summary>
    public static uint Lavender => 4294633190;

    /// <name>Crimson</name>
    /// <returns>uint</returns>
    /// <summary>The color crimson, either 4282127580.</summary>
    public static uint Crimson => 4282127580;
    
    /// <name>Rainbow</name>
    /// <returns>uint</returns>
    /// <summary>A color that cycles through all the full rainbow based on ElapsedTime.</summary>
    /// <param name="timeScale">Optional parameter that controls how fast the color changes.</param>
    public static uint Rainbow(float timeScale = 1f) => HslToRgb(ElapsedTime * 60f * Math.Abs(timeScale) % 360f, 1f, 0.5f);

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from 4 bytes in ABGR format (0xAABBGGRR).</summary>
    /// <param name="r">The R channel's value between [0, 255].</param>
    /// <param name="g">The G channel's value between [0, 255].</param>
    /// <param name="b">The B channel's value between [0, 255].</param>
    /// <param name="a">The A channel's value between [0, 255].</param>
    public static uint NewColor(byte r, byte g, byte b, byte a = 255) {
        return ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r;
    }

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from 4 floats in ABGR format (0xAABBGGRR).</summary>
    /// <param name="r">The R channel's value between [0.0, 1.0].</param>
    /// <param name="g">The G channel's value between [0.0, 1.0].</param>
    /// <param name="b">The B channel's value between [0.0, 1.0].</param>
    /// <param name="a">The A channel's value between [0.0, 1.0].</param>
    public static uint NewColor(float r, float g, float b, float a = 1f) {
        return NewColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
    }

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from a Vector3 and additional float in ABGR format (0xAABBGGRR).</summary>
    /// <param name="col">The R, G, and B channels, all between [0.0, 1.0].</param>
    /// <param name="a">The A channel between [0.0, 1.0].</param>
    public static uint NewColor(Vector3 col, float a = 1f) {
        return NewColor(col.X, col.Y, col.Z, a);
    }

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from a Vector4 in ABGR format (0xAABBGGRR).</summary>
    /// <param name="col">The R, G, B, and A channels, all between [0.0, 1.0].</param>
    public static uint NewColor(Vector4 col) {
        return NewColor(col.X, col.Y, col.Z, col.W);
    }

    /// <name>ToVec3</name>
    /// <returns>Vector3</returns>
    /// <summary>Creates a Vector3 from a color, will always return in RGB format.</summary>
    /// <param name="color">Optional parameter representing the color to convert to a Vector3.</param>
    public static Vector3 ToVec3(this uint color) {
        byte b = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte r = (byte)color;
        return new Vector3(r / 255f, g / 255f, b / 255f);
    }

    /// <name>ToVec4</name>
    /// <returns>Vector4</returns>
    /// <summary>Creates a Vector4 from a color, will always return in RGBA format.</summary>
    /// <param name="color">Optional parameter representing the color to convert to a Vector4.</param>
    public static Vector4 ToVec4(this uint color) {
        byte a = (byte)(color >> 24);
        byte b = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte r = (byte)color;
        return new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    /// <name>AverageColors</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from averaging two provided colors in ABGR format (0xAABBGGRR).</summary>
    /// <summary>The order in which parameters are supplied doesn't matter.</summary>
    /// <param name="color1">The first color to average with.</param>
    /// <param name="color2">The second color to average with.</param>
    public static uint AverageColors(uint color1, uint color2) {
        byte c01 = (byte)((color1 >> 0) & 0xFF);
        byte c02 = (byte)((color1 >> 8) & 0xFF);
        byte c03 = (byte)((color1 >> 16) & 0xFF);
        byte c04 = (byte)((color1 >> 24) & 0xFF);

        byte c11 = (byte)((color2 >> 0) & 0xFF);
        byte c12 = (byte)((color2 >> 8) & 0xFF);
        byte c13 = (byte)((color2 >> 16) & 0xFF);
        byte c14 = (byte)((color2 >> 24) & 0xFF);

        byte a1 = (byte)((c01 + c11) / 2);
        byte a2 = (byte)((c02 + c12) / 2);
        byte a3 = (byte)((c03 + c13) / 2);
        byte a4 = (byte)((c04 + c14) / 2);

        return ((uint)a4 << 24) | ((uint)a3 << 16) | ((uint)a2 << 8) | a1;
    }

    /// <name>LerpColors</name>
    /// <returns>uint</returns>
    /// <summary>Linearly interpolates between two colors in ABGR format (0xAABBGGRR).</summary>
    /// <param name="color1">The first color to interpolate from.</param>
    /// <param name="color2">The second color to interpolate to.</param>
    /// <param name="t">The interpolation factor between [0, 1], where 0 represents color1 and 1 represents color2.</param>
    public static uint LerpColors(uint color1, uint color2, float t) {
        byte r1 = color1.GetR();
        byte g1 = color1.GetG();
        byte b1 = color1.GetB();
        byte a1 = color1.GetA();

        return NewColor(
            (byte)(r1 + (color2.GetR() - r1) * t), 
            (byte)(g1 + (color2.GetG() - g1) * t), 
            (byte)(b1 + (color2.GetB() - b1) * t), 
            (byte)(a1 + (color2.GetA() - a1) * t));
    }

    /// <name>GetR</name>
    /// <returns>byte</returns>
    /// <summary>An extension method that extracts the red channel of the specified color in the range [0, 255].</summary>
    /// <param name="color">An optional parameter representing the color of which to extract the channel.</param>
    public static byte GetR(this uint color) {
        return (byte)(color & 0xFF);
    }

    /// <name>GetG</name>
    /// <returns>byte</returns>
    /// <summary>An extension method that extracts the green channel of the specified color in the range [0, 255].</summary>
    /// <param name="color">An optional parameter representing the color of which to extract the channel.</param>
    public static byte GetG(this uint color) {
        return (byte)((color >> 8) & 0xFF);
    }

    /// <name>GetB</name>
    /// <returns>byte</returns>
    /// <summary>An extension method that extracts the blue channel of the specified color in the range [0, 255].</summary>
    /// <param name="color">An optional parameter representing the color of which to extract the channel.</param>
    public static byte GetB(this uint color) {
        return (byte)((color >> 16) & 0xFF);
    }

    /// <name>GetA</name>
    /// <returns>byte</returns>
    /// <summary>An extension method that extracts the alpha channel of the specified color in the range [0, 255].</summary>
    /// <param name="color">An optional parameter representing the color of which to extract the channel.</param>
    public static byte GetA(this uint color) {
        return (byte)((color >> 24) & 0xFF); 
    }

    /// <name>SetR</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the red channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the red channel.</param>
    /// <param name="newR">The new value for the R channel of the color, in the range [0, 255].</param>
    public static uint SetR(this ref uint color, byte newR) {
        color = (color & 0xFFFFFF00) | newR;
        return color;
    }

    /// <name>SetR</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the red channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the red channel.</param>
    /// <param name="newR">The new value for the R channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetR(this ref uint color, float newR) {
        color.SetR((byte)(newR * 255f));
        return color;
    }

    /// <name>SetG</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the green channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the green channel.</param>
    /// <param name="newG">The new value for the G channel of the color, in the range [0, 255].</param>
    public static uint SetG(this ref uint color, byte newG) {
        color = (color & 0xFFFF00FF) | ((uint)newG << 8);
        return color;
    }

    /// <name>SetG</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the green channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the green  channel.</param>
    /// <param name="newG">The new value for the G channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetG(this ref uint color, float newG) {
        color.SetG((byte)(newG * 255f));
        return color;
    }

    /// <name>SetB</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the blue channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the blue channel.</param>
    /// <param name="newB">The new value for the B channel of the color, in the range [0, 255].</param>
    public static uint SetB(this ref uint color, byte newB) {
        color = (color & 0xFF00FFFF) | ((uint)newB << 16);
        return color;
    }

    /// <name>SetB</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the blue channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the blue channel.</param>
    /// <param name="newB">The new value for the B channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetB(this ref uint color, float newB) {
        color.SetB((byte)(newB * 255f));
        return color;
    }

    /// <name>SetA</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the alpha channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the alpha channel.</param>
    /// <param name="newA">The new value for the A channel of the color, in the range [0, 255].</param>
    public static uint SetA(this ref uint color, byte newA) {
        color = (color & 0x00FFFFFF) | ((uint)newA << 24);
        return color;
    }

    /// <name>SetA</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the alpha channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the alpha channel.</param>
    /// <param name="newA">The new value for the A channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetA(this ref uint color, float newA) {
        color.SetA((byte)(newA * 255f));
        return color;
    }


    /// <name>HslToRgb</name>
    /// <returns>uint</returns>
    /// <summary>Converts a color from HSL format to either RGBA (0xRRGGBBAA) or ABGR format (0xAABBGGRR) depending on the system's endianness.</summary>
    /// <param name="hue">The H channel's value.</param>
    /// <param name="saturation">The S channel's value.</param>
    /// <param name="lightness">The L channel's value.</param>
    public static uint HslToRgb(float hue, float saturation, float lightness) {
        float chroma = (1f - Math.Abs(2f * lightness - 1f)) * saturation;
        float huePrime = hue / 60f;
        float x = chroma * (1f - Math.Abs(huePrime % 2f - 1f));
        float red, green, blue;

        if (huePrime >= 0f && huePrime < 1f) {
            red = chroma;
            green = x;
            blue = 0f;
        } else if (huePrime >= 1f && huePrime < 2f) {
            red = x;
            green = chroma;
            blue = 0f;
        } else if (huePrime >= 2f && huePrime < 3f) {
            red = 0f;
            green = chroma;
            blue = x;
        } else if (huePrime >= 3f && huePrime < 4f) {
            red = 0f;
            green = x;
            blue = chroma;
        } else if (huePrime >= 4f && huePrime < 5f) {
            red = x;
            green = 0f;
            blue = chroma;
        } else {
            red = chroma;
            green = 0f;
            blue = x;
        }

        float lightnessMatch = lightness - 0.5f * chroma;
        red += lightnessMatch;
        green += lightnessMatch;
        blue += lightnessMatch;

        return NewColor((byte)(red * 255f), (byte)(green * 255f), (byte)(blue * 255f));
    }
#endregion colors

/// <region>Common</region>
#region common
    /// <name>RenderSettings</name>
    /// <returns>struct</returns>
    /// <summary>The struct defining the settings which will be applied when FL.Init is called.</summary>
    public struct RenderSettings {
        public RenderSettings() { }

        /// <name>PixelSize</name>
        /// <returns>int</returns>
        /// <summary>Gets or sets the pixel size of the window. Clamped in the range [1, 100].</summary>
        public int PixelSize {
            readonly get => pixelSize;
            set => pixelSize = Math.Clamp(value, 1, 100);
        }
        private int pixelSize = 1;
        
        /// <name>VSync</name>
        /// <returns>bool</returns>
        /// <summary>Gets or sets whether or not VSync is enabled.</summary>
        public bool VSync { get; set; } = true;
        
        /// <name>Multithreaded</name>
        /// <returns>bool</returns>
        /// <summary>Gets or sets whether or not the engine is multithreaded. Only applicable in DrawClear mode.</summary>
        public bool Multithreaded { get; set; } = false;
        
        /// <name>Accumulate</name>
        /// <returns>bool</returns>
        /// <summary>Gets or sets whether or not the engine accumulates previous frames with the current frame. Only applicable in PerPixel mode. Can be changed during runtime.</summary>
        public bool Accumulate {
            readonly get => e is PerPixelEngine p ? p.Accumulate : accumulate;
            set {
                if (e is PerPixelEngine p) {
                    p.Accumulate = value;
                } else {
                    accumulate = value;
                }
            }
        }
        private bool accumulate = false;

        /// <name>ScaleType</name>
        /// <returns>ScaleType</returns>
        /// <summary>Gets or sets how the engine renders when PixelSize > 1.</summary>
        public ScaleType ScaleType;
    }

    /// <name>Settings</name>
    /// <returns>RenderSettings</returns>
    /// <summary>Gets or sets the settings with which the engine will run. Not all settings can be changed during runtime, the ones that can't be must be set before Init.</summary>
    public static RenderSettings Settings {
        get => renderSettings;
        set {
            if (e is null) {
                renderSettings = value;
            } else {
                renderSettings.Accumulate = value.Accumulate;
            }
        }
    }
    private static RenderSettings renderSettings = new();

    /// <name>ElapsedTime</name>
    /// <returns>float</returns>
    /// <summary>The total time since Run was called.</summary>
    public static float ElapsedTime => e?.ElapsedTime ?? 0f;

    /// <name>DeltaTime</name>
    /// <returns>float</returns>
    /// <summary>The time from the current frame to the last frame.</summary>
    public static float DeltaTime => e?.DeltaTime ?? 0f;

    /// <name>Width</name>
    /// <returns>int</returns>
    /// <summary>The width of the window.</summary>
    public static int Width => windowWidth;

    /// <name>Height</name>
    /// <returns>int</returns>
    /// <summary>The height of the window.</summary>
    public static int Height => windowHeight;

    private static int windowWidth = 0, windowHeight = 0;

    /// <name>Mouse</name>
    /// <returns>Vector2</returns>
    /// <summary>The mouse position on the window.</summary>
    public static Vector2 Mouse => new(e?.MousePosition.X ?? 0f, windowHeight - e?.MousePosition.Y ?? 0f);

    /// <name>MouseDelta</name>
    /// <returns>Vector2</returns>
    /// <summary>The amount the mouse has moved from the last frame to the current frame.</summary>
    public static Vector2 MouseDelta => new(e?.MouseState.Delta.X ?? 0f, e?.MouseState.Delta.Y ?? 0f);

    //===========================================================
    // Lehmer Rand
    private static uint randState = 0;

    /// <name>Rand</name>
    /// <returns>uint</returns>
    /// <summary>Generates a random unsigned integer using the Lehmer random number generator.</summary>
    public static uint Rand() {
        randState += 0xE120FC15;
        ulong temp;
        temp = (ulong)randState * 0x4A39B70D;
        uint m1 = (uint)((temp >> 32) ^ temp);
        temp = (ulong)m1 * 0x12FAD5C9;
        uint m2 = (uint)((temp >> 32) ^ temp);
        return m2;
    }

    /// <name>Rand</name>
    /// <returns>int</returns>
    /// <summary>Generates a random integer within the specified range [min, max].</summary>
    /// <param name="min">The minimum possible value of a number that could be generated.</param>
    /// <param name="max">The maximum possible value of a number that could be generated.</param>
    public static int Rand(int min, int max) {
        if (min >= max) {
            return 0;
        }
        return (int)(Rand() % (max - min)) + min;
    }

    /// <name>Rand</name>
    /// <returns>float</returns>
    /// <summary>Generates a random float within the specified range [min, max].</summary>
    /// <param name="min">The minimum possible value of a number that could be generated.</param>
    /// <param name="max">The maximum possible value of a number that could be generated.</param>
    public static float Rand(float min, float max) {
        if (min >= max) {
            return 0f;
        }

        return (float)(min + Rand() / (double)uint.MaxValue * ((double)max - (double)min));
    }

    /// <name>RandInUnitSphere</name>
    /// <returns>Vector3</returns>
    /// <summary>Generates a random Vector3 on the unit sphere.</summary>
    public static Vector3 RandInUnitSphere() {
        return Vector3.Normalize(new Vector3(
            Rand(-1f, 1f),
            Rand(-1f, 1f),
            Rand(-1f, 1f)
        ));
    }

    //===========================================================
    // Input
    [System.Runtime.InteropServices.DllImport("user32.dll")] static extern short GetAsyncKeyState(int key);
    private static readonly Dictionary<char, bool> _previousKeyStates = new Dictionary<char, bool>();

    /// <name>GetKeyDown</name>
    /// <returns>bool</returns>
    /// <summary>Returns whether or not the specified key is currently being held down.</summary>
    /// <param name="char">The key to check represented by a key's keycode, e.g. 'Q' or 81 for the Q key.</param>
    public static bool GetKeyDown(char key) {
        return (GetAsyncKeyState(key) & 0x8000) != 0;
    }

    /// <name>GetKeyUp</name>
    /// <returns>bool</returns>
    /// <summary>Returns whether or not the specified key was just released, i.e. held down last frame but not this one.</summary>
    /// <param name="char">The key to check represented by a key's keycode, e.g. 'Q' or 81 for the Q key.</param>
    public static bool GetKeyUp(char key) {
        bool isKeyDown = GetKeyDown(key);
        bool wasKeyDown = _previousKeyStates.ContainsKey(key) && _previousKeyStates[key];

        _previousKeyStates[key] = isKeyDown;

        return wasKeyDown && !isKeyDown;
    }

    /// <name>LMBDown</name>
    /// <returns>bool</returns>
    /// <summary>Returns whether or not the left mouse button is being held down.</summary>
    public static bool LMBDown() {
        return (GetAsyncKeyState(0x01) & 0x8000) != 0;
    }

    /// <name>RMBDown</name>
    /// <returns>bool</returns>
    /// <summary>Returns whether or not the right mouse button is being held down.</summary>
    public static bool RMBDown() {
        return (GetAsyncKeyState(0x02) & 0x8000) != 0;
    }

    /// <name>LMBUp</name>
    /// <returns>bool</returns>
    /// <summary>Returns whether or not the left mouse button was just released.</summary>
    public static bool LMBUp() {
        return GetKeyUp((char)0x01);
    }

    /// <name>RMBUp</name>
    /// <returns>bool</returns>
    /// <summary>Returns whether or not the right mouse button was just released.</summary>
    public static bool RMBUp() {
        return GetKeyUp((char)0x02);
    }

    //===========================================================
    // Math
    /// <name>DegToRad</name>
    /// <returns>float</returns>
    /// <summary>Extension method to convert degrees to radians.</summary>
    /// <param name="deg">Optional parameter representing the degrees to convert.</param>
    public static float DegToRad(this float deg) {
        return MathF.PI / 180f * deg;
    }

    /// <name>RadToDeg</name>
    /// <returns>float</returns>
    /// <summary>Extension method to convert radians to degrees.</summary>
    /// <param name="rad">Optional parameter representing the radians to convert.</param>
    public static float RadToDeg(this float rad) {
        return 180f / MathF.PI * rad;
    }

    /// <name>Rotate</name>
    /// <returns>Vector2</returns>
    /// <summary>Extension method to rotate a Vector2 around the specified Vector2 by the specified angle.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="vec">Optional parameter representing the Vector2 to modify.</param>
    /// <param name="center">The center of rotation for 'vec'.</param>
    /// <param name="angle">The angle by which 'vec' will be rotated.</param>
    public static Vector2 Rotate(this ref Vector2 vec, Vector2 center, float angle) {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        float newX = center.X + (vec.X - center.X) * cos - (vec.Y - center.Y) * sin;
        float newY = center.Y + (vec.X - center.X) * sin + (vec.Y - center.Y) * cos;

        vec.X = newX;
        vec.Y = newY;

        return vec;
    }

    /// <name>Rotate</name>
    /// <returns>Vector2[]</returns>
    /// <summary>Extension method to rotate all Vector2s in an array around the specified Vector2 by the specified angle.</summary>
    /// <summary>Modifies the underlying array.</summary>
    /// <param name="arr">Optional parameter representing the array of Vector2s to modify.</param>
    /// <param name="center">The center of rotation for the vectors in 'arr'.</param>
    /// <param name="angle">The angle by which the vectors in 'arr' will be rotated.</param>
    public static Vector2[] Rotate(this Vector2[] arr, Vector2 center, float angle) {
        //Array.ForEach(arr, v => v.Rotate(center, angle));
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Rotate(center, angle);
        }

        return arr;
    }

    /// <name>Scale</name>
    /// <returns>Vector2</returns>
    /// <summary>Extension method to scale a Vector2 around the specified Vector2 by the specified factor.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="vec">Optional parameter representing the Vector2 to modify.</param>
    /// <param name="center">The center 'vec' will be scaled around.</param>
    /// <param name="factor">The factor by which 'vec' will be scaled.</param>
    public static Vector2 Scale(this ref Vector2 vec, Vector2 center, float factor) {
        float newX = center.X + (vec.X - center.X) * factor;
        float newY = center.Y + (vec.Y - center.Y) * factor;

        vec.X = newX;
        vec.Y = newY;

        return vec;
    }

    /// <name>Scale</name>
    /// <returns>Vector2[]</returns>
    /// <summary>Extension method to scale all Vector2s in an array around the specified Vector2 by the specified factor.</summary>
    /// <summary>Modifies the underlying array.</summary>
    /// <param name="arr">Optional parameter representing the array of Vector2s to modify.</param>
    /// <param name="center">The center for the vectors in 'arr' to be scaled around.</param>
    /// <param name="factor">The factor by which the vectors in 'arr' will be scaled.</param>
    public static Vector2[] Scale(this Vector2[] arr, Vector2 center, float factor) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Scale(center, factor);
        }

        return arr;
    }

    /// <name>AverageWith</name>
    /// <returns>Vector2</returns>
    /// <summary>Extension method to average two Vector2s together.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="vec">Optional parameter representing the Vector2 to modify.</param>
    /// <param name="other">The Vector2 to average 'vec' with.</param>
    public static Vector2 AverageWith(this ref Vector2 vec, Vector2 other) {
        vec.X = (vec.X + other.X) / 2f;
        vec.Y = (vec.Y + other.Y) / 2f;
    
        return vec;
    }

    /// <name>AverageWith</name>
    /// <returns>Vector2[]</returns>
    /// <summary>Extension method to average all Vector2s in an array with another specified Vector2.</summary>
    /// <summary>Modifies the underlying array.</summary>
    /// <param name="arr">Optional parameter representing the array of Vector2s to modify.</param>
    /// <param name="other">The Vector2 to average all the Vector2s in 'arr' with.</param>
    public static Vector2[] AverageWith(this Vector2[] arr, Vector2 other) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].AverageWith(other);
        }

        return arr;
    }

    /// <name>Translate</name>
    /// <returns>Vector2</returns>
    /// <summary>Extension method to translate a Vector2 by the specified amount.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="vec">Optional parameter representing the Vector2 to modify.</param>
    /// <param name="offsetX">The amount to move 'vec' by on the x-axis.</param>
    /// <param name="offsetY">The amount to move 'vec' by on the y-axis.</param>
    public static Vector2 Translate(this ref Vector2 vec, float offsetX, float offsetY) {
        vec.X += offsetX;
        vec.Y += offsetY;

        return vec;
    }

    /// <name>Translate</name>
    /// <returns>Vector2[]</returns>
    /// <summary>Extension method to translate all Vector2s in an array by the specified amount.</summary>
    /// <summary>Modifies the underlying array.</summary>
    /// <param name="arr">Optional parameter representing the array of Vector2s to modify.</param>
    /// <param name="offsetX">The amount to move the Vector2s in 'arr' by on the x-axis.</param>
    /// <param name="offsetY">The amount to move the Vector2s in 'arr' by on the y-axis.</param>
    public static Vector2[] Translate(this Vector2[] arr, float offsetX, float offsetY) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Translate(offsetX, offsetY);
        }

        return arr;
    }
#endregion common
}