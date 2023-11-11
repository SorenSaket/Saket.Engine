using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform;

/// <summary>
/// 
/// </summary>
public abstract class Display
{
    public abstract bool GetOrientation();

    public abstract int GetDPI();
}
