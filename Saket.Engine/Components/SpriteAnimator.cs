using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Components
{
    public struct SpriteAnimator
    {
        public int animation;
        public float speed = 1f;
        public float timer;

        public SpriteAnimator(int animation, float speed = 1)
        {
            this.animation = animation;
            this.speed = speed;
            this.timer = 0;
        }
    }
}
