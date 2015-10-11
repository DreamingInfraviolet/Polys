using SDL2;
using System;
using OpenGL;
using TiledSharp;

/**
* This class is responsible for handling the window and drawing the world.
*/
namespace Polys.Video
{
    class Video
    {
        private IntPtr mWindow;
        private IntPtr mGglContext;
        HardwareRenderTarget mHardwareRenderTarget;
        int mWidth, mHeight;
        
        public void updateWindowSize()
        {
            int width, height;
            SDL.SDL_GetWindowSize(mWindow, out width, out height);
            mWidth = width;
            mHeight = height;
            mHardwareRenderTarget.resize((uint)width, (uint)height);
            Gl.Viewport(0, 0, width, height);
        }

        public void setWindowSize(int width, int height)
        {
            SDL.SDL_SetWindowSize(mWindow, width, height);
            updateWindowSize();
        }

        Effect chromaticShiftFx;

        public Video(int width, int height)
        {

            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
                throw new Exception("Could not initialise video system");

            mWindow = SDL.SDL_CreateWindow("Polys",
                SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                width, height,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL |
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE |
                SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);

            mGglContext = SDL.SDL_GL_CreateContext(mWindow);

            //Set vsync on/off
            SDL.SDL_GL_SetSwapInterval(0);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            mHardwareRenderTarget = new HardwareRenderTarget((uint)width, (uint)height, 256, 192);

            chromaticShiftFx = new Effect(
                @"#version 130
                attribute vec4 vertuv;
                varying vec2 uv;
                void main()
                {
                    gl_Position = vec4(vertuv.xy,0,1);             
                    uv = vec2(vertuv.z, vertuv.w);
                }",

                //Fragment shader
                @"#version 130
                varying vec2 uv;
                uniform sampler2D diffuse;  
                uniform float time;
                vec2 amount = vec2(0.009, 0.009)*sin(time*0.01);

                void main()
                {
                    float r = texture(diffuse, uv+amount).r;
                    float g = texture(diffuse, uv).g;
                    float b = texture(diffuse, uv-amount).b;
                    float a = texture(diffuse, uv).a;
                    gl_FragColor = vec4(r,g,b,a);
                }");

        }

        public void shutdown()
        {
            mHardwareRenderTarget.shutdown();
            SDL.SDL_GL_DeleteContext(mGglContext);
            SDL.SDL_DestroyWindow(mWindow);
        }



        public void draw(Game.World world, long timeParameter)
        {
            Gl.ClearColor(0, 0, 0, 1);
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            
            Game.Scene scene = world.scene;
            Camera camera = world.camera;

            mHardwareRenderTarget.clear();

            //Draw the tilemap

            //Draw each layer:
            
            foreach(TileLayer layer in scene.layers)
            {
                //Draw grid
                mHardwareRenderTarget.draw(layer, camera);
            }

            mHardwareRenderTarget.lowresToHighres();
            //mHardwareRenderTarget.applyEffect(chromaticShiftFx, timeParameter);
            mHardwareRenderTarget.finaliseAndSet();


            SDL.SDL_GL_SwapWindow(mWindow);

            ErrorCode err = Gl.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine(err);
        }
    }
}