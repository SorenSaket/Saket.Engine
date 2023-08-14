using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;

namespace Saket.Typography.OpenFontFormat.Tables.Required
{
    /// <summary>
    /// OS/2 Global font information table 
    /// </summary>
    public class Table_OS2 : Table
    {
        /// <summary>
        /// Type flags. 
        /// </summary>
        /// <remarks>
        /// Indicates font embedding licensing rights for the font. Embeddable fonts may be stored in a
        /// document. When a document with embedded fonts is opened on a system that does not have
        /// the font installed (the remote system), the embedded font may be loaded for temporary (and in
        /// some cases, permanent) use on that system by an embedding-aware application. Embedding
        /// licensing rights are granted by the vendor of the font.
        /// The OFF Font Embedding DLL Applications that implement support for font embedding, either
        /// through use of the Font Embedding DLL or through other means, must not embed fonts which
        /// are not licensed to permit embedding. Further, applications loading embedded fonts for
        /// temporary use (see Preview & Print and Editable embedding below) must delete the fonts
        /// when the document containing the embedded font is closed.
        /// This version of the OS/2 table makes bits 0 - 3 a set of exclusive bits. In other words, at most
        /// one bit in this range may be set at a time. The purpose is to remove misunderstandings caused
        /// by previous behavior of using the least restrictive of the bits that are set. 
        /// </remarks>
        public enum TypeFlags : ushort
        {
            /// <summary>
            /// Restricted License embedding:
            /// Fonts that have only this bit set must not be modified, embedded or
            /// exchanged in any manner without first obtaining permission of the legal owner.
            /// Caution: For Restricted License embedding to take effect, it must be the
            /// only level of embedding selected
            /// </summary>
            Restrictive = 0x0002,
            /// <summary>
            /// Preview & Print embedding: When this bit is set, the font may be
            /// embedded, and temporarily loaded on the remote system. Documents
            /// containing Preview & Print fonts must be opened "read-only;" no edits
            /// can be applied to the document. 
            /// </summary>
            PreviewAndPrint = 0x0004,
            /// <summary>
            /// Editable embedding: When this bit is set, the font may be embedded but
            /// must only be installed temporarily on other systems. In contrast to
            /// Preview & Print fonts, documents containing Editable fonts may be
            /// opened for reading, editing is permitted, and changes may be saved.
            /// </summary>
            EditableEmbedding = 0x0008,
            /// <summary>
            /// No subsetting: When this bit is set, the font may not be subsetted prior to embedding. 
            /// Other embedding restrictions specified in bits 0-3 and 9 also apply. 
            /// </summary>
            NoSubsetting = 0x0100,
            /// <summary>
            /// Bitmap embedding only: When this bit is set, only bitmaps contained in
            /// the font may be embedded. No outline data may be embedded. If there
            /// are no bitmaps available in the font, then the font is considered
            /// unembeddable and the embedding services will fail. Other embedding
            /// restrictions specified in bits 0-3 and 8 also apply. 
            /// </summary>
            BitmapEmbeddingOnly = 0x0200
        }



        public override uint Tag => 0x4f532f32;

