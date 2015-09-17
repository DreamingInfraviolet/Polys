using System;
using System.Collections.Generic;
using SDL2;

using TiledSharp;

/**
* Responsible for handling scenes and storing all game data.
*/

namespace Polys.Game
{
    class World
    {
        SceneList mSceneList = new SceneList();

        Player mPlayer;

        CharacterController mController = new CharacterController();

        public Scene scene { get { return mSceneList.current; } private set { mSceneList.current = value; } }
        public Video.Camera camera { get; private set; }
        
        //This keytable is used to handle continuous key presses.
        //Once a key is pressed, it is stored until it is unpressed.
        //Maps SDL_Keycode value to bool, as it's a non-linear enum.
        public Dictionary<int, bool> mKeyTable;

        private void initKeyDictionary()
        {
            if (mKeyTable != null)
                return;

            mKeyTable = new Dictionary<int, bool>();
            string[] entries = Enum.GetNames(typeof(SDL.SDL_Keycode));

            for (int i = 0; i < entries.Length; ++i)
                mKeyTable.Add((int)Enum.Parse(typeof(SDL.SDL_Keycode), entries[i]), false);
        }


        public World()
        {
            camera = new Video.Camera();
            initKeyDictionary();
            mSceneList.current = mSceneList.get("startup.lua");
        }


        /**
        * Performs a frame of the scene, with the timeParameter indicating the current run time in milliseconds.
        * The run time is used to interpolate between the game actions, and to execute time-based events.
        * @return True if the program should continue. False otherwise.
        * Assumes that SDL is initialised for the event system.
        */
        public bool step(long timeParameter)
        {

            mController.begin(timeParameter);

            if (mKeyTable[(int)SDL.SDL_Keycode.SDLK_ESCAPE])
                return false;

            if (mKeyTable[(int)SDL.SDL_Keycode.SDLK_w] || mKeyTable[(int)SDL.SDL_Keycode.SDLK_UP])
                mController.addMoveVector(0, 1);
            if (mKeyTable[(int)SDL.SDL_Keycode.SDLK_s] || mKeyTable[(int)SDL.SDL_Keycode.SDLK_DOWN])
                mController.addMoveVector(0, -1);
            if (mKeyTable[(int)SDL.SDL_Keycode.SDLK_d] || mKeyTable[(int)SDL.SDL_Keycode.SDLK_RIGHT])
                mController.addMoveVector(1, 0);
            if (mKeyTable[(int)SDL.SDL_Keycode.SDLK_a] || mKeyTable[(int)SDL.SDL_Keycode.SDLK_LEFT])
                mController.addMoveVector(-1, 0);
            
            mController.end();

            camera.move(mController.movementX, mController.movementY);
            

            
            return true;
        }

        public void shutdown()
        {
            if (mSceneList != null)
                mSceneList.Dispose();
        }
    }
}
