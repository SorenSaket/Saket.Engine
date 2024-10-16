using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;

//https://github.com/aseprite/aseprite/blob/main/docs/ase-file-specs.md


namespace Saket.Engine.Formats.Aseprite;

#region Types
using BYTE = byte;
using WORD = UInt16;
using SHORT = Int16;
using DWORD = UInt32;
using LONG = Int32;
using FIXED = float; // FIXED 16.16 float. convert by dividing by 65536.0
using FLOAT = float;
using DOUBLE = double;
using QWORD = UInt64;
using LONG64 = Int64;

public struct STRING
{
    public WORD StringLength;
    public BYTE[] Characters; // UTF8

    public STRING(string str)
    {
        if (str.Length > ushort.MaxValue)
            throw new Exception("string too long");
        StringLength = (ushort)str.Length;
        Characters = Encoding.UTF8.GetBytes(str);
    }
    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        StringLength = reader.ReadUInt16();
        Characters = reader.ReadBytes(StringLength);
    }
    public void WriteToStream(Stream stream)
    {
        using var Writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        Writer.Write(StringLength);
        Writer.Write(Characters);
    }


    public override string? ToString()
    {
        return UTF8Encoding.UTF8.GetString(Characters);
    }
}
public struct POINT
{
    public LONG X;
    public LONG Y;
}
public struct SIZE
{
    public LONG Width;
    public LONG Height;
}
public struct RECT
{
    public POINT Origin;
    public SIZE Size;
}

#endregion

#region Custom/Infered Types
public struct Chunk_Palette_Old_Packet
{
    /// <summary>
    /// Number of palette entries to skip from the last packet (start from 0)
    /// </summary>
    public BYTE SkipCountFromLastPacket;

    public RGBColor[] Colors;
}
public struct RGBColor
{
    public BYTE Red;
    public BYTE Blue;
    public BYTE Green;
}
#endregion

public struct Header
{
    /// <summary>
    /// (bits per pixel)
    /// </summary>
    public enum Header_ColorDepth : WORD
    {
        Indexed = 8,
        Grayscale = 16,
        RGBA = 32
    }
    [Flags]
    public enum Header_Flags : DWORD
    {
        LayerOpacityHasValidValue = 1,
    }

    public DWORD FileSizeInBytes;
    public const WORD MagicNumber = 0xA5E0;
    public WORD Frames;
    public WORD WidthInPixels;
    public WORD HeightInPixels;
    public Header_ColorDepth ColorDepth;
    public Header_Flags Flags;
    /// <summary>
    /// Speed (milliseconds between frame, like in FLC files)
    /// DEPRECATED: You should use the frame duration field
    /// from each frame header
    /// </summary>
    public WORD Speed;
    /// <summary>
    /// Palette entry (index) which represent transparent color in all non-background layers(only for Indexed sprites).
    /// </summary>
    public BYTE PaletteEntry;
    /// <summary>
    /// Number of colors (0 means 256 for old sprites)
    /// </summary>
    public WORD NumberOfColors;
    /// <summary>
    ///  Pixel width (pixel ratio is "pixel width/pixel height").
    ///  If this or pixel height field is zero, pixel ratio is 1:1
    /// </summary>
    public BYTE PixelWidth;
    public BYTE PixelHeight;
    public SHORT GridPosX;
    public SHORT GridPosY;
    /// <summary>
    ///  Grid width (zero if there is no grid, grid size is 16x16 on Aseprite by default)
    /// </summary>
    public WORD GridWidth;
    /// <summary>
    ///  Grid height (zero if there is no grid)
    /// </summary>
    public WORD GridHeight;
    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        
        FileSizeInBytes = reader.ReadUInt32();
        if (reader.ReadUInt16() != MagicNumber)
            throw new Exception("Invalid Header");
        Frames = reader.ReadUInt16();
        WidthInPixels = reader.ReadUInt16();
        HeightInPixels = reader.ReadUInt16();
        ColorDepth = (Header_ColorDepth)reader.ReadUInt16();
        Flags = (Header_Flags)reader.ReadUInt32();
        Speed = reader.ReadUInt16();
        if (reader.ReadUInt32() != 0)
            throw new Exception("Invalid Header");
        if (reader.ReadUInt32() != 0)
            throw new Exception("Invalid Header");
        PaletteEntry = reader.ReadByte();
        reader.ReadBytes(3);
        NumberOfColors = reader.ReadUInt16();
        PixelWidth = reader.ReadByte(); 
        PixelHeight = reader.ReadByte();
        GridPosX = reader.ReadInt16();
        GridPosY = reader.ReadInt16();
        GridWidth = reader.ReadUInt16();
        GridHeight = reader.ReadUInt16();
        reader.ReadBytes(84);
    }
    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write(FileSizeInBytes);
        writer.Write(MagicNumber);
        writer.Write(Frames);
        writer.Write(WidthInPixels);
        writer.Write(HeightInPixels);
        writer.Write((WORD)ColorDepth);
        writer.Write((DWORD)Flags);
        writer.Write(Speed);
        writer.Write((uint)0);
        writer.Write((uint)0);
        writer.Write(PaletteEntry);
        writer.Write([0,0,0]);
        writer.Write(NumberOfColors);
        writer.Write(PixelWidth);
        writer.Write(PixelHeight);
        writer.Write(GridPosX);
        writer.Write(GridPosY);
        writer.Write(GridWidth);
        writer.Write(GridHeight);
        // Skip 84 bytes
        stream.Seek(84, SeekOrigin.Current);
    }
}

