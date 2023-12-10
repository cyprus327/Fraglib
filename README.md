# Fraglib

A powerful and simple-to-learn library that allows you to use C# like a fragment shader.

NOTE: This library is still in very early stages of development, and is in no way final. See [Fraglib on NuGet](https://www.nuget.org/packages/Fraglib) to see what the latest version is.

## Index
- [Features](https://github.com/cyprus327/Fraglib/blob/main/README.md#Features)
- [Installation](https://github.com/cyprus327/Fraglib/blob/main/README.md#Getting-Started)
- [Documentation](https://github.com/cyprus327/Fraglib/blob/main/Documentation.md)
- [**Tutorial**](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md)
- [DrawClear Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#DrawClear-tutorial)
- [PerPixel Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#perpixel-tutorial)
- [**Gallery**](https://github.com/cyprus327/Fraglib/blob/main/Gallery.md)

## Features

- **DrawClear Mode**: Individual control over every pixel on the window + helpful methods such as FillPolygon, DrawTexture, etc..
- **PerPixel Mode**: A perPixel function that runs for every pixel on the window and a perFrame function that runs each frame.
- **Many customizable settings**: Fraglib has lots of settings to get the ideal setup for your project, e.g. you can set the pixel size to any number (>= 1) without making other changes to your program.
- **Time-saving functions**: Fraglib provides a large library containing many functions that are commonly used in development, e.g. a deterministic random or common GLSL functions like Fract.

## Getting Started

Fraglib can be easily installed as a NuGet package.

```shell
dotnet add package Fraglib --version *
```

### PerPixel Mode Example
```csharp
using Fraglib;

// VSync must be off to set TargetFramerate
FL.Settings.VSync = false;
FL.Settings.TargetFramerate = 60;

// x = pixel x, y = pixel y, u = uniforms
FL.Init(1024, 768, "Window", (x, y, u) => { 
    float uvx = (float)x / u.Width, uvy = (float)y / u.Height;
    return FL.NewColor(uvx, uvy, 0f);
}, () => { // no parameters = called once per frame
    FL.DrawString("UV Gradient", 12, 12, 9, FL.White);
});
FL.Run();

/* inlined code above could also be written as below

FL.Init(1024, 768, "Window", PerPixel, PerFrame);
FL.Run();

uint PerPixel(int x, int y, Uniforms u) {
    // ...
}

void PerFrame() {
    // ...
}
*/
```
![Vertex Coord Colors](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/UVGradient.png)

### DrawClear Mode Example
```csharp
using Fraglib;

const int W = 1024, H = 768;

float ballX = W / 2f;
float ballY = H / 2f;
float ballRadius = 50f;
float ballSpeedX = 700f;
float ballSpeedY = 700f;
    
FL.Init(W, H, "Rainbow Ball", () => {
    // clear the last frame
    FL.Clear();

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
    FL.FillCircle((int)ballX, (int)ballY, (int)ballRadius, FL.Rainbow());
});
FL.Run();
```
![Rainbow Ball](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/RainbowBallGIF.gif)

You may have noticed there is nothing like "FL.Close()", which is on purpose. There's no unbinding, unloading, etc., everything is done for you behind the scenes.

If you're intrigued by these snippets, I implore you to check out the [gallery](https://github.com/cyprus327/Fraglib/blob/main/Gallery.md) or [tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md).
