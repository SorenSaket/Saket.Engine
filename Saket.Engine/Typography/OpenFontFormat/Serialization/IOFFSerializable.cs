using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Typography.OpenFontFormat.Serialization
{
    internal interface IOFFSerializable
    {
        public void Serialize(OFFWriter writer);
        public void Deserialize(OFFReader reader);
    }
}
