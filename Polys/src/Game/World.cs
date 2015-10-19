using MoonSharp.Interpreter;
using System.Collections.Generic;

/**
* Responsible for handling scenes and storing all game state.
*/

namespace Polys.Game
{
    class World : IIntentHandler, IScriptInitialisable
    {

        Stack<State> states = new Stack<State>();

        //The current list of scenes
        SceneList sceneList = new SceneList();

        //The active character controller
        CharacterController controller = new CharacterController();

        /** The current scene */
        public Scene scene { get { return sceneList.current; } private set { sceneList.current = value; } }

        /** The current camera */
        public Video.Camera camera { get; private set; }
        
        /** The active intent manager */
        public IntentManager intentManager = new IntentManager();
        
        /** Whether the world should be running */
        public bool running { get; private set; }
        
        /** This method is executed each frame before input is collected */
        public void beforeInput()
        {
            Time.startFrame();
            controller.begin(Time.deltaTime);
        }

        /** This method is executed each frame after input is collected */
        public void afterInput()
        {
            controller.end();
            camera.move(controller.movementX, controller.movementY);
        }

        /** This method is executed each frame just before the end of the frame */
        public void AfterLoop()
        {
            Time.endFrame();
        }

        /** Shuts down the world, freeing resources */
        public void shutdown()
        {
            if (sceneList != null)
                sceneList.Dispose();
        }

        /** Handles the intents for which the world is registered */
        public void handleIntent(IntentManager.IntentType intentCode, bool isKeyDown, bool isKeyUp, bool isKeyHeld)
        {
            if (intentCode == IntentManager.IntentType.ESC)
                end();
        }

        /** Marks that the world should no longer be running */
        public void end()
        {
            running = false;
        }

        /** Returns the script representation of the world table */
        public string ScriptName()
        {
            return "world";
        }

        /** Initialises the world from script */
        public void InitialiseFromScript(Table table)
        {
            running = true;
            camera = new Video.Camera();

            intentManager.register(this, IntentManager.IntentType.ESC, true, false, false);
            intentManager.register(controller, IntentManager.IntentType.WALK_DOWN, true, false, true);
            intentManager.register(controller, IntentManager.IntentType.WALK_LEFT, true, false, true);
            intentManager.register(controller, IntentManager.IntentType.WALK_RIGHT, true, false, true);
            intentManager.register(controller, IntentManager.IntentType.WALK_UP, true, false, true);
        }
    }
}