        /// <summary>
        /// The version number for this OS/2 table
        /// </summary>
        /// <remarks>
        /// The version number allows for identification of the precise contents and layout for the OS/2 table. 
        /// </remarks>
        public ushort version;
        /// <summary>
        /// Average weighted escapement. The Average Character Width parameter specifies the arithmetic average of the escapement (width) of all non-zero width glyphs in the font. 
        /// </summary>
        /// <remarks>
        /// The value for xAvgCharWidth is calculated by obtaining the arithmetic average of the width of
        /// all non-zero width glyphs in the font. Furthermore, it is strongly recommended that
        /// implementers do not rely on this value for computing layout for lines of text. Especially, for
        /// cases where complex scripts are used. The calculation algorithm differs from one being used in
        /// previous versions of OS/2 table. For details see Annex A. 
        /// </remarks>
        public short xAvgCharWidth;
        /// <summary>
        /// Weight class.  Indicates the visual weight (degree of blackness or thickness of strokes) of the characters in the font. Values from 1 to 1000 are valid. 
        /// </summary>
        /// <remarks>
        ///  usWeightClass values use the same scale as the 'wght' axis that is used in the 'fvar' table of
        /// variable fonts and in the 'STAT' table. While integer values from 1 to 1000 are supported, some
        /// legacy platforms may have limitations on supported values. 
        /// </remarks>
        public ushort usWeightClass;
        /// <summary>
        /// Width class. Indicates a relative change from the normal aspect ratio (width to height ratio) as specified by a font designer for the glyphs in a font.
        /// </summary>
        /// <remarks>
        /// Although every character in a font may have a different numeric aspect ratio, each character in
        /// a font of normal width has a relative aspect ratio of one. When a new type style is created of a
        /// different width class (either by a font designer or by some automated means) the relative aspect
        /// ratio of the characters in the new font is some percentage greater or less than those same
        /// characters in the normal font -- it is this difference that this parameter specifies.
        /// The valid usWidthClass values are shown in the following table. Note that the usWidthClass
        /// values are related to but distinct from the scale for the 'wdth' axis that is used in the 'fvar' table
        /// of variable fonts and in the 'STAT' table.
        /// </remarks>
        public ushort usWidthClass;
        /// <summary>
        /// Type flags.
        /// </summary>
        public ushort fsType;
        /// <summary>
        /// Subscript horizontal font size. The recommended horizontal size in font design units for subscripts for this font. 
        /// </summary>
        /// <remarks>
        /// If a font has two recommended sizes for subscripts, e.g., numerics and other, the numeric sizes
        /// should be stressed. This size field maps to the em square size of the font being used for a
        /// subscript. The horizontal font size specifies a font designer's recommended horizontal font size
        /// for subscript characters associated with this font. If a font does not include all of the required
        /// subscript characters for an application, and the application can substitute characters by scaling
        /// the character of a font or by substituting characters from another font, this parameter specifies
        /// the recommended em square for those subscript characters.
        /// For example, if the em square for a font is 2048 and ySubScriptXSize is set to 205, then the
        /// horizontal size for a simulated subscript character would be 1/10th the size of the normal
        /// </remarks>
        public short ySubscriptXSize;
        /// <summary>
        /// Subscript vertical font size. The recommended vertical size in font design units for subscripts for this font. 
        /// </summary>
        /// <remarks>
        /// If a font has two recommended sizes for subscripts, e.g. numerics and other, the numeric sizes
        /// should be stressed. This size field maps to the emHeight of the font being used for a subscript.
        /// The horizontal font size specifies a font designer's recommendation for horizontal font size of
        /// subscript characters associated with this font. If a font does not include all of the required
        /// subscript characters for an application, and the application can substitute characters by scaling
        /// the characters in a font or by substituting characters from another font, this parameter specifies
        /// the recommended horizontal EmInc for those subscript characters.
        /// For example, if the em square for a font is 2048 and ySubScriptYSize is set to 205, then the
        /// vertical size for a simulated subscript character would be 1/10th the size of the normal character. 
        /// </remarks>
        public short ySubscriptYSize;
        /// <summary>
        /// Subscript x offset. The recommended horizontal offset in font design untis for subscripts for this font. 
        /// </summary>
        /// <remarks>
        /// The Subscript X offset parameter specifies a font designer's recommended horizontal offset –
        /// from the character origin of the font to the character origin of the subscript's character – for
        /// subscript characters associated with this font. If a font does not include all of the required
        /// subscript characters for an application, and the application can substitute characters, this
        /// parameter specifies the recommended horizontal position from the character escapement point
        /// of the last character before the first subscript character. For upright characters, this value is
        /// usually zero; however, if the characters of a font have an incline (italic characters) the reference
        /// point for subscript characters is usually adjusted to compensate for the angle of incline. 
        /// </remarks>
        public short ySubscriptXOffset;
        /// <summary>
        /// Subscript y offset. The recommended vertical offset in font design units from the baseline for subscripts for this font. 
        /// </summary>
        /// <remarks>
        /// The Subscript Y offset parameter specifies a font designer's recommended vertical offset from
        /// the character baseline to the character baseline for subscript characters associated with this
        /// font. Values are expressed as a positive offset below the character baseline. If a font does not
        /// include all of the required subscript for an application, this parameter specifies the
        /// recommended vertical distance below the character baseline for those subscript characters. 
        /// </remarks>
        public short ySubscriptYOffset;
        /// <summary>
        /// Superscript horizontal font size. The recommended horizontal size in font design units for superscripts for this font. 
        /// </summary>
        /// <remarks>
        /// If a font has two recommended sizes for subscripts, e.g., numerics and other, the numeric sizes
        /// should be stressed. This size field maps to the em square size of the font being used for a
        /// subscript. The horizontal font size specifies a font designer's recommended horizontal font size
        /// for superscript characters associated with this font. If a font does not include all of the required
        /// superscript characters for an application, and the application can substitute characters by
        /// scaling the character of a font or by substituting characters from another font, this parameter
        /// specifies the recommended em square for those superscript characters.
        /// For example, if the em square for a font is 2048 and ySuperScriptXSize is set to 205, then the
        /// horizontal size for a simulated superscript character would be 1/10th the size of the normal character. 
        /// </remarks>
        public short ySuperscriptXSize;
        /// <summary>
        /// Superscript vertical font size. The recommended vertical size in font design units for superscripts for this font.
        /// <remarks>
        /// If a font has two recommended sizes for subscripts, e.g., numerics and other, the numeric sizes
        /// should be stressed. This size field maps to the emHeight of the font being used for a subscript.
        /// The vertical font size specifies a font designer's recommended vertical font size for superscript
        /// characters associated with this font. If a font does not include all of the required superscript
        /// characters for an application, and the application can substitute characters by scaling the
        /// character of a font or by substituting characters from another font, this parameter specifies the
        /// recommended EmHeight for those superscript characters.
        /// For example, if the em square for a font is 2048 and ySuperScriptYSize is set to 205, then the
        /// vertical size for a simulated superscript character would be 1/10th the size of the normal character. 
        /// </remarks>
        public short ySuperscriptYSize;
        /// <summary>
        /// Superscript x offset. The recommended horizontal offset in font design units for superscripts for this font. 
        /// </summary>
        /// <remarks>
        /// The Superscript X offset parameter specifies a font designer's recommended horizontal offset --
        /// from the character origin to the superscript character's origin for the superscript characters
        /// associated with this font. If a font does not include all of the required superscript characters for
        /// an application, this parameter specifies the recommended horizontal position from the
        /// escapement point of the character before the first superscript character. For upright characters,
        /// this value is usually zero; however, if the characters of a font have an incline (italic characters)
        /// the reference point for superscript characters is usually adjusted to compensate for the angle of incline. 
        /// </remarks>
        public short ySuperscriptXOffset;
        /// <summary>
        /// Superscript y offset. The recommended vertical offset in font design units from the baseline for superscripts for this font. 
        /// </summary>
        /// <remarks>
        /// The Superscript Y offset parameter specifies a font designer's recommended vertical offset --
        /// from the character baseline to the superscript character's baseline associated with this font.
        /// Values for this parameter are expressed as a positive offset above the character baseline. If a
        /// font does not include all of the required superscript characters for an application, this parameter
        /// specifies the recommended vertical distance above the character baseline for those superscript characters. 
        /// </remarks>
        public short ySuperscriptYOffset;
        /// <summary>
        /// Strikeout size. Width of the strikeout stroke in font design units. 
        /// </summary>
        /// <remarks>
        /// This field should normally be the width of the em dash for the current font. If the size is one, the
        /// strikeout line will be the line represented by the strikeout position field. If the value is two, the
        /// strikeout line will be the line represented by the strikeout position and the line immediately
        /// above the strikeout position. For a Roman font with a 2048 em square, 102 is suggested. 
        /// </remarks>
        public short yStrikeoutSize;
        /// <summary>
        /// Strikeout position. The position of the top of the strikeout stroke relative to the baseline in font design units. 
        /// </summary>
        /// <remarks>
        /// Positive values represent distances above the baseline, while negative values represent
        /// distances below the baseline. A value of zero falls directly on the baseline, while a value of one
        /// falls one pel above the baseline. The value of strikeout position should not interfere with the
        /// recognition of standard characters, and therefore should not line up with crossbars in the font.
        /// For a Roman font with a 2048 em square, 460 is suggested. 
        /// </remarks>
        public short yStrikeoutPosition;
        /// <summary>
        /// Font-family class and subclass. This parameter is a classification of font-family design. 
        /// </summary>
        /// <remarks>
        /// The font class and font subclass are registered values per Annex A. the to each font family.
        /// This parameter is intended for use in selecting an alternate font when the requested font is not
        /// available. The font class is the most general and the font subclass is the most specific. The high
        /// byte of this field contains the family class, while the low byte contains the family subclass. 
        /// </remarks>
        public short sFamilyClass;

