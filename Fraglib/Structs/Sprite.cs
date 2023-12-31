namespace Fraglib;

public static unsafe partial class FL {
    /// <region>Sprite</region>
    public struct Sprite {
        /// <name>ctor</name>
        /// <returns>Sprite</returns>
        /// <summary>Initializes a new Sprite on the given sprite sheet.</summary>
        public Sprite(Texture spriteSheet, int spriteWidth, int spriteHeight, int spacing = 0) {
            _tex = spriteSheet;
            Width = spriteWidth;
            Height = spriteHeight;
            Spacing = spacing;
        }

        /// <name>Width</name>
        /// <returns>int</returns>
        /// <summary>The width of the sprite in the spritesheet.</summary>
        public int Width { get; init; }
        
        /// <name>Height</name>
        /// <returns>int</returns>
        /// <summary>The height of the sprite in the spritesheet.</summary>
        public int Height { get; init; }
        
        /// <name>Width</name>
        /// <returns>int</returns>
        /// <summary>The spacing between sprites on the spritesheet.</summary>
        public int Spacing { get; init; }

        /// <name>X</name>
        /// <returns>int</returns>
        /// <summary>The current x position of the sprite in the spritesheet.</summary>
        public int X { get; set; } = 0;

        /// <name>Y</name>
        /// <returns>int</returns>
        /// <summary>The current y position of the sprite in the spritesheet.</summary>
        public int Y { get; set; } = 0;
        
        private readonly Texture _tex;

        /// <name>Step</name>
        /// <returns>void</returns>
        /// <summary>Steps the sprite forward once in the spritesheet based on the current position and spacing.</summary>
        public void Step() {
            X += Width + Spacing;

            if (X + Width > _tex.Width) {
                X = 0;
                Y += Height + Spacing;
            }

            if (Y + Height > _tex.Height) {
                Y = 0;
            }
        }

        /// <name>StepTo</name>
        /// <returns>void</returns>
        /// <summary>Moves the sprite to the specified incices in the spritesheet, with (0, 0) being the bottom left.</summary>
        /// <param name="xInd">The x index for the sprite to be moved to.</param>
        /// <param name="yInd">The y index for the sprite to be moved to.</param>
        public void StepTo(int xInd, int yInd) {
            X = xInd * (Width + Spacing);
            Y = yInd * (Height + Spacing);

            if (X + Width > _tex.Width) {
                X = 0;
            }

            if (Y + Height > _tex.Height) {
                Y = 0;
            }
        }

        /// <name>GetCurrent</name>
        /// <returns>Texture</returns>
        /// <summary>Gets the current sprite in the spritesheet.</summary>
        public readonly Texture GetCurrent() {
            return _tex.CropTo(X, Y, Width, Height);
        }
    }
}