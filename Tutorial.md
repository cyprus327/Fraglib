# Fraglib Tutorial

Quick links:
- [DrawClear tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#drawclear-tutorial)
- [PerPixel tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#perpixel-tutorial)
- [Textures](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#textures)

## Setup / First Window

First things first, add the latest version of Fraglib to your project.

```shell
dotnet add package Fraglib --version *
```

Use Fraglib
```csharp
using Fraglib;
```

Initialize and run the window
```csharp
private static void Main() {
    FL.Init(1024, 768, "Window Title");
    FL.Run();
}
```

When you run this, you should see a black window open with your specified resolution and title, something like the image below. Note: VSync is disabled in the screenshot below, but is enabled for the rest of this tutorial. The default value for VSync is enabled.

![Window VSync Off](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/BlackWindow.png)

Certain settings must be changed before FL.Init is called.
```csharp
// this is fine
FL.Settings.VSync = false;
FL.Init(1024, 768, "Window Without VSync");
FL.Run();

// this doesn't work (default for VSync is true)
FL.Init(1024, 768, "Window With VSync");
FL.Settings.VSync = false;
FL.Run();
```

We can also edit things about the window before running it, for example making it a different color.
```csharp
FL.Init(1024, 768, "Turquoise Window");
FL.Clear(FL.Turquoise);
FL.Run();
```

You should now see a window with the color of your choice!

![Turquoise Window](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/TurquoiseWindow.png)

Here's the full code of this section:
```csharp
using Fraglib;

internal static class Tutorial {    
    private static void Main() {
        FL.Init(1024, 768, "Turquoise Window");
        FL.Clear(FL.Turquoise);
        FL.Run();
    }
}
```

## DrawClear Tutorial

There are two main methods of drawing things in computer graphics, [immediate](https://en.wikipedia.org/wiki/Immediate_mode_(computer_graphics)) and [retained](https://en.wikipedia.org/wiki/Retained_mode).
Fraglib uses a mixture of both of these methods, allowing you more control.

Here's an example that demonstrates what this means.

Start with the standard DrawClear mode setup.
```csharp
using Fraglib;

internal static class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window");
        FL.Run();
    }
}
```
Now create a method for your program, and add it as a parameter to Init.
```csharp
using Fraglib;

internal static class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window", Program);
        FL.Run();
    }

    private static void Program() {
        // draw a blue bar in the middle of the window
        int pos = FL.Height / 2;
        for (int i = 0; i < FL.Height; i++) {
            FL.SetPixel(pos, i, FL.Blue);
        }
    }
}
```
You could also inline Program like below, however I personally prefer it to be it's own method.
```csharp
FL.Init(1024, 768, "Window", () => {
    int pos = FL.Width / 2;
    for (int i = 0; i < FL.Height; i++) {
        FL.SetPixel(pos, i, FL.Blue);
    }
});
```

Here's the output of both above programs:
![Blue line program](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/BlueLineWindow.png)

Let's make this at least a little bit more exciting by adding some motion!
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window", Program);
        FL.Run();
    }

    private static void Program() {
        // normalized value between 0 and 1
        float st = (float)Math.Sin(FL.ElapsedTime) * 0.5f + 0.5f;
        for (int i = 0; i < FL.Height; i++) {
            // multiply by width to make the bar move
            // across the full length of the window
            FL.SetPixel((int)(st * FL.Width), i, FL.Blue);
        }
    }
}
```

![Moving blue bar](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/BlueLineNoClearWindow.png)

That's not what we want right now, but that shows some of the control I mentioned earlier. Let's make this look how we want by clearing the window before drawing again.
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window", Program);
        FL.Run();
    }

    private static void Program() {
        // clear the window to black, you can pass
        // any color here to clear it to that color
        FL.Clear();

        float st = (float)Math.Sin(FL.ElapsedTime) * 0.5f + 0.5f;
        for (int i = 0; i < FL.Height; i++) {
            FL.SetPixel((int)(st * FL.Width), i, FL.Blue);
        }
    }
}
```

![Cleared blue moving bar](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/BlueLineClearWindow.png)

Much better!

For the final part of this section, let's make a rainbow ball that bounces around the screen. This might sound a lot more complicated than a blue bar at first, but Fraglib makes it a breeze.

**Step 1)** Initialize the variables for the ball
```csharp
float ballX = FL.Width / 2f;
float ballY = FL.Height / 2f;
float ballRadius = 50f;
float ballSpeedX = 700f;
float ballSpeedY = 700f;
```

**Step 2)** Create the program for moving the ball
```csharp
private static void Program() {
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
    FL.FillCircle(ballX, ballY, ballRadius, FL.Rainbow());
}
```

