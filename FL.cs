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
        e = new SetClearEngine(width, height, title, program);
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
        if (e is null) {
            return;
        }

        if (x < 0 || x >= e.ScaledWidth || y < 0 || y >= e.ScaledHeight) {
            return;
        }

        if (e is not SetClearEngine s) {
            return; 
        }

        s.SetPixel(x * e.PixelSize, y * e.PixelSize, color);
    }

    public static uint GetPixel(int x, int y) {
        if (e is null) {
            return 255;
        }

        if (x < 0 || x >= e.WindowWidth || y < 0 || y >= e.WindowHeight) {
            return 255;
        }

        if (e is not SetClearEngine s) {
            return 255; 
        }

        return s.GetPixel(x, y);
    }

    public static void Clear(uint color) {
        if (e is null) {
            return;
        }

        if (e is not SetClearEngine s) {
            return; 
        }

        s.Clear(color);
    }

    public static void FillRect(int x, int y, int width, int height, uint color) {
        if (e is null) {
            return;
        }

        if (x < 0 || x >= e.ScaledWidth || y < 0 || y >= e.ScaledHeight 
            || x + width >= e.ScaledWidth || y + height >= e.ScaledHeight) {
            return;
        }
        
        if (e is not SetClearEngine s) {
            return;
        }

        s.FillRect(x * e.PixelSize, y * e.PixelSize, width * e.PixelSize, height * e.PixelSize, color);
    }

    public static void SetCircle(float centerX, float centerY, float radius, uint color) {
        int x = (int)radius, y = 0;
        int decisionOver2 = 1 - x;

        while (y <= x) {
            SetCirclePixel(centerX, centerY, x, y++, color);

            if (decisionOver2 <= 0) {
                decisionOver2 += 2 * y + 1;
            } else {
                x--;
                decisionOver2 += 2 * (y - x) + 1;
            }

            SetCirclePixel(centerX, centerY, x, y, color);
        }
    }

    private static void SetCirclePixel(float cx, float cy, int ox, int oy, uint color) {
        SetPixel((int)(cx + ox), (int)(cy + oy), color);
        SetPixel((int)(cx - ox), (int)(cy + oy), color);
        SetPixel((int)(cx + ox), (int)(cy - oy), color);
        SetPixel((int)(cx - ox), (int)(cy - oy), color);
        SetPixel((int)(cx + oy), (int)(cy + ox), color);
        SetPixel((int)(cx - oy), (int)(cy + ox), color);
        SetPixel((int)(cx + oy), (int)(cy - ox), color);
        SetPixel((int)(cx - oy), (int)(cy - ox), color);
    }

    public static void FillCircle(float centerX, float centerY, float radius, uint color) {
        for (int x = (int)(centerX - radius); x <= centerX + radius; x++) {
            for (int y = (int)(centerY - radius); y <= centerY + radius; y++) {
                if (Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(radius, 2)) {
                    SetPixel(x, y, color);
                }
            }
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
    public static float ElapsedTime => e?.ElapsedTime ?? 0f;
    public static float DeltaTime => e?.DeltaTime ?? 0f;

    public static int Width => windowWidth;
    private static int windowWidth;

    public static int Height => windowHeight;
    private static int windowHeight;

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
#endregion common
}