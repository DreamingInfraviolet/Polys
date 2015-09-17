using System;
using OpenGL;

namespace Polys.Video
{
    class IndexedBitmap
    {
        public Palette palette { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }

        uint indexTexture;
        
        public IndexedBitmap(String pathIn)
        {
            String path = pathIn.Replace('/', '\\');
            try
            {
                //Load image
                String extension = System.IO.Path.GetExtension(path);
                if (!extension.Equals(".bmp", StringComparison.CurrentCultureIgnoreCase) &&
                   !extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase))
                    throw new Exception("expected bmp or png image.");
                System.Drawing.Bitmap img = new System.Drawing.Bitmap(path.Replace('/','\\'));

                if(img.PixelFormat != System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    throw new Exception("Image must be indexed.");

                //Get palette
                palette = new Palette(img.Palette);

                //Get indices
                System.Drawing.Imaging.BitmapData data = img.LockBits(
                        new System.Drawing.Rectangle(0,0,img.Width, img.Height), 
                        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
                
                //Upload
                indexTexture = Gl.GenTexture();
                Gl.BindTexture(TextureTarget.Texture2D, indexTexture);
                Gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8,
                    data.Width, data.Height, 0, PixelFormat.Red, PixelType.UnsignedByte, data.Scan0);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
                Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);

                width = data.Width;
                height = data.Height;
            }
            catch(Exception e)
            {
                throw new Exception(String.Format("Can not load \"{0}\": {1}", path, e.Message), e);
            }
        }

        public void bind()
        {
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2D, indexTexture);
        }
    }
}
