using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Platform.Windowing
{
    public interface IPlatform_Windowing
    {

        /// <summary>
        ///
        /// </summary>
        /// <returns>A new window</returns>
        public Window CreateWindow(WindowCreationArgs args);
    }
}
