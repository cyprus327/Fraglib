using System.Numerics;

namespace Fraglib;

public static class FL {
    private static Engine? e = null;

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
        scaledWidth = width / PixelSize;
        scaledHeight = height / PixelSize;
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
    /// <param name="perPixel">The funcion that gets invoked for every pixel on the window until the window is closed.</summary>
    /// <param name="perFrame">The funcion that gets invoked once per frame until the window is closed.</summary>
    public static void Init(int width, int height, string title, Func<int, int, Uniforms, uint> perPixel, Action? perFrame = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        windowWidth = width;
        windowHeight = height;
        scaledWidth = width / PixelSize;
        scaledHeight = height / PixelSize;
        perFrame ??= () => {};
        e = new PerPixelEngine(width, height, title, perPixel, perFrame);
    }

    /// <name>Init</name>
    /// <returns>void</returns>
    /// <summary>Starts the main loop of the engine.</summary>
    /// <summary>Must be called after Init for a window to appear.</summary>
    /// <summary>Any settings changed after this (PixelSize, VSync, etc.) won't affect anything.</summary>
    public static void Run() {
        if (!isDrawClear) {
            return;
        }

        e!.VSyncEnabled = VSync;
        e.PixelSize = Math.Abs(PixelSize);
        e.Run();
    }
#endregion setup

#region drawclear methods
    /// <name>SetPixel</name>
    /// <returns>void</returns>
    /// <summary>Sets the pixel at the specified position to the given color.</summary>
    /// <param name="x">The x coordinate of the pixel.</param>
    /// <param name="y">The y coordinate of the pixel.</param>
    /// <param name="color">The color of the pixel in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void SetPixel(int x, int y, uint color) {
        if (!isDrawClear || x < 0 || x >= scaledWidth || y < 0 || y >= scaledHeight) {
            return;
        }

        if (PixelSize == 1) {
            e!.Screen[y * windowWidth + x] = color;
            return;
        }

