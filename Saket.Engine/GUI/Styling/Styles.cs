using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.GUI.Styling
{
    public class StyleSheet
    {
        public Dictionary<int, Style> styles;


        public StyleSheet Add(string className, Style style)
        {
            styles.Add(className.GetHashCode(), style);
            return this;
        }

        /// <summary>
        /// Combines styles from all the classes
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public Style GetStyleForWidget(Widget widget)
        {
            var classes = Document.classGroups[widget.classGroup];


            Style style = new();
            foreach (var item in classes)
            {
                if (styles.ContainsKey(item))
                {
                    style.Combine(styles[item]);
                }
            }
            return style;
        }
    }
}