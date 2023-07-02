using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI.Styling
{
    public struct Selector
    {
        public string? id;
        public string[] classes;
        public Selector(string? id = null, params string[] classes)
        {
            this.id = id;
            this.classes = classes;
        }
    }
}