Done!

![Rainbow Ball](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/RainbowBallGIF.gif)

Here's the final code:
```csharp
using Fraglib;

internal sealed class Tutorial {
    // variables for the ball
    private static float ballX = FL.Width / 2f;
    private static float ballY = FL.Height / 2f;
    private static float ballRadius = 50f;
    private static float ballSpeedX = 700f;
    private static float ballSpeedY = 700f;
    
    private static void Main() {
        FL.Init(1024, 768, "Rainbow Ball", Program);
        FL.Run();
    }

    private static void Program() {
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
        FL.FillCircle(ballX, ballY, ballRadius, FL.Rainbow());
    }
}
```

Before moving on, try experimenting with some things. For example, see if you can get the ball to look like the screenshot below.

![Scaled rainbow ball](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/PixelatedRainbowBall.png)

## PerPixel Tutorial

In if you've skipped the previous tutorials you'll probably understand fine, but there is a lot said in the DrawClear tutorial that won't be repeated here, so if at any point there's something not specific to PerPixel mode that's unclear, it's probably explained in the [DrawClear Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#DrawClear-tutorial).

PerPixel mode is meant to be as similar as possible to writing a fragment shader, which means essentially anything you can make in a fragment shader you can make in this mode.

The only difference, setup-wise, when using PerPixel mode as opposed to DrawClear mode is that your Program method is a PerPixel method, so instead of the last parameter in Init being an Action, it's a Func<int, int, Uniforms, uint>. Below is an example of what this looks like.
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window", PerPixel);
        FL.Run();
    }

    private static uint PerPixel(int x, int y, Uniforms u) {
        float uvx = (float)x / u.Width, uvy = (float)y / u.Height;
        return FL.NewColor(uvx, uvy, 0f);
    }
}
```
![Vertex colors gif](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/UVGradient.png)

You may have noticed that in this code I use u.Width and u.Height instead of FL.Width and FL.Height. Using FL.Width/Height would work perfectly here, however there is a slight performance gain when using u.Width/Height instead. The same thing goes for other all other Uniform variables such as u.Time instead of FL.ElapsedTime.

Lets make a little shader, how about a spinning pinwheel?

**Step 1)** Building off the last example, get the uv coordinates
```csharp
float uvx = (float)x / u.Width, uvy = (float)y / u.Height;
```

**Step 2)** Define some constants for the pinwheel
```csharp
const float radius = 0.4f;
const float centerX = 0.5f;
const float centerY = 0.5f;
```

**Step 3)** Calculate the distance from the current pixel to the center of the pattern and the angle from the current pixel relative to the center of the pattern
```csharp
float distance = (float)Math.Sqrt((uvx - centerX) * (uvx - centerX) + (uvy - centerY) * (uvy - centerY));
float angle = (float)Math.Atan2(uvx - centerX, uvy - centerY);
```

**Step 4)** Color based on angle and time to make the pinwheel spin
```csharp
float r = MathF.Sin(u.Time + angle);
float g = MathF.Sin(u.Time + angle + 2);
float b = MathF.Sin(u.Time + angle + 4);
```

**Step 5)** Return based on the distance
```csharp
return distance >= radius ? FL.Black : FL.NewColor(r, g, b);
```

Done! Well, almost.

![Streched pinwheel](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/StrechedPinwheel.png)

Calculating uv coordinates like that only works for square resolutions, which causes our pinwheel to be streched out. To fix this, we can calculate them like below instead.
```csharp
float uvx = (-u.Width + 2.0f * x) / u.Height;
float uvy = (-u.Height + 2.0f * y) / u.Height;
```

NOTE: here's a better way to do this:
```csharp
float aspectRatio = (float)u.Width / u.Height;
float uvx = (2f * (x + 0.5f) / u.Width - 1f) * aspectRatio;
float uvy = 1f - 2f * (y + 0.5f) / u.Height;
```

![Zoomed out pinwheel](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/FixedPinwheel1.png)

But now it's like we've zoomed out, and the pinwheel is no longer in the center. :(

It's very simple to fix this, all we need to do is change the constants for the pinwheel around a little bit, and we have our beautiful pinwheel!

![Final pinwheel](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/FixedPinwheel2.png)

And here's the full code.
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Pinwheel Example", PerPixel);
        FL.Run();
    }

    private static uint PerPixel(int x, int y, Uniforms u) {
        // normalized uv coordinates
        float uvx = (-u.Width + 2f * x) / u.Height;
        float uvy = (-u.Height + 2f * y) / u.Height;

        // constants for the pinwheel
        const float radius  = 0.7f;
        const float centerX = 0.0f;
        const float centerY = 0.0f;

        // distance from the current pixel to the center of the pattern
        float distance = (float)Math.Sqrt((uvx - centerX) * (uvx - centerX) + (uvy - centerY) * (uvy - centerY));
        
        // if not part of the pinwheel, return black
        if (distance >= radius) {
            return FL.Black;
        }

        // angle of the current pixel relative to the center of the pattern
        float angle = (float)Math.Atan2(uvx - centerX, uvy - centerY);

        // color based on angle and time to add a little spin
        float r = MathF.Sin(u.Time + angle);
        float g = MathF.Sin(u.Time + angle + 2);
        float b = MathF.Sin(u.Time + angle + 4);
        return FL.NewColor(r, g, b);
    }
}
```

