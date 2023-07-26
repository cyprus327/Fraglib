using System.Numerics;

namespace Fraglib;

public static class FL {
    private static Engine? e = null;

#region setup
    public static uint PixelSize { get; set; } = 1;
    public static bool VSync { get; set; } = true;

    public static void Init(int width, int height, string title, Action? program = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        if (PixelSize < 1 || PixelSize >= height || PixelSize >= width) {
            PixelSize = 1;
        }

        windowWidth = width / (int)PixelSize;
        windowHeight = height / (int)PixelSize;
        program ??= () => {};
        e = new DrawClearEngine(width, height, title, program);
    }

    public static void Init(int width, int height, string title, Func<int, int, Uniforms, uint> perPixel, Action? perFrame = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        if (PixelSize < 1 || PixelSize >= height || PixelSize >= width) {
            PixelSize = 1;
        }

        windowWidth = width / (int)PixelSize;
        windowHeight = height / (int)PixelSize;
        perFrame ??= () => {};
        e = new PerPixelEngine(width, height, title, perPixel, perFrame);
    }

    public static void Run() {
        if (e is null) {
            return;
        }

        e.VSyncEnabled = VSync;
        e.PixelSize = (int)Math.Abs(PixelSize);
        e.Run();
    }
#endregion setup

#region setclear methods
    public static void SetPixel(int x, int y, uint color) {
        if (e is not DrawClearEngine s) {
            return; 
        }
        
        if (x < 0 || x >= e.ScaledWidth || y < 0 || y >= e.ScaledHeight) {
            return;
        }

        s.SetPixel(x * e.PixelSize, y * e.PixelSize, color);
    }

    private static void SetPixel(int x, int y, uint color, DrawClearEngine s) {
        if (x < 0 || x >= s.ScaledWidth || y < 0 || y >= s.ScaledHeight) {
            return;
        }

        s.SetPixel(x * s.PixelSize, y * s.PixelSize, color);
    }

    public static uint GetPixel(int x, int y) {
        if (e is not DrawClearEngine s) {
            return 255; 
        }

        if (x < 0 || x >= e.WindowWidth || y < 0 || y >= e.WindowHeight) {
            return 255;
        }

        return s.GetPixel(x, y);
    }

    public static void Clear(uint color) {
        if (e is not DrawClearEngine s) {
            return; 
        }

        s.Clear(color);
    }

    public static void FillRect(int x, int y, int width, int height, uint color) {
        if (e is not DrawClearEngine s) {
            return;
        }

        s.FillRect(x * e.PixelSize, y * e.PixelSize, width * e.PixelSize, height * e.PixelSize, color);
    }

    public static void DrawCircle(float centerX, float centerY, float radius, uint color) {
        if (e is not DrawClearEngine s) {
            return;
        }

        int x = (int)radius, y = 0;
        int decisionOver2 = 1 - x;

        while (y <= x) {
            SetCirclePixels(centerX, centerY, x, y++, color, s);

            if (decisionOver2 <= 0) {
                decisionOver2 += 2 * y + 1;
            } else {
                x--;
                decisionOver2 += 2 * (y - x) + 1;
            }

            SetCirclePixels(centerX, centerY, x, y, color, s);
        }
    }

    private static void SetCirclePixels(float cx, float cy, int ox, int oy, uint color, DrawClearEngine s) {
        SetPixel((int)(cx + ox), (int)(cy + oy), color, s);
        SetPixel((int)(cx - ox), (int)(cy + oy), color, s);
        SetPixel((int)(cx + ox), (int)(cy - oy), color, s);
        SetPixel((int)(cx - ox), (int)(cy - oy), color, s);
        SetPixel((int)(cx + oy), (int)(cy + ox), color, s);
        SetPixel((int)(cx - oy), (int)(cy + ox), color, s);
        SetPixel((int)(cx + oy), (int)(cy - ox), color, s);
        SetPixel((int)(cx - oy), (int)(cy - ox), color, s);
    }

    public static void FillCircle(float centerX, float centerY, float radius, uint color) {
        if (e is not DrawClearEngine s) {
            return;
        }

        for (int x = (int)(centerX - radius); x <= centerX + radius; x++) {
            for (int y = (int)(centerY - radius); y <= centerY + radius; y++) {
                if (Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(radius, 2)) {
                    SetPixel(x, y, color, s);
                }
            }
        }
    }

