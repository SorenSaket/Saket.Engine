using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Windows.Native
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hwnd"></param>
    /// <param name="uMsg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    public delegate IntPtr del_WindowProc(IntPtr hwnd, WindowMessage uMsg, IntPtr wParam, IntPtr lParam);

}
