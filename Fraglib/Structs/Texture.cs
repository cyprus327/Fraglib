namespace Fraglib;

public static unsafe partial class FL {
    /// <region>Texture</region>
    public readonly struct Texture {
        /// <name>ctor</name>
        /// <returns>Texture</returns>
        /// <summary>Creates a Texture from a Bitmap image. The alpha channel of the bitmap isn't taken into account for now.</summary>
        /// <param name="bmpImagePath">The path to a Bitmap image to create the texture from.</param>
        public Texture(string bmpImagePath) {
            if (!File.Exists(bmpImagePath)) {
                throw new FileNotFoundException("Specified image not found.");
            }

            using FileStream fs = new(bmpImagePath, FileMode.Open);

            byte[] header = new byte[54];
            fs.Read(header, 0, 54);
            
            if (header[0] != 'B' && header[1] != 'M') {
                throw new FileLoadException("File specified is not a bitmap (.bmp).");
            }

            Width = BitConverter.ToInt32(header, 18);
            Height = BitConverter.ToInt32(header, 22);

            int bpp = BitConverter.ToInt16(header, 28);
            if (bpp != 32) {
                throw new NotSupportedException($"Currently only 32 bit bitmaps are supported. Your bitmap is {bpp} bit. Here's a simple converter: https://online-converting.com/image/convert2bmp/");
            }

            int bmpSize = Height * Width;
            pixels = new uint[bmpSize];

            int pixelDataOffset = BitConverter.ToInt32(header, 10);
            fs.Seek(pixelDataOffset, SeekOrigin.Begin);

            byte[] buffer = new byte[4];
            for (int i = 0; i < bmpSize; i++) {
                fs.Read(buffer, 0, buffer.Length);
                pixels[i] = NewColor(buffer[2], buffer[1], buffer[0], buffer[3]);
            }
        }

        /// <name>Texture</name>
        /// <returns>Texture</returns>
        /// <summary>Clones a Texture.</summary>
        /// <param name="texture">The texture to create a copy of.</param>
        public Texture(Texture texture) {
            Width = texture.Width;
            Height = texture.Height;

            pixels = new uint[Width * Height];
            Array.Copy(texture.GetPixels(), pixels, texture.GetPixels().Length);
        }

        /// <name>Texture</name>
        /// <returns>Texture</returns>
        /// <summary>Creates an empty Texture of specified width and height.</summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="startColor">Optional starting color for the texture.</param>
        public Texture(int width, int height, uint startColor = 0) {
            Width = width;
            Height = height;

            pixels = new uint[width * height];
            Array.Fill(pixels, startColor);
        }

        /// <name>Width</name>
        /// <returns>int</returns>
        /// <summary>The texture's width.</summary>
        public readonly int Width;

        /// <name>Height</name>
        /// <returns>int</returns>
        /// <summary>The texture's height.</summary>
        public readonly int Height;

        private readonly uint[] pixels;

        /// <name>GetPixels</name>
        /// <returns>uint[]</returns>
        /// <summary>Gets the pixels of the texture.</summary>
        public readonly uint[] GetPixels() {
            return pixels;
        }

        /// <name>SetPixel</name>
        /// <returns>void</returns>
        /// <summary>Sets a pixel in the texture at specified coordinates to specified color.</summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <param name="color">The color to set the pixel.</param>
        public void SetPixel(int x, int y, uint color) {
            if (x < 0 || x >= Width || y < 0 || y >= Height) {
                return;
            }

            pixels[y * Width + x] = color;
        }

        /// <name>GetPixel</name>
        /// <returns>uint</returns>
        /// <summary>Gets a pixel in the texture at specified coordinates.</summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        public uint GetPixel(int x, int y) {
            int ind = y * Width + x;

            if (ind < 0 || ind >= pixels.Length) {
                return Black;
            }

            return pixels[ind];
        }

        /// <name>Clear</name>
        /// <returns>void</returns>
        /// <summary>Sets all pixels in the texture to the value specified.</summary>
        /// <param name="color">The color to set all the pixels in the texture to.</param>
        public void Clear(uint color = Black) {
            Array.Fill(pixels, color);
        }

        /// <name>ScaleTo</name>
        /// <returns>Texture</returns>
        /// <summary>Returns the parent texture scaled by the factors scaleX and scaleY.</summary>
        /// <param name="scaleX">The amount to scale the texture by horizontally.</param>
        /// <param name="scaleY">The amount to scale the texture by vertically.</param>
        public Texture ScaleTo(float scaleX, float scaleY) {
            return ScaleTo((int)(scaleX * Width), (int)(scaleY * Height));
        }

        /// <name>ScaleTo</name>
        /// <returns>Texture</returns>
        /// <summary>Returns the parent texture scaled to the resolution scaledX x scaledY.</summary>
        /// <param name="scaledX">The width to scale the texture to in pixels.</param>
        /// <param name="scaledY">The height to scale the texture to in pixels.</param>
        public Texture ScaleTo(int scaledX, int scaledY) {
            float scaleX = (float)scaledX / Width;
            float scaleY = (float)scaledY / Height;

            Texture scaledTexture = new(scaledX, scaledY);

            for (int y = 0; y < scaledY; y++) {
                for (int x = 0; x < scaledX; x++) {
                    uint pixel = GetPixel((int)(x / scaleX), (int)(y / scaleY));
                    scaledTexture.SetPixel(x, y, pixel);
                }
            }

            return scaledTexture;
        }

        /// <name>CropTo</name>
        /// <returns>Texture</returns>
        /// <summary>Returns the parent texture cropped to the resolution specified.</summary>
        /// <param name="texStartX">The x coordinate within the texture where cropping starts.</param>
        /// <param name="texStartY">The y coordinate within the texture where cropping starts.</param>
        /// <param name="texWidth">The width of the cropped texture section.</param>
        /// <param name="texHeight">The height of the cropped texture section.</param>
        public Texture CropTo(int texStartX, int texStartY, int texWidth, int texHeight) {
            if (texStartX < 0 || texStartY < 0 || texStartX >= Width || texStartY >= Height) {
                throw new ArgumentOutOfRangeException("Cropping region is out of bounds.");
            }

            texWidth = Math.Min(texWidth, Width - texStartX);
            texHeight = Math.Min(texHeight, Height - texStartY);

            Texture croppedTexture = new(texWidth, texHeight);

            for (int y = 0; y < texHeight; y++) {
                for (int x = 0; x < texWidth; x++) {
                    uint pixel = GetPixel(texStartX + x, texStartY + y);
                    croppedTexture.SetPixel(x, y, pixel);
                }
            }

            return croppedTexture;
        }
    }
}