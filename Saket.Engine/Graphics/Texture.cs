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
        public bool IsLoadedOnCPU => data != null;
        

        // Texture Data
        public byte[] data;
        public int width;
        public int height;
        public TextureMinFilter filter;

        /// <summary>
        /// Specifies the sized internal format to be used to store texture image data.
        /// </summary>
        public SizedInternalFormat sizedInternalFormat;

        /// <summary>
        /// Specifies the number of color components in the texture. Must be one of base internal formats given in Table 1, one of the sized internal formats given in Table 2, or one of the compressed internal formats given in Table 3, below.
        /// </summary>
        public PixelInternalFormat pixelInternalFormat;

        /// <summary>
        /// Specifies the format of the pixel data.
        /// </summary>
        public PixelFormat pixelFormat;
        /// <summary>
        /// Specifies the data type of the pixel data
        /// </summary>
        public PixelType pixelType;


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

            // These filters determine how the image scales
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)filter);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)filter);

            GL.TextureStorage2D(handle, 0, sizedInternalFormat, width, height);


            // https://registry.khronos.org/OpenGL-Refpages/gl4/
            GL.TexImage2D(
                TextureTarget.Texture2D, 
                0,
                pixelInternalFormat,
                width, height, 0,
                pixelFormat,
                pixelType,
                data
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
            data = null;
        }

        public void Replace(byte[] data)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TextureSubImage2D(handle, 0, 0, 0, width, height, pixelFormat, pixelType, data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }


        public Texture(byte[] data, int width, int height, 
            TextureMinFilter filter = TextureMinFilter.Linear,
            SizedInternalFormat sizedInternalFormat = SizedInternalFormat.Rgba8, 
            PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba,
            PixelFormat pixelFormat = PixelFormat.Rgba, 
            PixelType pixelType = PixelType.UnsignedByte )
        {
            this.data = data;
            this.width = width;
            this.height = height;

            this.filter = filter;
            this.sizedInternalFormat= sizedInternalFormat;
            this.pixelInternalFormat= pixelInternalFormat;
            this.pixelFormat= pixelFormat;
            this.pixelType = pixelType;
        }
    }
}