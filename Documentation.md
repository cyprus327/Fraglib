# Fraglib Documentation
## Init (void)
Initializes the window with the specified width, height, and title. 
This method must be called before using any other FL methods. 
- **width**: The width of the window in pixels.
- **height**: The height of the window in pixels.
- **program**: The delegate which will get invoked once per frame until the window is closed.
## Init (void)
Initializes the window with the specified width, height, title, perPixel function, and optional perFrame function. 
This method must be called before using any other FL methods. 
- **width**: The width of the window in pixels.
- **height**: The height of the window in pixels.
## Init (void)
Starts the main loop of the engine. 
Must be called after Init for a window to appear. 
Any settings changed after this (PixelSize, VSync, etc.) won't affect anything. 
## SetPixel (void)
Sets the pixel at the specified position to the given color. 
- **x**: The x coordinate of the pixel.
- **y**: The y coordinate of the pixel.
- **color**: The color of the pixel in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## GetPixel (uint)
Gets the pixel's color at the specified position. 
- **x**: The x coordinate of the pixel.
- **y**: The y coordinate of the pixel.
- **color**: The color of the pixel in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## Clear (void)
Clears the window to black. 
## Clear (void)
Clears the window to the specified color. 
- **color**: The color the window will get cleared to in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## FillRect (void)
Fills a solid rectangle of specified size and color at the specified coordinates. 
- **x**: The starting point of the rectangle's x coordinate.
- **y**: The starting point of the rectangle's y coordinate.
- **width**: The width of the rectangle.
- **height**: The height of the rectangle.
- **color**: The color of the rectangle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## DrawCircle (void)
Draws the outline of a circle of specified size and color at the specified coordinates. 
- **centerX**: The center coordinate of the circle along the x-axis.
- **centerY**: The center coordinate of the cirlce along the y-axis.
- **radius**: The radius of the circle.
- **color**: The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## FillCircle (void)
Fills a solid circle of specified size and color at the specified coordinates. 
- **centerX**: The center coordinate of the circle along the x-axis.
- **centerY**: The center coordinate of the cirlce along the y-axis.
- **radius**: The radius of the circle.
- **color**: The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## DrawLine (void)
Draws a line of specified color along the specified path. 
- **x0**: The starting x coordinate of the line.
- **y0**: The starting y coordinate of the line.
- **x1**: The ending x coordinate of the line.
- **y1**: The ending y coordinate of the line.
- **color**: The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## DrawLine (void)
Draws a vertical line of specified color along the specified path. 
Should be used over DrawLine if the line is vertical. 
- **x**: The x coordinate of the line.
- **y0**: The starting y coordinate of the line.
- **y1**: The ending y coordinate of the line.
- **color**: The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
## DrawPolygon (void)
Draws the outline of a polygon of specified color with specified vertices. 
- **color**: The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
- **vertices**: The vertices of the polygon to draw. Must have a length >= 3.
## FillPolygon (void)
Fills a solid polygon of specified color with specified vertices. 
- **color**: The color of the circle in either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness.
- **vertices**: The vertices of the polygon to draw. Must have a length >= 3.
## SaveScreen (void)
Saves the current state of the window to a buffer. 
- **state**: An int that corresponds to the saved buffer and can be passed into LoadScreen.
## LoadScreen (void)
Sets the window to a previously saved state. 
- **state**: An int that corresponds to the previously saved buffer, generated from SaveScreen.
## ClearStates (void)
Clears any previously saved states. 
## Black (uint)
The color black, either 4278190080 or 255. 
## Gray (uint)
The color gray, either 4286611584 or 2155905279. 
## White (uint)
The color white, represented as 4294967295. 
## Red (uint)
The color red, represented as 4278190335. 
## Green (uint)
The color green, either 4278255360 or 16711935. 
## Blue (uint)
The color blue, either 4294901760 or 65535. 
## Yellow (uint)
The color yellow, either 4278255615 or 4294902015. 
## Orange (uint)
The color orange, either 4278232575 or 4289003775. 
## Cyan (uint)
The color cyan, either 4294967040 or 16777215. 
## Magenta (uint)
The color magenta, either 4294902015 or 4278255615. 
## Turquoise (uint)
The color turquoise, either 4291878976 or 1088475391. 
## Lavender (uint)
The color lavender, either 4294633190 or 3873897215. 
## Crimson (uint)
The color crimson, either 4282127580 or 3692313855. 
## Rainbow (uint)
A color that cycles through all the full rainbow based on ElapsedTime. 
- **timeScale**: Optional parameter that controls how fast the color changes.
## NewColor (uint)
Creates a color from 4 bytes, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness. 
- **r**: The R channel's value between [0, 255].
- **g**: The G channel's value between [0, 255].
- **b**: The B channel's value between [0, 255].
- **a**: The A channel's value between [0, 255].
## NewColor (uint)
Creates a color from 4 floats, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness. 
- **r**: The R channel's value between [0.0, 1.0].
- **g**: The G channel's value between [0.0, 1.0].
- **b**: The B channel's value between [0.0, 1.0].
- **a**: The A channel's value between [0.0, 1.0].
## NewColor (uint)
Creates a color from a Vector3 and additional float, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness. 
- **col**: The R, G, and B channels, all between [0.0, 1.0].
- **a**: The A channel between [0.0, 1.0].
## NewColor (uint)
Creates a color from a Vector4, either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness. 
- **col**: The R, B, B, and A channels, all between [0.0, 1.0].
## GetR (byte)
An extension method that extracts the red channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
## GetG (byte)
An extension method that extracts the green channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
## GetB (byte)
An extension method that extracts the blue channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
## GetA (byte)
An extension method that extracts the alpha channel of the specified color in the range [0, 255]. 
- **color**: An optional parameter representing the color of which to extract the channel.
## SetR (uint)
An extension method that sets the red channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the red channel.
- **newR**: The new value for the R channel of the color, in the range [0, 255].
## SetR (uint)
An extension method that sets the red channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the red channel.
- **newR**: The new value for the R channel of the color, in the range [0.0, 1.0].
## SetG (uint)
An extension method that sets the green channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the green channel.
- **newG**: The new value for the G channel of the color, in the range [0, 255].
## SetG (uint)
An extension method that sets the green channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the green  channel.
- **newG**: The new value for the G channel of the color, in the range [0.0, 1.0].
## SetB (uint)
An extension method that sets the blue channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the blue channel.
- **newB**: The new value for the B channel of the color, in the range [0, 255].
## SetB (uint)
An extension method that sets the blue channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the blue channel.
- **newB**: The new value for the B channel of the color, in the range [0.0, 1.0].
## SetA (uint)
An extension method that sets the alpha channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the alpha channel.
- **newA**: The new value for the A channel of the color, in the range [0, 255].
## SetA (uint)
An extension method that sets the alpha channel of the specified color to the new color specified. 
Modifies the underlying variable. 
- **color**: An optional parameter representing the color of which to set the alpha channel.
- **newA**: The new value for the A channel of the color, in the range [0.0, 1.0].
## HslToRgb (uint)
Converts a color from HSL color space to either RGBA (0xRRGGBBAA) or ARGB format (0xAARRGGBB) depending on the system's endianness. 
- **hue**: The H channel's value.
- **saturation**: The S channel's value.
- **lightness**: The L channel's value.
## PixelSize (int)
The pixel size of the window. Locked between [1, 100]. 
## VSync (bool)
Whether or not VSync is enabled. 
## ElapsedTime (float)
The total time since Run was called. 
## DeltaTime (float)
The time from the current frame to the last frame. 
## Width (int)
The scaled width of the window, i.e. real window width / pixel size. 
## Height (int)
The scaled height of the window, i.e. real window height / pixel size. 
## Rand (uint)
Generates a random unsigned integer using the Lehmer random number generator. 
## Rand (int)
Generates a random integer within the specified range [min, max]. 
- **min**: The minimum possible value of a number that could be generated.
- **max**: The maximum possible value of a number that could be generated.
## Rand (float)
Generates a random float within the specified range [min, max]. 
- **min**: The minimum possible value of a number that could be generated.
- **max**: The maximum possible value of a number that could be generated.
## GetKeyDown (bool)
Returns whether or not the specified key is currently being held down. 
- **char**: The key to check represented by a key's keycode, e.g. 'Q' or 81 for the Q key.
## GetKeyUp (bool)
Returns whether or not the specified key was just released, i.e. held down last frame but not this one. 
- **char**: The key to check represented by a key's keycode, e.g. 'Q' or 81 for the Q key.
## LMBDown (bool)
Returns whether or not the left mouse button is being held down. 
## RMBDown (bool)
Returns whether or not the right mouse button is being held down. 
## LMBUp (bool)
Returns whether or not the left mouse button was just released. 
## RMBUp (bool)
Returns whether or not the right mouse button was just released. 
## DegToRad (float)
Extension method to convert degrees to radians. 
- **deg**: Optional parameter representing the degrees to convert.
## RadToDeg (float)
Extension method to convert radians to degrees. 
- **rad**: Optional parameter representing the radians to convert.
## Rotate (Vector2)
Extension method to rotate a Vector2 around the specified Vector2 by the specified angle. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **center**: The center of rotation for 'vec'.
- **angle**: The angle by which 'vec' will be rotated.
## Rotate (Vector2[])
Extension method to rotate all Vector2s in an array around the specified Vector2 by the specified angle. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **center**: The center of rotation for the vectors in 'arr'.
- **angle**: The angle by which the vectors in 'arr' will be rotated.
## Scale (Vector2)
Extension method to scale a Vector2 around the specified Vector2 by the specified factor. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **center**: The center 'vec' will be scaled around.
- **factor**: The factor by which 'vec' will be scaled.
## Scale (Vector2[])
Extension method to scale all Vector2s in an array around the specified Vector2 by the specified factor. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **center**: The center for the vectors in 'arr' to be scaled around.
- **factor**: The factor by which the vectors in 'arr' will be scaled.
## AverageWith (Vector2)
Extension method to average two Vector2s together. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **other**: The Vector2 to average 'vec' with.
## AverageWith (Vector2[])
Extension method to average all Vector2s in an array with another specified Vector2. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **other**: The Vector2 to average all the Vector2s in 'arr' with.
## Translate (Vector2)
Extension method to translate a Vector2 by the specified amount. 
Modifies the underlying variable. 
- **vec**: Optional parameter representing the Vector2 to modify.
- **offsetX**: The amount to move 'vec' by on the x-axis..
- **offsetY**: The amount to move 'vec' by on the y-axis..
## Translate (Vector2[])
Extension method to translate all Vector2s in an array by the specified amount. 
Modifies the underlying array. 
- **arr**: Optional parameter representing the array of Vector2s to modify.
- **offsetX**: The amount to move the Vector2s in 'arr' by on the x-axis..
- **offsetY**: The amount to move the Vector2s in 'arr' by on the y-axis..
