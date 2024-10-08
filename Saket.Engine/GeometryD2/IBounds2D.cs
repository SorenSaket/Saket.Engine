using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GeometryD2;


/// <summary>
/// 
/// </summary>
public interface IBounds2D
{
    public BoundingBox2D GetBounds();
}
