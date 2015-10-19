using System;
using SDL2;
using Polys;
using Polys.Video;
using Polys.Audio;
using Polys.Game;

class Program
{
    static Video video;
    static Audio audio;
    static World world;
    static Input input;

    /** The entry point of the program. It controls the initialisation and defines the main game loop. */
    static void Main()
    {
        //Temporary solution to set the resource directory:
        System.IO.Directory.SetCurrentDirectory("..\\..\\..\\res");

        //Initialise subsystems
        video = new Video();
        audio = new Audio();
        world = new World();
        input = new Input(world.intentManager);

        ScriptManager.initialiseFromScript("configure.lua", video, audio, world, input);

        //Main loop
        while (world.running)
        {
            world.beforeInput();

            handleEvents();
            input.finalise();

            world.afterInput();

            video.draw(world);
            audio.play(world);

            world.AfterLoop();
        }

        world.shutdown();
        audio.shutdown();
        video.shutdown();
    }
    
    /** Handles window events, feeding them into the relevant subsystems. */
    static void handleEvents()
    {
        SDL.SDL_Event e;
        while (SDL.SDL_PollEvent(out e) != 0)
        {
            switch (e.type)
            {
                case SDL.SDL_EventType.SDL_KEYDOWN:
                    input.keyDown(e.key.keysym.sym);
                    break;
                case SDL.SDL_EventType.SDL_KEYUP:
                    input.keyUp(e.key.keysym.sym);
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    break;
                case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                    break;
                case SDL.SDL_EventType.SDL_MOUSEMOTION:
                    break;
                case SDL.SDL_EventType.SDL_QUIT:
                    world.end();
                    break;
                case SDL.SDL_EventType.SDL_WINDOWEVENT:
                    switch (e.window.windowEvent)
                    {
                        case SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED:
                            video.updateWindowSize();
                            break;
                    }
                    break;
            }
        }
    }
}