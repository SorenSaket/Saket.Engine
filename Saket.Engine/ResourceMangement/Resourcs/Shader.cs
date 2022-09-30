using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using System.Diagnostics;
using System.Numerics;

namespace Saket.Engine
{
    // A simple class meant to help create shaders.
    public class Shader 
    {
        public readonly int Handle;

        private readonly Dictionary<string, int> _uniformLocations;


        internal Shader(string vertexcode, string fragmentcode, string? geometrycode = null)
        {
            Handle = GL.CreateProgram();
            
            // ---- Vertex ----

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexcode);
            CompileShader(vertexShader);
            GL.AttachShader(Handle, vertexShader);
            // ---- Fragment ----

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentcode);
            CompileShader(fragmentShader);
            GL.AttachShader(Handle, fragmentShader);

            int? geometryShader = null;
            // ---- geometry ----
            if (geometrycode != null) { 

                geometryShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geometryShader.Value, geometrycode);
                CompileShader(geometryShader.Value);
                GL.AttachShader(Handle, geometryShader.Value);
            }

            LinkProgram(Handle);

       
            var info = GL.GetProgramInfoLog(Handle);
            Debug.WriteLine(info);


            // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.
            GL.DetachShader(Handle, vertexShader);
            GL.DeleteShader(vertexShader);

            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);

            if (geometryShader != null)
            {
                GL.DetachShader(Handle, geometryShader.Value);
                GL.DeleteShader(geometryShader.Value);
            }

            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            //GL.GetActiveUniforms()
            // Next, allocate the dictionary to hold the locations.
            _uniformLocations = new Dictionary<string, int>();

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                var key = GL.GetActiveUniform(Handle, i, out _, out _);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);
            }
            void CompileShader(int shader)
            {
                // Try to compile the shader
                GL.CompileShader(shader);

                // Check for compilation errors
                GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
                if (code != (int)All.True)
                {
                    // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                    var infoLog = GL.GetShaderInfoLog(shader);
                    throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
                }
            }
            void LinkProgram(int program)
            {
                // We link the program
                GL.LinkProgram(program);

                // Check for linking errors
                GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
                if (code != (int)All.True)
                {
                    // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                    Debug.WriteLine("error: " + GL.GetProgramInfoLog(program));
                    throw new Exception($"Error occurred whilst linking Program({program})");
                }
            }
        }
       


        // A wrapper function that enables the shader program.
        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }
        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(_uniformLocations[name], data);
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public unsafe void SetMatrix4(string name, Matrix4x4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], 1, false, (float*)&data);
        }
        public unsafe void SetMatrix4(string name, OpenTK.Mathematics.Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(_uniformLocations[name], 1, true, (float*)&data);
        }
        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public unsafe void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(_uniformLocations[name],1, (float*)&data);
        }

        public void Dispose()
        {
            GL.DeleteProgram(Handle);
        }

    }
}
