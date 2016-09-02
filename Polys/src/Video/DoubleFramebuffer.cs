using System;
using OpenGL;

namespace Polys.Video
{
    class DoubleFramebuffer : Util.DoubleBuffer<Framebuffer>, IFramebuffer, IDisposable
    {
        public int width()
        {
            return front.width();
        }

        public int height()
        {
            return front.height();
        }

        public void clear()
        {
            front.clear();
        }

        public void bind()
        {
            front.bind();
        }

        public FBO framebuffer()
        {
            return front.framebuffer();
        }

        public uint[] textures()
        {
            return front.textures();
        }

        public DoubleFramebuffer(int width, int height)
            : base(new Framebuffer(width, height), new Framebuffer(width, height)) {}

        public void resize(int width, int height)
        {
            front.resize(width, height);
            back.resize(width, height);
        }

        public void Dispose()
        {
            front.Dispose();
            back.Dispose();
        }
    }
}
