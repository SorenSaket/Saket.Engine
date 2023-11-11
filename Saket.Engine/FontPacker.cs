using Saket.Engine.Graphics;
using Saket.Engine.Graphics.SDF;
using Saket.Engine.Math.Geometry;
using Saket.Engine.Math.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saket.Engine
{
    public class FontPacker
    {/*
        public struct SettingsFontPacker
        {
            /// <summary>
            /// Pixels per unit
            /// </summary>
            public float PPU = 32f;

            public SettingsFontPacker()
            {
            }
        }



        int sdfSize = 64;
        private float fill = 0;
        Texture tex;
        Vector2 offset;
        float scale = 60f / 1468f;
        Shape shape;
        byte[] data;


        public void AddToAtlas(Atlas atlas, Font font, SettingsFontPacker settings = default, Int2 range = default)
        {
            // For each character to add
            for (int i = range.X; i < range.Y; i++)
            {
                atlas.Add(new Graphics.Packing.Tile());
            }

        }




        void GenerateSDF()
        {
            float size = 64f;
            float half = size / 2f;
            float margin = 6f;

            float radius = 2f;
            float a = 2f;


            //shape = font.glyphs['a'];

            float[] color = new float[sdfSize * sdfSize];
            SDFGenerator gen = new SDFGenerator();
            gen.GenerateSDF(
                new SDFGenerator.Settings() { type = SDFGenerator.SDFType.SDF, inverseY = false, inverseX = false },
                color,
                sdfSize,
                shape,
                1f,
                new Vector2(scale, scale),
                offset * 10
            );

            {
                // remap values from 0..255
                float min = float.PositiveInfinity;
                float max = float.NegativeInfinity;
                for (int i = 0; i < color.Length; i++)
                {
                    if (color[i] < min)
                        min = color[i];
                    if (color[i] > max)
                        max = color[i];
                }

                float r = MathF.Max(MathF.Abs(max), MathF.Abs(min));

                for (int i = 0; i < color.Length; i++)
                {
                    color[i] = Mathf.Remap(color[i], -r, r, 0f, 1f);
                }
            }
            data = color.SelectMany(x => BitConverter.GetBytes(x)).ToArray();
            tex.Replace(data);
        }

        */



    }
}