#region Frame
public struct FrameHeader
{
    public DWORD BytesInFrame;
    public const WORD MagicNumber = 0xF1FA;

    /// <summary>
    /// Frame duration (in milliseconds)
    /// </summary>
    public WORD FrameDuration;

    /// <summary>
    /// New field which specifies the number of "chunks" in this frame(if this is 0, use the old field)
    /// </summary>
    public DWORD NumberOfChunks;



    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        BytesInFrame = reader.ReadUInt32();
        if (reader.ReadUInt16() != MagicNumber)
            throw new Exception("Invalid Header");
        var Old = reader.ReadUInt16();
        FrameDuration = reader.ReadUInt16();
        reader.ReadBytes(2);
        var New = reader.ReadUInt32();

        if (Old < WORD.MaxValue)
            NumberOfChunks = New;
        else
            NumberOfChunks = Old;
    }
    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write(BytesInFrame);
        writer.Write(MagicNumber);
        if (NumberOfChunks >= WORD.MaxValue)
            writer.Write(WORD.MaxValue);
        else
            writer.Write((WORD)NumberOfChunks);
        writer.Write(FrameDuration);
        writer.Write([0,0]);
        writer.Write(NumberOfChunks);
    }
}
public struct Frame
{
    public FrameHeader Header;
    public IChunk[] Chunks;
}
#endregion

