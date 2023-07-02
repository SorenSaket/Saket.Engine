using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ExCSS;
using Shape = Saket.Engine.Math.Geometry.StyledShapeCollection;
using SvgPathProperties;


namespace Saket.Engine.Resources.Parsers
{
    public class ParserSVG
    {
        public void Parse(Stream stream)
        {
            // Load the xml document
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            // Get the svg element
            doc.SelectNodes("svg");

            // Iterate paths and create shapes
                // Apply styling
                    // Fill stroke? needs polyfill. no support for outlines yet
            
            // return shape

            ExCSS.StylesheetParser parser = new ExCSS.StylesheetParser();
            //parser.Parse(path);

        }
        /*
        public Shape ParsePath(string str)
        {

        }
        */
    }
}
