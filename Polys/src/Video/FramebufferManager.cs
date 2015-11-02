using System;
using OpenGL;

/** Takes care of the majority of the graphical state.
  * Any primitives are first drawn onto a low resolution buffer.
  * This buffer is then coped to a high-resolution buffer, which is when effects may be applied.
  * After that, the result is copied to the screen (if required).
  * For effect application, it relies on two screen-sized floating point textures, using double buffering.
  */

namespace Polys.Video
{
    public class FramebufferManager : IDisposable
    {
        #region Public
        //The low resolution buffer dimensions
        public int lowResWidth  { get; private set; }
        public int lowResHeight { get; private set;  }

        /** Initialises the renderer, creating the internal buffers with the given dimensions. */
        public FramebufferManager(int width, int height, int lowResWidth, int lowResHeight)
        {
            HighLevelRenderer.shaderDrawSprite = Video.loadShader("drawSprite");
            HighLevelRenderer.shaderIndexedBitmapSprite = Video.loadShader("indexedBitmapSprite");

            resize(width, height);

            this.lowResWidth = lowResWidth;
            this.lowResHeight = lowResHeight;
            lowResBuffer = createFramebuffer(lowResWidth, lowResHeight);
        }

        /** Shuts down the system, disposing of resources. */
        public void Dispose()
        {
            disposeOfFxBuffers();
            if (lowResBuffer != null)
                lowResBuffer.Dispose();
        }

        /** The render target framebuffer */
        public void bind()
        {
            LowLevelRenderer.framebuffer = lowResBuffer;
        }
        
        /** Clears all the internal buffers. */
        public void clear()
        {
            LowLevelRenderer.framebuffer = sourceFxBuffer;
            LowLevelRenderer.clear();
            LowLevelRenderer.framebuffer = targetFxBuffer;
            LowLevelRenderer.clear();
            LowLevelRenderer.framebuffer = lowResBuffer;
            LowLevelRenderer.clear();
            LowLevelRenderer.resetFramebuffer(highResWidth, highResHeight);
            LowLevelRenderer.clear();
        }

        /** Copies the low resolution target onto the high resolution target.
          * If targetIsScreen is false, it is copied into one of the internal buffers.
          * otherwise it copies directly onto the screen. */
        public void lowresToHighres(bool targetIsScreen)
        {
            //Bind shader
            LowLevelRenderer.shader = HighLevelRenderer.shaderDrawSprite;

            if (targetIsScreen)
                LowLevelRenderer.resetFramebuffer(highResWidth, highResHeight);
            else
                LowLevelRenderer.framebuffer = sourceFxBuffer;
            
            //Set target size uniforms
            HighLevelRenderer.shaderDrawSprite["transformMatrix"].SetValue(
                Util.Maths.matrixFitRectIntoScreen(lowResWidth, lowResHeight, highResWidth, highResHeight));

            HighLevelRenderer.shaderDrawSprite["uvMatrix"].SetValue(new Matrix4(new float[] {1,0,0,0,
                                                                                            0,-1,0,0,
                                                                                            0,0,0,0,
                                                                                            0,0,0,1}));
            
            //Bind source texture
            Gl.BindTexture(TextureTarget.Texture2D, lowResBuffer.TextureID[0]);

            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.draw();
        }

        /** Copies the content of the current high resolution buffer to the screen */
        public void highresToScreen()
        {
            LowLevelRenderer.shader = HighLevelRenderer.shaderDrawSprite;
            HighLevelRenderer.shaderDrawSprite["transformMatrix"].SetValue(Matrix4.Identity);
            HighLevelRenderer.shaderDrawSprite["uvMatrix"].SetValue(Matrix4.Identity);
            
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Gl.BindTexture(TextureTarget.Texture2D, sourceFxBuffer.TextureID[0]);

            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.draw();
        }

        /** Resizes all of the internal framebuffers to the given size. */
        public void resize(int width, int height)
        {
            highResWidth = width;
            highResHeight = height;
            
            disposeOfFxBuffers();
            sourceFxBuffer = createFramebuffer(width, height);
            targetFxBuffer = createFramebuffer(width, height);
        }

        /** Applies an effect to the high res buffer. */
        public void applyEffect(Effect effect, long timeParameter)
        {
            LowLevelRenderer.framebuffer = targetFxBuffer;

            effect.bind();
            effect.setUniforms(timeParameter);
            
            Gl.BindTexture(TextureTarget.Texture2D, sourceFxBuffer.TextureID[0]);

            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.draw();

            flipBuffers();
        }

        #endregion

        #region Private

        //The three framebuffer objects in use if effects are enabled.
        FBO sourceFxBuffer, targetFxBuffer, lowResBuffer;

        //The high resolution buffer dimensions
        int highResWidth, highResHeight;


        /** Creates a framebuffer, setting the correct texture filtering properties. */
        static FBO createFramebuffer(int width, int height)
        {
            FBO f = new FBO(width, height, FramebufferAttachment.ColorAttachment0, PixelInternalFormat.Rgba32f, false);
            Gl.BindTexture(TextureTarget.Texture2D, f.TextureID[0]);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            return f;
        }

        void disposeOfFxBuffers()
        {
            if (sourceFxBuffer != null)
                sourceFxBuffer.Dispose();
            if (targetFxBuffer != null)
                targetFxBuffer.Dispose();
        }
        void flipBuffers()
        {
            FBO tmp = sourceFxBuffer;
            sourceFxBuffer = targetFxBuffer;
            targetFxBuffer = sourceFxBuffer;
        }

        #endregion
    }
}
