﻿This implementation of the Open Font Format provides loading and saving of the following file types:
.tff, .otf, .woff, .woff2


Provides simple API:

// Create a new OpenFont from stream
public OpenFont OpenFont.FromStream(Stream stream);
public OpenFont OpenFont.FromStreamAsync(Stream stream);

//
public Stream OpenFont.ToStream()

// Checks to see if the font is valid
public Bool OpenFont.IsValid()


Full Documentation
Rich Enums and Structs where applicable for easier interop with C# code.
Fully Managed C# code.

Mostly 1-1 naming with specification

Provides full implementation of the Open Font Format Specification.


Other reference implementations used and referenced:
https://github.com/freetype/freetype
https://github.com/LayoutFarm/Typography
https://github.com/MikePopoloski/SharpFont




<hr/>

 How the Open Font Format works
 How ta
