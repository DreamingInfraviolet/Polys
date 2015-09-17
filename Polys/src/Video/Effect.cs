using System;
using OpenGL;

namespace Polys.Video
{
    class Effect
    {
        public ShaderProgram program { get; private set; }

        public Effect(String vertex, String fragment)
        {
            program = new ShaderProgram(vertex, fragment);
            if (program.FragmentShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling fragment shader: " + program.FragmentShader.ShaderLog);
            if (program.VertexShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling vertex shader: " + program.VertexShader.ShaderLog);
            if (program.ProgramLog.Length != 0)
                throw new Exception("Error linking shader: " + program.ProgramLog);
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
