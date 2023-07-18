# Fraglib

Fraglib is a powerful and simple-to-learn framework that allows you to use C# like a fragment shader, or to help render things in general.

## Index
- [Features](https://github.com/cyprus327/Fraglib/blob/main/README.md#Features)
- [Installation](https://github.com/cyprus327/Fraglib/blob/main/README.md#Getting-Started)
- [Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md)
- [SetClear Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#setclear-tutorial)
- [PerPixel Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#perpixel-tutorial)

## Features

- **SetClear Mode**: Individual control over every pixel on the window.
- **PerPixel Mode**: A perPixel function that runs for every pixel on the window.
- **Flexible pixel size**: You can set the pixel size to any number (>= 1) without making other changes to your program. Fraglib handles everything automatically.
- **Time-saving functions**: Fraglib provides many functions that are commonly used in development, such as a deterministic random, to speed up your workflow.

## Getting Started

Fraglib can be easily installed as a NuGet package.

```shell
dotnet add package Fraglib --version *
```

## Examples:

### PerPixel Mode Example
```csharp
using Fraglib;

internal sealed class Example {
    private static void Main() {
        FL.Init(1024, 768, "Example Window", PerPixel);
        FL.Run();
    }

    // the function that gets run for each pixel on the window
    private static uint PerPixel(int x, int y, Uniforms u) {
        float uvx = (float)x / u.Width, uvy = (float)y / u.Height;
        byte r = (byte)(uvx * 255),
             g = (byte)(uvy * 255),
             b = 0;
        return FL.NewColor(r, g, b);
    }
}
```
![Vertex Coord Colors, ~0.0z](https://github.com/cyprus327/Fraglib/assets/76965606/cd0a9e46-fb12-4126-b2fa-fd2a1e4b42f1)

### SetClear Mode Example
```csharp
using Fraglib;

internal sealed class Example {
    // variables for the ball
    private static float ballX = FL.Width / 2f;
    private static float ballY = FL.Height / 2f;
    private static float ballRadius = 50f;
    private static float ballSpeedX = 700f;
    private static float ballSpeedY = 700f;
    
    private static void Main() {
        FL.VSync = true;
        FL.Init(1024, 768, "Rainbow Ball", Program);
        FL.Run();
    }

    private static void Program() {
        // clear the last frame
        FL.Clear(FL.Black);

        // update ball position
        ballX += ballSpeedX * FL.DeltaTime;
        ballY += ballSpeedY * FL.DeltaTime;

        // handle collision with screen edges
        if (ballX + ballRadius >= FL.Width || ballX - ballRadius <= 0) {
            ballSpeedX *= -1f;
        }
        if (ballY + ballRadius >= FL.Height || ballY - ballRadius <= 0) {
            ballSpeedY *= -1f;
        }

        // draw the ball
        FL.FillCircle(ballX, ballY, ballRadius, FL.Rainbow());
    }
}
```
![Rainbow Ball](https://github.com/cyprus327/Fraglib/assets/76965606/c192aa0f-c844-43fb-906e-eb7992d9bde0)

You may have noticed there is nothing like "FL.Close()", which is on purpose. There's no unbinding, unloading, etc. in Fraglib, everything happens automatically behind the scenes.
