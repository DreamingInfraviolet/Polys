using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    class PlayState : State
    {
        CharacterController controller = new CharacterController(100.0f);
        Video.Camera camera = new Video.Camera();
        Player player = new Player("Anima", new Video.DrawableSprite("assets/sprites/player.bmp"));

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

            Video.HighLevelRenderer.draw(player.sprite, camera);

            return StateManager.StateRenderResult.StopDrawing;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }


        public StateManager.StateUpdateResult updateAfterInput()
        {
            controller.finishGatheringInput();
            player.sprite.posX = (int)controller.position.x;
            player.sprite.posY = (int)controller.position.y;
            System.Console.WriteLine(player.sprite.posX + " " + player.sprite.posY);
            camera.centreOn(player.sprite.posX, player.sprite.posY);

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
