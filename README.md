# Fraglib

An simple framework for using C# like a fragment shader.

## About Fraglib

There are 2 modes in Fraglib, PerPixel and SetClear.
- SetClear mode: You have individual control over every pixel on the window.
- PerPixel mode: You make a perPixel function that runs for every pixel on the window.

## Using Fraglib

Fraglib can be downloaded as a NuGet package
```
dotnet add package Fraglib --version *
```

## Examples: 

### SetClear example
```csharp
using Fraglib;

internal sealed class Example {
    // the width and height of the window
    const int WIDTH = 1024, HEIGHT = 768;

    private static void Main() {
        // initialize and run a window with it's background color set to crimson
        FL.Init(WIDTH, HEIGHT, "Example Window", Program); // Program explained below
        FL.Clear(FL.Crimson);
        FL.Run();
    }

    // the code that will get run after FL.Run is executed
    private static void Program() {
        float st = (float)Math.Sin(FL.GetElapsedTime()) * 0.5f + 0.5f;
        float ct = (float)Math.Cos(FL.GetElapsedTime()) * 0.5f + 0.5f;
        uint color = FL.NewColor(50, (byte)(st * 255), (byte)FL.Rand(100, 200));

        // draws a rectangle
        FL.Clear(
            x: (int)(st * (WIDTH - 100)), 
            y: (int)(ct * (HEIGHT - 100)), 
            width: 10 + (int)(st * 100), 
            height: 100 + (int)(st * 100), 
            color: color);

        // the code below is the same as the call to FL.Clear, however calling 
        // FL.Clear here is much better (both performance-wise and readability-wise)
        /* 
        int x = (int)(st * (WIDTH - 100)), y = (int)(ct * (HEIGHT - 100));
        int width = 10 + (int)(st * 100), height = 100 + (int)(st * 100);
        for (int xc = x; xc < x + width; xc++) {
            for (int yc = y; yc < y + height; yc++) {
                FL.SetPixel(xc, yc, color);
            }
        }
        */
    }
}
```
(the gif has artifacts not the window)

![Set Clear Example 1](https://github.com/cyprus327/Fraglib/assets/76965606/48562524-507f-4cbb-8581-484d3ce89090)

## PerPixel example
```csharp
using Fraglib;

internal sealed class Example {
    // the width and height of the window
    const int WIDTH = 1024, HEIGHT = 768;
    
    private static void Main() {
        // the function that gets run for each pixel on the window
        uint perPixel(int x, int y, PerPixelVars u) { // u == the "uniforms" provided
            float uvx = (float)x / WIDTH, uvy = (float)y / HEIGHT;
            return FL.NewColor((byte)(uvx * 255), (byte)(uvy * 255), (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255));
        }

        // initialize and run the window
        FL.Init(WIDTH, HEIGHT, "Example Window", perPixel);
        FL.Run();
    }
}
```
Creates a window that fades between the two images below.

![Vertex Coord Colors, ~0.0z](https://github.com/cyprus327/Fraglib/assets/76965606/cd0a9e46-fb12-4126-b2fa-fd2a1e4b42f1)
![Vertex Coord Colors, ~1.0z](https://github.com/cyprus327/Fraglib/assets/76965606/b86aab81-26df-4a28-8eb7-b4e8896fd2a1)

### Using Custom Pixel Size SetClear Example 
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        // initialize with additional parameter, pixelSize
        FL.Init(1280, 720, "Example Window", Program, pixelSize: 20);
        FL.Run();
    }

    private static void Program() {
        // clear the last frame
        FL.Clear(FL.Black);

        float st = (float)Math.Sin(FL.ElapsedTime) * 0.5f + 0.5f;

        // it's important to use FL.ScaledHeight and 
        // FL.ScaledWidth when using a pixel size > 0
        for (int i = 0; i < FL.ScaledHeight; i++) {
            FL.SetPixel((int)(st * FL.ScaledWidth), i, FL.Blue);
        }
    }
}
```
Makes a blue bar scroll back and forth.

### Using Custom Pixel Size PerPixel Example
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        // initialize with additional parameter, pixelSize
        FL.Init(1280, 720, "Example Window", PerPixel, pixelSize: 20);
        FL.Run();
    }

    private static uint PerPixel(int x, int y, PerPixelVars u) {
        float uvx = (float)x / FL.ScaledWidth, uvy = (float)y / FL.ScaledHeight;
        return FL.NewColor((byte)(uvx * 255), (byte)(uvy * 255), (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255));
    }
}
```
Exact same result as the first PerPixel example, just with 20x20 pixels.

You may have noticed there is nothing like "FL.Close()", which is on purpose. There's no unbinding, unloading, etc. in Fraglib, everything happens automatically behind the scenes.