        public byte[] panose;
        public uint ulUnicodeRange1;
        public uint ulUnicodeRange2;
        public uint ulUnicodeRange3;
        public uint ulUnicodeRange4;
        public Tag[] achVendID;
        public ushort fsSelection;
        public ushort usFirstCharIndex;
        public ushort usLastCharIndex;

        public short sTypoAscender;
        public short sTypoDescender;
        public short sTypoLineGap;

        public ushort usWinAscent;
        public ushort usWinDescent;

        public uint ulCodePageRange1;
        public uint ulCodePageRange2;

        public short sxHeight;
        public short sCapHeight;

        public ushort usDefaultChar;
        public ushort usBreakChar;
        public ushort usMaxContext;
        public ushort usLowerOpticalPointSize;
        public ushort usUpperOpticalPointSize;

        public override void Deserialize(OFFReader reader)
        {
            reader.LoadBytes(32 + 10 + 4);

            reader.ReadUInt16(ref version);
            reader.ReadInt16(ref xAvgCharWidth);
            reader.ReadUInt16(ref usWeightClass);
            reader.ReadUInt16(ref usWidthClass);
            reader.ReadUInt16(ref fsType);
            reader.ReadInt16(ref ySubscriptXSize);
            reader.ReadInt16(ref ySubscriptYSize);
            reader.ReadInt16(ref ySubscriptXOffset);
            reader.ReadInt16(ref ySubscriptYOffset);
            reader.ReadInt16(ref ySuperscriptXSize);
            reader.ReadInt16(ref ySuperscriptYSize);
            reader.ReadInt16(ref ySuperscriptXOffset);
            reader.ReadInt16(ref ySuperscriptYOffset);
            reader.ReadInt16(ref yStrikeoutSize);
            reader.ReadInt16(ref yStrikeoutPosition);
            reader.ReadInt16(ref sFamilyClass);

            panose = new byte[10];
            for (int i = 0; i < 10; i++)
            {
                reader.ReadUInt8(ref panose[i]);
            }

            reader.ReadUInt32(ref ulUnicodeRange1);
            if (version > 0)
            {
                reader.LoadBytes(4 * 3);
                reader.ReadUInt32(ref ulUnicodeRange2);
                reader.ReadUInt32(ref ulUnicodeRange3);
                reader.ReadUInt32(ref ulUnicodeRange4);
            }
            reader.LoadBytes(4 * 4 + 8 * 2);
            achVendID = new Tag[4];
            for (int i = 0; i < 4; i++)
            {
                reader.ReadTag(ref achVendID[i]);
            }

            reader.ReadUInt16(ref fsSelection);
            reader.ReadUInt16(ref usFirstCharIndex);
            reader.ReadUInt16(ref usLastCharIndex);

            reader.ReadInt16(ref sTypoAscender);
            reader.ReadInt16(ref sTypoDescender);
            reader.ReadInt16(ref sTypoLineGap);

            reader.ReadUInt16(ref usWinAscent);
            reader.ReadUInt16(ref usWinDescent);
            if (version > 0)
            {
                reader.LoadBytes(4 * 2);
                reader.ReadUInt32(ref ulCodePageRange1);
                reader.ReadUInt32(ref ulCodePageRange2);
            }

            if (version > 1)
            {
                reader.LoadBytes(10);
                reader.ReadInt16(ref sxHeight);
                reader.ReadInt16(ref sCapHeight);
                reader.ReadUInt16(ref usDefaultChar);
                reader.ReadUInt16(ref usBreakChar);
                reader.ReadUInt16(ref usMaxContext);
            }
            if (version > 4)
            {
                reader.LoadBytes(4);
                reader.ReadUInt16(ref usLowerOpticalPointSize);
                reader.ReadUInt16(ref usUpperOpticalPointSize);
            }
        }

        public override void Serialize(OFFWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}