using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Saket.Typography.OpenFontFormat.Serialization;

namespace Saket.Typography.OpenFontFormat
{
    public abstract class Table : IOFFSerializable
    {
        public abstract uint Tag { get; }

        public abstract void Deserialize(OFFReader reader);
        public abstract void Serialize(OFFWriter writer);
    }
}