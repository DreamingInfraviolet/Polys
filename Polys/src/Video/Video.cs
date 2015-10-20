using MoonSharp.Interpreter;
using OpenGL;
using SDL2;
using System;

namespace Polys.Video
{
    /**
    * This class is responsible for handling the window and doing high-level drawing.
    */

    class Video : IScriptInitialisable
    {
        //A pointer to the window
        IntPtr window;

        //A pointer to the GL context
        IntPtr mGglContext;

        //The low level renderer that handles the screen buffers
        FramebufferManager framebufferManager;

        //The width and height of the window
        int width, height;

        //A temporary effect, later to be replaced with an effect pipeline.
        Effect chromaticShiftFx;

        /** If this is enabled, the post processing filters are active. This introduces an intermediary texture to which the game
          * is rendered before it is sent to the screen. If this is false, then that stage is skipped, resulting in a dramatic performance
          * increase on systems with weak graphics cards but also skipping all post-processing steps (e.g., Effects). */
        public bool postFx { get; set; }

        /** Updates internal parameters to correctly handle the new size of the window. Call his whenever the window is resized */
        public void updateWindowSize()
        {
            SDL.SDL_GetWindowSize(window, out this.width, out this.height);
            framebufferManager.resize(width, height);
            Gl.Viewport(0, 0, width, height);
        }

        /** Forces the window to resized to a particular size. Updates internal state automatically. */
        public void setWindowSize(int width, int height)
        {
            SDL.SDL_SetWindowSize(window, width, height);
            updateWindowSize();
        }

        /** The constructor creates the window and initialises a graphics state.
          * Note: For some reason, if put elsewhere this code causes a DLL error. */
        public Video()
        {
            //Initialise SDL
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
                throw new Exception("Could not initialise video system");

            //Create a window
            window = SDL.SDL_CreateWindow("Polys",
                SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                640, 480,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL |
                SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE |
                SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS);

            //Create the GL context
            mGglContext = SDL.SDL_GL_CreateContext(window);

            LowLevelRenderer.initialise();
            LowLevelRenderer.blending = true;

            framebufferManager = new FramebufferManager(640, 480, 256, 192);
            
            //Temporarily initialise chromatic shift effect
            chromaticShiftFx = new Effect(loadShader("effects/chromaticShift"));
        }

        /** Shuts down the video state, destroying the window and GL context. */
        public void shutdown()
        {
            framebufferManager.Dispose();
            SDL.SDL_GL_DeleteContext(mGglContext);
            SDL.SDL_DestroyWindow(window);
        }

        /** Draws everything in a world that should be drawn. */
        public void draw(Game.World world)
        {
            //Clear render buffers

            framebufferManager.clear();

            framebufferManager.bind();

            HighLevelRenderer.setTargetSize(framebufferManager.lowResWidth, framebufferManager.lowResHeight);
 
            //Draw all layers to the low-res target
            foreach (TileLayer layer in world.scene.layers)
                HighLevelRenderer.draw(layer, world.camera);

            framebufferManager.lowresToHighres(!postFx);

            if (postFx)
            {
                framebufferManager.applyEffect(chromaticShiftFx, Time.currentTime);
                framebufferManager.highresToScreen();
            }

            //Present to the user
            SDL.SDL_GL_SwapWindow(window);

            Util.Util.CheckGl();
        }

        /** Loads a shader from file, checking for any errors.
          * @param relativePathWithoutExtension The path to the shader file pair in shaders/. For example,
                   for two files shaders/test.frag and shaders/test.vert, the input would be "test". */
        public static ShaderProgram loadShader(string relativePathWithoutExtension)
        {
            //Get the text for the shaders and compile
            string vert = System.IO.File.ReadAllText(String.Format("shaders/{0}.vert", relativePathWithoutExtension));
            string frag = System.IO.File.ReadAllText(String.Format("shaders/{0}.frag", relativePathWithoutExtension));
            ShaderProgram program = new ShaderProgram(vert, frag);

            //Check for errors
            if (program.FragmentShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling fragment shader: " + program.FragmentShader.ShaderLog);
            if (program.VertexShader.ShaderLog.Length != 0)
                throw new Exception("Error compiling vertex shader: " + program.VertexShader.ShaderLog);
            if (program.ProgramLog.Length != 0)
                throw new Exception("Error linking shader: " + program.ProgramLog);

            return program;
        }

        /** Inherited from IScriptInitialisable */
        public string ScriptName()
        {
            return "video";
        }

        /** Inherited from IScriptInitialisable */
        public void InitialiseFromScript(Table table)
        {
            //Set screen size
            setWindowSize(ScriptManager.retrieveValue(table, "width", 600),
                       ScriptManager.retrieveValue(table, "height", 400));

            //Post effects?
            postFx = ScriptManager.retrieveValue(table, "postFx", 1) != 0;

            //Set vsync on/off
            SDL.SDL_GL_SetSwapInterval((ScriptManager.retrieveValue(table, "vsync", 0) != 0) ? 1 : 0);

            //Fullscreen?
            if (ScriptManager.retrieveValue(table, "fullscreen", 0) != 0)
                SDL.SDL_SetWindowFullscreen(window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
        }
    }
}


