using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HackAttack.Components;

internal struct Player
{
    public ClientInput input;
    public ECSPointer buildobj;
    public Vector2 lastPosition;
}
