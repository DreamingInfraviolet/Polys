using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using SDL2;


/**
* Responsible for handling scenes and storing all game data.
*/

namespace Polys.Game
{
    class World : IIntentHandler, IScriptInitialisable
    {
        SceneList mSceneList = new SceneList();

        CharacterController mController = new CharacterController();

        public Scene scene { get { return mSceneList.current; } private set { mSceneList.current = value; } }
        public Video.Camera camera { get; private set; }
        
        public IntentManager intentManager = new IntentManager();
        
        public bool running { get; private set; }


        public void beforeInput()
        {
            Time.startFrame();
            mController.begin(Time.deltaTime);
        }

        /**
        * Performs a frame of the scene, with the timeParameter indicating the current run time in milliseconds.
        * The run time is used to interpolate between the game actions, and to execute time-based events.
        * @return True if the program should continue. False otherwise.
        * Assumes that SDL is initialised for the event system.
        */
        public void afterInput()
        {
            mController.end();
            camera.move(mController.movementX, mController.movementY);
        }

        public void AfterLoop()
        {
            Time.endFrame();
        }

        public void shutdown()
        {
            if (mSceneList != null)
                mSceneList.Dispose();
        }

        public void handleIntent(IntentManager.IntentType intentCode, bool isKeyDown, bool isKeyUp, bool isKeyHeld)
        {
            if (intentCode == IntentManager.IntentType.ESC)
                end();
        }

        public void end()
        {
            running = false;
        }

        public string ScriptName()
        {
            return "world";
        }

        public void InitialiseFromScript(Table table)
        {
            running = true;
            camera = new Video.Camera();
            mSceneList.current = mSceneList.get("startup.lua");


            intentManager.register(this, IntentManager.IntentType.ESC, true, false, false);
            intentManager.register(mController, IntentManager.IntentType.WALK_DOWN, true, false, true);
            intentManager.register(mController, IntentManager.IntentType.WALK_LEFT, true, false, true);
            intentManager.register(mController, IntentManager.IntentType.WALK_RIGHT, true, false, true);
            intentManager.register(mController, IntentManager.IntentType.WALK_UP, true, false, true);
        }
    }
}
