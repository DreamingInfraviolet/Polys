using System;
using OpenGL;

namespace Polys.Video
{
    /** Represents a piece of geometry in GPU memory */
    public class GeometryGPU
        : IDisposable
    {
        public uint vbo { get; private set; }
        public int vertexCount { get; private set; }

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

        /** Destroys the object */
        public void Dispose()
        {
            Gl.DeleteFramebuffers(1, new uint[] { vbo });
        }
    }
}
