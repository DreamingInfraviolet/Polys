using System;
using OpenGL;

namespace Polys.Video
{
    /** Represents a piece of geometry in GPU memory */
    class GeometryGPU
        : IDisposable
    {
        uint vbo;
        int vertexCount;

        /** Upload the following vertex data.
          * The vertex data is in the form {posx, posy, uvx, uvy}
          */
        public GeometryGPU(float[] vertexData)
        {
            if (vertexData.Length % 4 != 0)
                throw new Exception("Initialising geometry with vertex data not a multiple of 4");

            vbo = Gl.GenBuffer();
            vertexCount = vertexData.Length / 4;

            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.BufferData<float>(BufferTarget.ArrayBuffer, sizeof(float) * vertexData.Length, vertexData, BufferUsageHint.StaticDraw);
        }

        /** Binds the object for rendering */
        public void bind()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);
        }

        /** Destroys the object */
        public void Dispose()
        {
            Gl.DeleteFramebuffers(1, new uint[] { vbo });
        }

        /** Draws using the object */
        public void draw()
        {
            Gl.DrawArrays(BeginMode.Triangles, 0, vertexCount);
        }
    }
}
