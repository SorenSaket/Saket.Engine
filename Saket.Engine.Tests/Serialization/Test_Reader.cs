using Saket.Engine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Tests.Serialization
{
    public struct TestSerializable : ISerializable
    {
        public int baseValue;
        public int value;
        public int[] modifiers;

        public TestSerializable(int baseValue, int value, int[] modifiers)
        {
            this.baseValue = baseValue;
            this.value = value;
            this.modifiers = modifiers;
        }

        public void Deserialize(ref SerializerReader reader)
        {
            baseValue = reader.Read<int>();
            value = reader.Read<int>();
            modifiers = reader.ReadArray<int>();
        }

        public override bool Equals(object? obj)
        {
            return obj is TestSerializable serializable &&
                   baseValue == serializable.baseValue &&
                   value == serializable.value &&
                   Enumerable.SequenceEqual(modifiers, serializable.modifiers);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(baseValue, value, modifiers);
        }

        public void Serialize(SerializerWriter writer)
        {
            writer.Write(baseValue);
            writer.Write(value);
            writer.Write(modifiers);
        }

        public static bool operator ==(TestSerializable left, TestSerializable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TestSerializable left, TestSerializable right)
        {
            return !(left == right);
        }
    }
    public enum TestEnumUShort : ushort
    {
        hello = 0,
        world = 9000,
        max = 65535
    }


    [TestClass]
    public class Test_Reader
    {
       

        [TestMethod]
        public void Creation_Array()
        {
            byte[] data = new byte[64];
            var reader = new SerializerReader(ref data,10);

            Assert.AreEqual(10, reader.AbsolutePosition);
            Assert.AreEqual(0, reader.RelativePosition);

        }
        [TestMethod]
        public void Creation_ArraySegment()
        {
            byte[] data = new byte[64];
            var reader = new SerializerReader(new ArraySegment<byte>(data, 10, 12));

            Assert.AreEqual(10, reader.AbsolutePosition);
            Assert.AreEqual(0, reader.RelativePosition);
            Assert.AreEqual(12, reader.Capacity);
        }


        [TestMethod]
        public void Read_Primitive()
        {
            byte[] data = new byte[64];
            var reader = new SerializerReader(ref data,4);
            reader.Read<float>();
            Assert.AreEqual(8, reader.AbsolutePosition);
            Assert.AreEqual(4, reader.RelativePosition);
        }
        [TestMethod]
        public void Read_Enum()
        {
            byte[] data = new byte[64];
            var reader = new SerializerReader(ref data, 4);
            reader.Read<TestEnumUShort>();
            Assert.AreEqual(6, reader.AbsolutePosition);
            Assert.AreEqual(2, reader.RelativePosition);
        }
        [TestMethod]
        public void Read_Serializable()
        {
            
        }


        [TestMethod]
        public void DebugSafety()
        {
            Assert.ThrowsException<IndexOutOfRangeException>(() => {
                byte[] data = new byte[64];
                var reader = new SerializerReader(ref data, 64);
                reader.Read<float>(); 
            });
        }

    }
}
