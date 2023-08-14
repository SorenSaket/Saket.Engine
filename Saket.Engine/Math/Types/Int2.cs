
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Math.Types
{
    public struct Int2
    {
        public int X; 
        public int Y;
        
        public Int2(int value) : this(value, value)
        {
        }

        public Int2(int x, int y)
        {
            this.X= x; 
            this.Y = y;
        }

        public static Int2 Zero
        {
            get => default;
        }

        /// <summary>Gets a vector whose 2 elements are equal to one.</summary>
        /// <value>A vector whose two elements are equal to one (that is, it returns the vector <c>(1,1)</c>.</value>
        public static Int2 One
        {
            get => new Int2(1,0);
        }

        /// <summary>Gets the vector (1,0).</summary>
        /// <value>The vector <c>(1,0)</c>.</value>
        public static Int2 UnitX
        {
            get => new Int2(1, 0);
        }

        /// <summary>Gets the vector (0,1).</summary>
        /// <value>The vector <c>(0,1)</c>.</value>
        public static Int2 UnitY
        {
            get => new Int2(0, 1);
        }

    }
}
