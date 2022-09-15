using Saket.ECS;
using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace TechWars.Shared
{
    public static class Utils
    {

        public static PlayerState GetPlayerState(Entity e)
        {
            Transform2D? tranform = e.TryGet<Transform2D>();

            if (tranform == null)
                return default;


            return new PlayerState(tranform.Value.x, tranform.Value.y);
        }

    }
}
