using System;
using SDL2;

class Program
{

    static T retrieveValue<T> (MoonSharp.Interpreter.Table table, String str, T def)
    {
        MoonSharp.Interpreter.DynValue value = table.Get(str);
        if (value.IsNilOrNan())
            return def;
        else
        {
            //Do some type checking
            if (def is string && value.Type == MoonSharp.Interpreter.DataType.String)
                return (T)Convert.ChangeType(value.String, typeof(T));
            if (def is int && value.Type == MoonSharp.Interpreter.DataType.Number)
                return (T)Convert.ChangeType((int)value.Number, typeof(T));
            if (def is double && value.Type == MoonSharp.Interpreter.DataType.Number)
                return (T)Convert.ChangeType(value.Number, typeof(T));
            else
                throw new Exception("Invalid type for variable " + value.ToString() + " in LUA table.");

        }
    }
    
    /** Initialises the systems from script. */
    static void Genesis(out Polys.Video.Video video, out Polys.Audio.Audio audio, out Polys.Game.World world)
    {
        MoonSharp.Interpreter.Script configurationScript = new MoonSharp.Interpreter.Script();


        //Create tables
        MoonSharp.Interpreter.Table videoTable = new MoonSharp.Interpreter.Table(configurationScript);
        MoonSharp.Interpreter.Table audioTable = new MoonSharp.Interpreter.Table(configurationScript);
        MoonSharp.Interpreter.Table worldTable = new MoonSharp.Interpreter.Table(configurationScript);

        //Add them to the script
        configurationScript.Globals["video"] = videoTable;
        configurationScript.Globals["audio"] = audioTable;
        configurationScript.Globals["world"] = worldTable;

        //Fill in
        configurationScript.DoFile("configure.lua");

        //Init video
        video = new Polys.Video.Video(retrieveValue(videoTable, "width", 600), retrieveValue(videoTable, "height", 480));
        video.postFx = retrieveValue(videoTable, "postFx", 1) != 0;

        //Init audio
        audio = new Polys.Audio.Audio();

        //Init world
        world = new Polys.Game.World();
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

        while (world.running)
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