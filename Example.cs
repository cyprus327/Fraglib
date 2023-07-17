using Fraglib;

internal sealed class Example {
    // variables for the ball
    private static float ballX = FL.Width / 2f;
    private static float ballY = FL.Height / 2f;
    private static float ballRadius = 50f;
    private static float ballSpeedX = 700f;
    private static float ballSpeedY = 700f;
    
    private static void Main() {
        FL.Init(1280, 720, "Rainbow Ball", Program);
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
        uint ballColor = FL.Rainbow();
        for (int x = (int)(ballX - ballRadius); x <= ballX + ballRadius; x++) {
            for (int y = (int)(ballY - ballRadius); y <= ballY + ballRadius; y++) {
                if (Math.Pow(x - ballX, 2) + Math.Pow(y - ballY, 2) <= Math.Pow(ballRadius, 2)) {
                    FL.SetPixel(x, y, ballColor);
                }
            }
        }
    }
}