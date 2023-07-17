# Fraglib Basics

## Setup

First things first, add the latest version of Fraglib to your project.

```
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

If you want to change settings, such as VSync, change them before initializing as shown below.
```csharp
FL.VSync = false;
FL.Init(1024, 768, "Window With No VSync");
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

## Basics of SetClear Mode

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

For the final part of this section, let's make a rainbow ball that bounces around the screen. This might sound complicated at first, but Fraglib makes it a breeze.

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
Step 3) Done!

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

Coming soon