        x *= PixelSize;
        y *= PixelSize;
        int xMax = Math.Min(x + PixelSize, windowWidth);
        int yMax = Math.Min(y + PixelSize, windowHeight);
        unsafe {
            fixed (uint* ptr = e!.Screen) {
                for (int py = y; py < yMax; py++) {
                    int yo = py * windowWidth;
                    for (int px = x; px < xMax; px++) {
                        ptr[yo + px] = color;
                    }
                }
            }
        }
    }

    private static unsafe void SetPixel(int x, int y, uint color, uint* ptr) {
        if (!isDrawClear || x < 0 || x >= scaledWidth || y < 0 || y >= scaledHeight) {
            return;
        }

        if (PixelSize == 1) {
            ptr[y * windowWidth + x] = color;
            return;
        }

        x *= PixelSize;
        y *= PixelSize;
        int xMax = Math.Min(x + PixelSize, windowWidth);
        int yMax = Math.Min(y + PixelSize, windowHeight);
        for (int py = y; py < yMax; py++) {
            int yo = py * windowWidth;
            for (int px = x; px < xMax; px++) {
                ptr[yo + px] = color;
            }
        }
    }

    /// <name>GetPixel</name>
    /// <returns>uint</returns>
    /// <summary>Gets the pixel's color at the specified position.</summary>
    /// <param name="x">The x coordinate of the pixel.</param>
    /// <param name="y">The y coordinate of the pixel.</param>
    /// <param name="color">The color of the pixel in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
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
    /// <param name="color">The color the window will get cleared to in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void Clear(uint color) {
        if (!isDrawClear) {
            return; 
        }

        Array.Fill(e!.Screen, color);
    }

    /// <name>FillRect</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid rectangle of specified size and color at the specified coordinates.</summary>
    /// <param name="x">The starting point of the rectangle's x coordinate.</param>
    /// <param name="y">The starting point of the rectangle's y coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void FillRect(int x, int y, int width, int height, uint color) {
        if (!isDrawClear || width <= 0 || height <= 0) {
            return;
        }

        x = Math.Max(x, 0);
        y = Math.Max(y, 0);
        width = Math.Min(x + width, windowWidth);
        height = Math.Min(y + height, windowHeight);

        x *= PixelSize;
        y *= PixelSize;
        width *= PixelSize;
        height *= PixelSize;
        unsafe {
            fixed (uint* ptr = e!.Screen) {
                for (int r = y; r < height; r++) {
                    int ro = r * windowWidth;
                    for (int c = x; c < width; c++) {
                        ptr[ro + c] = color;
                    }
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
    /// <param name="color">The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void DrawCircle(float centerX, float centerY, float radius, uint color) {
        if (!isDrawClear) {
            return;
        }

        int x = (int)radius, y = 0;
        int decisionOver2 = 1 - x;

        unsafe {
            fixed (uint* ptr = e!.Screen) {
                while (y <= x) {
                    SetCirclePixels(centerX, centerY, x, y++, color, ptr);

                    if (decisionOver2 <= 0) {
                        decisionOver2 += 2 * y + 1;
                    } else {
                        x--;
                        decisionOver2 += 2 * (y - x) + 1;
                    }

                    SetCirclePixels(centerX, centerY, x, y, color, ptr);
                }
            }
        }
    }

    private static unsafe void SetCirclePixels(float cx, float cy, int ox, int oy, uint color, uint* ptr) {
        SetPixel((int)(cx + ox), (int)(cy + oy), color, ptr);
        SetPixel((int)(cx - ox), (int)(cy + oy), color, ptr);
        SetPixel((int)(cx + ox), (int)(cy - oy), color, ptr);
        SetPixel((int)(cx - ox), (int)(cy - oy), color, ptr);
        SetPixel((int)(cx + oy), (int)(cy + ox), color, ptr);
        SetPixel((int)(cx - oy), (int)(cy + ox), color, ptr);
        SetPixel((int)(cx + oy), (int)(cy - ox), color, ptr);
        SetPixel((int)(cx - oy), (int)(cy - ox), color, ptr);
    }

    /// <name>FillCircle</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid circle of specified size and color at the specified coordinates.</summary>
    /// <param name="centerX">The center coordinate of the circle along the x-axis.</param>
    /// <param name="centerY">The center coordinate of the cirlce along the y-axis.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void FillCircle(float centerX, float centerY, float radius, uint color) {
        if (!isDrawClear) {
            return;
        }

        unsafe {
            fixed (uint* ptr = e!.Screen) {
                for (int x = (int)(centerX - radius); x <= centerX + radius; x++) {
                    for (int y = (int)(centerY - radius); y <= centerY + radius; y++) {
                        if (Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(radius, 2)) {
                            SetPixel(x, y, color, ptr);
                        }
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
    /// <param name="color">The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void DrawLine(int x0, int y0, int x1, int y1, uint color) {
        if (!isDrawClear) {
            return;
        }

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int er = dx - dy;

        unsafe {
            fixed (uint* ptr = e!.Screen) {
                while (true) {
                    SetPixel(x0, y0, color, ptr);

                    if (x0 == x1 && y0 == y1) {
                        break;
                    }

                    int e2 = 2 * er;

                    if (e2 > -dy) {
                        er -= dy;
                        x0 += sx;
                    }

                    if (e2 < dx) {
                        er += dx;
                        y0 += sy;
                    }
                }
            }
        }
    }

    /// <name>DrawLine</name>
    /// <returns>void</returns>
    /// <summary>Draws a vertical line of specified color along the specified path.</summary>
    /// <summary>Should be used over DrawLine if the line is vertical.</summary>
    /// <param name="x">The x coordinate of the line.</param>
    /// <param name="y0">The starting y coordinate of the line.</param>
    /// <param name="y1">The ending y coordinate of the line.</param>
    /// <param name="color">The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    public static void DrawVerticalLine(int x, int y0, int y1, uint color) {
        if (!isDrawClear || x < 0 || x >= scaledWidth || y0 < 0 || y1 >= scaledHeight || y0 > y1) {
            return;
        }

        // NOTE: don't replace with SetPixel, ~110fps faster to do it like this
        if (PixelSize == 1) {
            unsafe { 
                fixed (uint* ptr = e!.Screen) {
                    int stride = Width;
                    for (int y = y0; y <= y1; y++) {
                        ptr[y * stride + x] = color;
                    }
                }
            }
            return;
        }

        int scaledX = x * PixelSize;
        int scaledY0 = y0 * PixelSize;
        int scaledY1 = y1 * PixelSize;

        int xBounds = Math.Min(scaledX + PixelSize, windowWidth);
        int yBounds = Math.Min(scaledY1 + PixelSize, windowHeight);

        unsafe {
            fixed (uint* ptr = e!.Screen) {
                for (int sy = scaledY0; sy < yBounds; sy++) {
                    int yo = sy * windowWidth;
                    for (int sx = scaledX; sx < xBounds; sx++) {
                        ptr[yo + sx] = color;
                    }
                }
            }
        }
    }

    /// <name>DrawPolygon</name>
    /// <returns>void</returns>
    /// <summary>Draws the outline of a polygon of specified color with specified vertices.</summary>
    /// <param name="color">The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    /// <param name="vertices">The vertices of the polygon to draw. Must have a length >= 3.</param>
    public static void DrawPolygon(uint color, params Vector2[] vertices) {
        if (!isDrawClear || vertices is null || vertices.Length < 3) {
            return;
        }

        int vertexCount = vertices.Length;
        for (int i = 0; i < vertexCount; i++) {
            int next = (i + 1) % vertexCount;
            DrawLine((int)vertices[i].X, (int)vertices[i].Y, (int)vertices[next].X, (int)vertices[next].Y, color);
        }
    }

    /// <name>FillPolygon</name>
    /// <returns>void</returns>
    /// <summary>Fills a solid polygon of specified color with specified vertices.</summary>
    /// <param name="color">The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</param>
    /// <param name="vertices">The vertices of the polygon to draw. Must have a length >= 3.</param>
    public static void FillPolygon(uint color, params Vector2[] vertices) {
        if (!isDrawClear || vertices is null || vertices.Length < 3) {
            return;
        }

        int minX = (int)vertices.Min(v => v.X);
        int maxX = (int)vertices.Max(v => v.X);
        int minY = (int)vertices.Min(v => v.Y);
        int maxY = (int)vertices.Max(v => v.Y);

        unsafe {
            fixed (uint* ptr = e!.Screen) {
                int length = vertices.Length;
                for (int y = minY; y <= maxY; y++) {
                    List<int> intersections = new List<int>();
                    for (int i = 0; i < length; i++) {
                        Vector2 currentVertex = vertices[i];
                        Vector2 nextVertex = vertices[(i + 1) % length];

                        if (currentVertex.Y <= y && nextVertex.Y >= y || nextVertex.Y <= y && currentVertex.Y >= y) {
                            int intersectionX = (int)(currentVertex.X + (y - currentVertex.Y) * (nextVertex.X - currentVertex.X) / (nextVertex.Y - currentVertex.Y));
                            intersections.Add(intersectionX);
                        }
                    }

                    intersections.Sort();
                    
                    int count = intersections.Count;
                    for (int i = 0; i < count; i += 2) {
                        int xStart = intersections[i];
                        int xEnd = intersections[(i + 1) % count];

                        for (int x = xStart; x <= xEnd; x++) {
                            SetPixel(x, y, color, ptr);
                        }
                    }
                }
            }
        }
    }
#endregion drawclear methods

#region states
    private static readonly List<uint[]> _states = new();

    /// <name>SaveScreen</name>
    /// <returns>void</returns>
    /// <summary>Saves the current state of the window to a buffer.</summary>
    /// <param name="state">An int that corresponds to the saved buffer and can be passed into LoadScreen.</param>
    public static void SaveScreen(out int state) {
        if (e is null) {
            state = -1;
            return;
        }

        int index = _states.Count;
        _states.Add((uint[])e.Screen.Clone());
        state = index;
    }

    /// <name>LoadScreen</name>
    /// <returns>void</returns>
    /// <summary>Sets the window to a previously saved state.</summary>
    /// <param name="state">An int that corresponds to the previously saved buffer, generated from SaveScreen.</param>
    public static void LoadScreen(int state) {
        if (e is null) {
            return;
        }

        if (state < 0 || state >= _states.Count) {
            return;
        }

        Array.Copy(_states[state], e.Screen, e.Screen.Length);
        //Buffer.BlockCopy(_states[state], 0, e.Screen, 0, e.Screen.Length * sizeof(uint));
    }

    /// <name>ClearStates</name>
    /// <returns>void</returns>
    /// <summary>Clears any previously saved states.</summary>
    public static void ClearStates() {
        _states.Clear();
    }
#endregion states

#region colors
    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color black, either 4278190080 or 255.</summary>
    public static uint Black => BitConverter.IsLittleEndian ? 4278190080 : 255;
    
    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color gray, either 4286611584 or 2155905279.</summary>
    public static uint Gray => BitConverter.IsLittleEndian ? 4286611584 : 2155905279;
    
    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color white, represented as 4294967295.</summary>
    public static uint White => 4294967295;
    
    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color red, represented as 4278190335.</summary>
    public static uint Red => 4278190335;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color green, either 4278255360 or 16711935.</summary>
    public static uint Green => BitConverter.IsLittleEndian ? 4278255360 : 16711935;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color blue, either 4294901760 or 65535.</summary>
    public static uint Blue => BitConverter.IsLittleEndian ? 4294901760 : 65535;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color yellow, either 4278255615 or 4294902015.</summary>
    public static uint Yellow => BitConverter.IsLittleEndian ? 4278255615 : 4294902015;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color orange, either 4278232575 or 4289003775.</summary>
    public static uint Orange => BitConverter.IsLittleEndian ? 4278232575 : 4289003775;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color cyan, either 4294967040 or 16777215.</summary>
    public static uint Cyan => BitConverter.IsLittleEndian ? 4294967040 : 16777215;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color magenta, either 4294902015 or 4278255615.</summary>
    public static uint Magenta => BitConverter.IsLittleEndian ? 4294902015 : 4278255615;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color turquoise, either 4291878976 or 1088475391.</summary>
    public static uint Turquoise => BitConverter.IsLittleEndian ? 4291878976 : 1088475391;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color lavender, either 4294633190 or 3873897215.</summary>
    public static uint Lavender => BitConverter.IsLittleEndian ? 4294633190 : 3873897215;

    /// <name>Black</name>
    /// <returns>uint</returns>
    /// <summary>The color crimson, either 4282127580 or 3692313855.</summary>
    public static uint Crimson => BitConverter.IsLittleEndian ? 4282127580 : 3692313855;
    
    /// <name>Rainbow</name>
    /// <returns>uint</returns>
    /// <summary>A color that cycles through all the full rainbow based on ElapsedTime.</summary>
    /// <param name="timeScale">Optional parameter that controls how fast the color changes.</param>
    public static uint Rainbow(float timeScale = 1f) => HslToRgb((ElapsedTime * 60f * Math.Abs(timeScale)) % 360f, 1f, 0.5f);

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from 4 bytes, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</summary>
    /// <param name="r">The R channel's value between [0, 255].</param>
    /// <param name="g">The G channel's value between [0, 255].</param>
    /// <param name="b">The B channel's value between [0, 255].</param>
    /// <param name="a">The A channel's value between [0, 255].</param>
    public static uint NewColor(byte r, byte g, byte b, byte a = 255) {
        return BitConverter.IsLittleEndian ?
            ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r :
            ((uint)r << 24) | ((uint)g << 16) | ((uint)b << 8) | a;
    }

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from 4 floats, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</summary>
    /// <param name="r">The R channel's value between [0.0, 1.0].</param>
    /// <param name="g">The G channel's value between [0.0, 1.0].</param>
    /// <param name="b">The B channel's value between [0.0, 1.0].</param>
    /// <param name="a">The A channel's value between [0.0, 1.0].</param>
    public static uint NewColor(float r, float g, float b, float a = 1f) {
        return NewColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
    }

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from a Vector3 and additional float, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</summary>
    /// <param name="col">The R, G, and B channels, all between [0.0, 1.0].</param>
    /// <param name="a">The A channel between [0.0, 1.0].</param>
    public static uint NewColor(Vector3 col, float a = 1f) {
        return NewColor(col.X, col.Y, col.Z, a);
    }

    /// <name>NewColor</name>
    /// <returns>uint</returns>
    /// <summary>Creates a color from a Vector4, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</summary>
    /// <param name="col">The R, B, B, and A channels, all between [0.0, 1.0].</param>
    public static uint NewColor(Vector4 col) {
        return NewColor(col.X, col.Y, col.Z, col.W);
    }

    /// <name>GetR</name>
    /// <returns>byte</returns>
    /// <summary>An extension method that extracts the red channel of the specified color in the range [0, 255].</summary>
    /// <param name="color">An optional parameter representing the color of which to extract the channel.</param>
    public static byte GetR(this uint color) {
        return BitConverter.IsLittleEndian ? 
            (byte)(color & 0xFF) : 
            (byte)((color >> 24) & 0xFF);
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
        return BitConverter.IsLittleEndian ? 
            (byte)((color >> 16) & 0xFF) : 
            (byte)((color >> 8) & 0xFF);
    }

    /// <name>GetA</name>
    /// <returns>byte</returns>
    /// <summary>An extension method that extracts the alpha channel of the specified color in the range [0, 255].</summary>
    /// <param name="color">An optional parameter representing the color of which to extract the channel.</param>
    public static byte GetA(this uint color) {
        return BitConverter.IsLittleEndian ? 
            (byte)((color >> 24) & 0xFF) : 
            (byte)(color & 0xFF);
    }

    /// <name>SetR</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the red channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the red channel.</param>
    /// <param name="newR">The new value for the R channel of the color, in the range [0, 255].</param>
    public static uint SetR(this ref uint color, byte newR) {
        color = BitConverter.IsLittleEndian ?
            (color & 0xFFFFFF00) | newR :
            (color & 0xFF00FFFF) | ((uint)newR << 24);
        
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
        color = BitConverter.IsLittleEndian ?
            (color & 0xFFFF00FF) | ((uint)newG << 8) :
            (color & 0xFF00FFFF) | ((uint)newG << 16);
        
        return color;
    }

    /// <name>SetG</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the green channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the green  channel.</param>
    /// <param name="newG">The new value for the G channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetG(this ref uint color, float newG) {
        color.SetR((byte)(newG * 255f));
        return color;
    }

    /// <name>SetB</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the blue channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the blue channel.</param>
    /// <param name="newB">The new value for the B channel of the color, in the range [0, 255].</param>
    public static uint SetB(this ref uint color, byte newB) {
        color = BitConverter.IsLittleEndian ?
            (color & 0xFF00FFFF) | ((uint)newB << 16) :
            color = (color & 0xFFFFFF00) | newB;
        
        return color;
    }

    /// <name>SetB</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the blue channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the blue channel.</param>
    /// <param name="newB">The new value for the B channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetB(this ref uint color, float newB) {
        color.SetR((byte)(newB * 255f));
        return color;
    }

    /// <name>SetA</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the alpha channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the alpha channel.</param>
    /// <param name="newA">The new value for the A channel of the color, in the range [0, 255].</param>
    public static uint SetA(this ref uint color, byte newA) {
        color = BitConverter.IsLittleEndian ?
            (color & 0x00FFFFFF) | ((uint)newA << 24) :
            (color & 0xFFFFFF00) | newA;
        
        return color;
    }

    /// <name>SetA</name>
    /// <returns>uint</returns>
    /// <summary>An extension method that sets the alpha channel of the specified color to the new color specified.</summary>
    /// <summary>Modifies the underlying variable.</summary>
    /// <param name="color">An optional parameter representing the color of which to set the alpha channel.</param>
    /// <param name="newA">The new value for the A channel of the color, in the range [0.0, 1.0].</param>
    public static uint SetA(this ref uint color, float newA) {
        color.SetR((byte)(newA * 255f));
        return color;
    }


    /// <name>HslToRgb</name>
    /// <returns>uint</returns>
    /// <summary>Converts a color from HSL color space to either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.</summary>
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

#region common
    //===========================================================
    // Properties
    private static int pixelSize = 1;
    /// <name>PixelSize</name>
    /// <returns>int</returns>
    /// <summary>The pixel size of the window. Locked between [1, 100].</summary>
    public static int PixelSize {
        get => pixelSize;
        set => pixelSize = Math.Clamp(value, 1, 100);
    }

    /// <name>VSync</name>
    /// <returns>bool</returns>
    /// <summary>Whether or not VSync is enabled.</summary>
    public static bool VSync { get; set; } = true;

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
    /// <summary>The scaled width of the window, i.e. real window width / pixel size.</summary>
    public static int Width => scaledWidth;
    private static int scaledWidth;

    /// <name>Height</name>
    /// <returns>int</returns>
    /// <summary>The scaled height of the window, i.e. real window height / pixel size.</summary>
    public static int Height => scaledHeight;
    private static int scaledHeight;

    private static int windowWidth = 0, windowHeight = 0;

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
        return (int)(Rand() % (max - min)) + min;
    }

    /// <name>Rand</name>
    /// <returns>float</returns>
    /// <summary>Generates a random float within the specified range [min, max].</summary>
    /// <param name="min">The minimum possible value of a number that could be generated.</param>
    /// <param name="max">The maximum possible value of a number that could be generated.</param>
    public static float Rand(float min, float max) {
        return (float)Rand() / (float)uint.MaxValue * (max - min) + min;
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
    /// <param name="offsetX">The amount to move 'vec' by on the x-axis..</param>
    /// <param name="offsetY">The amount to move 'vec' by on the y-axis..</param>
    public static Vector2 Translate(this ref Vector2 vec, float offsetX, float offsetY) {
        vec.X += offsetX / PixelSize;
        vec.Y += offsetY / PixelSize;

        return vec;
    }

    /// <name>Translate</name>
    /// <returns>Vector2[]</returns>
    /// <summary>Extension method to translate all Vector2s in an array by the specified amount.</summary>
    /// <summary>Modifies the underlying array.</summary>
    /// <param name="arr">Optional parameter representing the array of Vector2s to modify.</param>
    /// <param name="offsetX">The amount to move the Vector2s in 'arr' by on the x-axis..</param>
    /// <param name="offsetY">The amount to move the Vector2s in 'arr' by on the y-axis..</param>
    public static Vector2[] Translate(this Vector2[] arr, float offsetX, float offsetY) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Translate(offsetX, offsetY);
        }

        return arr;
    }
#endregion common
}