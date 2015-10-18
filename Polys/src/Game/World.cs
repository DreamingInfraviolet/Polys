using System;
using System.Collections.Generic;
using SDL2;
using Artemis;
    

/**
* Responsible for handling scenes and storing all game data.
*/

namespace Polys.Game
{
    class World : IntentHandler
    {
        SceneList mSceneList = new SceneList();

        CharacterController mController = new CharacterController();

        public Scene scene { get { return mSceneList.current; } private set { mSceneList.current = value; } }
        public Video.Camera camera { get; private set; }
        
        public IntentManager intentManager = new IntentManager();

        public bool running { get; private set; }


        public World()
        {
            running = true;
            camera = new Video.Camera();
            mSceneList.current = mSceneList.get("startup.lua");
            intentManager.addBinding(SDL.SDL_Keycode.SDLK_RIGHT, IntentManager.IntentType.WALK_RIGHT);
            intentManager.addBinding(SDL.SDL_Keycode.SDLK_LEFT, IntentManager.IntentType.WALK_LEFT);
            intentManager.addBinding(SDL.SDL_Keycode.SDLK_UP, IntentManager.IntentType.WALK_UP);
            intentManager.addBinding(SDL.SDL_Keycode.SDLK_DOWN, IntentManager.IntentType.WALK_DOWN);
            intentManager.addBinding(SDL.SDL_Keycode.SDLK_ESCAPE, IntentManager.IntentType.ESC);
                
            intentManager.register(this, IntentManager.IntentType.ESC, IntentManager.KeyType.DOWN);
            intentManager.register(mController, IntentManager.IntentType.WALK_DOWN, IntentManager.KeyType.HELD);
            intentManager.register(mController, IntentManager.IntentType.WALK_LEFT, IntentManager.KeyType.HELD);
            intentManager.register(mController, IntentManager.IntentType.WALK_RIGHT, IntentManager.KeyType.HELD);
            intentManager.register(mController, IntentManager.IntentType.WALK_UP, IntentManager.KeyType.HELD);  
        }

        public void preIntent(long timeParameter)
        {
            mController.begin(timeParameter);
        }

        /**
        * Performs a frame of the scene, with the timeParameter indicating the current run time in milliseconds.
        * The run time is used to interpolate between the game actions, and to execute time-based events.
        * @return True if the program should continue. False otherwise.
        * Assumes that SDL is initialised for the event system.
        */
        public void postIntent(long timeParameter)
        {
            mController.end();
            camera.move(mController.movementX, mController.movementY);
        }

        public void shutdown()
        {
            if (mSceneList != null)
                mSceneList.Dispose();
        }

        public void handleIntent(IntentManager.IntentType intentCode, IntentManager.KeyType type)
        {
            if (intentCode == IntentManager.IntentType.ESC)
                running = false;
        }
    }
}
