using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Networking
{
    internal interface IInterpolatable<T>
    {
        public void Interpolate(T a, T b, float t);
    }
}