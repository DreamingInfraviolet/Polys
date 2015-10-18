using System;
using SDL2;

class Program
{
    /** Initialises the systems from script. */
    static void Genesis(out Polys.Video.Video video, out Polys.Audio.Audio audio, out Polys.Game.World world)
    {
        video = new Polys.Video.Video(400, 300);
        audio = new Polys.Audio.Audio();
        world = new Polys.Game.World();

        //Configure systems:

        //Register classes to be configured
        MoonSharp.Interpreter.UserData.RegisterType<Polys.Video.Video>();
        MoonSharp.Interpreter.UserData.RegisterType<Polys.Audio.Audio>();
        MoonSharp.Interpreter.UserData.RegisterType<Polys.Game.World>();

        //Create MS objects:
        MoonSharp.Interpreter.DynValue videoDN = MoonSharp.Interpreter.UserData.Create(video);
        MoonSharp.Interpreter.DynValue audioDN = MoonSharp.Interpreter.UserData.Create(audio);
        MoonSharp.Interpreter.DynValue worldDN = MoonSharp.Interpreter.UserData.Create(world);

        //Create script and assign objects to LUA variables:
        MoonSharp.Interpreter.Script configurationScript = new MoonSharp.Interpreter.Script();
        configurationScript.Globals.Set("video", videoDN);
        configurationScript.Globals.Set("audio", audioDN);
        configurationScript.Globals.Set("world", worldDN);

        //Perform initialisation
        configurationScript.DoFile("configure.lua");
    }

    static void Main()
    {
        //Temporary solution to set the directory:
        System.IO.Directory.SetCurrentDirectory("..\\..\\..\\res");

        Polys.Video.Video video;
        Polys.Audio.Audio audio;
        Polys.Game.World world;


        Genesis(out video, out audio, out world);
        
        System.Diagnostics.Stopwatch worldTimeBegin = new System.Diagnostics.Stopwatch();
        worldTimeBegin.Start();

        Polys.Input input = new Polys.Input(world.intentManager);
        
        bool run = true;

        while (world.running && run)
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
                        run = false;
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

            long time = worldTimeBegin.ElapsedMilliseconds;
            world.preIntent(time);
            input.process();
            world.postIntent(time);
            video.draw(world, time);
            audio.play(world);
        }

        world.shutdown();
        audio.shutdown();
        video.shutdown();
    }
}