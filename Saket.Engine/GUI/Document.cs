using Saket.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.UI
{
    public class Document
    {
        // string are managed
        public List<string> strings = new List<string>();

        // All
        public List<Widget> widgets;

        public Document()
        {
            
        }



        /*
        public Entity CreateWidget(int parent = -1, string id = null, string classes = null)
        {
            
        }

        public void CreateButton(string text, int parent = -1, string id = null, string classes = null)
        {   
           
        }

        internal void CreatePrimitive()
        {
           
        }*/
    }
}
