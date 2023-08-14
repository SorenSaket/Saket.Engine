using Saket.Typography.OpenFontFormat.Serialization;
using Saket.Typography.OpenFontFormat.Tables.Required;

namespace Saket.Typography.OpenFontFormat
{
    public class OpenFont
    {
        public Dictionary<string, Table> tables = new Dictionary<string, Table>();

        public Table_offset offset;
        public Table_directory[] directories;


        public OpenFont(Stream stream)
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
                        table = new Table_hmtx(0, 0);
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
        }

    }
}