using System.Collections.Generic;

namespace Saket.Engine.Graphics.Text
{
    /// <summary>
    /// Common font format for the engine. 
    /// </summary>
    public class Font
    {
        public int unitsPerEm;

        // This should store everything nessary to render the text
        // Character mapping, glyphs
        // Layout info
        public Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();
    }
}