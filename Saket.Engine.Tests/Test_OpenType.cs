
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saket.Engine.Typography.OpenFontFormat.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Tests
{
    [TestClass]
    public class Test_OpenType
    {

        [TestMethod]
        public void Reader()
        {
            byte[] buffer = new byte[2] { 0b01011000, 0b00011011 };
            MemoryStream stream = new MemoryStream(buffer);
            OFFReader reader = new OFFReader(stream);
            reader.LoadBytes(2);
            ushort r = 0;
            reader.ReadUInt16(ref r);

            Assert.AreEqual(7000, r);
        }


        /*
        [TestMethod]
        public void F2DOT14()
        {
            float a1 = new F2DOT14(0x7fff).Value;
            float a2 = new F2DOT14(0x7000).Value;
            float a3 = new F2DOT14(0x0001).Value;
            float a4 = new F2DOT14(0x0000).Value;
            float a5 = new F2DOT14(0xffff).Value;
            float a6 = new F2DOT14(0x8000).Value;

            Assert.IsTrue(MathF.Abs(1.999939f - a1) < 0.0001);
            Assert.IsTrue(MathF.Abs(1.75f - a2) < 0.0001);
            Assert.IsTrue(MathF.Abs(0.000061f - a3) < 0.0001);
            Assert.IsTrue(MathF.Abs(0.0f - a4) < 0.0001);
            Assert.IsTrue(MathF.Abs(-0.000061f - a5) < 0.1);
            Assert.IsTrue(MathF.Abs(-2.0f - a6) < 0.0001);

        }*/
    }
}
