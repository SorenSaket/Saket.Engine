using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Bundles
{
    public class BundleSprite : Saket.ECS.Bundle
    {
        public override Type[] Components => components;
        public override object[] Data => data;


        private readonly static Type[] components = new Type[] { typeof(Sprite), typeof(Transform2D)};
        private object[] data;

        public BundleSprite(Sprite sprite, Transform2D? transform )
        {
            data = new object[2]
            {
                sprite,
                transform.GetValueOrDefault()
            };
        }
    }
}
