using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using StbImageSharp;
namespace Saket.Engine
{
    // TODO
    // Convert to platform angostic image loading
    // QOI image format
    // Unload image from memory uption after upload to gpu, so that it doesn't take ram.

    public class Texture 
    {
        public int handle = -1;

        public bool IsLoadedOnGPU;
        public bool IsLoadedOnCPU => image != null;

        public ImageResult image;

        public void LoadToGPU()
        {
            if (!IsLoadedOnCPU)
                throw new Exception("Texture is not loaded");
            if (IsLoadedOnGPU)
                throw new Exception("Texture is already loaded");
           
            IsLoadedOnGPU = true;

            GL.Enable(EnableCap.Texture2D);
            handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TextureStorage2D(handle, 0, SizedInternalFormat.Rgba8, image.Width, image.Height);


            // https://registry.khronos.org/OpenGL-Refpages/gl4/
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0, 
                PixelInternalFormat.Rgba,
                image.Width, image.Height, 0, 
                OpenTK.Graphics.OpenGL4.PixelFormat.Rgba, 
                PixelType.UnsignedByte,
                image.Data
                );
            
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void UnloadFromGPU()
        {
            if (!IsLoadedOnGPU)
                throw new Exception("Cannot Unload Texture: texture is not loaded");
            // Unload
            GL.DeleteTexture(handle);
            handle = -1;
        }

        public void UnloadFromCPU()
        {
            image = null;
        }


        public Texture(ImageResult image)
        {
            this.image = image;
        }
    }
}