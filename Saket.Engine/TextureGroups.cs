using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    // workaround. TODO deltete shit 




    public class TextureGroups
    {
        public List<TextureGroup> groups = new();

        public void Add(Texture tex, Sheet sheet)
        {
            groups.Add(new (tex, sheet));
        }
    }

    public struct TextureGroup
    {
        public Texture tex;
        public Sheet sheet;

        public TextureGroup(Texture tex, Sheet sheet)
        {
            this.tex = tex;
            this.sheet = sheet;
        }
    }
}
