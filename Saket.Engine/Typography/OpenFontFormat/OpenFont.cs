using Saket.Engine.Filetypes.Font.OpenFontFormat;
using Saket.Engine.Filetypes.Font.OpenFontFormat.Tables;
using Saket.Engine.Typography.OpenFontFormat.Serialization;
using Saket.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Saket.Engine.Typography
{
    public class OpenFont
    {
        public Dictionary<string, Table> tables = new Dictionary<string, Table>();

        public Table_offset offset;
        public Table_directory[] directories;


        public OpenFont (Stream stream)
        {
            var reader = new OFFReader(stream);

            offset = new Table_offset();
            offset.Deserialize(reader);

            directories = new Table_directory[offset.numTables];

            for (int i = 0; i < offset.numTables; i++)
            {
                directories[i] = new Table_directory();
                directories[i].Deserialize(reader);
            }





            for (int i = 0; i < offset.numTables; i++)
			{
                Table table;

                switch (directories[i].tableName)
				{
					case "cmap":
                        table = new Table_cmap();
						break;
                    case "head":
                        table = new Table_head();
                        break;
                    case "hhea":
                        table = new Table_hhea();
                        break;
                    case "hmtx":
                        table = new Table_hmtx(0,0);
                        break;
                    case "maxp":
                        table = new Table_maxp();
                        break;
                    case "name":
                        table = new Table_name();
                        break;
                    case "os2":
                        table = new Table_OS2();
                        break;
                    default:
						continue;
				}

                stream.Seek(directories[i].offset, SeekOrigin.Begin);
                table.Deserialize(reader);
                tables.Add(directories[i].tableName, table);
            }


            // Required Tables : cmap, head, hhea, hmtx, maxp, name, OS/2, post
            Table_cmap cmap = (Table_cmap)tables["cmap"];
            Table_head head = (Table_head)tables["head"];
            Table_hhea hhea = (Table_hhea)tables["hhea"];
            Table_hmtx hmtx = (Table_hmtx)tables["hmtx"];
            Table_maxp maxp = (Table_maxp)tables["maxp"];
            Table_name name = (Table_name)tables["name"];
            Table_OS2 os2   = (Table_OS2)tables["os2"];

            // Load Glyphs
            if (directories.Any(x=>x.tableName == "glyf"))
            {
                Table_loca loca = new Table_loca(maxp.numGlyphs, head.indexToLocFormat);
                stream.Seek(directories.First(x => x.tableName == "loca").offset, SeekOrigin.Begin);
                loca.Deserialize(reader);


                uint glyfOffset = directories.First(x => x.tableName == "glyf").offset;
                
            }
        }

    }
}