A part of PerPixel mode I haven't mentioned yet is the PerFrame function. PerFrame is an Action and functions exactly as you'd think it would, it's called once per frame.

Here's an example of using PerFrame with a Mandelbrot renderer.

```csharp
using System.Numerics;
using Fraglib;

internal sealed class Tutorial {
    private static float zoom = 0.07f;
    private static Vector2 center = new(-1.555466652f, 0f);

    private static void Main() {
        FL.Init(800, 450, "Mandelbrot Renderer", PerPixel, PerFrame);
        FL.Run();
    }

    // PerFrame used to handle user input
    private static void PerFrame() {
        if (FL.GetKeyDown('W')) {
            zoom *= MathF.Pow(2f, FL.DeltaTime);
        } else if (FL.GetKeyDown('S')) {
            zoom *= MathF.Pow(0.5f, FL.DeltaTime);
        }
    }

    private static uint PerPixel(int x, int y, Uniforms u) {
        Vector2 uv = (new Vector2(x, y) - new Vector2(u.Width / 2f, u.Height / 2f)) / u.Height / zoom + center;
        Vector2 z = new(0f);

        const int MAX_ITER = 190;
        int iter = 0;
        while (iter < MAX_ITER && Vector2.Dot(z, z) < 4.0) {
            z = new Vector2(z.X * z.X - z.Y * z.Y, 2f * z.X * z.Y) + uv;
            iter++;
        }

        if (iter == MAX_ITER) {
            return FL.Black;
        }

        float t = (float)iter / MAX_ITER;
        float r = 9f * (1f - t) * t * t * t;
        float g = 15f * (1f - t) * (1f - t) * t * t;
        float b = 8.5f * (1f - t) * (1f - t) * (1f - t) * t;
        return FL.NewColor(r, g, b);
    }
}
```

![Mandelbrot Zoom](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/MandelbrotGIF.gif)

With that, this tutorial comes to an end. If you'd like to see some more things made with Fraglib, check out the [gallery](https://github.com/cyprus327/Fraglib/blob/main/Gallery.md).

## Textures

