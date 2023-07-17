# Fraglib

An easy to learn framework for using C# like a fragment shader.

## About Fraglib

There are 2 modes in Fraglib, PerPixel and SetClear. The experience in PerPixel mode is similar to that of something like ShaderToy, where you're given the pixel's coordinates and some uniforms with which you create your program. in SetClear mode you have full control individual pixels are their color. This mode allows you to set any pixels to any color, and requires you to clear the screen for the pixel's color to reset itself.

## A simple PerPixel example
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        int width = 1024, height = 768;
        uint perPixel(int x, int y, Fraglib.PerPixelVars u) {
            float uvx = (float)x / width, uvy = (float)y / height;
            return FL.NewColor((byte)(uvx * 255), (byte)(uvy * 255), (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255));
        }
        FL.Init(width, height, "Example Window", perPixel);
        FL.Run();
    }
}
```

## A simple SetClear example
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        int width = 1024, height = 768;
        FL.Init(width, height, "Example Window", Program);
        FL.Run();
    }

    private static void Program() {
        FL.Clear(FL.NewColor(255, 0, 0));
        FL.SetPixel(0, 0, FL.NewColor(0, 0, 255));
    }
}
```

## Using Fraglib

Fraglib can be downloaded as a NuGet package.
