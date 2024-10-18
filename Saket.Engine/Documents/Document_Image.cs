using QoiSharp;
using QoiSharp.Codec;
using Saket.Engine.Formats.Pyxel;
using Saket.Engine.GeometryD2.Shapes;
using Saket.Engine.Graphics;
using Saket.Engine.Types;
using Saket.Serialization;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Text.Json;
using Color = Saket.Engine.Graphics.Color;

namespace Saket.Engine.Documents;

public struct ImageIndex
{
    public int index_pixel;
    public int index_layer;
    public ImageIndex()
    {
        this.index_pixel = -1;
        this.index_layer = 0;
    }
    public ImageIndex(int index_pixel, int index_layer = 0)
    {
        this.index_pixel = index_pixel;
        this.index_layer = index_layer;
    }
}

// TODO MOVE
public enum Visibility
{
    Undefined   = 0,
    Normal      = 1,
    Solo        = 2,
    Mute        = 3,
}

public class Document_Image : Document
{
    public class Layer : ISerializable
    {
        public string name;
        public int Width;
        public int Height;
        public byte[] Data;

        public Visibility visibility_global;
        public Visibility visibility_local;

        public bool locked_global;
        public bool locked_local;

        public Layer()
        {
            name = "";
            Data = [];

            visibility_local = Visibility.Normal;
            visibility_global = Visibility.Normal;
        }

        public byte this[int index]
        {
            get
            {
                
                return Data[index];
            }
            set
            {

                Data[index] = value;
            }
        }

        public void Serialize(ISerializer serializer)
        {
            serializer.Serialize(ref name);
            serializer.Serialize(ref Width);
            serializer.Serialize(ref Height);
            if (serializer.IsReader)
            {
                byte[] data = [];
                serializer.Serialize(ref data);
                QoiImage decoded = QoiSharp.QoiDecoder.Decode(data);
                Data = decoded.Data;
            }
            else
            {
                QoiImage image = new QoiImage(Data, Width, Height, Channels.RgbWithAlpha, ColorSpace.Linear);
                byte[] encoded = QoiSharp.QoiEncoder.Encode(image);
                serializer.Serialize(ref encoded);
            }
        }
    }

    public int Width;
    public int Height;

    public List<Layer> Layers;

    #region Constructors
    public Document_Image()
    {
        Width = 0;
        Height = 0;
        Layers = [];
    }

    public Document_Image(int width, int height, string name)
    {
        Width = width;
        Height = height;
        Layers = [];
    }
    #endregion

    #region Editing

    public bool TryResize(int newWidth, int newHeight, Direction anchor = Direction.Undefined)
    {
        if (newWidth <= 0 || newHeight <= 0)
            return false;

        if (newWidth == this.Width && newHeight == this.Height)
            return false;


        for (int i = 0; i < Layers.Count; i++)
        {
            byte[] newData = new byte[newWidth * newHeight * 4];
            ImageTexture.Blit(Layers[i].Data, Width, Height, newData, newWidth, newHeight, anchor);
            Layers[i].Data = newData;
        }

        Width = newWidth;
        Height = newHeight;

        return true;
    }

    public bool TryFlatten()
    {
        throw new NotImplementedException();
        
    }

    public void FillAllPixels(Color color, int layer = 0)
    {
        throw new NotImplementedException();
    }

    public ImageTexture FlattenToImage()
    {
        ImageTexture img = new ImageTexture(Width,Height);
        if (Width <= 0 || Height <= 0)
            return img;

        for (int i = Layers.Count-1; i >= 0; i--)
        {
            for (int j = 0; j < Layers[i].Data.Length; j++)
            {
                img.Data[j] = Layers[i].Data[j];
            }

        }
        return img;
    }

    public ImageTexture GetThumbnail()
    {
        int minDimension = Math.Min(Width, Height);

        ImageTexture img = new ImageTexture(minDimension, minDimension);
        
        Rectangle rect_source = new Rectangle(
            new Vector2(Width, Height) /2f, 
            new Vector2(minDimension, minDimension));

        Rectangle rect_target = new Rectangle(
          new Vector2(minDimension, minDimension) / 2f,
          new Vector2(minDimension, minDimension));

        foreach (var layer in Layers)
        {
            ImageTexture.Blit(layer.Data, Width, Height, rect_source,
                img.Data, img.Width, img.Height, rect_target);
        }

        return img;
    }