    public static void DrawLine(int x0, int y0, int x1, int y1, uint color) {
        if (e is not DrawClearEngine s) {
            return;
        }

        int dx = Math.Abs(x1 - x0);
        int dy = Math.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int er = dx - dy;

        while (true) {
            SetPixel(x0, y0, color, s);

            if (x0 == x1 && y0 == y1)
                break;

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

    public static void DrawPolygon(uint color, params Vector2[] vertices) {
        if (vertices is null || vertices.Length < 3) {
            return;
        }

        int vertexCount = vertices.Length;
        for (int i = 0; i < vertexCount; i++) {
            int next = (i + 1) % vertexCount;
            DrawLine((int)vertices[i].X, (int)vertices[i].Y, (int)vertices[next].X, (int)vertices[next].Y, color);
        }
    }

    public static void FillPolygon(uint color, params Vector2[] vertices) {
        if (e is not DrawClearEngine s) {
            return;
        }

        if (vertices is null || vertices.Length < 3) {
            return;
        }

        int minY = int.MaxValue;
        int maxY = int.MinValue;
        foreach (var vertex in vertices) {
            if (vertex.Y < minY) {
                minY = (int)vertex.Y;
            }
            if (vertex.Y > maxY) {
                maxY = (int)vertex.Y;
            }
        }

        List<List<int>> edgeTable = new(maxY - minY + 1);
        for (int i = 0; i < maxY - minY + 1; i++) {
            edgeTable.Add(new List<int>());
        }

        int vertexCount = vertices.Length;
        for (int i = 0; i < vertexCount; i++) {
            int nextIndex = (i + 1) % vertexCount;
            Vector2 currentVertex = vertices[i];
            Vector2 nextVertex = vertices[nextIndex];

            if (currentVertex.Y == nextVertex.Y) {
                continue;
            }

            int yStart = (int)Math.Min(currentVertex.Y, nextVertex.Y) - minY;
            int yEnd = (int)Math.Max(currentVertex.Y, nextVertex.Y) - minY;
            int xStart = currentVertex.Y < nextVertex.Y ? (int)currentVertex.X : (int)nextVertex.X;
            int xEnd = currentVertex.Y < nextVertex.Y ? (int)nextVertex.X : (int)currentVertex.X;

            int xStep = (xEnd - xStart) / (yEnd - yStart);
            int xCurrent = xStart;

            for (int y = yStart; y < yEnd; y++) {
                edgeTable[y].Add(xCurrent);
                xCurrent += xStep;
            }
        }

        List<int> activeEdgeTable = new();
        for (int y = 0; y < edgeTable.Count; y++) {
            foreach (int x in edgeTable[y]) {
                activeEdgeTable.Add(x);
            }

            activeEdgeTable.Sort();

            for (int i = 0; i < activeEdgeTable.Count; i += 2) {
                int xStart = activeEdgeTable[i];
                int xEnd = activeEdgeTable[i + 1];

                for (int x = xStart; x <= xEnd; x++) {
                    SetPixel(x, y + minY, color, s);
                }
            }

            activeEdgeTable.Clear();
        }
    }
#endregion setclear methods

#region colors
    public static uint Black => BitConverter.IsLittleEndian ? 4278190080 : 255;
    public static uint Gray => BitConverter.IsLittleEndian ? 4286611584 : 2155905279;
    public static uint White => 4294967295;
    public static uint Red => 4278190335;
    public static uint Green => BitConverter.IsLittleEndian ? 4278255360 : 16711935;
    public static uint Blue => BitConverter.IsLittleEndian ? 4294901760 : 65535;
    public static uint Yellow => BitConverter.IsLittleEndian ? 4278255615 : 4294902015;
    public static uint Orange => BitConverter.IsLittleEndian ? 4278232575 : 4289003775;
    public static uint Cyan => BitConverter.IsLittleEndian ? 4294967040 : 16777215;
    public static uint Magenta => BitConverter.IsLittleEndian ? 4294902015 : 4278255615;
    public static uint Turquoise => BitConverter.IsLittleEndian ? 4291878976 : 1088475391;
    public static uint Lavender => BitConverter.IsLittleEndian ? 4294633190 : 3873897215;
    public static uint Crimson => BitConverter.IsLittleEndian ? 4282127580 : 3692313855;
    public static uint Rainbow(float timeScale = 1f) => HslToRgb((ElapsedTime * 60f * Math.Abs(timeScale)) % 360f, 1f, 0.5f);

    public static uint NewColor(float r, float g, float b, float a = 1f) {
        return NewColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
    }

    public static uint NewColor(byte r, byte g, byte b, byte a = 255) {
        return BitConverter.IsLittleEndian ?
            ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r :
            ((uint)r << 24) | ((uint)g << 16) | ((uint)b << 8) | a;
    }

    public static byte GetR(uint color) {
        return BitConverter.IsLittleEndian ?
            (byte)color : 
            (byte)(color >> 24);
    }

    public static byte GetB(uint color) {
        return BitConverter.IsLittleEndian ?
            (byte)(color >> 16) : 
            (byte)(color >> 8);
    }

    public static byte GetG(uint color) {
        return BitConverter.IsLittleEndian ?
            (byte)(color >> 8) : 
            (byte)(color >> 16);
    }

    public static byte GetA(uint color) {
        return BitConverter.IsLittleEndian ?
            (byte)(color >> 24) : 
            (byte)color;
    }

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
    public static float ElapsedTime => e?.ElapsedTime ?? 0f;
    public static float DeltaTime => e?.DeltaTime ?? 0f;

    public static int Width => windowWidth;
    private static int windowWidth;

    public static int Height => windowHeight;
    private static int windowHeight;

    //===========================================================
    // Lehmer Rand
    private static uint randState = 0;
    public static uint Rand() {
        randState += 0xE120FC15;
        ulong temp;
        temp = (ulong)randState * 0x4A39B70D;
        uint m1 = (uint)((temp >> 32) ^ temp);
        temp = (ulong)m1 * 0x12FAD5C9;
        uint m2 = (uint)((temp >> 32) ^ temp);
        return m2;
    }
    public static int Rand(int min, int max) {
        return (int)(Rand() % (max - min)) + min;
    }
    public static double Rand(double min, double max) {
        return (double)Rand() / (double)uint.MaxValue * (max - min) + min;
    }

    //===========================================================
    // Input
    [System.Runtime.InteropServices.DllImport("user32.dll")] static extern short GetAsyncKeyState(int key);
    private static readonly Dictionary<char, bool> _previousKeyStates = new Dictionary<char, bool>();
    public static bool GetKeyDown(char key) {
        return (GetAsyncKeyState(key) & 0x8000) != 0;
    }
    public static bool GetKeyUp(char key) {
        bool isKeyDown = GetKeyDown(key);
        bool wasKeyDown = _previousKeyStates.ContainsKey(key) && _previousKeyStates[key];

        _previousKeyStates[key] = isKeyDown;

        return wasKeyDown && !isKeyDown;
    }
    public static bool LMBDown() {
        return (GetAsyncKeyState(0x01) & 0x8000) != 0;
    }
    public static bool RMBDown() {
        return (GetAsyncKeyState(0x02) & 0x8000) != 0;
    }
    public static bool LMBUp() {
        return GetKeyUp((char)0x01);
    }
    public static bool RMBUp() {
        return GetKeyUp((char)0x02);
    }

    //===========================================================
    // Math
    public static void Rotate(this ref Vector2 vec, Vector2 center, float angle) {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        float newX = center.X + (vec.X - center.X) * cos - (vec.Y - center.Y) * sin;
        float newY = center.Y + (vec.X - center.X) * sin + (vec.Y - center.Y) * cos;

        vec.X = newX;
        vec.Y = newY;
    }
    public static void Rotate(this Vector2[] arr, Vector2 center, float angle) {
        //Array.ForEach(arr, v => v.Rotate(center, angle));
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Rotate(center, angle);
        }
    }

    public static void Scale(this ref Vector2 vec, Vector2 center, float factor) {
        float newX = center.X + (vec.X - center.X) * factor;
        float newY = center.Y + (vec.Y - center.Y) * factor;

        vec.X = newX;
        vec.Y = newY;
    }
    public static void Scale(this Vector2[] arr, Vector2 center, float factor) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Scale(center, factor);
        }
    }

    public static void AverageWith(this ref Vector2 vec, Vector2 other) {
        vec.X = (vec.X + other.X) / 2f;
        vec.Y = (vec.Y + other.Y) / 2f;
    }
    public static void AverageWith(this Vector2[] arr, Vector2 other) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].AverageWith(other);
        }
    }
#endregion common
}