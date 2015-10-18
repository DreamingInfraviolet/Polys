using System;
using OpenGL;

namespace Polys.Video
{
    class Effect
    {
        public ShaderProgram program { get; private set; }

        public Effect(OpenGL.ShaderProgram shader)
        {
            program = shader;
        }

        public void setUniforms(long time)
        {
            program["time"].SetValue((float)time);
        }

        public void bind()
        {
            Gl.UseProgram(program);
        }
    }
}
