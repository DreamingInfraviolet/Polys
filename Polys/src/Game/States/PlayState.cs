using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    class PlayState : State
    {
        CharacterController controller = new CharacterController(0.3f);
        Video.Camera camera = new Video.Camera();

        public PlayState()
        {
            IntentManager.register(controller, IntentManager.IntentType.WALK_DOWN, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_LEFT, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_RIGHT, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_UP, true, false, true);
        }

        //The current list of scenes
        SceneList sceneList = new SceneList();


        public StateManager.StateRenderResult draw()
        {
            foreach(var layer in sceneList.current.layers)
            {
                Video.HighLevelRenderer.draw(layer, camera);
            }

            return StateManager.StateRenderResult.StopDrawing;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }


        public StateManager.StateUpdateResult updateAfterInput()
        {
            controller.finishGatheringInput();
            camera.centreOn(controller.positionX, controller.positionY);

            return StateManager.StateUpdateResult.Finish;
        }

        public StateManager.StateUpdateResult updateAfterFrame()
        {
            return StateManager.StateUpdateResult.Finish;
        }




        public void Dispose()
        {
            sceneList.Dispose();
        }
    }
}
