using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI
{
    // Input: of the layouting should be the elements
    // Output: another type of element? with absolute positions only????+
    internal class Layouting
    {
        /*
        public void dotheThingy(World world, int screenWidth, int screenHeight)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (!layout.Dirty[i])
                    continue;

                // If no parent exist
                if (elements.Parents[i] == -1)
                {
                    if(elements.PositionXMeasurement[i] == Measurement.Percentage)
                        layout.PositionX[i] = screenWidth * elements.PositionX[i];
                    else
                        layout.PositionX[i] = elements.PositionX[i];

                    if (elements.PositionYMeasurement[i] == Measurement.Percentage)
                        layout.PositionY[i] = screenHeight * elements.PositionY[i];
                    else
                        layout.PositionY[i] = elements.PositionY[i];

                    if (elements.WidthMeasurement[i] == Measurement.Percentage)
                        layout.Width[i] = screenWidth * elements.Width[i];
                    else
                        layout.Width[i] = elements.Width[i];

                    if (elements.HeightMeasurement[i] == Measurement.Percentage)
                        layout.Height[i] = screenHeight * elements.Height[i];
                    else
                        layout.Height[i] = elements.Height[i];

                    layout.Dirty[i] = false;
                }
            }
        }*/
    }

    public struct LayoutELement
    {
        public float x;
        public float y;
        public float w;
        public float h;
        public bool Dirty;
    }
}