    #endregion

    #region IEnumerable Selectors
    public IEnumerable<int> Enumerator_FloodFill(Func<ImageIndex, bool> predicate, ImageIndex index_source, Direction dirs = Direction.Cardial)
    {
        HashSet<int> fill = [];

        // Positions to search
        Stack<int> positionsToSearch = new Stack<int>();
        positionsToSearch.Push(index_source.index_pixel);

        // Perform interative flood fill
        while (positionsToSearch.Count > 0)
        {
            int p = positionsToSearch.Pop();

            if (!IsValidIndex(new ImageIndex(p, index_source.index_layer)))
                continue;

            if (!fill.Contains(p) && predicate(new ImageIndex(p, index_source.index_layer))) // todo remove contains check
            {
                fill.Add(p);
                yield return p;

                foreach (var item in dirs.EnumerateFlags())
                {
                    if (item == Direction.Undefined)
                        continue;

                    if (ValidMove(p, item, Width, Height, out var index_new))
                        positionsToSearch.Push(index_new);
                }
            }
        }
    }
    public IEnumerable<int> Enumerator_GetSquareSelection(Vector2 min, Vector2 max)
    {
        min = min.Round();
        max = max.Round();

        Vector2 realMin = new Vector2(Math.Min(min.X, max.X), Math.Min(min.Y, max.Y));
        Vector2 realMax = new Vector2(Math.Max(min.X, max.X), Math.Max(min.Y, max.Y));

        realMin = Vector2.Max(realMin, Vector2.Zero);
        realMax = Vector2.Min(realMax, new Vector2(Width, Height));

        for (int y = (int)realMin.Y; y < (int)realMax.Y; y++)
        {
            for (int x = (int)realMin.X; x < (int)realMax.X; x++)
            {
                yield return (GetPixelIndex(x, y));
            }
        }
    }
    public IEnumerable<int> Enumerator_AllMatchingOnLayer(Func<ImageIndex, bool> predicate, int index_layer)
    {
        int size = Width * Height;
        for (int i = 0; i < size; i++)
        {
            if (predicate(new ImageIndex(i, index_layer)))
                yield return i;
        }
    }
    public IEnumerable<int> Enumerator_AllPixels()
    {
        int size = Width * Height;
        for (int i = 0; i < size; i++)
        {
            yield return i;
        }
    }

    #endregion

    #region Selection


    /// <summary>
    /// Returns list of connected indicies using flood fill
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="predicate"></param>
    /// <param name="index_source"></param>
    /// <returns></returns>
    public List<int> FloodFill(Func<ImageIndex, bool> predicate, ImageIndex index_source)
    {
        List<int> fill = [];

        // Positions to search
        Stack<int> positionsToSearch = new Stack<int>();
        positionsToSearch.Push(index_source.index_pixel);

        // Perform interative flood fill
        while (positionsToSearch.Count > 0)
        {
            int p = positionsToSearch.Pop();

            if (!IsValidIndex(new ImageIndex(p, index_source.index_layer)))
                continue;

            if (!fill.Contains(p) && predicate(new ImageIndex(p, index_source.index_layer))) // todo remove contains check
            {
                fill.Add(p);

                int x_base = p % (int)Width;

                if ((x_base + 1) < (int)Width)
                    positionsToSearch.Push(p + 1);
                if ((x_base - 1) >= 0)
                    positionsToSearch.Push(p - 1);

                positionsToSearch.Push(p - (int)Width);
                positionsToSearch.Push(p + (int)Width);
            }
        }

        return fill;
    }

    public List<int> GetAllMatchingOnLayer(Func<ImageIndex, bool> predicate, int index_layer)
    {
        List<int> fill = [];

        int size = Width * Height;
        for (int i = 0; i < size; i++)
        {
            if (predicate(new ImageIndex(i, index_layer)))
                fill.Add(i);
        }

        return fill;
    }




