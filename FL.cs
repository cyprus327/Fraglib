using System.Numerics;

namespace Fraglib;

public static class FL {
    private static Engine? e = null;

#region setup
    private static int pixelSize = 1;
    public static int PixelSize {
        get => pixelSize;
        set => pixelSize = Math.Clamp(value, 1, 100);
    }

    public static bool VSync { get; set; } = true;

    private static bool isDrawClear = false;

    public static void Init(int width, int height, string title, Action? program = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        windowWidth = width / PixelSize;
        windowHeight = height / PixelSize;
        program ??= () => {};
        e = new DrawClearEngine(width, height, title, program);
        isDrawClear = true;
    }

    public static void Init(int width, int height, string title, Func<int, int, Uniforms, uint> perPixel, Action? perFrame = null) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        windowWidth = width / PixelSize;
        windowHeight = height / PixelSize;
        perFrame ??= () => {};
        e = new PerPixelEngine(width, height, title, perPixel, perFrame);
    }

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
    public static void SetPixel(int x, int y, uint color) {
        if (!isDrawClear || x < 0 || x >= e!.ScaledWidth || y < 0 || y >= e.ScaledHeight) {
            return;
        }

        if (PixelSize == 1) {
            e.Screen[y * e.WindowWidth + x] = color;
            return;
        }

        x *= PixelSize;
        y *= PixelSize;
        int xMax = Math.Min(x + PixelSize, e.WindowWidth);
        int yMax = Math.Min(y + PixelSize, e.WindowHeight);
        unsafe {
            fixed (uint* ptr = e.Screen) {
                for (int py = y; py < yMax; py++) {
                    int yo = py * e.WindowWidth;
                    for (int px = x; px < xMax; px++) {
                        ptr[yo + px] = color;
                    }
                }
            }
        }
    }

    private static unsafe void SetPixel(int x, int y, uint color, uint* ptr) {
        if (!isDrawClear || x < 0 || x >= e!.ScaledWidth || y < 0 || y >= e.ScaledHeight) {
            return;
        }

        if (PixelSize == 1) {
            ptr[y * e.WindowWidth + x] = color;
            return;
        }

        x *= PixelSize;
        y *= PixelSize;
        int xMax = Math.Min(x + PixelSize, e.WindowWidth);
        int yMax = Math.Min(y + PixelSize, e.WindowHeight);
        for (int py = y; py < yMax; py++) {
            int yo = py * e.WindowWidth;
            for (int px = x; px < xMax; px++) {
                ptr[yo + px] = color;
            }
        }
    }

    public static uint GetPixel(int x, int y) {
        if (!isDrawClear) {
            return 0; 
        }

        if (x < 0 || x >= e!.WindowWidth || y < 0 || y >= e.WindowHeight) {
            return 0;
        }

        return e.Screen[y * e.WindowWidth + x];
    }

    public static void Clear() {
        Clear(Black);
    }

    public static void Clear(uint color) {
        if (!isDrawClear) {
            return; 
        }

        Array.Fill(e!.Screen, color);
    }

    public static void FillRect(int x, int y, int width, int height, uint color) {
        if (!isDrawClear || width <= 0 || height <= 0) {
            return;
        }

        x = Math.Max(x, 0);
        y = Math.Max(y, 0);
        width = Math.Min(x + width, e!.WindowWidth);
        height = Math.Min(y + height, e.WindowHeight);

        x *= PixelSize;
        y *= PixelSize;
        width *= PixelSize;
        height *= PixelSize;
        unsafe {
            fixed (uint* ptr = e.Screen) {
                for (int r = y; r < height; r++) {
                    int ro = r * e.WindowWidth;
                    for (int c = x; c < width; c++) {
                        ptr[ro + c] = color;
                    }
                }
            }
        }
    }

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

    public static void DrawVerticalLine(int x, int y0, int y1, uint color) {
        if (!isDrawClear || x < 0 || x >= e!.ScaledWidth || y0 < 0 || y1 >= e.ScaledHeight || y0 > y1) {
            return;
        }

        // NOTE: don't replace with SetPixel, ~110fps faster to do it like this
        if (PixelSize == 1) {
            unsafe { 
                fixed (uint* ptr = e.Screen) {
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

        int xBounds = Math.Min(scaledX + PixelSize, e.WindowWidth);
        int yBounds = Math.Min(scaledY1 + PixelSize, e.WindowHeight);

        unsafe {
            fixed (uint* ptr = e.Screen) {
                for (int sy = scaledY0; sy < yBounds; sy++) {
                    int yo = sy * e.WindowWidth;
                    for (int sx = scaledX; sx < xBounds; sx++) {
                        ptr[yo + sx] = color;
                    }
                }
            }
        }
    }

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

    public static void SaveScreen(out int state) {
        if (e is null) {
            state = -1;
            return;
        }

        int index = _states.Count;
        _states.Add((uint[])e.Screen.Clone());
        state = index;
    }

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

    public static void ClearStates() {
        _states.Clear();
    }
#endregion states

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

    public static uint NewColor(byte r, byte g, byte b, byte a = 255) {
        return BitConverter.IsLittleEndian ?
            ((uint)a << 24) | ((uint)b << 16) | ((uint)g << 8) | r :
            ((uint)r << 24) | ((uint)g << 16) | ((uint)b << 8) | a;
    }
    public static uint NewColor(float r, float g, float b, float a = 1f) {
        return NewColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255), (byte)(a * 255));
    }
    public static uint NewColor(Vector3 col, float a = 1f) {
        return NewColor(col.X, col.Y, col.Z, a);
    }
    public static uint NewColor(Vector4 col) {
        return NewColor(col.X, col.Y, col.Z, col.W);
    }

    public static byte GetR(this uint color) {
        return BitConverter.IsLittleEndian ? 
            (byte)(color & 0xFF) : 
            (byte)((color >> 24) & 0xFF);
    }

    public static byte GetG(this uint color) {
        return (byte)((color >> 8) & 0xFF);
    }

    public static byte GetB(this uint color) {
        return BitConverter.IsLittleEndian ? 
            (byte)((color >> 16) & 0xFF) : 
            (byte)((color >> 8) & 0xFF);
    }

    public static byte GetA(this uint color) {
        return BitConverter.IsLittleEndian ? 
            (byte)((color >> 24) & 0xFF) : 
            (byte)(color & 0xFF);
    }

    public static uint SetR(this ref uint color, byte newR) {
        color = BitConverter.IsLittleEndian ?
            (color & 0xFFFFFF00) | newR :
            (color & 0xFF00FFFF) | ((uint)newR << 24);
        
        return color;
    }
    public static uint SetR(this ref uint color, float newR) {
        color.SetR((byte)(newR * 255f));
        return color;
    }

    public static uint SetG(this ref uint color, byte newG) {
        color = BitConverter.IsLittleEndian ?
            (color & 0xFFFF00FF) | ((uint)newG << 8) :
            (color & 0xFF00FFFF) | ((uint)newG << 16);
        
        return color;
    }
    public static uint SetG(this ref uint color, float newG) {
        color.SetR((byte)(newG * 255f));
        return color;
    }

    public static uint SetB(this ref uint color, byte newB) {
        color = BitConverter.IsLittleEndian ?
            (color & 0xFF00FFFF) | ((uint)newB << 16) :
            color = (color & 0xFFFFFF00) | newB;
        
        return color;
    }
    public static uint SetB(this ref uint color, float newB) {
        color.SetR((byte)(newB * 255f));
        return color;
    }

    public static uint SetA(this ref uint color, byte newA) {
        color = BitConverter.IsLittleEndian ?
            (color & 0x00FFFFFF) | ((uint)newA << 24) :
            (color & 0xFFFFFF00) | newA;
        
        return color;
    }
    public static uint SetA(this ref uint color, float newA) {
        color.SetR((byte)(newA * 255f));
        return color;
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
    public static float DegToRad(this float deg) {
        return MathF.PI / 180f * deg;
    }

    public static float RadToDeg(this float rad) {
        return 180f / MathF.PI * rad;
    }

    public static Vector2 Rotate(this ref Vector2 vec, Vector2 center, float angle) {
        float cos = MathF.Cos(angle);
        float sin = MathF.Sin(angle);

        float newX = center.X + (vec.X - center.X) * cos - (vec.Y - center.Y) * sin;
        float newY = center.Y + (vec.X - center.X) * sin + (vec.Y - center.Y) * cos;

        vec.X = newX;
        vec.Y = newY;

        return vec;
    }
    public static Vector2[] Rotate(this Vector2[] arr, Vector2 center, float angle) {
        //Array.ForEach(arr, v => v.Rotate(center, angle));
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Rotate(center, angle);
        }

        return arr;
    }

    public static Vector2 Scale(this ref Vector2 vec, Vector2 center, float factor) {
        float newX = center.X + (vec.X - center.X) * factor;
        float newY = center.Y + (vec.Y - center.Y) * factor;

        vec.X = newX;
        vec.Y = newY;

        return vec;
    }
    public static Vector2[] Scale(this Vector2[] arr, Vector2 center, float factor) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Scale(center, factor);
        }

        return arr;
    }

    public static Vector2 AverageWith(this ref Vector2 vec, Vector2 other) {
        vec.X = (vec.X + other.X) / 2f;
        vec.Y = (vec.Y + other.Y) / 2f;
    
        return vec;
    }
    public static Vector2[] AverageWith(this Vector2[] arr, Vector2 other) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].AverageWith(other);
        }

        return arr;
    }

    public static Vector2 Translate(this ref Vector2 vec, float offsetX, float offsetY) {
        vec.X += offsetX / PixelSize;
        vec.Y += offsetY / PixelSize;

        return vec;
    }
    public static Vector2[] Translate(this Vector2[] arr, float offsetX, float offsetY) {
        for (int i = 0; i < arr.Length; i++) {
            arr[i].Translate(offsetX, offsetY);
        }

        return arr;
    }
#endregion common
}