In Fraglib, the only images that can be used (for now) as Textures are 32 bit Bitmaps. This is because of [how the images are loaded into the Texture](https://github.com/cyprus327/Fraglib/blob/d55a77f1c38dc4bbddfa8205b5c641540e64167f/FL.cs#L678C19-L678C19) and the fact that the approach is tailor made specifically for 32 bit Bitmaps.

To create a new Texture in Fraglib, create a new FL.Texture, as shown below.
```csharp
// load a Texture from image.bmp
FL.Texture texture = new(@"path\to\image.bmp");

// create a clone of 'texture'
FL.Texture clonedTexture = new(texture);

// create a blank Texture with size 256x256
FL.Texture blankTexture = new(256, 256);
```

There are multiple overloads to DrawTexture, all of which support transparency. This does make them significantly slower than DrawTextureFast, however, which doesn't support transparency. 

The Texture struct of course has the methods you would expect, e.g. 'SetPixel(x, y, color)', 'GetPixel(x, y)', 'GetPixels'.

## Cameras
The Camera struct is a very easy way to make a simple 3D application with Fraglib.

To create a new Camera in Fraglib, create a new FL.Camera, as shown below.
```csharp
FL.Camera camera = new(Vector3.Zero, RENDER_WIDTH, RENDER_HEIGHT, yawRad, pitchRad, fovDeg);
```

RENDER_WIDTH and RENDER_HEIGHT are the dimensions of the target for the camera object to render to.
For example, if the camera is meant for use with a texture, RENDER_WIDTH would be the texture's width
and RENDER_HEIGHT the texture's height, if the camera is meant for the entire window,
RENDER_WIDTH would be the window's width and RENDER_HEIGHT the window's height.

Below is an example creating a 10x10x10 point cloud.
```csharp
using Fraglib;

const int WIDTH = 1024, HEIGHT = 768;

FL.Camera cam = new(new(0f, 0f, 0f), WIDTH, HEIGHT);
FL.Init(WIDTH, HEIGHT, "Point Cloud", () => {
    FL.Clear();
    // iterate over the point cloud
    for (int x = 0; x < 50; x++) {
        for (int y = 0; y < 50; y++) {
            for (int z = 0; z < 50; z++) {
                // project the point (x, y, z) to screen coordinates (sc)
                cam.ProjectPointToScreen((x, y, z), out (int x, int y) sc, out bool inCamView);
                // only draw the point if it can be seen by the camera
                if (inCamView) {
                    FL.SetPixel(sc.x, sc.y, FL.NewColor(x / 50f, y / 50f, z / 50f));
                }
            }
        }
    }

    // default input handling so I don't have to write
    // if (FL.GetKeyDown('W')) cam.MoveForward(10f) and so on
    cam.HandleInputDefault(10f);
});
FL.Run();
```

![Point Cloud](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/PointCloud.png)

There are also other helper projection methods, such as ProjectCircleToScreen and ProjectRectToScreen,
below is an example that projects billboard textures to the screen.

```csharp
using System.Numerics;
using Fraglib;

const int WIDTH = 1024, HEIGHT = 768;

FL.Texture colorwheel = new(@"C:\Test\colorwheel.bmp");

// same setup as before
FL.Camera cam = new(Vector3.Zero, WIDTH, HEIGHT);
FL.Init(WIDTH, HEIGHT, "Billboard Textures", () => {
    FL.Clear(FL.Turquoise);
    for (int x = 0; x < 50; x += 10) {
        for (int y = 0; y < 50; y += 10) {
            for (int z = 0; z < 50; z += 10) {
                // project a rectangle for the texture
                cam.ProjectRectToScreen(
                    new(x, y, z),
                    colorwheel.Width / 100, colorwheel.Height / 100,
                    out Vector2 sc,
                    out float sw, out float sh,
                    out bool inCamView
                );
                // draw the texture if it can be seen
                if (inCamView) {
                    FL.DrawTexture((int)sc.X, (int)sc.Y, (int)sw, (int)sh, colorwheel);
                }
            }
        }
    }

    cam.HandleInputDefault(10f);
});
FL.Run();
```

![Texture Billboards](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/TextureBillboards.png)

I haven't yet shown the effect of having different values for a camera object's target width and height,
so that will be shown in the next section.

## Render Target
The SetRenderTarget method is used when changing the target of the drawing methods in Fraglib.
For example, if I wanted to draw a circle onto an FL.Texture object, I would first have to Fraglib's
rendering target to that texture.

Here's what that would look like.
```csharp
// assuming tex is the texture we want to draw onto
FL.SetRenderTarget(tex);
// draw a red circle of radius 50 at (10, 10)
FL.DrawCircle(10, 10, 50, FL.Red);
// go back to drawing on the main window
FL.ResetRenderTarget();
```

A more advanced / in depth example of using this is the splitscreen game below.
Note that you may not understand what's going on here unless you've looked at the previous section
on the Camera struct.
```csharp
using System.Numerics;
using Fraglib;

const int WIDTH = 1024, HEIGHT = 768;

// initialize red player
FL.Texture redTex = new(WIDTH, HEIGHT / 2);
FL.Camera redCam = new(Vector3.Zero, redTex.Width, redTex.Height);

// initialize blue player
FL.Texture blueTex = new(WIDTH, HEIGHT / 2);
FL.Camera blueCam = new(Vector3.Zero, blueTex.Width, blueTex.Height);

FL.Init(WIDTH, HEIGHT, "Splitscreen Game", () => {
    redTex.Clear(FL.LerpColors(FL.Red, FL.Black, 0.9f));
    blueTex.Clear(FL.LerpColors(FL.Blue, FL.Black, 0.9f));
    
    // draw the point cloud world
    bool inCamView;
    for (int x = 0; x < 50; x += 2) {
        for (int y = 0; y < 50; y += 2) {
            for (int z = 0; z < 50; z += 2) {
                uint col = FL.NewColor(x / 50f, y / 50f, z / 50f);

                redCam.ProjectPointToScreen((x, y, z), out var screenCoord, out inCamView);
                if (inCamView) {
                    redTex.SetPixel(screenCoord.x, screenCoord.y, col);
                }

                blueCam.ProjectPointToScreen((x, y, z), out screenCoord, out inCamView);
                if (inCamView) {
                    blueTex.SetPixel(screenCoord.x, screenCoord.y, col);
                }
            }
        }
    }

    // draw the players for each other
    redCam.ProjectCircleToScreen(blueCam.Pos, 1f, out var sc, out float sr, out inCamView);
    if (inCamView) {
        FL.SetRenderTarget(redTex);
        FL.DrawCircle((int)sc.X, (int)sc.Y, (int)sr, FL.Blue);
        FL.ResetRenderTarget();
    }
    blueCam.ProjectCircleToScreen(redCam.Pos, 1f, out sc, out sr, out inCamView);
    if (inCamView) {
        FL.SetRenderTarget(blueTex);
        FL.DrawCircle((int)sc.X, (int)sc.Y, (int)sr, FL.Red);
        FL.ResetRenderTarget();
    }

    // handle input for each player
    if (FL.GetKeyDown(' ')) {
        blueCam.HandleInputDefault(8f);
    } else {
        redCam.HandleInputDefault(8f);
    }

    // draw each player's POV
    FL.DrawTextureFast(0, HEIGHT / 2, redTex);
    FL.DrawTextureFast(0, 0, blueTex);
});
FL.Run();
```

![Splitscreen Game](https://github.com/cyprus327/Fraglib/blob/main/.githubResources/SplitscreenGame.png)

## States

The 'SaveState' and 'LoadState' methods can be extremely helpful in certain circumstances.
For example, if you have a texture or pattern you want to be the background of the window, instead of drawing the texture every frame, you can simply draw it once, save it, and then load it repeatedly.
Below is what this would look like.

```csharp
int background = -1;

FL.Init(800, 600, "Window", () => {
    FL.LoadState(background);
    // ...
});

for (int y = 0; y < FL.Height; y += 5) {
    for (int x = 0; x < FL.Width; x += 5) {
        FL.FillRect(x, y, 4, 4, FL.Rand());
    }
}
FL.SaveState(out background);

FL.Run();
```

Instead of below, where you're drawing the background pattern every frame.

```csharp
FL.Init(800, 600, "Window", () => {
    for (int y = 0; y < FL.Height; y += 5) {
        for (int x = 0; x < FL.Width; x += 5) {
            FL.FillRect(x, y, 4, 4, FL.Rand());
        }
    }
    // ...
});

FL.Run();
```

In case you're confused about the first snippet and why things are in the order they are, the simple explaination is that when you call Init, the program supplied isn't actually run until Run is called, so anything in betweem those two calls will be called before your program, which in this case allows us to save the background pattern.

You may notice I initialize 'background' to -1, this doesn't actually effect anything, it just makes it look better to me. Initializing it to any other integer would work perfectly here as well. Also, if something like this is the only state you're saving you really don't need to save it to a variable, the output from SaveState is just an index so in this case (since this is the first state we've saved) 'background' will be set to 0.