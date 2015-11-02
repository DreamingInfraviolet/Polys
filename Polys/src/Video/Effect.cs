using OpenGL;

namespace Polys.Video
{
    /** Represents an effect that can be applied to the image during a post-processing step.
      * An effect is a GLSL shader. */
    public class Effect
    {
        /** The shader program of the effect. */
        ShaderProgram program;

        /** Constructs the effect from a shader program (references the shader) */
        public Effect(OpenGL.ShaderProgram shader)
        {
            program = shader;
        }

        /** Sets the uniforms for the effect */
        public void setUniforms(long time)
        {
            program["time"].SetValue((float)time);
        }

        /** Binds the effect for use */
        public void bind()
        {
            Gl.UseProgram(program);
        }
    }
}