    public static void SquareTraceContour(List<int> indicies, int width, int height, out List<int> contour, out List<List<Vector2>> outlines)
    {
        List<int> unsearchedIndicies = new(indicies);
        contour = [];
        outlines = [];

        // If invalid or empty
        bool IsEmpty(int index)
        {
            return !indicies.Contains(index);
        }
        Vector2 GetPixelPosition(int index)
        {
            return new Vector2(index % width, index / width);
        }

        // Iterate all until all pixels are searched
        while (unsearchedIndicies.Count > 0)
        {
            int index_start = -1, index_next = -1;
            Direction direction_next = Direction.Up;

            // Find starting point
            // if not invalid starting point. continue
            do
            {

                index_start = unsearchedIndicies[^1];
                unsearchedIndicies.RemoveAt(unsearchedIndicies.Count - 1);

                if (!index_start.IsWithin(0, width * height))
                    continue;

                bool hasEdge = GetCardinalEdges(indicies, width, height, index_start, out var edges, out _);
                if (hasEdge)
                {
                    index_next = index_start;

                    // The first direction that is filled and the one next to it is empty
                    for (int i = 0; i < 3; i++)
                    {
                        bool validW = ValidMove(index_next, direction_next.GetNextClockwiseDirection(-2), (int)width, (int)height, out var W);
                        bool validN = ValidMove(index_next, direction_next, (int)width, (int)height, out var N);
                        bool filledFront = !IsEmpty(N);
                        bool emptyLeft = IsEmpty(W);
                        if (
                          (validN && filledFront) &&
                          (!validW || (validW && emptyLeft))
                          )
                        {
                            break;
                        }
                        else
                        {
                            // Turn right
                            direction_next = direction_next.GetNextClockwiseDirection(2);
                        }
                    }

                    break;
                }

            } while (index_next == -1 && unsearchedIndicies.Count > 0);

            // Invalid starting point
            if (index_next == -1)
                break;

            //
            contour.Add(index_start);

            var outline = new List<Vector2>();
            outlines.Add(outline);
            outline.Add(GetPixelPosition(index_start) + new Vector2(0.5f) + (direction_next.GetNextClockwiseDirection(-3).DirectionToSquareVector2() * 0.5f));//

            // trace contour until at start
            bool foundMove = false;
            do
            {
                foundMove = false;

                // Search all directions until a valid move is found
                for (int i = 0; i < 3; i++)
                {
                    bool validW = ValidMove(index_next, direction_next.GetNextClockwiseDirection(-2), (int)width, (int)height, out var W);
                    bool validN = ValidMove(index_next, direction_next, (int)width, (int)height, out var N);

                    bool filledFront = !IsEmpty(N);
                    bool emptyLeft = IsEmpty(W);

                    outline.Add(GetPixelPosition(index_next) + new Vector2(0.5f) + (direction_next.GetNextClockwiseDirection(-1).DirectionToSquareVector2() * 0.5f)); // 
                    /*
                    if(outline.Count > 2)
                    {
                        if(Extensions_Vector2.IsCollinear(outline[^3], outline[^2], outline[^1]))
                        {
                            outline.RemoveAt(outline.Count - 2);
                        }
                    }*/


                    // If the left is empty and forward is not
                    if (
                        (validN && filledFront) &&
                        (!validW || (validW && emptyLeft))
                        )
                    {
                        // The forward pixel
                        {
                            contour.Add(N);
                            int index = unsearchedIndicies.IndexOf(N);
                            if (index != -1)
                                unsearchedIndicies.RemoveAt(index);
                        }

                        // The upper left
                        bool validNW = ValidMove(index_next, direction_next.GetNextClockwiseDirection(-1), (int)width, (int)height, out var NW);
                        // If it's an inside corner
                        if ((validNW && !IsEmpty(NW)))
                        {
                            index_next = NW;
                            direction_next = direction_next.GetNextClockwiseDirection(-2);

                            outline.Add(GetPixelPosition(index_next) + new Vector2(0.5f) + (direction_next.GetNextClockwiseDirection(-1).DirectionToSquareVector2() * 0.5f)); // 

                            {
                                contour.Add(index_next);
                                int index = unsearchedIndicies.IndexOf(index_next);
                                if (index != -1)
                                    unsearchedIndicies.RemoveAt(index);
                            }
                        }
                        else
                        {
                            index_next = N;
                        }

                        foundMove = true;
                        break;
                    }
                    else
                    {
                        // Turn right
                        direction_next = direction_next.GetNextClockwiseDirection(2);
                    }
                }

                // this should never be the case?
                if (foundMove != true)
                    break;

            }
            while (index_start != index_next);

        }

    }
    public static bool ValidMove(int index_base, Direction move, int width, int height, out int index_new)
    {
        Vector2 v = move.DirectionToSquareVector2();
        index_new = GetIndexFromBaseDir(index_base, width, move);

        // Prevent vertical out of bounds
        if (index_new < 0 || index_new >= (width * height))
            return false;

        int x_base = index_base % width;

        int x_new = (x_base + (int)v.X);
        // If it's a horizontal move prevent wrapping
        if (x_new < 0 || x_new >= width)
            return false;

        return true;
    }
    public static bool GetCardinalEdges(List<int> indicies, int width, int height, int index, out Direction directions_edge, out Vector2 edgeNormal)
    {
        directions_edge = Direction.Undefined;
        edgeNormal = new Vector2(0, 0);

        if (index < 0 || index >= (width * height))
        {
            throw new Exception("Index out of range");
        }

        // If the right is the end of the canvas
        if ((index + 1) % width == 0)
        {
            directions_edge |= Direction.Right;
            edgeNormal += new Vector2(1, 0);
        }
        // If the right is empty
        if (!indicies.Contains(index + 1))
        {
            directions_edge |= Direction.Right;
            edgeNormal += new Vector2(1, 0);
        }

        // If the left is the end of the canvas
        if ((index - 1) % width < 0)
        {
            directions_edge |= Direction.Left;
            edgeNormal += new Vector2(-1, 0);
        }
        // if the left is empty
        if (!indicies.Contains(index - 1))
        {
            directions_edge |= Direction.Left;
            edgeNormal += new Vector2(-1, 0);
        }

        // if bottom is empty
        if (!indicies.Contains(index - (int)width))
        {
            directions_edge |= Direction.Down;
            edgeNormal += new Vector2(0, -1);
        }

        // if top is empty
        if (!indicies.Contains(index + (int)width))
        {
            directions_edge |= Direction.Up;
            edgeNormal += new Vector2(0, 1);
        }

        if (directions_edge == Direction.Undefined)
            return false;

        if (edgeNormal.LengthSquared() > 0)
            edgeNormal = Vector2.Normalize(edgeNormal);
        return true;
    }
    public static int GetIndexFromBaseDir(int index, int width, Direction dir)
    {
        // Prevent out of canvas
        switch (dir)
        {
            case Direction.E:
                return index + 1;
            case Direction.SE:
                return index + 1 - ((int)width);
            case Direction.S:
                return index + 0 - ((int)width);
            case Direction.SW:
                return index - 1 - ((int)width);
            case Direction.W:
                return index - 1;
            case Direction.NW:
                return index - 1 + ((int)width);
            case Direction.N:
                return index + 0 + ((int)width);
            case Direction.NE:
                return index + 1 + ((int)width);
            default:
                return index;
        }
    }

