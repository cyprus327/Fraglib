# Fraglib

An easy to learn framework for using C# like a fragment shader.

## About Fraglib

There are 2 modes in Fraglib, PerPixel and SetClear.
- SetClear mode: You have individual control over every pixel on the window.
- PerPixel mode: You make a perPixel function that runs for every pixel on the window.

## A simple SetClear example
```csharp
using Fraglib;

internal sealed class Example {
    // the width and height of the window
    const int WIDTH = 1024, HEIGHT = 768;

    private static void Main() {
        // initialize and run a window with it's background color set to red
        FL.Init(WIDTH, HEIGHT, "Example Window", Program); // Program explained below
        FL.Clear(FL.NewColor(255, 0, 0));
        FL.Run();
    }

    // the code that will get run after FL.Run is executed
    private static void Program() {
        float t = (float)Math.Sin(FL.GetElapsedTime()) * 0.5f + 0.5f;
        uint color = FL.NewColor(50, (byte)(t * 255), (byte)FL.Rand(100, 200));
        for (int i = HEIGHT / 4; i < HEIGHT - HEIGHT / 4; i++) {
            FL.SetPixel((int)(t * WIDTH), i, color);
        }
    }
}
```

## A simple PerPixel example
```csharp
using Fraglib;

internal sealed class Example {
    // the width and height of the window
    const int WIDTH = 1024, HEIGHT = 768;
    
    private static void Main() {
        // the function that gets run for each pixel on the window
        uint perPixel(int x, int y, Fraglib.PerPixelVars u) { // u == the "uniforms" provided
            float uvx = (float)x / WIDTH, uvy = (float)y / HEIGHT;
            return FL.NewColor((byte)(uvx * 255), (byte)(uvy * 255), (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255));
        }

        // initialize and run the window
        FL.Init(WIDTH, HEIGHT, "Example Window", perPixel);
        FL.Run();
    }
}
```

You may have noticed there is nothing like "FL.Close()" anywhere, which is because that's not needed and is all handled behind the scenes.

## Using Fraglib

Fraglib can be downloaded as a NuGet package.