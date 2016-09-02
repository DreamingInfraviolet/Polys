using System;
using OpenGL;

namespace Polys.Video
{
    class Framebuffer : IDisposable
    {
        FBO fbo;

        public uint[] textures
        {
            get
            {
                return fbo.TextureID;
            }
            private set
            {
                throw new NotImplementedException();
            }
        }

        public Framebuffer(int width, int height)
        {
            resize(width, height);
        }

        private FBO createFramebuffer(int width, int height)
        {
            FBO ret = new FBO(width, height, FramebufferAttachment.ColorAttachment0, PixelInternalFormat.Rgba8, false);
            Gl.BindTexture(TextureTarget.Texture2D, ret.TextureID[0]);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            return ret;
        }

        public int width { get; private set; }

        public int height { get; private set; }

        public void resize(int width, int height)
        {
            Dispose();
            fbo = createFramebuffer(width, height);
        }

        public void clear()
        {
            var prev = LowLevelRenderer.framebuffer;
            LowLevelRenderer.framebuffer = fbo;
            LowLevelRenderer.clear();
            LowLevelRenderer.framebuffer = prev;
        }

        public void Dispose()
        {
            if (fbo != null)
            {
                fbo.Dispose();
                fbo = null;
            }
        }

        public void bind()
        {
            LowLevelRenderer.framebuffer = fbo;
        }
    }
}
