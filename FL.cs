namespace Fraglib;

public static class FL {
    private static Engine? e = null;

#region setup
    public static void Init(int width, int height, string title, Action program) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        e = new SetClearEngine(width, height, title, program);
    }

    public static void Init(int width, int height, string title, Func<int, int, PerPixelVars, uint> perPixel) {
        if (e is not null) {
            return;
        }

        if (width <= 0 || width >= 5000 || height <= 0 || height >= 4000) {
            return;
        }

        e = new PerPixelEngine(width, height, title, perPixel);
    }

    public static void Run() {
        if (e is null) {
            return;
        }

        e.Run();
    }
#endregion setup

#region setclear methods
    public static void SetPixel(int x, int y, uint color) {
        if (e is null) {
            return;
        }

        if (x < 0 || x >= e.Width || y < 0 || y >= e.Height) {
            return;
        }

        if (e is not SetClearEngine s) {
            return; 
        }

        s.SetPixel(x, y, color);
    }

    public static uint GetPixel(int x, int y) {
        if (e is null) {
            return 255;
        }

        if (x < 0 || x >= e.Width || y < 0 || y >= e.Height) {
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

    public static void Clear(int x, int y, int width, int height, uint color) {
        if (e is null) {
            return;
        }

        if (x < 0 || x >= e.Width || y < 0 || y >= e.Height) {
            return;
        }
        
        if (e is not SetClearEngine s) {
            return;
        }

        for (int i = x; i < x + width; i++) {
            if (i >= e.Width) {
                break;
            }

            s.SetVerticalSection(i, y, y + height, color);
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
#endregion colors

#region common
    public static float GetElapsedTime() {
        if (e is null) {
            return 0f;
        }

        return e.ElapsedTime;
    }

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
#endregion common
}