    #endregion

    #region Indexing
    public Color GetPixel(ImageIndex position)
    {
        var layer = Layers[position.index_layer];
         
        int a = position.index_pixel * 4;
        return new Color(layer[a + 2], layer[a + 1], layer[a], layer[a + 3]);
    }
    public bool SetPixel(ImageIndex position, Color color)
    {
        var layer = Layers[position.index_layer];

        int a = position.index_pixel * 4;
        layer[a + 2] = color.R;
        layer[a + 1] = color.G;
        layer[a] = color.B;
        layer[a + 3] = color.A;
        return true;
    }

    public bool IsWithinCanvas(Vector2 pos)
    {
        return pos.X >= 0 && pos.X < Width && pos.Y >= 0 && pos.Y < Height;
    }

    public bool IsValidIndex(ImageIndex pos)
    {
        return 
            pos.index_layer.IsWithin(0, Layers.Count) &&
            pos.index_pixel.IsWithin(0, (Width * Height));
    }

    /// <summary>
    /// Returns true if the index_pixel is within canvas bounds
    /// </summary>
    /// <param name="index_pixel"></param>
    /// <returns></returns>
    public bool IsValidPixelIndex(int index_pixel)
    {
        return index_pixel.IsWithin(0, (Width * Height));
    }
    public bool IsValidLayerIndex(int index_layer)
    {
        return index_layer.IsWithin(0, Layers.Count);
    }


