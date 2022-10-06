using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Net
{
    public interface IInterpolatable<T>
    {
        public void Interpolate(T a, T b, float t);
    }
}