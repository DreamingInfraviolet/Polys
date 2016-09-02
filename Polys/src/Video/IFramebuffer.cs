using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    /**
     * It is useful to be able to use a single and double buffer interchangeably without any code changes.
     * This way we should be able to disable or enable double buffered drawing without worrying about
     * breaking anything. This interface helps with this ^_^ */
    public interface IFramebuffer : IDisposable
    {
        int width();
        int height();

        void clear();

        void resize(int width, int height);

        void bind();

        OpenGL.FBO framebuffer();
        uint[] textures();
    }
}
