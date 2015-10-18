using System;
using System.Collections.Generic;
using OpenGL;

/** Stores the image to be shown in hardware. Uses internal double buffering to allow one to apply
    image effects. Uses window resolution.
    */

namespace Polys.Video
{
    class HardwareRenderTarget
    {
        // Current index is the fx source, while the previous one is the target. Use source for drawing.
        bool mBufferIndex;
        FBO mBuffer1, mBuffer2, mLowResBuffer;
        uint mScreenWidth, mScreenHeight;
        int mDrawTargetWidth, mDrawTargetHeight;

        ShaderProgram shaderLowResRenderTargetToScreen;
        ShaderProgram shaderCopyFlipped;

        ShaderProgram shaderIndexedBitmapSprite;

        //6 vertices in {x,y,u,v} format.
        float[] mScreenQuad = {
        -1.0f, -1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 0.0f,
        -1.0f, 1.0f, 0.0f, 1.0f,
        -1.0f, 1.0f, 0.0f, 1.0f,
        1.0f, -1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f};
        uint mScreenQuadVBO;

        public HardwareRenderTarget(uint width, uint height, int lowResWidth, int lowResHeight)
        {
            initQuad();
            initShaders();
            resize(width, height);

            mDrawTargetWidth = lowResWidth;
            mDrawTargetHeight = lowResHeight;
            mLowResBuffer = createFramebuffer(new System.Drawing.Size(lowResWidth, lowResHeight));
        }
        public void shutdown()
        {
            if (mBuffer1 != null)
                mBuffer1.Dispose();
            if (mBuffer2 != null)
                mBuffer2.Dispose();
            if (shaderLowResRenderTargetToScreen != null)
                shaderLowResRenderTargetToScreen.Dispose();
        }
        void initQuad()
        {
            mScreenQuadVBO = ~0u;
            mScreenQuadVBO = Gl.GenBuffer();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.BufferData<float>(BufferTarget.ArrayBuffer, sizeof(float) * mScreenQuad.Length, mScreenQuad, BufferUsageHint.StaticDraw);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);
        }

        public void clear()
        {
            setSourceBuffer();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            setTargetBuffer();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            mLowResBuffer.Enable();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }
        
        public void draw(TileLayer layer, Camera camera)
        {
            if (layer.visible == false)
                return;

            shaderIndexedBitmapSprite.Use();

            bindScreenQuad();

            //In normalised GL coordinates:
            Vector2 screenVec = new Vector2(mDrawTargetWidth, mDrawTargetHeight);
            
            foreach (var tiles in layer.tileDict)
            {
                //Bind tilesets
                tiles.Key.image.bind();
                tiles.Key.mPalette.bind();

                int tileWidth = tiles.Key.tileWidth;
                int tileHeight = tiles.Key.tileHeight;
                int tilesetWidth = tiles.Key.image.width;
                int tilesetHeight = tiles.Key.image.height;

                foreach (Tile tile in tiles.Value)
                {
                    if (!tile.visible)
                        return;

                    //Get start coordinates and end
                    int startDrawX = tile.worldX * tileWidth;
                    int startDrawY = tile.worldY * tileHeight;
                    camera.worldToScreen(ref startDrawX, ref startDrawY);

                    //Check if they are on the screen
                    if (startDrawX + tileWidth < 0 || startDrawY + tileHeight < 0 ||
                        startDrawX >= (int)mDrawTargetWidth || startDrawY >= (int)mDrawTargetHeight)
                        continue;

                    Vector2 p = (new Vector2(startDrawX, startDrawY) + 0.5f) / screenVec * 2.0f - 1.0f;
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

                    shaderIndexedBitmapSprite["orthoMatrix"].SetValue(orthoMatrix);
                    shaderIndexedBitmapSprite["uvMatrix"].SetValue(uvMatrix);

                    Gl.DrawArrays(BeginMode.Triangles, 0, 6);
                }
            }
        }

        void _TextureToFullres(uint lowResTexId, int width, int height, bool targetIsScreen = false)
        {
            shaderLowResRenderTargetToScreen.Use();

            if (targetIsScreen)
            {
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                Gl.Viewport(0, 0, (int)mScreenWidth, (int)mScreenHeight);
                shaderLowResRenderTargetToScreen["uvMultiplier"].SetValue(-1.0f);
            }
            else
            {
                setSourceBuffer();
                shaderLowResRenderTargetToScreen["uvMultiplier"].SetValue(1.0f);
            }
            
            Gl.ClearColor(0, 0, 0, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit);

            shaderLowResRenderTargetToScreen["sourceWidth"].SetValue((float)width);
            shaderLowResRenderTargetToScreen["sourceHeight"].SetValue((float)height);

            bindScreenQuad();

            Gl.BindTexture(TextureTarget.Texture2D, lowResTexId);
            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }

        public void lowresToHighres(bool directlyToScreen)
        {
            _TextureToFullres(mLowResBuffer.TextureID[0], mLowResBuffer.Size.Width, mLowResBuffer.Size.Height, directlyToScreen);
        }

        //Sets this as the current render target to be shown to screen
        public void highresToScreen()
        {
            shaderCopyFlipped.Use();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            bindScreenQuad();
            Gl.BindTexture(TextureTarget.Texture2D, getSourceBuffer().TextureID[0]);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }
        
        void bindScreenQuad()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);
        }

        private void setBuffer(bool index)
        {
            if (index)
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, mBuffer2.BufferID);
            else
                Gl.BindFramebuffer(FramebufferTarget.Framebuffer, mBuffer1.BufferID);

            Gl.DrawBuffer(DrawBufferMode.ColorAttachment0);
            Gl.Viewport(0, 0, (int)mScreenWidth, (int)mScreenHeight);
        }
        private void setSourceBuffer() { setBuffer(mBufferIndex); }
        private void setTargetBuffer() { setBuffer(!mBufferIndex); }
        
        private FBO getBuffer(bool index)
        {
            if (index)
                return mBuffer2;
            else
                return mBuffer1;
        }
        private FBO getSourceBuffer() { return getBuffer(mBufferIndex); }
        private FBO getTargetBuffer() { return getBuffer(!mBufferIndex); }
        
        
        public static FBO createFramebuffer(System.Drawing.Size size)
        {
            FBO f = new FBO(size, FramebufferAttachment.ColorAttachment0, PixelInternalFormat.Rgba32f, false);
            Gl.BindTexture(TextureTarget.Texture2D, f.TextureID[0]);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, TextureParameter.Nearest);
            Gl.TexParameteri(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, TextureParameter.Nearest);
            return f;
        }

        public void resize(uint width, uint height)
        {
            mScreenWidth = width;
            mScreenHeight = height;

            shaderLowResRenderTargetToScreen.Use();
            shaderLowResRenderTargetToScreen["screenWidth"].SetValue((float)width);
            shaderLowResRenderTargetToScreen["screenHeight"].SetValue((float)height);
            if (mBuffer1 != null)
                mBuffer1.Dispose();
            if (mBuffer2 != null)
                mBuffer2.Dispose();

            System.Drawing.Size sds = new System.Drawing.Size((int)width, (int)height);
            mBuffer1 = createFramebuffer(sds);
            mBuffer2 = createFramebuffer(sds);
        }
        
        private void initShaders()
        {
            shaderLowResRenderTargetToScreen = Video.loadShader("lowResRenderTargetToScreen");
            shaderCopyFlipped = Video.loadShader("copyFlipped");
            shaderIndexedBitmapSprite = Video.loadShader("indexedBitmapSprite");
        }

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

            mBufferIndex = !mBufferIndex;
        }
    }
}
