using MoonSharp.Interpreter;
using System.Collections.Generic;

/**

*/

namespace Polys.Game
{
    public class World : IIntentHandler, IScriptInitialisable, System.IDisposable
    {
        public States.StateManager stateManager = new States.StateManager(new States.MainMenuState());
        
        /** The current camera */
        public Video.Camera camera { get; private set; }
        
        /** Whether the world should be running */
        public bool running { get; private set; }
        
        /** This method is executed each frame before input is collected */
        public void beforeInput()
        {
            Time.startFrame();
            stateManager.update(States.StateManager.UpdateType.BeforeInput);
        }

        /** This method is executed each frame after input is collected */
        public void afterInput()
        {
            stateManager.update(States.StateManager.UpdateType.AfterInput);
        }

        /** This method is executed each frame just before the end of the frame */
        public void AfterLoop()
        {
            Time.endFrame();
            stateManager.update(States.StateManager.UpdateType.AfterFrame);
        }

        /** Shuts down the world, freeing resources */
        public void Dispose()
        {
            stateManager.Dispose();
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

            IntentManager.register(this, IntentManager.IntentType.ESC, true, false, false);
        }
    }
}
