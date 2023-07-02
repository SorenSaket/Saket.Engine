using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine.Resources.Loaders
{
    public class LoaderShader : ResourceLoader<Shader>
    {
        public override Shader Load(string shaderName, ResourceManager resourceManager)
        {
            string basepath = "shader_" + shaderName + "_";
            string path_fragment = basepath + "frag.glsl";
            string path_vertex = basepath + "vert.glsl";

            string code_fragment = null;
            string code_vertex = null;

            {
                // Load fragment shader code
                if (resourceManager.TryGetStream(path_fragment, out Stream? stream))
                {
                    StreamReader reader = new StreamReader(stream);
                    code_fragment = reader.ReadToEnd();
                }
            }

            {
                // Load vertex shader code
                if (resourceManager.TryGetStream(path_vertex, out Stream? stream))
                {
                    StreamReader reader = new StreamReader(stream);
                    code_vertex = reader.ReadToEnd();
                }
            }
            if (code_fragment == null || code_vertex == null)
                throw new Exception("Invalid Shader. Missing files");


            return new Shader(code_vertex, code_fragment);

        }
    }
}