#region Chunks
public enum ChunkType : WORD
{
    /// <summary>
    /// Ignore this chunk if you find the new palette chunk (0x2019). Aseprite v1.1 saves both chunks (0x0004 and 0x2019) just for backward compatibility. Aseprite v1.3.5 writes this chunk if the palette doesn't have alpha channel and contains 256 colors or less (because this chunk is smaller), in other case the new palette chunk (0x2019) will be used (and the old one is not saved anymore).
    /// </summary>
    PaletteOld1 = 0x0004,
    /// <summary>
    /// Ignore this chunk if you find the new palette chunk (0x2019)
    /// </summary>
    PaletteOld2 = 0x0011,
    /// <summary>
    /// n the first frame should be a set of layer chunks to determine the entire layers layout
    /// </summary>
    Layer = 0x2004,
    /// <summary>
    /// Cel Chunk 
    /// </summary>
    Cel = 0x2005,
    /// <summary>
    /// Adds extra information to the latest read cel.
    /// </summary>
    CelExtra = 0x2006,
    /// <summary>
    /// Color profile for RGB or grayscale values.
    /// </summary>
    ColorProfile = 0x2007,
    /// <summary>
    /// A list of external files linked with this file can be found in the first frame. It might be used to reference external palettes, tilesets, or extensions that make use of extended properties.
    /// </summary>
    ExternalFiles = 0x2008,
    /// <summary>
    /// Mask Chunk DEPRECATED
    /// </summary>
    Mask = 0x2016,
    /// <summary>
    /// Never used.
    /// </summary>
    Path = 0x2017,
    /// <summary>
    /// After the tags chunk, you can write one user data chunk for each tag. E.g. if there are 10 tags, you can then write 10 user data chunks one for each tag.
    /// </summary>
    Tags = 0x2018,
    /// <summary>
    /// Palette Chunk
    /// </summary>
    Palette = 0x2019,
    /// <summary>
    /// we've read is a layer and then this chunk appears, this user data belongs to that layer, if we've read a cel, it belongs to that cel, etc. There are some special cases:
    /// <list type="number">
    /// <item>After a Tags chunk, there will be several user data chunks, one for each tag, you should associate the user data in the same order as the tags are in the Tags chunk.</item>
    /// <item>After the Tileset chunk, it could be followed by a user data chunk (empty or not) and then all the user data chunks of the tiles ordered by tile index, or it could be followed by none user data chunk (if the file was created in an older Aseprite version of if no tile has user data).</item>
    /// <item>In Aseprite v1.3 a sprite has associated user data, to consider this case there is an User Data Chunk at the first frame after the Palette Chunk.</item>
    /// </list>
    /// </summary>
    UserData = 0x2020,
    /// <summary>
    /// Slice Chunk
    /// </summary>
    Slice = 0x2022,
    /// <summary>
    /// Tileset Chunk 
    /// </summary>
    Tileset = 0x2023
}
public interface IChunk
{
    public ChunkType ChunkType { get; }
    public void WriteToStream(Stream stream);
}
public struct ChunkHeader
{
    public DWORD SizeInBytes;
    public ChunkType ChunkType; 
    
    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        SizeInBytes = reader.ReadUInt32();
        ChunkType = (ChunkType)reader.ReadInt16();
    }
}
/// <summary>
/// Ignore this chunk if you find the new palette chunk (0x2019). Aseprite v1.1 saves both chunks (0x0004 and 0x2019) just for backward compatibility. Aseprite v1.3.5 writes this chunk if the palette doesn't have alpha channel and contains 256 colors or less (because this chunk is smaller), in other case the new palette chunk (0x2019) will be used (and the old one is not saved anymore).
/// </summary>
public struct Chunk_Palette_Old1 : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.PaletteOld1;
 
    public Chunk_Palette_Old_Packet[] Packets;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        var numberOfPackets = reader.ReadUInt16();
        Packets = new Chunk_Palette_Old_Packet[numberOfPackets];
        for (int i = 0; i < Packets.Length; i++)
        {
            Packets[i].SkipCountFromLastPacket = reader.ReadByte();
            BYTE count = reader.ReadByte();
            if (count == 0)
                count = BYTE.MaxValue;
            Packets[i].Colors = new RGBColor[count];
            for (int c = Packets[i].SkipCountFromLastPacket; c < count; c++)
            {
                Packets[i].Colors[c].Red = reader.ReadByte();
                Packets[i].Colors[c].Blue = reader.ReadByte();
                Packets[i].Colors[c].Green = reader.ReadByte();
            }
        }
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write((WORD)Packets.Length);
        for (int i = 0; i < Packets.Length; i++)
        {
            writer.Write(Packets[i].SkipCountFromLastPacket);
            writer.Write((BYTE)Packets[i].Colors.Length);
            for (int c = Packets[i].SkipCountFromLastPacket; c < Packets[i].Colors.Length; c++)
            {
                writer.Write(Packets[i].Colors[c].Red);
                writer.Write(Packets[i].Colors[c].Blue);
                writer.Write(Packets[i].Colors[c].Green);
            }
        }
    }
}
/// <summary>
/// Ignore this chunk if you find the new palette chunk (0x2019)
/// </summary>
public struct Chunk_Palette_Old2
{
    public readonly ChunkType ChunkType => ChunkType.PaletteOld2;

