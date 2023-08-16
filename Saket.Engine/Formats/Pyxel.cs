using Saket.Engine.Graphics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Saket.Engine.Formats
{
    [System.Serializable]
    public struct PyxelDocData
    {
        public string name { get; set; }
        public Palette palette { get; set; }
        // settings
        // animations
        public string version { get; set; }
        public Canvas canvas { get; set; }
        public Tileset tileset { get; set; }

        public struct Canvas
        {
            public Dictionary<string, Layer> layers { get; set; }
            public uint numLayers { get; set; }
            public uint width { get; set; }
            public uint tileWidth { get; set; }
            public uint height { get; set; }
            public uint tileHeight { get; set; }
            public uint currentLayerIndex { get; set; }

            public struct Layer
            {
                public bool hidden { get; set; }
                public byte alpha { get; set; }
                public string name { get; set; }
                public int parentIndex { get; set; }
                public string type { get; set; }
                public bool collapsed { get; set; }
                public bool muted { get; set; }
                // tilerefs
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
            public uint height { get; set; }
            public uint numColors { get; set; }
            public Dictionary<string, string> colors { get; set; }
            public uint width { get; set; }
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

            ImageResult resutl = ImageResult.FromStream(uncompressedImagestream, ColorComponents.RedGreenBlueAlpha);

            Image image = new Image(resutl.Data, (uint)resutl.Width, (uint)resutl.Height); 


            uint columns = docData.canvas.width / docData.canvas.tileWidth;
            uint rows = docData.canvas.height / docData.canvas.tileHeight;

            return new TextureAtlas(image, columns, rows);
            
            throw new Exception("Failed to load");
        }
    }
}