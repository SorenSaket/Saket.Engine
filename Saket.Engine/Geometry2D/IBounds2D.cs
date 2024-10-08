using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Geometry2D;


/// <summary>
/// 
/// </summary>
public interface IBounds2D
{
    public BoundingBox2D GetBounds();
}
