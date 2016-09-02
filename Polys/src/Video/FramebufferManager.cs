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
        public int lowResWidth
        {
            get { return lowResBuffer.width(); }
        }
        public int lowResHeight
        {
            get { return lowResBuffer.height(); }
        }

        public int highResWidth
        {
            get { return highResBuffer.width(); }
        }
        public int highResHeight
        {
            get { return highResBuffer.height(); }
        }

        /** Initialises the renderer, creating the internal buffers with the given dimensions. */
        public FramebufferManager(int width, int height, int lowResWidth, int lowResHeight)
        {
            HighLevelRenderer.shaderDrawSprite = Video.loadShader("drawSprite");
            HighLevelRenderer.shaderIndexedBitmapSprite = Video.loadShader("indexedBitmapSprite");

            highResBuffer = new Framebuffer(width, height);
            lowResBuffer = new Framebuffer(lowResWidth, lowResHeight);
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
            lowResBuffer.bind();
        }
        
        /** Clears all the internal buffers. */
        public void clear()
        {
            highResBuffer.clear();
            lowResBuffer.clear();
            LowLevelRenderer.resetFramebuffer(highResWidth, highResHeight);
            LowLevelRenderer.clear();
        }

        public void copyFramebuffer(IFramebuffer source, IFramebuffer target, bool fromLowres)
        {
            //Bind shader
            LowLevelRenderer.shader = HighLevelRenderer.shaderDrawSprite;
            target.bind();

            //Set target size uniforms
            var tmatrix = fromLowres ? Util.Maths.matrixFitRectIntoScreen(lowResWidth, lowResHeight, highResWidth, highResHeight)
                                     : Matrix4.Identity;
                                        
            HighLevelRenderer.shaderDrawSprite["transformMatrix"].SetValue(tmatrix);

            float yflip = fromLowres ? -1 : 1;
            HighLevelRenderer.shaderDrawSprite["uvMatrix"].SetValue(new Matrix4(new float[] {1,0,0,0,
                                                                                            0,yflip,0,0,
                                                                                            0,0,0,0,
                                                                                            0,0,0,1}));
            //Bind source texture
            Gl.BindTexture(TextureTarget.Texture2D, source.textures()[0]);
            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.draw();
        }

        public void lowresFxActive(bool active)
        {
            var w = lowResBuffer.width();
            var h = lowResBuffer.height(); 
            lowResBuffer.Dispose();
            if (active)
                lowResBuffer = new DoubleFramebuffer(w, h);
            else
                lowResBuffer = new Framebuffer(w, h);
        }

        public void highresFxActive(bool active)
        {
            var w = highResBuffer.width();
            var h = highResBuffer.height();
            highResBuffer.Dispose();
            if (active)
                highResBuffer = new DoubleFramebuffer(w, h);
            else
            {
                highResBuffer = screenFramebuffer;
            }
            resizeHighRes(w, h);
        }

        /** Resizes all of the internal framebuffers to the given size. */
        public void resizeHighRes(int width, int height)
        {
            highResBuffer.resize(width, height);
            screenFramebuffer.resize(width, height);
        }

        public void finalise()
        {
           
            // This should be a method really :)
            if(lowResBuffer.GetType() == typeof(DoubleFramebuffer))
            {
                var doublebuffer = (DoubleFramebuffer)lowResBuffer;
                // ... apply low res effects here
            }
            copyFramebuffer(lowResBuffer, highResBuffer, true);
            if (highResBuffer.GetType() == typeof(ScreenFramebuffer))
            {
                // Nothing to do, image is already on screen
            }
            else if(highResBuffer.GetType() == typeof(DoubleFramebuffer))
            {
                var doublebuffer = (DoubleFramebuffer)highResBuffer;
                // ... apply high res effects here
                copyFramebuffer(highResBuffer, screenFramebuffer, false);
            }
        }

        /** Applies an effect to the high res buffer. */
        /*
        public void applyEffect(Effect effect, long timeParameter)
        {
            targetFxBuffer.bind();

            effect.bind();
            effect.setUniforms(timeParameter);
            
            Gl.BindTexture(TextureTarget.Texture2D, sourceFxBuffer.textures[0]);

            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.draw();

            flipBuffers();
        }
        */

        #endregion

        #region Private

        //The three framebuffer objects in use if effects are enabled.
        IFramebuffer highResBuffer, lowResBuffer;
        ScreenFramebuffer screenFramebuffer = new ScreenFramebuffer();

        /** Creates a framebuffer, setting the correct texture filtering properties. */
        static FBO createFramebuffer(int width, int height)
        {
            FBO f = new FBO(width, height, FramebufferAttachment.ColorAttachment0, PixelInternalFormat.Rgba8, false);
            Gl.BindTexture(TextureTarget.Texture2D, f.TextureID[0]);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            return f;
        }

        void disposeOfFxBuffers()
        {
            highResBuffer.Dispose();
            lowResBuffer.Dispose();
        }
            
        #endregion
    }
}