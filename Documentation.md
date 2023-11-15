# Fraglib Documentation
## Setup
### Init (void)
Initializes the window with the specified width, height, and title. 
This method must be called before using any other FL methods. 
- **width**: The width of the window in pixels.
- **height**: The height of the window in pixels.
- **program**: The delegate which will get invoked once per frame until the window is closed.
### Init (void)
Initializes the window with the specified width, height, title, perPixel function, and optional perFrame function. 
This method must be called before using any other FL methods. 
- **width**: The width of the window in pixels.
- **height**: The height of the window in pixels.
### Run (void)
Starts the main loop of the engine. 
Must be called after Init for a window to appear. 
Any settings changed after this (PixelSize, VSync, etc.) won't affect anything. 
## DrawClear Methods
### SetPixel (void)
Sets the pixel at the specified position to the given color. 
- **x**: The x coordinate of the pixel.
- **y**: The y coordinate of the pixel.
- **color**: The color of the pixel.
### GetPixel (uint)
Gets the pixel's color at the specified position. 
- **x**: The x coordinate of the pixel.
- **y**: The y coordinate of the pixel.
### Clear (void)
Clears the window to the specified color. 
- **color**: The color the window will get cleared to, default value of black (0xFF000000).
### DrawRect (void)
Draws a rectangle of specified size and color at the specified coordinates. 
- **x**: The rectangle's x coordinate.
- **y**: The rectangle's y coordinate.
- **width**: The width of the rectangle.
- **height**: The height of the rectangle.
- **color**: The color of the rectangle.
### FillRect (void)
Fills a solid rectangle of specified size and color at the specified coordinates. 
- **x**: The rectangle's x coordinate.
- **y**: The rectangle's y coordinate.
- **width**: The width of the rectangle.
- **height**: The height of the rectangle.
- **color**: The color of the rectangle.
### DrawCircle (void)
Draws the outline of a circle of specified size and color at the specified coordinates. 
- **centerX**: The center coordinate of the circle along the x-axis.
- **centerY**: The center coordinate of the cirlce along the y-axis.
- **radius**: The radius of the circle.
- **color**: The color of the circle.
### FillCircle (void)
Fills a solid circle of specified size and color at the specified coordinates. 
- **centerX**: The center coordinate of the circle along the x-axis.
- **centerY**: The center coordinate of the cirlce along the y-axis.
- **radius**: The radius of the circle.
- **color**: The color of the circle.
### DrawLine (void)
Draws a line of specified color along the specified path. 
- **x0**: The starting x coordinate of the line.
- **y0**: The starting y coordinate of the line.
- **x1**: The ending x coordinate of the line.
- **y1**: The ending y coordinate of the line.
- **color**: The color of the line.
### DrawVerticalLine (void)
Draws a vertical line of specified color along the specified path. 
Should be used over DrawLine if the line is vertical. 
- **x**: The x coordinate of the line.
- **y0**: The starting y coordinate of the line.
- **y1**: The ending y coordinate of the line.
- **color**: The color of the line.
### DrawHorizontalLine (void)
Draws a horizontal line of specified color along the specified path. 
Should be used over DrawLine if the line is horizontal. 
- **x0**: The starting x coordinate of the line.
- **x1**: The ending x coordinate of the line.
- **y**: The y coordinate of the line.
- **color**: The color of the line.
### DrawTriangle (void)
Draws the outline of a triangle of specified color with specified vertices. Should be used over DrawPolygon if the polygon is a triangle. 
- **x0**: The x coordinate of the 1st vertex.
- **y0**: The y coordinate of the 1st vertex.
- **x1**: The x coordinate of the 2nd vertex.
- **y1**: The y coordinate of the 2nd vertex.
- **x2**: The x coordinate of the 3rd vertex.
- **y2**: The y coordinate of the 3rd vertex.
- **color**: The color of the triangle.
### DrawTriangle (void)
Draws the outline of a triangle of specified color with specified vertices. Should be used over DrawPolygon if the polygon is a triangle. 
- **v0**: The 1st vertex.
- **v1**: The 2nd vertex.
- **v2**: The 3rd vertex.
- **color**: The color of the triangle.
### FillTriangle (void)
Fills a solid triangle of specified color with specified vertices. Should be used over FillPolygon if the polygon is a triangle. 
- **x0**: The x coordinate of the 1st vertex.
- **y0**: The y coordinate of the 1st vertex.
- **x1**: The x coordinate of the 2nd vertex.
- **y1**: The y coordinate of the 2nd vertex.
- **x2**: The x coordinate of the 3rd vertex.
- **y2**: The y coordinate of the 3rd vertex.
- **color**: The color of the triangle.
### FillTriangle (void)
Fills a solid triangle of specified color with specified vertices. Should be used over FillPolygon if the polygon is a triangle. 
- **v0**: The 1st vertex.
- **v1**: The 2nd vertex.
- **v2**: The 3rd vertex.
- **color**: The color of the triangle.
### DrawPolygon (void)
Draws the outline of a polygon of specified color with specified vertices. 
- **color**: The color of the polygon.
- **vertices**: The vertices of the polygon to draw. Must have a length >= 3.
### FillPolygon (void)
Fills a solid polygon of specified color with specified vertices. 
- **color**: The color of the polygon.
- **vertices**: The vertices of the polygon to draw. Must have a length >= 3.
### DrawString (void)
Draws a string of specified color and font size to the window at the specified coordinates. 
- **str**: The string to draw.
- **x**: The x coordinate on the window to draw the string to.
- **y**: The y coordinate on the window to draw the string to.
- **fontSize**: The font size the string will be drawn at.
- **color**: The color the string will br drawn with.
### DrawTextureFast (void)
Draws a texture to the window at the specified coordinates. 
- **x**: The x coordinate to draw the texture at.
- **y**: The y coordinate to draw the texture at.
- **texture**: The Texture to draw.
### DrawTexture (void)
Use DrawTextureFast if your texture doesn't have transparency. 
Draws a texture to the window at the specified coordinates, with transparency. 
- **x**: The x coordinate to draw the texture at.
- **y**: The y coordinate to draw the texture at.
- **texture**: The Texture to draw.
### DrawTexture (void)
Draws a texture to the window at the specified coordinates with the specified scale. 
- **x**: The x coordinate to draw the texture at.
- **y**: The y coordinate to draw the texture at.
- **scaleX**: The amount to scale the by texture horizontally.
- **scaleY**: The amount to scale the by texture vertically.
- **texture**: The Texture to draw.
### DrawTexture (void)
Draws a texture to the window at the specified coordinates with the specified scale. 
- **x**: The x coordinate to draw the texture at.
- **y**: The y coordinate to draw the texture at.
- **scaledX**: The width to scale the texture to in pixels
- **scaledY**: The height to scale the texture to in pixels.
- **texture**: The Texture to draw.
### DrawTexture (void)
Draws a cropped section of a texture to the window at the specified coordinates. 
The cropped section is defined by the starting coordinates (texStartX, texStartY) 
and dimensions (texWidth, texHeight) within the provided texture. 
- **x**: The x coordinate to draw the cropped texture section at.
- **y**: The y coordinate to draw the cropped texture section at.
- **texStartX**: The x coordinate within the texture where cropping starts.
- **texStartY**: The y coordinate within the texture where cropping starts.
- **texWidth**: The width of the cropped texture section.
- **texHeight**: The height of the cropped texture section.
- **texture**: The Texture from which to draw the cropped section.
## Textures
### Texture (Texture)
Creates a Texture from a Bitmap image. The alpha channel of the bitmap isn't taken into account for now. 
- **bmpImagePath**: The path to a Bitmap image to create the texture from.
### Texture (Texture)
Clones a Texture. 
- **texture**: The texture to create a copy of.
### Texture (Texture)
Creates an empty Texture of specified width and height. 
- **width**: The width of the texture.
- **height**: The height of the texture.
### Width (int)
The texture's width. 
### Height (int)
The texture's height. 
### GetPixels (uint[])
Gets the pixels of the texture. 
### SetPixel (void)
Sets a pixel in the texture at specified coordinates to specified color. 
- **x**: The x coordinate of the pixel.
- **y**: The y coordinate of the pixel.
- **color**: The color to set the pixel.
### GetPixel (uint)
Gets a pixel in the texture at specified coordinates. 
- **x**: The x coordinate of the pixel.
- **y**: The y coordinate of the pixel.
### ScaleTo (Texture)
Returns the parent texture scaled by the factors scaleX and scaleY. 
- **scaleX**: The amount to scale the texture by horizontally.
- **scaleY**: The amount to scale the texture by vertically.
### ScaleTo (Texture)
Returns the parent texture scaled to the resolution scaledX x scaledY. 
- **scaledX**: The width to scale the texture to in pixels.
- **scaledY**: The height to scale the texture to in pixels.
### CropTo (Texture)
Returns the parent texture cropped to the resolution specified. 
- **texStartX**: The x coordinate within the texture where cropping starts.
- **texStartY**: The y coordinate within the texture where cropping starts.
- **texWidth**: The width of the cropped texture section.
- **texHeight**: The height of the cropped texture section.
## States
### SaveState (void)
Saves the current state of the window to a buffer. 
- **state**: An int that corresponds to the saved buffer and can be passed into LoadScreen.
### LoadState (void)
Sets the window to a previously saved state. 
- **state**: An int that corresponds to the previously saved buffer, generated from SaveScreen.
### ClearStates (void)
Clears any previously saved states. 
## Colors
### Black (uint)
The color black, 4278190080. 
### Gray (uint)
The color gray, 4286611584. 
### White (uint)
The color white, 4294967295. 
### Red (uint)
The color red, 4278190335. 
### Green (uint)
The color green, 4278255360. 
### Blue (uint)
The color blue, 4294901760. 
### Yellow (uint)
The color yellow, either 4278255615. 
### Orange (uint)
The color orange, either 4278232575. 
### Cyan (uint)
The color cyan, either 4294967040. 
### Magenta (uint)
The color magenta, either 4294902015. 
### Turquoise (uint)
The color turquoise, either 4291878976. 
### Lavender (uint)
The color lavender, either 4294633190. 
### Crimson (uint)
The color crimson, either 4282127580. 
### Rainbow (uint)
A color that cycles through all the full rainbow based on ElapsedTime. 
- **timeScale**: Optional parameter that controls how fast the color changes.
### NewColor (uint)
Creates a color from 4 bytes in ABGR format (0xAABBGGRR). 
- **r**: The R channel's value between [0, 255].
- **g**: The G channel's value between [0, 255].
- **b**: The B channel's value between [0, 255].
- **a**: The A channel's value between [0, 255].
### NewColor (uint)
Creates a color from 4 floats in ABGR format (0xAABBGGRR). 
- **r**: The R channel's value between [0.0, 1.0].
- **g**: The G channel's value between [0.0, 1.0].
- **b**: The B channel's value between [0.0, 1.0].
- **a**: The A channel's value between [0.0, 1.0].
### NewColor (uint)
Creates a color from a Vector3 and additional float in ABGR format (0xAABBGGRR). 
- **col**: The R, G, and B channels, all between [0.0, 1.0].
- **a**: The A channel between [0.0, 1.0].
### NewColor (uint)
Creates a color from a Vector4 in ABGR format (0xAABBGGRR). 
- **col**: The R, G, B, and A channels, all between [0.0, 1.0].
### ToVec3 (Vector3)
Creates a Vector3 from a color, will always return in RGB format. 
- **color**: Optional parameter representing the color to convert to a Vector3.
### ToVec4 (Vector4)
Creates a Vector4 from a color, will always return in RGBA format. 
- **color**: Optional parameter representing the color to convert to a Vector4.
### AverageColors (uint)
Creates a color from averaging two provided colors in ABGR format (0xAABBGGRR). 
The order in which parameters are supplied doesn't matter. 
- **color1**: The first color to average with.
- **color2**: The second color to average with.
### LerpColors (uint)
Linearly interpolates between two colors in ABGR format (0xAABBGGRR). 
- **color1**: The first color to interpolate from.
- **color2**: The second color to interpolate to.
- **t**: The interpolation factor between [0, 1], where 0 represents color1 and 1 represents color2.
### GetR (byte)
An extension method that extracts the red channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
### GetG (byte)
An extension method that extracts the green channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
### GetB (byte)
An extension method that extracts the blue channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
### GetA (byte)
An extension method that extracts the alpha channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
### SetR (uint)
An extension method that sets the red channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the red channel.
- **newR**: The new value for the R channel of the color, in the range [0, 255].
### SetR (uint)
An extension method that sets the red channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the red channel.
- **newR**: The new value for the R channel of the color, in the range [0.0, 1.0].
### SetG (uint)
An extension method that sets the green channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the green channel.
- **newG**: The new value for the G channel of the color, in the range [0, 255].
### SetG (uint)
An extension method that sets the green channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the green  channel.
- **newG**: The new value for the G channel of the color, in the range [0.0, 1.0].
### SetB (uint)
An extension method that sets the blue channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the blue channel.
- **newB**: The new value for the B channel of the color, in the range [0, 255].
### SetB (uint)
An extension method that sets the blue channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the blue channel.
- **newB**: The new value for the B channel of the color, in the range [0.0, 1.0].
### SetA (uint)
An extension method that sets the alpha channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the alpha channel.
- **newA**: The new value for the A channel of the color, in the range [0, 255].
### SetA (uint)
An extension method that sets the alpha channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the alpha channel.
- **newA**: The new value for the A channel of the color, in the range [0.0, 1.0].
### HslToRgb (uint)
Converts a color from HSL format to ABGR format (0xAABBGGRR). 
- **hue**: The H channel's value.
- **saturation**: The S channel's value.
- **lightness**: The L channel's value.
## Render Settings
### RenderSettings (struct)
The struct defining the settings which will be applied when FL.Init is called. 
### PixelSize (int)
Gets or sets the pixel size of the window. Clamped in the range [1, 100]. 
### VSync (bool)
Gets or sets whether or not VSync is enabled. 
### DesiredFramerate (int)
Gets or sets the target framerate engine. Only changes anything is VSync == false. 
### Accumulate (bool)
Gets or sets whether or not the engine accumulates previous frames with the current frame. Only applicable in PerPixel mode. Can be changed during runtime. 
### ScaleType (ScaleType)
Gets or sets how the engine renders when PixelSize > 1. 
### Settings (RenderSettings)
Gets or sets the settings with which the engine will run. Not all settings can be changed during runtime, the ones that can't be must be set before Init. 
### ElapsedTime (float)
The total time since Run was called. 
### DeltaTime (float)
The time from the current frame to the last frame. 
### Width (int)
The width of the window. 
### Height (int)
The height of the window. 
### Mouse (Vector2)
The mouse position on the window. 
### MouseDelta (Vector2)
The amount the mouse has moved from the last frame to the current frame. 
### Rand (uint)
Generates a random unsigned integer using the Lehmer random number generator. 
### Rand (int)
Generates a random integer within the specified range [min, max]. 
- **min**: The minimum possible value of a number that could be generated.
- **max**: The maximum possible value of a number that could be generated.
### Rand (float)
Generates a random float within the specified range [min, max]. 
- **min**: The minimum possible value of a number that could be generated.
- **max**: The maximum possible value of a number that could be generated.
### RandInUnitSphere (Vector3)
Generates a random Vector3 on the unit sphere. 
### GetKeyDown (bool)
Returns whether or not the specified key is currently being held down. 
- **char**: The key to check represented by a key's keycode, e.g. 'Q' or 81 for the Q key.
### GetKeyUp (bool)
Returns whether or not the specified key was just released, i.e. held down last frame but not this one. 
- **char**: The key to check represented by a key's keycode, e.g. 'Q' or 81 for the Q key.
### LMBDown (bool)
Returns whether or not the left mouse button is being held down. 
### RMBDown (bool)
Returns whether or not the right mouse button is being held down. 
### LMBUp (bool)
Returns whether or not the left mouse button was just released. 
### RMBUp (bool)
Returns whether or not the right mouse button was just released. 
### DegToRad (float)
Extension method to convert degrees to radians. 
- **deg**: Optional parameter representing the degrees to convert.
### RadToDeg (float)
Extension method to convert radians to degrees. 
- **rad**: Optional parameter representing the radians to convert.
### Lerp (float)
Performs linear interpolation between two values. 
- **a**: The value from which to interpolate.
- **b**: The value to interpolate to.
- **t**: The percent as a decimal in range [0, 1] to interpolate by.
### Rotate (Vector2)
Extension method to rotate a Vector2 around the specified Vector2 by the specified angle. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **center**: The center of rotation for 'vec'.
- **angle**: The angle by which 'vec' will be rotated.
### Rotate (Vector2[])
Extension method to rotate all Vector2s in an array around the specified Vector2 by the specified angle. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **center**: The center of rotation for the vectors in 'arr'.
- **angle**: The angle by which the vectors in 'arr' will be rotated.
### Scale (Vector2)
Extension method to scale a Vector2 around the specified Vector2 by the specified factor. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **center**: The center 'vec' will be scaled around.
- **factor**: The factor by which 'vec' will be scaled.
### Scale (Vector2[])
Extension method to scale all Vector2s in an array around the specified Vector2 by the specified factor. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **center**: The center for the vectors in 'arr' to be scaled around.
- **factor**: The factor by which the vectors in 'arr' will be scaled.
### AverageWith (Vector2)
Extension method to average two Vector2s together. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **other**: The Vector2 to average 'vec' with.
### AverageWith (Vector2[])
Extension method to average all Vector2s in an array with another specified Vector2. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **other**: The Vector2 to average all the Vector2s in 'arr' with.
### Translate (Vector2)
Extension method to translate a Vector2 by the specified amount. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **offsetX**: The amount to move 'vec' by on the x-axis.
- **offsetY**: The amount to move 'vec' by on the y-axis.
### Translate (Vector2[])
Extension method to translate all Vector2s in an array by the specified amount. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **offsetX**: The amount to move the Vector2s in 'arr' by on the x-axis.
- **offsetY**: The amount to move the Vector2s in 'arr' by on the y-axis.
