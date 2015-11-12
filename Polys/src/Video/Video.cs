using MoonSharp.Interpreter;
using OpenGL;
using SDL2;
using System;

namespace Polys.Video
{
    /**
    * This class is responsible for handling the window and doing high-level drawing.
    */

    public class Video : IScriptInitialisable, IDisposable
    {
        //A pointer to the window
        IntPtr window;

        //A pointer to the GL context
        IntPtr mGglContext;

        //The low level renderer that handles the screen buffers
        FramebufferManager framebufferManager;

        //The width and height of the window
        int width, height;

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

            SDL.SDL_DisplayMode mode;
            SDL.SDL_GetDesktopDisplayMode(0, out mode);
            int drawTargetWidth = 256, drawTargetHeight = 192;

            //Set the width of the window to span the screen, and the height to best fit the screen
            //resolution while being a multiple of the drawing target.
            int windowWidth = mode.w;
            int windowHeight = drawTargetHeight * (mode.h / drawTargetHeight);



            //Create a window. The dimensions are the biggest that is a multiple of the low res target.
            window = SDL.SDL_CreateWindow("Polys",
                SDL.SDL_WINDOWPOS_CENTERED, 0,
                windowWidth, windowHeight,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL |
                SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS |
                SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS);

            //Create the GL context
            mGglContext = SDL.SDL_GL_CreateContext(window);

            LowLevelRenderer.initialise();
            LowLevelRenderer.blending = true;

            framebufferManager = new FramebufferManager(windowWidth, windowHeight, drawTargetWidth, drawTargetHeight);

            HighLevelRenderer.setTargetSize(framebufferManager.lowResWidth, framebufferManager.lowResHeight);

        }

        /** Draws everything in a world that should be drawn. */
        public void draw(Game.World world)
        {
            //Clear render buffers

            framebufferManager.clear();
            framebufferManager.bind();
            HighLevelRenderer.setTargetSize(framebufferManager.lowResWidth, framebufferManager.lowResHeight);
            LowLevelRenderer.geometry = LowLevelRenderer.quad;
            LowLevelRenderer.shader = HighLevelRenderer.shaderIndexedBitmapSprite;

            //Draw all to low res target
            world.stateManager.draw();

            framebufferManager.lowresToHighres(!postFx);

            if (postFx)
            {
                //Apply effects here
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


            //Check for errors.
            int[] vertexStatus = new int[1];
            int[] fragmentStatus = new int[1];
            int[] linkStatus = new int[1];
            Gl.GetShaderiv(program.VertexShader.ShaderID, ShaderParameter.CompileStatus, vertexStatus);
            Gl.GetShaderiv(program.VertexShader.ShaderID, ShaderParameter.CompileStatus, fragmentStatus);
            Gl.GetShaderiv(program.VertexShader.ShaderID, ShaderParameter.CompileStatus, linkStatus);

            if (vertexStatus[0] == 0)
                throw new Exception("Error compiling fragment shader: " + program.FragmentShader.ShaderLog);
            if (fragmentStatus[0] == 0)
                throw new Exception("Error compiling vertex shader: " + program.VertexShader.ShaderLog);
            if (linkStatus[0] == 0)
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
            //Post effects?
            postFx = ScriptManager.retrieveValue(table, "postFx", 1) != 0;

            //Set vsync on/off
            SDL.SDL_GL_SetSwapInterval((ScriptManager.retrieveValue(table, "vsync", 0) != 0) ? 1 : 0);

            //Fullscreen?
            if (ScriptManager.retrieveValue(table, "fullscreen", 0) != 0)
                SDL.SDL_SetWindowFullscreen(window, (uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN_DESKTOP);
        }

        public void Dispose()
        {
            framebufferManager.Dispose();
            SDL.SDL_GL_DeleteContext(mGglContext);
            SDL.SDL_DestroyWindow(window);
        }
    }
}


