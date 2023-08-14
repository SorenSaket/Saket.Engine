using Saket.ECS;
using Saket.Engine.GUI.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI.Layouting
{
    public class IterativeLayout
    {
        static Query query = new Query().With<(Widget, HierarchyEntity, GUILayout, Style)>();


        public void LayoutWorld(World world)
        {


            var entities = world.Query(query);

            bool finished = true;

            do
            {
                finished = true;



                foreach (var entitiy in entities)
                {

                }


                // https://www.rfleury.com/p/ui-part-2-build-it-every-frame-immediate
                // [Any] Calculate fixed

                // [Top down] parent dependent sizing (percentage)

                // [Bottom up] ( stretch)

                // [top down] Solve overflow






            } while (!finished);

        }
    }
}
