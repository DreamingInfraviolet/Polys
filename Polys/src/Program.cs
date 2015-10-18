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

    static void Main()
    {
        //Temporary solution to set the resource directory:
        System.IO.Directory.SetCurrentDirectory("..\\..\\..\\res");

        //Initialise subsystems
        initialiseSubsystems(out video, out audio, out world);

        //Main loop
        while (world.running)
        {
            world.frameStart();

            handleEvents();
            input.process();

            world.frameMiddle();

            video.draw(world);
            audio.play(world);

            world.frameEnd();
        }

        world.shutdown();
        audio.shutdown();
        video.shutdown();
    }
    
    /** Initialises the systems. */
    static void initialiseSubsystems(out Video video, out Audio audio, out World world)
    {
        //Create script
        MoonSharp.Interpreter.Script configurationScript = new MoonSharp.Interpreter.Script();

        //Create tables
        MoonSharp.Interpreter.Table tVideo = new MoonSharp.Interpreter.Table(configurationScript);
        MoonSharp.Interpreter.Table tAudio = new MoonSharp.Interpreter.Table(configurationScript);
        MoonSharp.Interpreter.Table tWorld = new MoonSharp.Interpreter.Table(configurationScript);

        //Add them to the script
        configurationScript.Globals["video"] = tVideo;
        configurationScript.Globals["audio"] = tAudio;
        configurationScript.Globals["world"] = tWorld;

        //Fill in the tables
        configurationScript.DoFile("configure.lua");

        //Init video
        video = new Video(ScriptManager.retrieveValue(tVideo, "width", 600),
                          ScriptManager.retrieveValue(tVideo, "height", 480));

        video.postFx = ScriptManager.retrieveValue(tVideo, "postFx", 1) != 0;

        //Init audio
        audio = new Audio();

        //Init world
        world = new World();

        //Init input
        input = new Input(world.intentManager);
    }

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