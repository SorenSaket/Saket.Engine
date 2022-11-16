using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Filetypes.Font.Tables
{
    /// <summary>
    /// OpenType table.
    /// Contains glyph definition data. Indicates glyph classes, caret locations for ligatures, and provides an attachment point cache. (Attachment points must be specified in GPOS too.)
    /// Reference: <see href="https://learn.microsoft.com/typography/opentype/spec/gdef"></see>
    /// </summary>
    public struct Table_GDEF
    {
        /// <summary>
        /// Major version of the GDEF table
        /// </summary>
        UInt16 majorVersion;
        /// <summary>
        /// Minor version of the GDEF table
        /// </summary>
        UInt16 minorVersion;
        /// <summary>
        /// Offset to class definition table for glyph type, from beginning of GDEF header (may be NULL)
        /// </summary>
        UInt16 glyphClassDefOffset;
        /// <summary>
        /// Offset to attachment point list table, from beginning of GDEF header (may be NULL)
        /// </summary>
        UInt16 attachListOffset;
        /// <summary>
        /// Offset to ligature caret list table, from beginning of GDEF header (may be NULL)
        /// </summary>
        UInt16 ligCaretListOffset;
        /// <summary>
        /// Offset to class definition table for mark attachment type, from beginning of GDEF header (may be NULL)
        /// </summary>
        UInt16 markAttachClassDefOffset;
        /// <summary>
        /// Offset to the table of mark glyph set definitions, from beginning of GDEF header (may be NULL)
        /// </summary>
        UInt16 markGlyphSetsDefOffset;
        /// <summary>
        /// Offset to the Item Variation Store table, from beginning of GDEF header (may be NULL)
        /// </summary>
        UInt32 itemVarStoreOffset;
    }
}
