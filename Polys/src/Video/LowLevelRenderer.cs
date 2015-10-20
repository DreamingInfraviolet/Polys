using OpenGL;
using System;

namespace Polys.Video
{
    class LowLevelRenderer
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
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, value.BufferID);
                Gl.DrawBuffer(DrawBufferMode.ColorAttachment0); //Do I need this here?
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

        public static ShaderProgram shader
        {
            set
            {
                value.Use();
            }
        }

        static int geomVertexCount = 0;

        public static GeometryGPU geometry
        {
            set
            {
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
