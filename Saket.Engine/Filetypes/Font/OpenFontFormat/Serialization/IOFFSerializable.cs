using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat
{
    internal interface IOFFSerializable
    {
        public void Serialize(OFFWriter reader);
        public void Deserialize(OFFReader reader);
    }
}
