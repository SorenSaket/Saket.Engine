using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Saket.Engine.Graphics.Text;
using Saket.Engine.Math.Geometry;
using Saket.Engine.Resources.Loaders;
using Saket.Typography.OpenFontFormat;
using Saket.Typography.OpenFontFormat.Serialization;
using Saket.Typography.OpenFontFormat.Tables.Required;
using Saket.Typography.OpenFontFormat.Tables.Truetype;

namespace Saket.Engine.Resources.Loaders
{
    public class LoaderFont : ResourceLoader<Font>
    {
        public override Font Load(string name, ResourceManager resourceManager)
        {
            if (resourceManager.TryGetStream(name, out var stream))
            {
                Font font = new();


                var reader = new OFFReader(stream);

                Table_offset offset = new Table_offset();
                offset.Deserialize(reader);

                Dictionary<string, Table_directory> directories = new(offset.numTables);

                for (int i = 0; i < offset.numTables; i++)
                {
                    var d = new Table_directory();
                    d.Deserialize(reader);
                    directories.Add(d.tableName, d);
                }

                T LoadTable<T>(string name, T table) where T : Table
                {
                    if (!directories.ContainsKey(name))
                        throw new Exception($"OFF has missing table {name}");
                    stream.Seek(directories[name].offset, SeekOrigin.Begin);
                    table.Deserialize(reader);
                    return table;
                }


                // Required Tables : cmap, head, hhea, hmtx, maxp, name, OS/2, post
                Table_cmap cmap = LoadTable("cmap", new Table_cmap());
                Table_maxp maxp = LoadTable("maxp", new Table_maxp());

                Table_head head = LoadTable("head", new Table_head());

                font.unitsPerEm = head.unitsPerEm;

                //Table_hhea hhea = LoadTable("hhea", new Table_hhea()); 
                //Table_hmtx hmtx = LoadTable("hmtx", new Table_hmtx()); 
                //Table_name name = LoadTable("name", new Table_name()); 
                //Table_OS2 os2 = LoadTable("os2", new Table_OS2()); 

                // Load Glyphs
                Table_loca loca = LoadTable("loca", new Table_loca(maxp.numGlyphs, head.indexToLocFormat));

                //Table_glyf glyf = LoadTable("loca", new Table_glyf(maxp.numGlyphs));
                uint offset_glyftable = directories["glyf"].offset;

                // Get suitable mapping


                Dictionary<int, int> mapping = null;


                for (int i = 0; i < cmap.characterMaps.Length; i++)
                {
                    if (cmap.characterMaps[i]?.format == 4)
                    {
                        mapping = cmap.characterMaps[0].MapToDictionary();
                    }
                }



                foreach (var item in mapping)
                {
                    uint location = loca.GetLocation(item.Value);

                    stream.Seek(offset_glyftable + location, SeekOrigin.Begin);
                    // Read header of glyph
                    Table_glyf.GlyphHeader gh = new Table_glyf.GlyphHeader();
                    gh.Deserialize(reader);

                    StyledShapeCollection shape = ReadShape(gh, reader);
                    /*
                    if(shape != null)
                    {
                        // Adjust for Unitsperem
                        for (int q = 0; q < shape.Count; q++)
                        {
                            for (int w = 0; w < shape[q].points.Count; w++)
                            {
                                shape[q].points[w] -= new Vector2(gh.xMin, gh.yMin);
                                shape[q].points[w] /= unitsPerEm;
                            }
                        }
                    }*/

                   font. glyphs.Add((char)item.Key,
                        new Glyph(shape,
                        (gh.xMax - gh.xMin) / (float )font.unitsPerEm,
                        (gh.yMax - gh.yMin) / (float)font.unitsPerEm)
                        );
                    return font;
                }
            }
            throw new Exception("Failed to load font");
        }

        public static StyledShapeCollection ReadShape(Table_glyf.GlyphHeader gh, OFFReader reader)
        {
            if (gh.numberOfContours >= 0)
            {
                Table_glyf.SimpleGlyph sg = new Table_glyf.SimpleGlyph();
                sg.Deserialize(reader, gh.numberOfContours);

                List<IShape> splines = new(gh.numberOfContours);

                Vector2 position = Vector2.Zero;
                Table_glyf.SimpleGlyph.Flags flags_previous = new Table_glyf.SimpleGlyph.Flags();
                int last = 0;

                for (int i = 0; i < gh.numberOfContours; i++)
                {
                    List<Vector2> points = new List<Vector2>();

                    // For each point in countor
                    for (int y = last; y < sg.endPtsOfContours[i] + 1; y++)
                    {
                        Vector2 delta = new Vector2(sg.xCoordinates[y], sg.yCoordinates[y]);
                        // If the previous point is the same as this point an extra point should be placed between
                        // If both are ONCURVE this is a straight line
                        if ((sg.flags[y] & Table_glyf.SimpleGlyph.Flags.ON_CURVE_POINT) ==
                            (flags_previous & Table_glyf.SimpleGlyph.Flags.ON_CURVE_POINT) && y != last)
                        {
                            points.Add((position + (delta + position)) / 2);
                        }

                        position += delta;
                        points.Add(position);

                        flags_previous = sg.flags[y];
                    }

                    if (points.Last() != points.First())
                    {
                        // Add last point
                        points.Add((position + points[0]) / 2);
                        points.Add(points[0]);
                    }

                    last = sg.endPtsOfContours[i] + 1; // TODO VALIDATE

                    splines[i] = new Spline2D(points);
                }

                return new StyledShapeCollection(splines);
            }
            else
            {
                // Composite Glyph
            }

            return null;
        }
    }
}