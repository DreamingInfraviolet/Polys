using OpenGL;
using System;

namespace Polys.Video
{
    public class LowLevelRenderer
    {

        //6 vertices in {x,y,u,v} format representing the screen.
        public static GeometryGPU quad;

        public static void initialise()
        {
            quad = new GeometryGPU(new float[] {
            -1.0f, -1.0f, 0.0f, 0.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f});

            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public static bool blending
        {
            set
            {
                if (value)
                    Gl.Enable(EnableCap.Blend);
                else
                    Gl.Disable(EnableCap.Blend);
            }
        }

        public static FBO framebuffer
        {
            set
            {
                if (value == null)
                    return;
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, value.BufferID);
                Gl.Viewport(0, 0, value.Size.Width, value.Size.Height);
            }

            get { return null; }
        }

        public static void resetFramebuffer(int width, int height)
        {
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.Viewport(0, 0, width, height);
        }

        public static void clear(float r = 0, float g = 0, float b = 0, float a = 1)
        {
            Gl.ClearColor(r, g, b, a);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        static ShaderProgram mShader;
        public static ShaderProgram shader
        {
            set
            {
                if (mShader == value)
                    return;
                mShader = value;
                value.Use();
            }
            get
            {
                return mShader;
            }
        }

        static int geomVertexCount = 0;

        static GeometryGPU mGeometry;
        public static GeometryGPU geometry
        {
            set
            {
                if (mGeometry == value)
                    return;
                geomVertexCount = value.vertexCount;
                Gl.BindBuffer(BufferTarget.ArrayBuffer, value.vbo);
                Gl.EnableVertexAttribArray(0);
                Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);
            }
        }

        public static void draw()
        {
            Gl.DrawArrays(BeginMode.Triangles, 0, geomVertexCount);
        }
    }
}
