using System;
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
        uint mWidth, mHeight;

        ShaderProgram mpSoftwareToHardware;
        ShaderProgram mpHardwareToScreen;

        ShaderProgram mDrawIndexedBitmapShader;

        //6 vertices in {x,y,u,v} format.
        float[] mScreenQuad = {
        -1.0f, -1.0f, 0.0f, 0.0f,
        1.0f, -1.0f, 1.0f, 0.0f,
        -1.0f, 1.0f, 0.0f, 1.0f,
        -1.0f, 1.0f, 0.0f, 1.0f,
        1.0f, -1.0f, 1.0f, 0.0f,
        1.0f, 1.0f, 1.0f, 1.0f};
        uint mScreenQuadVBO;

        public void draw(TileLayer layer, Camera camera)
        {

            if (layer.visible == false)
                return;

            mDrawIndexedBitmapShader.Use();
            mLowResBuffer.Enable();
            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);
            
            foreach (Tile tile in layer.tiles)
            {
                if (!tile.visible)
                    return;

                //Get start coordinates and end
                int startDrawX = tile.worldX * tile.tileset.tileWidth;
                int startDrawY = tile.worldY * tile.tileset.tileHeight;
                camera.worldToScreen(ref startDrawX, ref startDrawY);

                int endDrawX = startDrawX + tile.tileset.tileWidth;
                int endDrawY = startDrawY + tile.tileset.tileHeight;

                //Check if they are on the screen
                if (endDrawX < 0 || endDrawY < 0 || startDrawX >= (int)mWidth || startDrawY >= (int)mHeight)
                    return;
                
                tile.tileset.image.bind();
                tile.tileset.mPalette.bind();

                Gl.DrawArrays(BeginMode.Triangles, 0, 6);
            }
        }

        public void clear()
        {
            setSourceBuffer();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            setTargetBuffer();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
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

        public void shutdown()
        {
            if (mBuffer1 != null)
                mBuffer1.Dispose();
            if (mBuffer2 != null)
                mBuffer2.Dispose();
            if (mpSoftwareToHardware != null)
                mpSoftwareToHardware.Dispose();
        }

        public HardwareRenderTarget(uint width, uint height, int lowResWidth, int lowResHeight)
        {
            initQuad();
            initShaders();
            resize(width, height);

            mLowResBuffer = new FBO(new System.Drawing.Size(lowResWidth, lowResHeight));
        }

        private void setBuffer(bool index)
        {
            if (index)
                mBuffer2.Enable();
            else
                mBuffer1.Enable();

            Gl.DrawBuffer(DrawBufferMode.ColorAttachment0);
            Gl.Viewport(0, 0, (int)mWidth, (int)mHeight);
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

        
        void _TextureToFullres(uint lowResTexId, int width, int height)
        {
            setSourceBuffer();
            Gl.ClearColor(0, 0, 1, 1);

            Gl.Clear(ClearBufferMask.ColorBufferBit);

            mpSoftwareToHardware.Use();
            mpSoftwareToHardware["sourceWidth"].SetValue((float)width);
            mpSoftwareToHardware["sourceHeight"].SetValue((float)height);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);

            Gl.BindTexture(TextureTarget.Texture2D, lowResTexId);
            Gl.ActiveTexture(TextureUnit.Texture0);

            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }

        public void lowresToHighres()
        {
            _TextureToFullres(mLowResBuffer.TextureID[0], mLowResBuffer.Size.Width, mLowResBuffer.Size.Height);
        }

        //Sets this as the current render target to be shown to screen
        public void finaliseAndSet()
        {
            mpHardwareToScreen.Use();
            Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            Gl.BindBuffer(BufferTarget.ArrayBuffer, mScreenQuadVBO);
            Gl.EnableVertexAttribArray(0);
            Gl.VertexAttribPointer(0, 4, VertexAttribPointerType.Float, false, sizeof(float) * 4, IntPtr.Zero);

            Gl.BindTexture(TextureTarget.Texture2D, getSourceBuffer().TextureID[0]);
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.DrawArrays(BeginMode.Triangles, 0, 6);
        }

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
            mWidth = width;
            mHeight = height;

            mpSoftwareToHardware.Use();
            mpSoftwareToHardware["screenWidth"].SetValue((float)width);
            mpSoftwareToHardware["screenHeight"].SetValue((float)height);
            if (mBuffer1 != null)
                mBuffer1.Dispose();
            if (mBuffer2 != null)
                mBuffer2.Dispose();

            System.Drawing.Size sds = new System.Drawing.Size((int)width, (int)height);
            mBuffer1 = createFramebuffer(sds);
            mBuffer2 = createFramebuffer(sds);
        }

        public void renderSoftwareRenderTarget(SoftwareRenderTarget rt)
        {
            _TextureToFullres(rt.upload(), (int)rt.width, (int)rt.height);
        }

        private void initShaders()
        {
            mpSoftwareToHardware = new OpenGL.ShaderProgram(
            //Vertex shader
              @"#version 130
                attribute vec4 vertuv;
                varying vec2 uv;
                uniform float screenWidth;
                uniform float screenHeight;
                uniform float sourceWidth;
                uniform float sourceHeight;

                void main()
                {
                        float aspecti = sourceWidth/sourceHeight;
                        float aspectr = screenWidth/screenHeight;

                        vec2 scale = vec2(1,1);
                        if(aspectr<aspecti)
                            scale.y = aspectr/aspecti;
                        else
                            scale.x = aspecti/aspectr;
                      
                    gl_Position = vec4(vertuv.xy*scale,0,1);
                
                uv = vec2(vertuv.z, vertuv.w);
                }",

                //Fragment shader
                @"#version 130
                varying vec2 uv;
                uniform sampler2D diffuse;

                void main()
                {
                    gl_FragColor = texture(diffuse, uv);
                }");

            mpHardwareToScreen = new OpenGL.ShaderProgram(
              //Vertex shader
              @"#version 130
                attribute vec4 vertuv;
                varying vec2 uv;
                void main()
                {
                    gl_Position = vec4(vertuv.xy,0,1);             
                    uv = vec2(vertuv.z, -vertuv.w);
                }",

                //Fragment shader
                @"#version 130
                varying vec2 uv;
                uniform sampler2D diffuse;

                void main()
                {
                    gl_FragColor = texture(diffuse, uv);
                }");

            mDrawIndexedBitmapShader = new ShaderProgram(
  //Vertex shader
  @"#version 130
                attribute vec4 vertuv;
                varying vec2 uv;
                void main()
                {
                    gl_Position = vec4(vertuv.xy,0,1);             
                    uv = vec2(vertuv.z, -vertuv.w);
                }",

    //Fragment shader
    @"#version 130
                varying vec2 uv;
                uniform sampler2D indexTexture;
                uniform sampler1D paletteTexture;

                void main()
                {
                    //gl_FragColor = texture(paletteTexture, texture(indexTexture, uv).r);
gl_FragColor=vec4(1,1,1,1);
                }");
            if (mDrawIndexedBitmapShader.FragmentShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling fragment shader: " + mDrawIndexedBitmapShader.FragmentShader.ShaderLog);
            if (mDrawIndexedBitmapShader.VertexShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling vertex shader: " + mDrawIndexedBitmapShader.VertexShader.ShaderLog);
            if (mDrawIndexedBitmapShader.ProgramLog.Length != 0)
                throw new Exception("Error linking shader: " + mDrawIndexedBitmapShader.ProgramLog);

            if (mpSoftwareToHardware.FragmentShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling fragment shader: " + mpSoftwareToHardware.FragmentShader.ShaderLog);
            if (mpSoftwareToHardware.VertexShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling vertex shader: " + mpSoftwareToHardware.VertexShader.ShaderLog);
            if (mpSoftwareToHardware.ProgramLog.Length != 0)
                throw new Exception("Error linking shader: " + mpSoftwareToHardware.ProgramLog);

            if (mpHardwareToScreen.FragmentShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling fragment shader: " + mpHardwareToScreen.FragmentShader.ShaderLog);
            if (mpHardwareToScreen.VertexShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling vertex shader: " + mpHardwareToScreen.VertexShader.ShaderLog);
            if (mpHardwareToScreen.ProgramLog.Length != 0)
                throw new Exception("Error linking shader: " + mpHardwareToScreen.ProgramLog);
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
