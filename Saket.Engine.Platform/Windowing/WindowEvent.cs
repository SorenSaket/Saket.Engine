using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Windowing
{
    public enum WindowEvent
    {
        None = 0,
        Create,
        Destroy,
        Move,
        Resize,
        Activate,
        SetFocus,
        KillFocus,
        Enable,
        SetRedraw,
        SetText,
        GetText,
        GetTextLength,
        Paint,
        Close,
        Exit,
    }
}
