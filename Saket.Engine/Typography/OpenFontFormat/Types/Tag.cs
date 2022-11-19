using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Filetypes.Font.OpenFontFormat
{
    public struct Tag
    {
		public uint value;

        public Tag(uint value)
        {
            this.value = value;
        }

        public static implicit operator Tag(uint packed)
        {
            return new Tag(packed);
        }
    }
}
