using Fraglib;

internal sealed class Example {
    // variables for the ball
    private static float ballX = FL.Width / 2f;
    private static float ballY = FL.Height / 2f;
    private static float ballRadius = 50f / FL.PixelSize;
    private static float ballSpeedX = 700f / FL.PixelSize;
    private static float ballSpeedY = 700f / FL.PixelSize;
    
    private static void Main() {
        FL.PixelSize = 8;
        FL.Init(1024, 768, "Rainbow Ball", Program);
        FL.Run();
    }

    private static void Program() {
        FL.Clear(FL.Black);

        ballX += ballSpeedX * FL.DeltaTime;
        ballY += ballSpeedY * FL.DeltaTime;

        if (ballX + ballRadius >= FL.Width || ballX - ballRadius <= 0) {
            ballSpeedX *= -1f;
        }
        if (ballY + ballRadius >= FL.Height || ballY - ballRadius <= 0) {
            ballSpeedY *= -1f;
        }

        FL.FillCircle(ballX, ballY, ballRadius, FL.Rainbow());
    }
}