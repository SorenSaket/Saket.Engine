using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Typography.OpenFontFormat.Tables.Advanced
{
    /// <summary>
    /// OpenType table.
    /// Contains glyph definition data. Indicates glyph classes, caret locations for ligatures, and provides an attachment point cache. (Attachment points must be specified in GPOS too.)
    /// Reference: <see href="https://learn.microsoft.com/typography/opentype/spec/gdef"></see>
    /// </summary>
    public class Table_GDEF
    {
        /// <summary>
        /// Major version of the GDEF table
        /// </summary>
        ushort majorVersion;
        /// <summary>
        /// Minor version of the GDEF table
        /// </summary>
        ushort minorVersion;
        /// <summary>
        /// Offset to class definition table for glyph type, from beginning of GDEF header (may be NULL)
        /// </summary>
        ushort glyphClassDefOffset;
        /// <summary>
        /// Offset to attachment point list table, from beginning of GDEF header (may be NULL)
        /// </summary>
        ushort attachListOffset;
        /// <summary>
        /// Offset to ligature caret list table, from beginning of GDEF header (may be NULL)
        /// </summary>
        ushort ligCaretListOffset;
        /// <summary>
        /// Offset to class definition table for mark attachment type, from beginning of GDEF header (may be NULL)
        /// </summary>
        ushort markAttachClassDefOffset;
        /// <summary>
        /// Offset to the table of mark glyph set definitions, from beginning of GDEF header (may be NULL)
        /// </summary>
        ushort markGlyphSetsDefOffset;
        /// <summary>
        /// Offset to the Item Variation Store table, from beginning of GDEF header (may be NULL)
        /// </summary>
        uint itemVarStoreOffset;
    }
}
