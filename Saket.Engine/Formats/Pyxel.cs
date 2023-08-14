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
    public struct PyxelDocData
    {
        public string name;
        public Palette palette;
        // settings
        // animations
        public string version;
        public Canvas canvas;
        public Tileset tileset;

        public struct Canvas
        {
            public Dictionary<string, Layer> layers;
            public uint numLayers;
            public uint width;
            public uint tileWidth;
            public uint height;
            public uint tileHeight;
            public uint currentLayerIndex;

            public struct Layer
            {
                public bool hidden;
                public byte alpha;
                public string name;
                public int parentIndex;
                public string type;
                public bool collapsed;
                public bool muted;
                // tilerefs
                public string blendMode;
                public bool soloed;
            }
        }
        public struct Tileset
        {
            public uint tileWidth;
            public uint numTiles;
            public uint tilesWide;
            public uint tileHeight;
            public bool fixedWith;
        }

        public struct Palette
        {
            public uint height;
            public uint numColors;
            public Dictionary<string, string> colors;
            public uint width;
        }
    }
    public static class Pyxel
    {
        public static TextureAtlas LoadFromPyxel(Stream stream)
        {
            ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);

            ZipArchiveEntry? entry = zipArchive.GetEntry("docData.json") ?? throw new Exception("Failed to load");

            var entrystream = new StreamReader(entry.Open()).ReadToEnd();


            PyxelDocData docData = JsonSerializer.Deserialize<PyxelDocData>(entrystream);

            //entrystream.Close();

            StbImage.stbi_set_flip_vertically_on_load(1);
            ZipArchiveEntry? entry_layer0 = zipArchive.GetEntry("layer0.png") ?? throw new Exception("Failed to load");

            var imageStream = entry_layer0.Open();// new DeflateStream(entry_layer0.Open(), CompressionMode.Decompress);
            MemoryStream uncompressedImagestream = new((int)entry_layer0.Length);
            imageStream.CopyTo(uncompressedImagestream);

            ImageResult image = ImageResult.FromStream(uncompressedImagestream, ColorComponents.RedGreenBlueAlpha);

            Image tex = new Image(image.Width, image.Height);
            tex.data = image.Data;

            imageStream.Close();

            uint columns = docData.canvas.width / docData.canvas.tileWidth;
            uint rows = docData.canvas.height / docData.canvas.tileHeight;

            return new TextureAtlas(tex, columns, rows);

            throw new Exception("Failed to load");
        }
    }
}