    public Chunk_Palette_Old_Packet[] Packets;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        var numberOfPackets = reader.ReadUInt16();
        Packets = new Chunk_Palette_Old_Packet[numberOfPackets];
        for (int i = 0; i < Packets.Length; i++)
        {
            Packets[i].SkipCountFromLastPacket = reader.ReadByte();
            BYTE count = reader.ReadByte();
            if (count == 0)
                count = BYTE.MaxValue;
            Packets[i].Colors = new RGBColor[count];
            for (int c = Packets[i].SkipCountFromLastPacket; c < count; c++)
            {
                Packets[i].Colors[c].Red = reader.ReadByte();
                Packets[i].Colors[c].Blue = reader.ReadByte();
                Packets[i].Colors[c].Green = reader.ReadByte();
            }
        }
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write((WORD)Packets.Length);
        for (int i = 0; i < Packets.Length; i++)
        {
            writer.Write(Packets[i].SkipCountFromLastPacket);
            writer.Write((BYTE)Packets[i].Colors.Length);
            for (int c = Packets[i].SkipCountFromLastPacket; c < Packets[i].Colors.Length; c++)
            {
                writer.Write(Packets[i].Colors[c].Red);
                writer.Write(Packets[i].Colors[c].Blue);
                writer.Write(Packets[i].Colors[c].Green);
            }
        }
    }
}
/// <summary>
/// In the first frame should be a set of layer chunks to determine the entire layers layout:
/// </summary>
public struct Chunk_Layer : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.Layer;
    [Flags]
    public enum Chunk_Layer_Flags : WORD
    {
        Visible = 1,
        Editable = 2,
        LockMovement = 4,
        Background = 8,
        PreferLinkedCels = 16,
        LayerGroupDisplayedCollapsed = 32,
        IsRefereceLayer = 64,
    }
    
    public enum Chunk_Layer_LayerType : WORD
    {
        Normal = 0,
        Group = 1,
        Tilemap = 2,
    }

    public enum Chunk_Layer_Blendmode : WORD
    {
        Normal = 0,
        Multiply = 1,
        Screen = 2,
        Overlay = 3,
        Darken = 4,
        Lighten = 5,
        ColorDodge = 6,
        ColorBurn = 7,
        HardLight = 8,
        SoftLight = 9,
        Difference = 10,
        Exclusion = 11,
        Hue = 12,
        Saturation = 13,
        Color = 14,
        Luminosity = 15,
        Addition = 16,
        Subtract = 17,
        Divide = 18,
    }
    public Chunk_Layer_Flags Flags;
    public Chunk_Layer_LayerType LayerType;
    /// <summary>
    /// The child level is used to show the relationship of this layer with the last one read, for example:
    ///    Layer name and hierarchy      Child Level
    ///-----------------------------------------------
    /// - Background                    0
    ///     `- Layer1                   1
    /// - Foreground                    0
    ///     |- My set1                  1
    ///     |  `- Layer2                2
    ///     `- Layer3                   1
    /// </summary>
    public WORD LayerChildLevel;
    /// <summary>
    /// Ignored
    /// </summary>
    public WORD DefaultLayerWidthInPixels;
    /// <summary>
    /// Ignored
    /// </summary>
    public WORD DefaultLayerHeightInPixels;
    public Chunk_Layer_Blendmode Blendmode;
    /// <summary>
    /// Note: valid only if file header flags field has bit 1 set
    /// </summary>
    public BYTE Opacity;
    public string LayerName;
    public DWORD TilesetIndex;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        Flags = (Chunk_Layer_Flags)reader.ReadUInt16();
        LayerType = (Chunk_Layer_LayerType)reader.ReadUInt16();
        LayerChildLevel = reader.ReadUInt16();
        DefaultLayerWidthInPixels = reader.ReadUInt16();
        DefaultLayerHeightInPixels = reader.ReadUInt16();
        Blendmode = (Chunk_Layer_Blendmode)reader.ReadUInt16();
        Opacity = reader.ReadByte();
        reader.ReadBytes(3);
        STRING str = new STRING();
        str.ReadFromStream(stream);
        LayerName = str.ToString();

        if(LayerType == Chunk_Layer_LayerType.Tilemap)
            TilesetIndex = reader.ReadUInt16();
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write((WORD)Flags);
        writer.Write((WORD)LayerType);
        writer.Write(LayerChildLevel);
        writer.Write(DefaultLayerWidthInPixels);
        writer.Write(DefaultLayerHeightInPixels);
        writer.Write((WORD)Blendmode);
        writer.Write(Opacity);
        writer.Write([0,0,0]);
        new STRING(LayerName).WriteToStream(stream);

        if (LayerType == Chunk_Layer_LayerType.Tilemap)
            writer.Write(TilesetIndex);
    }
}
/// <summary>
/// This chunk determine where to put a cel in the specified layer/frame.
/// </summary>
public struct Chunk_Cel : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.Cel;
    public enum Chunk_Cel_CelType : WORD
    {
        /// <summary>
        /// (unused, compressed image is preferred)
        /// </summary>
        RawImageData = 0,
        LinkedCel = 1,
        CompressedImage = 2,
        CompressedTilemap = 3,
    }

    /// <summary>
    /// The layer index is a number to identify a layer in the sprite. Layers are numbered in the same order as Layer Chunks (0x2004) appear in the file, for example:
    ///    Layer name and hierarchy      Layer index
    ///-----------------------------------------------
    /// - Background                    0
    ///     `- Layer1                   1
    /// - Foreground                    2
    ///     |- My set1                  3
    ///     |  `- Layer2                4
    ///     `- Layer3                   5
    /// </summary>
    public WORD LayerIndex;
    public SHORT Xposition;
    public SHORT Yposition;
    public BYTE OpacityLevel;
    public Chunk_Cel_CelType CelType;
    public SHORT ZIndex;

    public WORD WidthInPixels;
    public WORD HeightInPixels;
    public byte[] RawPixelData;

    public WORD FrameLinkPosition;

    public WORD WidthInTiles;
    public WORD HeightInTiles;
    public WORD BitsPerTile;
    public DWORD BitmaskTileID;
    public DWORD BitmaskXFlip;
    public DWORD BitmaskYFlip;
    public DWORD BitmaskDiagonalFlip;
    public byte[] Tiles;

    public void ReadFromStream(Stream stream, Header.Header_ColorDepth header_ColorDepth)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        LayerIndex = reader.ReadUInt16();
        Xposition = reader.ReadInt16();
        Yposition = reader.ReadInt16();
        OpacityLevel = reader.ReadByte();
        CelType = (Chunk_Cel_CelType)reader.ReadUInt16();
        ZIndex = reader.ReadInt16();
        reader.ReadBytes(5);

        switch (CelType)
        {
            case Chunk_Cel_CelType.RawImageData:
                {
                    WidthInPixels = reader.ReadUInt16();
                    HeightInPixels = reader.ReadUInt16();
                    int numberOfBytes = (int)WidthInPixels * (int)HeightInPixels * ((int)header_ColorDepth / 8);
                    RawPixelData = reader.ReadBytes(numberOfBytes);
                    break;
                }
            case Chunk_Cel_CelType.LinkedCel:
                FrameLinkPosition = reader.ReadUInt16();
                break;
            case Chunk_Cel_CelType.CompressedImage:
                {
                    WidthInPixels = reader.ReadUInt16();
                    HeightInPixels = reader.ReadUInt16();
                    int numberOfBytes = (int)WidthInPixels * (int)HeightInPixels * ((int)header_ColorDepth / 8);
                    RawPixelData = new BYTE[numberOfBytes];
                    using (var compressedStream = new ZLibStream(stream, CompressionMode.Decompress, true))
                    {
                        compressedStream.ReadExactly(RawPixelData, 0, numberOfBytes);
                    }
                }
                break;
            case Chunk_Cel_CelType.CompressedTilemap:
                {
                    WidthInTiles = reader.ReadUInt16();
                    HeightInTiles = reader.ReadUInt16();
                    BitsPerTile = reader.ReadUInt16();
                    BitmaskTileID = reader.ReadUInt32();
                    BitmaskXFlip = reader.ReadUInt32();
                    BitmaskYFlip = reader.ReadUInt32();
                    BitmaskDiagonalFlip = reader.ReadUInt32();
                    reader.ReadBytes(10);
                    int numberOfBytes = (BitsPerTile / 8) * WidthInTiles * HeightInTiles;
                    Tiles = new BYTE[numberOfBytes];
                    using (var compressedStream = new ZLibStream(stream, CompressionMode.Decompress, true))
                    {
                        compressedStream.ReadExactly(Tiles, 0, numberOfBytes);
                    }
                }
                break;
            default:
                break;
        }
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write(LayerIndex);
        writer.Write(Xposition);
        writer.Write(Yposition);
        writer.Write(OpacityLevel);
        writer.Write((WORD)CelType);
        writer.Write(ZIndex);
        writer.Write([0,0,0,0,0]);

        switch (CelType)
        {
            case Chunk_Cel_CelType.RawImageData:
                {
                    writer.Write(WidthInPixels);
                    writer.Write(HeightInPixels);
                    writer.Write(RawPixelData); // TODO validate length
                    break;
                }
            case Chunk_Cel_CelType.LinkedCel:
                writer.Write(FrameLinkPosition);
                break;
            case Chunk_Cel_CelType.CompressedImage:
                {
                    writer.Write(WidthInPixels);
                    writer.Write(HeightInPixels);
                    using (var compressedStream = new ZLibStream(stream, CompressionMode.Compress, true))
                    {
                        compressedStream.Write(RawPixelData);
                    }
                }
                break;
            case Chunk_Cel_CelType.CompressedTilemap:
                writer.Write(WidthInTiles);
                writer.Write(HeightInTiles);
                writer.Write(BitsPerTile);
                writer.Write(BitmaskTileID);
                writer.Write(BitmaskXFlip);
                writer.Write(BitmaskYFlip);
                writer.Write(BitmaskDiagonalFlip);
                writer.Write(BitmaskYFlip);
                writer.Write([0,0,0,0,0,0,0,0,0,0]);
                using (var compressedStream = new ZLibStream(stream, CompressionMode.Compress, true))
                {
                    compressedStream.Write(Tiles);
                }
                break;
            default:
                break;
        }
    }
}
/// <summary>
/// Adds extra information to the latest read cel.
/// </summary>
public struct Chunk_CelExtra : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.CelExtra;
    [Flags]
    public enum Chunk_CelExtra_Flags : DWORD
    {
        PreciseBoundsAreSet = 1,
    }

    public Chunk_CelExtra_Flags Flags;
    public float PreciseX;
    public float PreciseY;
    /// <summary>
    /// Width of the cel in the sprite (scaled in real-time)
    /// </summary>
    public float Width;
    public float Height;


    public void ReadFromStream(Stream stream, Header.Header_ColorDepth header_ColorDepth)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        Flags = (Chunk_CelExtra_Flags)reader.ReadUInt32();

        PreciseX = (float)reader.ReadInt32() / 65536.0f;
        PreciseY = (float)reader.ReadInt32() / 65536.0f;
        Width   = (float)reader.ReadInt32() / 65536.0f;
        Height  = (float)reader.ReadInt32() / 65536.0f;
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write((DWORD)(Flags));
        writer.Write((int)(PreciseX * 65536));
        writer.Write((int)(PreciseY * 65536));
        writer.Write((int)(Width * 65536));
        writer.Write((int)(Height * 65536));
    }
}
/// <summary>
/// Color profile for RGB or grayscale values.
/// </summary>
public struct Chunk_ColorProfile : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.ColorProfile;
    public enum Chunk_ColorProfile_ProfileType : WORD
    {
        NoColorProfile = 0,
        sRGBProfile = 1,
        EmbeddedICCProfile = 2,
    }
    [Flags]
    public enum Chunk_ColorProfile_Flags : WORD
    {
        UseSpecialFixedGamme = 1,
    }


    public Chunk_ColorProfile_ProfileType ProfileType;
    public Chunk_ColorProfile_Flags Flags;
    /// <summary>
    /// Fixed gamma (1.0 = linear)
    ///Note: The gamma in sRGB is 2.2 in overall but it doesn't use
    ///       this fixed gamma, because sRGB uses different gamma sections
    ///       (linear and non - linear).If sRGB is specified with a fixed
    ///      gamma = 1.0, it means that this is Linear sRGB.
    /// </summary>
    public float FixedGamma;
    public BYTE[] ICCProfile;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        ProfileType = (Chunk_ColorProfile_ProfileType)reader.ReadInt16();
        Flags = (Chunk_ColorProfile_Flags)reader.ReadInt16();
        FixedGamma = (float)reader.ReadInt32()/65536.0f;
        reader.ReadBytes(8);

        if (ProfileType == Chunk_ColorProfile_ProfileType.EmbeddedICCProfile)
        {
            int length = reader.ReadInt16();
            ICCProfile = reader.ReadBytes(length);
        }
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write((WORD)ProfileType);
        writer.Write((WORD)Flags);
        writer.Write((int)(FixedGamma * 65536));
        writer.Write([0,0,0,0,0,0,0,0]);
        if (ProfileType == Chunk_ColorProfile_ProfileType.EmbeddedICCProfile)
        {
            if(ICCProfile != null)
            {
                writer.Write((SHORT)ICCProfile.Length);
                writer.Write(ICCProfile);
            }
            else 
                writer.Write((SHORT)0);
        }
    }
}
/// <summary>
/// A list of external files linked with this file can be found in the first frame. It might be used to reference external palettes, tilesets, or extensions that make use of extended properties.
/// </summary>
public struct Chunk_ExternalFiles : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.ExternalFiles;
    public enum Chunk_ExternalFiles_EntryType : BYTE
    {
        ExternalPalette = 0,
        ExternalTileset = 1,
        ExtensionNameForProperties = 2,
        ExtensionNameForTileManagement = 3,
    }

    public struct Chunk_ExternalFiles_Entry
    {
        public DWORD EntryID;
        public Chunk_ExternalFiles_EntryType Type;
        /// <summary>
        /// The extension ID must be a string like publisher/ExtensionName, for example, the Aseprite Attachment System uses aseprite/Attachment-System.
        /// This string will be used in a future to automatically link to the extension URL in the Aseprite Store.
        /// </summary>
        public string ExternalFileNameOrExtensionID;
    }

    public Chunk_ExternalFiles_Entry[] Entires;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        DWORD numberOfEntires = reader.ReadUInt32();
        reader.ReadBytes(8);
        Entires = new Chunk_ExternalFiles_Entry[numberOfEntires];
        for (int i = 0; i < Entires.Length; i++)
        {
            Entires[i].EntryID = reader.ReadUInt32();
            Entires[i].Type = (Chunk_ExternalFiles_EntryType)reader.ReadByte();
            reader.ReadBytes(7);
            STRING str = new();
            str.ReadFromStream(stream);
            Entires[i].ExternalFileNameOrExtensionID = str.ToString();
        }

    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        writer.Write((DWORD)Entires.Length);
        writer.Write([0,0,0,0,0,0,0,0]);
        for (int i = 0; i < Entires.Length; i++)
        {
            writer.Write(Entires[i].EntryID);
            writer.Write((BYTE)Entires[i].Type);
            writer.Write([0, 0, 0, 0, 0, 0, 0]);
            STRING str = new(Entires[i].ExternalFileNameOrExtensionID);
            str.WriteToStream(stream);
        }
    }

}
/// <summary>
/// After the tags chunk, you can write one user data chunk for each tag. E.g. if there are 10 tags, you can then write 10 user data chunks one for each tag.
/// </summary>
public struct Chunk_Tags : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.Tags;
    public enum Chunk_Tags_LoopAnimationDirection : BYTE
    {
        Forward = 0,
        Reverse = 1,
        PingPong = 2,
        PingPong_Reverse = 3,
    }

    public struct Chunk_Tags_Entry
    {
        public WORD FromFrame;
        public WORD ToFrame;
        public Chunk_Tags_LoopAnimationDirection LoopAnimationDirection;
        public WORD RepeatCount;
        public RGBColor TagColor;
        public STRING TagName;
        /// <summary>
        /// The extension ID must be a string like publisher/ExtensionName, for example, the Aseprite Attachment System uses aseprite/Attachment-System.
        /// This string will be used in a future to automatically link to the extension URL in the Aseprite Store.
        /// </summary>
        public string ExternalFileNameOrExtensionID;
    }

    public Chunk_Tags_Entry[] Tags;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        //TODO
        throw new NotImplementedException();
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        throw new NotImplementedException();
        //TODO
    }

}
/// <summary>
/// 
/// </summary>
public struct Chunk_Palette : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.Palette;
    /// <summary>
    /// total number of entries
    /// </summary>
    public DWORD NewPaletteSize;
    public DWORD FirstColorIndexToChange;
    public DWORD LastColorIndexToChange;

    public PaletteEntry[] paletteEntries;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        NewPaletteSize = reader.ReadUInt32();
        FirstColorIndexToChange = reader.ReadUInt32();
        LastColorIndexToChange = reader.ReadUInt32();
        reader.ReadBytes(8);
        paletteEntries = new PaletteEntry[LastColorIndexToChange - FirstColorIndexToChange + 1];
        for (DWORD i = FirstColorIndexToChange; i <= LastColorIndexToChange; i++)
        {
            paletteEntries[i].ReadFromStream(stream);
        }
    }

    public void WriteToStream(Stream stream)
    {

    }
}
/// <summary>
/// Specifies the user data (color/text/properties) to be associated with the last read chunk/object. E.g. If the last chunk we've read is a layer and then this chunk appears, this user data belongs to that layer, if we've read a cel, it belongs to that cel, etc. There are some special cases:
/// <list type="number">
/// <item>After a Tags chunk, there will be several user data chunks, one for each tag, you should associate the user data in the same order as the tags are in the Tags chunk. </item>
/// <item>After the Tileset chunk, it could be followed by a user data chunk (empty or not) and then all the user data chunks of the tiles ordered by tile index, or it could be followed by none user data chunk (if the file was created in an older Aseprite version of if no tile has user data).</item>
/// <item>In Aseprite v1.3 a sprite has associated user data, to consider this case there is an User Data Chunk at the first frame after the Palette Chunk.</item>
/// </list>
/// </summary>
public struct Chunk_UserData : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.UserData;
    
    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        throw new NotImplementedException();
    }

    public void WriteToStream(Stream stream)
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 
/// </summary>
public struct Chunk_Slice : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.Slice;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        throw new NotImplementedException();
    }

    public void WriteToStream(Stream stream)
    {
        throw new NotImplementedException();
    }
}
/// <summary>
/// 
/// </summary>
public struct Chunk_Tileset : IChunk
{
    public readonly ChunkType ChunkType => ChunkType.Tileset;

    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        throw new NotImplementedException();
    }

    public void WriteToStream(Stream stream)
    {
        throw new NotImplementedException();
    }
}
#endregion

