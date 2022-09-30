using Saket.Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Serialization
{
    /// <summary>
    /// Interface for implementing custom serializable types.
    /// </summary>
    public interface ISerializable
    {
        void Serialize(SerializerWriter writer);
        void Deserialize(SerializerReader reader);
    }
}
