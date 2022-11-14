using Saket.Engine.Filetypes.Font.OpenFontFormat;
using Saket.Engine.Filetypes.Font.OpenFontFormat.Tables;
using Saket.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace Saket.Engine.Filetypes.Font.TrueType
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
				switch (directories[i].tableName)
				{
					case "cmap":
						Table_cmap cmap = new Table_cmap();
						stream.Seek(directories[i].offset, SeekOrigin.Begin);
						cmap.Deserialize(reader);
						tables.Add("cmap", cmap);
						break;
					default:
						continue;
				}
			}


        }

    }
}
