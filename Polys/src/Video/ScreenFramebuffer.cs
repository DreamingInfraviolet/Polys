using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenGL;

namespace Polys.Video
{
    class ScreenFramebuffer : IFramebuffer
    {
        int mWidth, mHeight;

        public void bind()
        {
            LowLevelRenderer.resetFramebuffer(mWidth, mHeight);
        }

        public void clear()
        {
            var tmp = LowLevelRenderer.framebuffer;
            LowLevelRenderer.resetFramebuffer(mWidth, mHeight);
            LowLevelRenderer.framebuffer = tmp; 
        }

        public void Dispose()
        {
        }

        public int height()
        {
            return mHeight;
        }

        public void resize(int width, int height)
        {
            mWidth = width;
            mHeight = height;
        }

        public int width()
        {
            return mWidth;
        }

        public FBO framebuffer()
        {
            return null;
        }

        public uint[] textures()
        {
            return null;
        }
    }
}
