using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class Palette
    {
        //rgba format
        public byte[] colours = new byte[256*4];
        uint colourTexture=~0u;

        public Palette() {}

        public Palette(String path)
        {
            try
            {
                if (System.IO.Path.GetExtension(path) != ".pal")
                    throw new Exception("palette must be a .pal file.");
                String[] lines = System.IO.File.ReadAllLines(path);
                if (lines.Length < 3)
                    throw new Exception("invalid .pal file: Less that 3 lines in length.");

                int numberOfColours = int.Parse(lines[2]);

                if (lines.Length - 3 < numberOfColours)
                    throw new Exception("invalid number of colours entries.");

                for (int i = 0; i < numberOfColours; ++i)
                {
                    Colour colour = new Colour(lines[i + 3]);
                    int index = i * 4;
                    colours[index] = colour.r;
                    colours[index+1] = colour.g;
                    colours[index+2] = colour.b;
                    colours[index+3] = colour.a;
                }
                upload();
            }
            catch(Exception e)
            {
                throw new Exception(String.Format("Error loading palette \"{0}\": {1}", path, e.Message, e));
            }
        }

        public Palette(System.Drawing.Imaging.ColorPalette p)
        {
            for (int i = 0; i < p.Entries.Length; ++i)
            {
                Colour colour = new Colour(p.Entries[i]);
                int index = i * 4;
                colours[index] = colour.r;
                colours[index + 1] = colour.g;
                colours[index + 2] = colour.b;
                colours[index + 3] = colour.a;
            }
            upload();
        }

        void upload()
        {
            if(colourTexture==~0u)
                colourTexture = OpenGL.Gl.GenTexture();

            OpenGL.Gl.BindTexture(OpenGL.TextureTarget.Texture2D, colourTexture);
            OpenGL.Gl.TexImage2D(OpenGL.TextureTarget.Texture2D, 0, OpenGL.PixelInternalFormat.Rgba8, 256, 1, 0, OpenGL.PixelFormat.Rgba, 
                OpenGL.PixelType.UnsignedByte, System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(colours, 0));
            OpenGL.Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMagFilter, OpenGL.TextureParameter.Nearest);
            OpenGL.Gl.TexParameteri(OpenGL.TextureTarget.Texture2D, OpenGL.TextureParameterName.TextureMinFilter, OpenGL.TextureParameter.Nearest);
        }

        public void bind()
        {
            OpenGL.Gl.ActiveTexture(OpenGL.TextureUnit.Texture1);
            OpenGL.Gl.BindTexture(OpenGL.TextureTarget.Texture2D, colourTexture);
        }
    }
}
