using Saket.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechWars.Shared;

namespace TechWars
{
    internal class BundlePlayer : Saket.ECS.Bundle
    {
        public override Type[] Components => components;
        public override object[] Data => data;


        private readonly static Type[] components = new Type[] { typeof(Transform2D), typeof(Velocity), typeof(Player), typeof( Sprite) };
        private object[] data;

        public BundlePlayer(Player player, Transform2D transform)
        {
            data = new object[4]
            {
                transform,
                new Velocity(),
                player,
                new Sprite(0,0,uint.MaxValue)
            };
        }
    }
}
