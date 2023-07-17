# Fraglib

An simple framework for using C# like a fragment shader.

## Some Features of Fraglib

SetClear mode: You have individual control over every pixel on the window.
PerPixel mode: You make a perPixel function that runs for every pixel on the window.
You can set the pixel size to be any number (>= 1) without making any other change to your program, Fraglib will handle everything.
There are many functions that you'd probably wind up making yourself (e.g. a deterministic random) to speed up development.

## Getting Fraglib

Fraglib can be downloaded as a NuGet package
```
dotnet add package Fraglib --version *
```

## Examples:

### Simple SetClear Mode Example (tutorial [here](https://github.com/cyprus327/Fraglib/blob/main/SetClearTutorial.md))
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        // initialize a 1280x720 SetClear mode window
        FL.Init(1280, 720, "Example Window", Program);
        FL.Run();
    }

    // the code that will get run after FL.Run is executed
    private static void Program() {
        // clear the last frame
        FL.Clear(FL.Black);

        // draw a blue bar that scrolls across the window
        float st = (float)Math.Sin(FL.ElapsedTime) * 0.5f + 0.5f;
        for (int i = 0; i < FL.Height; i++) {
            FL.SetPixel((int)(st * FL.Width), i, FL.Blue);
        }
    }
}
```

### Simple PerPixel Mode Example (tutorial [here](https://github.com/cyprus327/Fraglib/blob/main/PerPixelTutorial.md))
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        // initialize and run the window
        FL.Init(1024, 768, "Example Window", PerPixel);
        // if you wanted to use a different pixel size,
        // simply add an additional parameter after PerPixel,
        // e.g.: FL.Init(..., PerPixel, 20); sets the pixel
        // size to 20x20, with no additional changes needed
        FL.Run();
    }

    // the function that gets run for each pixel on the window
    // u == the "uniforms"
    private static uint PerPixel(int x, int y, PerPixelVars u) {
        float uvx = (float)x / FL.Width, uvy = (float)y / FL.Height;
        byte r = (byte)(uvx * 255),
             g = (byte)(uvy * 255),
             b = (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255);
        return FL.NewColor(r, g, b);
    }
}
```
Creates a window that fades between the two images below.

![Vertex Coord Colors, ~0.0z](https://github.com/cyprus327/Fraglib/assets/76965606/cd0a9e46-fb12-4126-b2fa-fd2a1e4b42f1)
![Vertex Coord Colors, ~1.0z](https://github.com/cyprus327/Fraglib/assets/76965606/b86aab81-26df-4a28-8eb7-b4e8896fd2a1)


You may have noticed there is nothing like "FL.Close()", which is on purpose. There's no unbinding, unloading, etc. in Fraglib, everything happens automatically behind the scenes.