    /// <summary>
    /// Converts from vector2 to a pixel index
    /// </summary>
    /// <param name="pixPos"></param>
    /// <returns></returns>
    public int GetPixelIndex(Vector2 pixPos)
    {
        pixPos = Extensions_Vector2.Floor(pixPos);
        return (int)pixPos.X + ((int)pixPos.Y * (int)Width);
    }
    public Vector2 GetPixelPosition(int index_pixel)
    {
        return new Vector2(index_pixel % Width, index_pixel / Width);
    }
    public int GetPixelIndex(int x, int y)
    {
        return x + (y * (int)Width);
    }

    #endregion

    #region Saving And Loading

    public override void SaveToPath(string path)
    {
        string ext = Path.GetExtension(path);

        if (ext == ".pyxel")
        {
            using FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Update);
            {
                ZipArchiveEntry? entry = zipArchive.GetEntry("docData.json");

                PyxelDocData docData = new();

                if (entry != null)
                {
                    using var stream_entry = entry.Open();
                    docData = JsonSerializer.Deserialize<PyxelDocData>(stream_entry, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                    stream_entry.Close();
                    entry.Delete();
                }

                docData.name = this.Name;

                var layerDocs = new Dictionary<string, PyxelDocData.Canvas.Layer>();
                int count = 0;
                foreach (var layer in Layers)
                {
                    var doc = new PyxelDocData.Canvas.Layer()
                    {
                        blendMode = "normal",
                        alpha = 255,
                        name = layer.name,
                        type = "tile_layer",
                        parentIndex = -1,
                        tileRefs = []
                    };

                    layerDocs.Add(count.ToString(), doc);
                    count++;
                }

                docData.canvas = new PyxelDocData.Canvas()
                {
                    width = this.Width,
                    height = this.Height,
                    tileWidth = this.Width,
                    tileHeight = this.Height,
                    numLayers = Layers.Count,
                    layers = layerDocs
                };

                {
                    entry = zipArchive.CreateEntry("docData.json");
                    using var stream_entry = entry.Open();
                    JsonSerializer.Serialize(stream_entry, docData, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true, WriteIndented = true });
                }
            }
            {
                int count = 0;
                foreach (var layer in Layers)
                {
                    string fileName = "layer" + count + ".png";
                    // Delete previous image
                    ZipArchiveEntry? entry = zipArchive.GetEntry(fileName);
                    if (entry != null)
                        entry.Delete();

                    entry = zipArchive.CreateEntry(fileName);
                    using var stream_entry = entry.Open();

                    var data_layer = new byte[layer.Data.Length];
                    //Iterate all pixels image and switch r<->b
                    for (int i = 0; i < data_layer.Length / 4; ++i)
                    {
                        data_layer[i * 4 + 0] = layer.Data[i * 4 + 2];
                        data_layer[i * 4 + 1] = layer.Data[i * 4 + 1];
                        data_layer[i * 4 + 2] = layer.Data[i * 4 + 0];
                        data_layer[i * 4 + 3] = layer.Data[i * 4 + 3];
                    }
                    StbImageWriteSharp.StbImageWrite.stbi_flip_vertically_on_write(1);
                    new StbImageWriteSharp.ImageWriter().WritePng(data_layer, (int)layer.Width, (int)layer.Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream_entry);

                    count++;
                }
            }

        }
        else if (ext ==".ase" || ext == ".aseprite")
        {

        }
        else
        {
            var image_canvas = FlattenToImage();
            image_canvas.SaveToPath(path);
        }
    }

    public override void LoadFromPath(string path)
    {
        string ext = Path.GetExtension(path);
        if (ext == ".pyxel")
        {
            using var stream = new FileStream(path, FileMode.Open);

            using ZipArchive zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);


            PyxelDocData docData = new();
            {
                ZipArchiveEntry? entry = zipArchive.GetEntry("docData.json") ?? throw new Exception("Failed to load");

                using Stream entrystream = entry.Open();

                docData = JsonSerializer.Deserialize<PyxelDocData>(entrystream, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }

            this.Layers = [];


            for (int i = 0; i < docData.canvas.numLayers; i++)
            {
                this.Layers.Add(new Layer()
                {
                    Width = docData.canvas.width,
                    Height = docData.canvas.height,
                    name = docData.canvas.layers[i.ToString()].name,
                });
            } 

            for (int i = 0; i < docData.canvas.numLayers; i++)
            {
                {
                    ZipArchiveEntry? entry_layer = zipArchive.GetEntry("layer" + i + ".png") ?? throw new Exception("Failed to load");
                    using Stream compressedStream = entry_layer.Open();
                    // Uncompress it
                    using MemoryStream uncompressedImagestream = new((int)entry_layer.Length);
                    compressedStream.CopyTo(uncompressedImagestream);
                    uncompressedImagestream.Seek(0, SeekOrigin.Begin);
                    StbImage.stbi_set_flip_vertically_on_load(1);
                    ImageResult result = ImageResult.FromStream(uncompressedImagestream, ColorComponents.RedGreenBlueAlpha);

                    ImageTexture.FlipRedBlue(result.Data);
                    this.Layers[i].Data = result.Data;
                }
            }

            this.Width = docData.canvas.width;
            this.Height = docData.canvas.height;

        }
        else if (ext == ".ase" || ext == ".aseprite")
        {

        }
        else
        {
            var image_canvas = new ImageTexture(path);
            this.Width = (int)image_canvas.Width;
            this.Height = (int)image_canvas.Height;
            Layers.Add(new() { Data = [.. image_canvas.Data], name = "Layer 0", Width = this.Width, Height = this.Height });
        }
    }

    public override void Serialize(ISerializer serializer)
    {
        serializer.Serialize(ref Width);
        serializer.Serialize(ref Height);
        serializer.Serialize(ref Layers);
    }
    /// <summary>
    /// Load image from path
    /// </summary>
    /// <param name="path"></param>
    /*public Image(string path, bool flipVertically = true)
    {
        var stream = File.ReadAllBytes(path);

        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

        ImageResult result = ImageResult.FromMemory(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

        // convert from rgba to bgra
        for (int i = 0; i < result.Width * result.Height; ++i)
        {
            byte temp = result.Data[i * 4];
            result.Data[i * 4] = result.Data[i * 4 + 2];
            result.Data[i * 4 + 2] = temp;
        }

        this.name = Path.GetFileNameWithoutExtension(path);
        this.data = result.Data;
        this.format = TextureFormat.BGRA8Unorm;
        this.width = result.Width;
        this.height = result.Height;
    }

    /// <summary>
    /// Load image from memory
    /// </summary>
    /// <param name="path"></param>
    public Image(byte[] file, string name = "image", bool flipVertically = true)
    {
        StbImage.stbi_set_flip_vertically_on_load(flipVertically ? 1 : 0);

        ImageResult result = ImageResult.FromMemory(file, StbImageSharp.ColorComponents.RedGreenBlueAlpha);

        // convert from rgba to bgra
        for (int i = 0; i < result.Width * result.Height; ++i)
        {
            byte temp = result.Data[i * 4];
            result.Data[i * 4] = result.Data[i * 4 + 2];
            result.Data[i * 4 + 2] = temp;
        }

        this.name = name;
        this.data = result.Data;
        this.format = TextureFormat.BGRA8Unorm;
        this.width = result.Width;
        this.height = result.Height;
    }

    /// <summary>
    /// Save Image to path
    /// </summary>
    /// <param name="path"></param>
    public void SaveToPath(string path, bool flipVertically = true)
    {
        string ext = Path.GetExtension(path);


        byte[] data = new byte[Data.Length];

        // convert back from bgra to rgba
        for (int i = 0; i < data.Length / 4; ++i)
        {
            data[i * 4 + 0] = Data[i * 4 + 2];
            data[i * 4 + 1] = Data[i * 4 + 1];
            data[i * 4 + 2] = Data[i * 4 + 0];
            data[i * 4 + 3] = Data[i * 4 + 3];
        }

        // TODO covert back to rgba
        StbImageWriteSharp.StbImageWrite.stbi_flip_vertically_on_write(flipVertically ? 1 : 0);

        using (Stream stream = File.OpenWrite(path))
        {
            var w = new StbImageWriteSharp.ImageWriter();
            switch (ext)
            {
                case ".png":
                    w.WritePng(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                    break;
                case ".jpeg":
                case ".jpg":
                    w.WriteJpg(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream, 100);
                    break;
                case ".bmp":
                    w.WriteBmp(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                    break;
                case ".tga":
                    w.WriteTga(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                    break;
                case ".hdr":
                    w.WriteHdr(data, (int)this.width, (int)this.height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, stream);
                    break;
                default:
                    break;
            }
        }
    }*/
    #endregion
}