#region Palette

public struct PaletteEntry
{
    [Flags]
    public enum PaletteEntry_Flags : WORD
    {
        HasName = 1,
    }

    public byte Red;
    public byte Green;
    public byte Blue;
    public byte Alpha;
    public string? Name; 
    
    public void ReadFromStream(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        PaletteEntry_Flags flags = (PaletteEntry_Flags)reader.ReadUInt16();
        Red = reader.ReadByte();
        Green = reader.ReadByte();
        Blue = reader.ReadByte();
        Alpha = reader.ReadByte();
        if (flags.HasFlag(PaletteEntry_Flags.HasName)) 
        {
            STRING str = new STRING();
            str.ReadFromStream(stream);
            Name = str.ToString();
        }
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);

        PaletteEntry_Flags flags = Name != null ? PaletteEntry_Flags.HasName : 0;
        writer.Write((WORD)flags);
        writer.Write(Red);
        writer.Write(Green);
        writer.Write(Blue);
        writer.Write(Alpha);

        if (Name != null)
        {
            new STRING(Name).WriteToStream(stream);
        }
    }
}
public struct Palette
{
    public List<PaletteEntry> Entries;

}
#endregion

public class Aseprite
{
    public Header Header;
    public Frame[] Frames;

    public void ReadFromStream(Stream stream)
    {
        Header.ReadFromStream(stream);

        Frames = new Frame[Header.Frames];
        for (int f = 0; f < Frames.Length; f++)
        {
            Frames[f].Header.ReadFromStream(stream);

            uint numberOfChunks = Frames[f].Header.NumberOfChunks;
            Frames[f].Chunks = new IChunk[numberOfChunks];

            for (global::System.Int32 c = 0; c < numberOfChunks; c++)
            {
                ChunkHeader chunkHeader = new();
                chunkHeader.ReadFromStream(stream);


                if (!Enum.IsDefined(chunkHeader.ChunkType))
                {
                    Debug.WriteLine("Chunk " + c + " is unkown ChunkType of " + chunkHeader.ChunkType);
                    stream.Seek(chunkHeader.SizeInBytes - 6, SeekOrigin.Current);
                }

                switch (chunkHeader.ChunkType)
                {
                    case ChunkType.PaletteOld1:
                        {
                            var chunk = new Chunk_Palette_Old1();
                            chunk.ReadFromStream(stream);
                            Frames[f].Chunks[c] = chunk;
                            break;
                        }
                    case ChunkType.Layer:
                        {
                            var chunk = new Chunk_Layer();
                            chunk.ReadFromStream(stream);
                            Frames[f].Chunks[c] = chunk;
                            break;
                        }
                    case ChunkType.Cel:
                        {
                            var chunk = new Chunk_Cel();
                            chunk.ReadFromStream(stream, Header.ColorDepth);
                            Frames[f].Chunks[c] = chunk;
                            break;
                        }
                    case ChunkType.ColorProfile:
                        {
                            var chunk = new Chunk_ColorProfile();
                            chunk.ReadFromStream(stream);
                            Frames[f].Chunks[c] = chunk;
                            break;
                        }
                    case ChunkType.Palette:
                        {
                            var chunk = new Chunk_Palette();
                            chunk.ReadFromStream(stream);
                            Frames[f].Chunks[c] = chunk;
                            break;
                        }
                    default:
                        // Skip the Chunk
                        stream.Seek(chunkHeader.SizeInBytes-6, SeekOrigin.Current);
                        break;
                }
            }
        }
    }

