using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI
{
    public struct Window
    {
        
    }

    public static partial class Components
    {
        
    }

    public static partial class Systems
    {
        private static readonly Query query_window = new Query().With<Window>();

        public static void Window_Render(World world)
        {
            UIEvent e = world.GetResource<UIEvent>();

            var result = world.Query(query_window);
        }
    }
}