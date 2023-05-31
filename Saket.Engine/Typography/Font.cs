using Saket.Engine.Filetypes.Font.OpenFontFormat.Tables;
using Saket.Engine.Filetypes.Font.OpenFontFormat;
using Saket.Engine.Math.Geometry;
using Saket.Engine.Typography.OpenFontFormat.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Saket.Engine.Typography
{
    /// <summary>
    /// Common font format for the engine. 
    /// </summary>
    public class Font
    {
		// This should store everything nessary to render the text
        // Character mapping, glyphs
        // Layout info

        public Dictionary<char, Shape> glyphs = new Dictionary<char, Shape>();


        public void LoadFromOFF(Stream stream)
        {
            var reader = new OFFReader(stream);

            Table_offset offset = new Table_offset();
            offset.Deserialize(reader);

            Dictionary<string, Table_directory> directories = new (offset.numTables);

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
            //Table_hhea hhea = LoadTable("hhea", new Table_hhea()); 
            //Table_hmtx hmtx = LoadTable("hmtx", new Table_hmtx()); 
            //Table_name name = LoadTable("name", new Table_name()); 
            //Table_OS2 os2 = LoadTable("os2", new Table_OS2()); 

            // Load Glyphs
            Table_loca loca = LoadTable("loca", new Table_loca(maxp.numGlyphs, head.indexToLocFormat));

            //Table_glyf glyf = LoadTable("loca", new Table_glyf(maxp.numGlyphs));
            uint offset_glyftable = directories["glyf"].offset;


            Dictionary<int, int> mapping = cmap.characterMaps[0].MapToDictionary();

            int index = mapping[66];
            uint location = loca.GetLocation(index);

            stream.Seek(offset_glyftable+ location, SeekOrigin.Begin);




            glyphs.Add('a', ReadGlyth(reader));
        }

        public Shape ReadGlyth(OFFReader reader)
        {
            // Read header of glyph
            Table_glyf.GlyphHeader gh = new Table_glyf.GlyphHeader();
            gh.Deserialize(reader);

            if (gh.numberOfContours >= 0)
            {
                Table_glyf.SimpleGlyph sg = new Table_glyf.SimpleGlyph();
                sg.Deserialize(reader, gh.numberOfContours);

                Spline2D[] splines = new Spline2D[gh.numberOfContours];

                Vector2 position = Vector2.Zero;
                Table_glyf.SimpleGlyph.Flags flags_previous = new Table_glyf.SimpleGlyph.Flags();
                int last = 0;

                for (int i = 0; i < gh.numberOfContours; i++)
                {
                    List<Vector2> points = new List<Vector2>();
                    
                    // For each point in countor
                    for (int y = last; y < sg.endPtsOfContours[i]+1; y++)
                    {
                        Vector2 delta = new Vector2(sg.xCoordinates[y], sg.yCoordinates[y]);
                        // If the previous point is the same as this point an extra point should be placed between
                        // If both are ONCURVE this is a straight line
                        if ((sg.flags[y] & Table_glyf.SimpleGlyph.Flags.ON_CURVE_POINT) == 
                            (flags_previous & Table_glyf.SimpleGlyph.Flags.ON_CURVE_POINT) && y != last)
                        {
                            points.Add((position + (delta+position) )/2);
                        }

                        position += delta;
                        points.Add(position);
                        
                        flags_previous = sg.flags[y];
                    }

                    if(points.Last() != points.First())
                    {
                        // Add last point
                        points.Add((position + points[0]) / 2);
                        points.Add(points[0]);
                    }

                    last = sg.endPtsOfContours[i]+1; // TODO VALIDATE

                    splines[i] = new Spline2D(points);
                }

                return new Shape(splines);
            }
            else
            {
                // Composite Glyph
            }

            return null;
        }

        


    }
}