    public void WriteToStream(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        var position_fileStart = writer.BaseStream.Position;
        Header.Frames = (ushort)Frames.Length;
        Header.WriteToStream(stream);

        for (int f = 0; f < Frames.Length; f++)
        {
            var position_frameStart = writer.BaseStream.Position;
            Frames[f].Header.NumberOfChunks = (uint)Frames[f].Chunks.Length;
            Frames[f].Header.WriteToStream(stream);

            for (int c = 0; c < Frames[f].Header.NumberOfChunks; c++)
            {
                // Allocate Header
                var position_chunkStart = writer.BaseStream.Position;
                writer.BaseStream.Seek(6, SeekOrigin.Current);

                Frames[f].Chunks[c]?.WriteToStream(stream);

                var position_chunkEnd = writer.BaseStream.Position;
                var size_chunk =  position_chunkEnd - position_chunkStart;
                Debug.Assert(size_chunk >= 6);
                // To back to start to write header
                writer.BaseStream.Seek(position_chunkStart, SeekOrigin.Begin);
                writer.Write((DWORD)size_chunk);
                WORD ChunkType = (WORD)Frames[f].Chunks[c].ChunkType;
                writer.Write(ChunkType);

                // Restore position
                writer.BaseStream.Seek(position_chunkEnd, SeekOrigin.Begin);
            }

            var position_frameEnd = writer.BaseStream.Position;
            var size_frame = position_frameEnd-position_frameStart;

            writer.BaseStream.Seek(position_frameStart, SeekOrigin.Begin);
            writer.Write((DWORD)size_frame);
            writer.BaseStream.Seek(position_frameEnd, SeekOrigin.Begin);

        }
        var position_fileEnd = writer.BaseStream.Position;
        var size_file = position_fileEnd - position_fileStart;
        if (size_file > DWORD.MaxValue)
            throw new Exception("FileTooBig");
        writer.BaseStream.Seek(position_fileStart, SeekOrigin.Begin);
        writer.Write((DWORD)size_file);
        writer.BaseStream.Seek(position_fileEnd, SeekOrigin.Begin);
        Debug.Assert(writer.BaseStream.Position == size_file);
    }
}
