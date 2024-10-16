using Saket.Engine.Graphics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Text.Json;

namespace Saket.Engine.Formats.Pyxel;

[System.Serializable]
public struct PyxelDocData
{
    public string name { get; set; }
    public Palette palette { get; set; }
    public string version { get; set; }
    public Canvas canvas { get; set; }
    public Tileset tileset { get; set; }
    public Settings settings { get; set; }
    public Dictionary<string, Animation> animations { get; set; }

    public struct Canvas
    {
        public Dictionary<string, Layer> layers { get; set; }
        public int numLayers { get; set; }
        public int width { get; set; }
        public int tileWidth { get; set; }
        public int height { get; set; }
        public int tileHeight { get; set; }
        public int currentLayerIndex { get; set; }

        public struct Layer
        {
            public struct TileRef
            {
                public bool flipX { get; set; }
                public int index { get; set; }
                public int rot { get; set; }
            }

            public bool hidden { get; set; }
            public int alpha { get; set; }
            public string name { get; set; }
            public int parentIndex { get; set; }
            public string type { get; set; }
            public bool collapsed { get; set; }
            public bool muted { get; set; }
            public Dictionary<string, TileRef> tileRefs { get; set; }
            public string blendMode { get; set; }
            public bool soloed { get; set; }
        }
    }
    public struct Tileset
    {
        public uint tileWidth { get; set; }
        public uint numTiles { get; set; }
        public uint tilesWide { get; set; }
        public uint tileHeight { get; set; }
        public bool fixedWith { get; set; }
    }
    public struct Palette
    {
        public int height { get; set; }
        public int numColors { get; set; }
        public Dictionary<string, string> colors { get; set; }
        public int width { get; set; }
    }
    public struct Settings
    {
        public string ExportImagePanel_prefOverwrite { get; set; }
        public string ExportImagePanel_prefFormat { get; set; }
        public string ExportImagePanel_prefSeparateFiles { get; set; }
        public string ExportImagePanel_prefPath { get; set; }
        public string ExportImagePanel_prefFileName { get; set; }
        public string ExportImagePanel_prefTranspMatteColor { get; set; }
        public string ExportImagePanel_prefScaling { get; set; }
    }
    public struct Animation
    {
        public string name { get; set; }
        public int basetile { get; set; }
        public int frameDuration { get; set; }
        public int length { get; set; }
        public List<int> FrameDurationMultipliers { get; set; }
    }

}
    
public static class Pyxel
{
    public static TextureAtlas Load(Stream stream)
    {
        ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);

        ZipArchiveEntry? entry = zipArchive.GetEntry("docData.json") ?? throw new Exception("Failed to load");

        var entrystream = new StreamReader(entry.Open()).ReadToEnd();


        PyxelDocData docData = JsonSerializer.Deserialize<PyxelDocData>(entrystream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

        //entrystream.Close();
            
        StbImage.stbi_set_flip_vertically_on_load(1);
        ZipArchiveEntry? entry_layer0 = zipArchive.GetEntry("layer0.png") ?? throw new Exception("Failed to load");

        var imageStream = entry_layer0.Open();// new DeflateStream(entry_layer0.Open(), CompressionMode.Decompress);
        MemoryStream uncompressedImagestream = new((int)entry_layer0.Length);
        imageStream.CopyTo(uncompressedImagestream);

        imageStream.Close();

        uncompressedImagestream.Seek(0, SeekOrigin.Begin);

        ImageResult result = ImageResult.FromStream(uncompressedImagestream, ColorComponents.RedGreenBlueAlpha);

        // convert from rgba to bgra
        for (int i = 0; i < result.Width * result.Height; ++i)
        {
            byte temp = result.Data[i * 4];
            result.Data[i * 4] = result.Data[i * 4 + 2];
            result.Data[i * 4 + 2] = temp;
        }

        ImageTexture image = new ImageTexture(result.Data, result.Width, result.Height);

           
        int columns = docData.canvas.width / docData.canvas.tileWidth;
        int rows = docData.canvas.height / docData.canvas.tileHeight;

        return new TextureAtlas(image, columns, rows);
            
        throw new Exception("Failed to load");
    }
}
