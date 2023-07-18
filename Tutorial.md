# Fraglib Basics

## Setup

First things first, add the latest version of Fraglib to your project.

```shell
dotnet add package Fraglib --version *
```

## First Window

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

When you run this, you should see a black window open with your specified resolution and title, something like the image below.

![Window VSync Off](https://github.com/cyprus327/Fraglib/assets/76965606/14a85c41-92c4-45be-9631-245603a3e81c)

If you want to change settings, such as PixelSize, change them before calling FL.Init as shown below.
```csharp
// this is fine
FL.PixelSize = 10;
FL.Init(1024, 768, "Window With Big Pixels");
FL.Run();

// this doesn't work
FL.Init(1024, 768, "Window With Normal Pixels");
FL.PixelSize = 10;
FL.Run();
```

We can also edit things about the window before running it, for example making it a different color.
```csharp
FL.Init(1024, 768, "Turquoise Window");
FL.Clear(FL.Turquoise);
FL.Run();
```

You should now see a window with the color of your choice!

![Turquoise Window](https://github.com/cyprus327/Fraglib/assets/76965606/c73ddc1e-5a21-410d-a283-741e0b6d9c38)

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

## SetClear Tutorial

There are two main methods of drawing things in computer graphics, [immediate](https://en.wikipedia.org/wiki/Immediate_mode_(computer_graphics)) and [retained](https://en.wikipedia.org/wiki/Retained_mode).
Fraglib uses a mixture of both of these methods, allowing you to be much more in control.

Here's an example that demonstrates what this means.

Start with the standard SetClear mode setup.
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
using Fraglib;

internal sealed class Tutorial {    
    private static void Main() {
        FL.Init(1024, 768, "Window", () => {
            int pos = FL.Width / 2;
            for (int i = 0; i < FL.Height; i++) {
                FL.SetPixel(pos, i, FL.Blue);
            }
        });
        FL.Run();
    }
}
```

Here's the output of both above programs:
![Blue line program](https://github.com/cyprus327/Fraglib/assets/76965606/31952d37-dde4-469b-9b06-fa449e5a045f)

Let's make something at least a little bit more exciting by adding some motion!
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

![Moving blue bar](https://github.com/cyprus327/Fraglib/assets/76965606/b8c3a5c0-a5b0-40ba-93d8-f4fe68a1ddf4)

That's not what we want right now, but that shows some of the control I mentioned earlier. Let's make this look how we want by clearing the window before drawing again.
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window", Program);
        FL.Run();
    }

    private static void Program() {
        // clear the window
        FL.Clear(FL.Black);

        float st = (float)Math.Sin(FL.ElapsedTime) * 0.5f + 0.5f;
        for (int i = 0; i < FL.Height; i++) {
            FL.SetPixel((int)(st * FL.Width), i, FL.Blue);
        }
    }
}
```

![Cleared blue moving bar](https://github.com/cyprus327/Fraglib/assets/76965606/35a728b2-16cc-434d-b52e-ed3fc57d13de)

Much better!

For the final part of this section, let's make a rainbow ball that bounces around the screen. This might sound a lot more complicated than a blue bar at first, but Fraglib makes it a breeze.

Step 1) Initialize the variables for the ball
```csharp
float ballX = FL.Width / 2f;
float ballY = FL.Height / 2f;
float ballRadius = 50f;
float ballSpeedX = 700f;
float ballSpeedY = 700f;
```

Step 2) Create the program for moving the ball
```csharp
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
```

Done!

![Rainbow Ball](https://github.com/cyprus327/Fraglib/assets/76965606/c192aa0f-c844-43fb-906e-eb7992d9bde0)

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

Before moving on, try experimenting with some things. For example, see if you can get the ball to look like the screenshot below

![Scaled rainbow ball](https://github.com/cyprus327/Fraglib/assets/76965606/f9a68f0a-278e-4636-ab60-696a35ffd817)

Here's a hint:
```csharp
FL.PixelSize = 8;
```
And another hint: Scale your variables!

## PerPixel Tutorial

In if you've skipped the previous tutorials you'll probably understand fine, but there is a lot said in the SetClear tutorial that won't be repeated here, so if at any point there's something not specific to PerPixel mode that's unclear, it's probably explained in the [SetClear Tutorial](https://github.com/cyprus327/Fraglib/blob/main/Tutorial.md#setclear-tutorial).

PerPixel mode is meant to be as similar as possible to writing a fragment shader, which means essentially anything you can make in a fragment shader you can make in this mode.

The only difference, setup-wise, when using PerPixel mode as opposed to SetClear mode is that your Program method is a PerPixel method, so instead of the last parameter in Init being an Action, it's a Func<int, int, Uniforms, uint>. Below is an example of what this looks like.
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Window", PerPixel);
        FL.Run();
    }

    private static uint PerPixel(int x, int y, Uniforms u) {
        float uvx = (float)x / u.Width, uvy = (float)y / u.Height;
        byte r = (byte)(uvx * 255),
             g = (byte)(uvy * 255),
             b = (byte)((Math.Sin(u.Time) * 0.5 + 0.5) * 255);
        return FL.NewColor(r, g, b);
    }
}
```
![Vertex colors gif](https://github.com/cyprus327/Fraglib/assets/76965606/3088f216-b363-4df7-9611-6ab3e5a0d66d)

You may have noticed that in this code I use u.Width and u.Height instead of FL.Width and FL.Height. Using FL.Width/Height would work perfectly here, however there is a slight performance gain when using u.Width/Height instead. The same thing goes for u.Time instead of FL.ElapsedTime, except there is a quite noticeable performance difference here.

Let's end the PerPixel segment with a cooler looking shader, how about an RGB spinning pinwheel?

Step 1) Building off the last example, get the uv coordinates.
```csharp
float uvx = (float)x / u.Width, uvy = (float)y / u.Height;
```

Step 2) Define some constants for the pinwheel.
```csharp
const float radius = 0.4f;
const float centerX = 0.5f;
const float centerY = 0.5f;
```

Step 3) Calculate the distance from the current pixel to the center of the pattern and the angle from the current pixel relative to the center of the pattern.
```csharp
float distance = (float)Math.Sqrt((uvx - centerX) * (uvx - centerX) + (uvy - centerY) * (uvy - centerY));
float angle = (float)Math.Atan2(uvx - centerX, uvy - centerY);
```

Step 4) Color based on angle and time to make the pinwheel spin
```csharp
float r = MathF.Sin(u.Time + angle);
float g = MathF.Sin(u.Time + angle + 2);
float b = MathF.Sin(u.Time + angle + 4);
```

Step 5) Return based on the distance
```csharp
return distance >= radius ? FL.Black : FL.NewColor(r, g, b);
```

Done! Well, almost.

![Streched pinwheel](https://github.com/cyprus327/Fraglib/assets/76965606/7eb03679-d4d7-424c-a842-122c8cb43a94)

Calculating uv coordinates like that only works for square resolutions, which causes our pinwheel to be streched out. To fix this, we can calculate them like below instead.
```csharp
float uvx = (-u.Width + 2.0f * x) / u.Height;
float uvy = (-u.Height + 2.0f * y) / u.Height;
```

![Zoomed out pinwheel](https://github.com/cyprus327/Fraglib/assets/76965606/15c52364-02f1-402f-8b52-d8573aedff29)

But now it's like we've zoomed out, and the pinwheel is no longer in the center :(

It's very simple to fix this, all we need to do is change the constants for the pinwheel around a little bit, and we have our beautiful pinwheel!

![Final pinwheel](https://github.com/cyprus327/Fraglib/assets/76965606/ceb27a01-8e63-4223-8912-ea529ebc4ef5)

And here's the full code
```csharp
using Fraglib;

internal sealed class Tutorial {
    private static void Main() {
        FL.Init(1024, 768, "Pinwheel Example", PerPixel);
        FL.Run();
    }

    private static uint PerPixel(int x, int y, Uniforms u) {
        // normalized uv coordinates
        float uvx = (-u.Width + 2.0f * x) / u.Height;
        float uvy = (-u.Height + 2.0f * y) / u.Height;

        // constants for the pinwheel
        const float radius = 0.7f;
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

With that, the this tutorial comes to an end. I hope you like Fraglib, and make something amazing!
