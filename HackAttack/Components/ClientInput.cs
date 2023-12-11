using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack.Components;

internal struct ClientInput
{
    public int targetX;
    public int targetY;


    public float axis_x;
    public float axis_y;
    public float rotation;
    public bool shooting;

}