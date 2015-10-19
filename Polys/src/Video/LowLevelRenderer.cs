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
    class LowLevelRenderer
    {
        // Indicates which of the high-res buffers should be used as a source-target.
        // Current index is the fx source, while the previous one is the target.
        // Effects are applied from the source onto the target.
        // The low-res texture is drawn onto the source.
        bool bufferIndex;

        //The three framebuffer objects in use if effects are enabled.
        FBO buffer1, buffer2, lowResBuffer;

        //The high resolution buffer dimensions
        int highResWidth, highResHeight;

        //The low resolution buffer dimensions
        int lowResWidth, lowResHeight;

        //Various shader programs used during rendering
        ShaderProgram shaderLowResRenderTargetToScreen;
        ShaderProgram shaderCopyFlipped;
        ShaderProgram shaderIndexedBitmapSprite;

        //6 vertices in {x,y,u,v} format representing the screen.
        float[] mScreenQuad = {
        -1.0f, -1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 0.0f,
        -1.0f, 1.0f, 0.0f, 1.0f,
        -1.0f, 1.0f, 0.0f, 1.0f,
        1.0f, -1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f};

        //The screen vertex buffer object
        uint mScreenQuadVBO;

        /** Initialises the renderer, creating the internal buffers with the given dimensions. */
        public LowLevelRenderer(int width, int height, int lowResWidth, int lowResHeight)
        {
            initQuad();
            initShaders();
            resize(width, height);

            this.lowResWidth = lowResWidth;
            this.lowResHeight = lowResHeight;
            lowResBuffer = createFramebuffer(new System.Drawing.Size(lowResWidth, lowResHeight));
        }

        /** Shuts down the system, disposing of resources. */
        public void shutdown()
        {
            if (buffer1 != null)
                buffer1.Dispose();
            if (buffer2 != null)
                buffer2.Dispose();
            if (lowResBuffer != null)
                lowResBuffer.Dispose();
            if (shaderLowResRenderTargetToScreen != null)
                shaderLowResRenderTargetToScreen.Dispose();
            if (shaderCopyFlipped != null)
                shaderCopyFlipped.Dispose();
            if (shaderIndexedBitmapSprite != null)
                shaderIndexedBitmapSprite.Dispose();
        }

        /** Initialies the screen quad. */
        void initQuad()
        {
            mScreenQuadVBO = ~0u;
            mScreenQuadVBO = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.BufferData<float>(BufferTarget.ArrayBuffer, sizeof(float) * mScreenQuad.Length, mScreenQuad, BufferUsageHint.StaticDraw);
        }

        /** Clears all the internal buffers. */
        public void clear()
        {
            setSourceBuffer();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            setTargetBuffer();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            lowResBuffer.Enable();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /** Draws a tile layer with a given camera. */
        public void draw(TileLayer layer, Camera camera)
        {
            if (layer.visible == false)
                return;

            //Prepare
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, lowResBuffer.BufferID);
            shaderIndexedBitmapSprite.Use();
            bindScreenQuad();

            //In normalised GL coordinates:
            Vector2 screenVec = new Vector2(lowResWidth, lowResHeight);

            foreach (var tiles in layer.tileDict)
            {
                //Bind tileset textures
                tiles.Key.bind();

                //Retrieve dimensions
                int tileWidth = tiles.Key.tileWidth;
                int tileHeight = tiles.Key.tileHeight;
                int tilesetWidth = tiles.Key.width;
                int tilesetHeight = tiles.Key.height;

                //For each tile
                foreach (Tile tile in tiles.Value)
                {
                    if (!tile.visible)
                        return;

                    //Get screen coordinates of the tile in pixels
                    int screenPosX = tile.worldX * tileWidth;
                    int screenPosY = tile.worldY * tileHeight;
                    camera.worldToScreen(ref screenPosX, ref screenPosY);

                    //Skip tiles outside the screen
                    if (screenPosX + tileWidth < 0 || screenPosY + tileHeight < 0 ||
                        screenPosX >= (int)lowResWidth || screenPosY >= (int)lowResHeight)
                        continue;

                    //Find projection and UV matrices.
                    Vector2 p = (new Vector2(screenPosX, screenPosY) + 0.5f) / screenVec * 2.0f - 1.0f;
                    Vector2 s = new Vector2(tileWidth, tileHeight) / screenVec;

                    Matrix4 orthoMatrix = new Matrix4(new float[] { s.x, 0, 0, 0,
                                                           0, s.y, 0, 0,
                                                           0, 0, 0, 0,
                                                           s.x+p.x, s.y+p.y, 0, 1});

                    Matrix4 uvMatrix = new Matrix4(new float[] { (float)tileWidth /tilesetWidth, 0, 0, 0,
                                                             0, (float)tileHeight/tilesetHeight, 0, 0,
                                                            0, 0, 0, 0,
                                                            (float)(tile.tilesetX*tileWidth+0.5f)/tilesetWidth,
                                                            (float)(tile.tilesetY*tileHeight+0.5f)/tilesetHeight, 0, 1});


                    //Set matrix uniforms
                    shaderIndexedBitmapSprite["orthoMatrix"].SetValue(orthoMatrix);
                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(uvMatrix);

                    //Draw
                    Gl.DrawArrays(BeginMode.Triangles, 0, 6);
                }
            }
        }

        /** Copies the low resolution target onto the high resolution target.
          * If targetIsScreen is false, it is copied into one of the internal buffers.
          * otherwise it copies directly onto the screen. */
        public void lowresToHighres(bool targetIsScreen)
        {
            //Bind shader
            shaderLowResRenderTargetToScreen.Use();

            if (targetIsScreen)
            {
                //Bind screen
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                Gl.Viewport(0, 0, highResWidth, highResHeight);

                shaderLowResRenderTargetToScreen["uvMultiplier"].SetValue(-1.0f);
            }
            else
            {
                //Bind source buffer
                setSourceBuffer();
                shaderLowResRenderTargetToScreen["uvMultiplier"].SetValue(1.0f);
            }

            //Clear current buffer
            Gl.ClearColor(0, 0, 0, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            //Set target size uniforms
            shaderLowResRenderTargetToScreen["sourceWidth"].SetValue((float)lowResBuffer.Size.Width);
            shaderLowResRenderTargetToScreen["sourceHeight"].SetValue((float)lowResBuffer.Size.Height);

            bindScreenQuad();

            //Bind source texture
            Gl.BindTexture(TextureTarget.Texture2D, lowResBuffer.TextureID[0]);
            Gl.ActiveTexture(TextureUnit.Texture0);

            //Draw
            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }

        /** Copies the content of the current high resolution buffer to the screen */
        public void highresToScreen()
        {
            shaderCopyFlipped.Use();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            bindScreenQuad();
            Gl.BindTexture(TextureTarget.Texture2D, getSourceBuffer().TextureID[0]);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }

        /** Binds the screen quad geometry for rendering */
        void bindScreenQuad()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);
        }

        /** Sets the correct framebuffer as target */
        private void setBuffer(bool index)
        {
            if (index)
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, buffer2.BufferID);
            else
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, buffer1.BufferID);

            Gl.DrawBuffer(DrawBufferMode.ColorAttachment0);
            Gl.Viewport(0, 0, (int)highResWidth, (int)highResHeight);
        }

        /** Sets the source framebuffer as render target */
        private void setSourceBuffer() { setBuffer(bufferIndex); }

        /** Sets the target framebuffer as render target */
        private void setTargetBuffer() { setBuffer(!bufferIndex); }

        /** Returns the appropriate framebuffer */
        private FBO getBuffer(bool index)
        {
            if (index)
                return buffer2;
            else
                return buffer1;
        }

        /** Returns the source framebuffer */
        private FBO getSourceBuffer() { return getBuffer(bufferIndex); }

        /** Returns the target framebuffer */
        private FBO getTargetBuffer() { return getBuffer(!bufferIndex); }

        /** Creates a framebuffer, setting the correct texture filtering properties. */
        public static FBO createFramebuffer(System.Drawing.Size size)
        {
            FBO f = new FBO(size, FramebufferAttachment.ColorAttachment0, PixelInternalFormat.Rgba32f, false);
            Gl.BindTexture(TextureTarget.Texture2D, f.TextureID[0]);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            return f;
        }

        /** Resizes all of the internal framebuffers to the given size. */
        public void resize(int width, int height)
        {
            highResWidth = width;
            highResHeight = height;

            shaderLowResRenderTargetToScreen.Use();
            shaderLowResRenderTargetToScreen["screenWidth"].SetValue((float)width);
            shaderLowResRenderTargetToScreen["screenHeight"].SetValue((float)height);
            if (buffer1 != null)
                buffer1.Dispose();
            if (buffer2 != null)
                buffer2.Dispose();

            System.Drawing.Size sds = new System.Drawing.Size((int)width, (int)height);
            buffer1 = createFramebuffer(sds);
            buffer2 = createFramebuffer(sds);
        }

        /** Initialises the shaders used within the pipeline */
        private void initShaders()
        {
            shaderLowResRenderTargetToScreen = Video.loadShader("lowResRenderTargetToScreen");
            shaderCopyFlipped = Video.loadShader("copyFlipped");
            shaderIndexedBitmapSprite = Video.loadShader("indexedBitmapSprite");
        }

        /** Applies an effect to the high res buffer. */
        public void applyEffect(Effect effect, long timeParameter)
        {
            setTargetBuffer();

            effect.bind();
            effect.setUniforms(timeParameter);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);

            Gl.BindTexture(TextureTarget.Texture2D, getSourceBuffer().TextureID[0]);
            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.DrawArrays(BeginMode.Triangles, 0, 6);

            bufferIndex = !bufferIndex;
        }
    }
}
