using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    public class PlayState : State
    {
        CharacterController controller = new CharacterController(30);
        Video.Camera camera = new Video.Camera();
        Player player = new Player("Anima", new Video.Sprite("assets/sprites/player.bmp", new Util.Rect(0, 0, 16, 32)));

        Video.Font font = new Video.Font("assets/fonts/default.bmp", 8, 16);

        public PlayState()
        {
            IntentManager.register(controller, IntentManager.IntentType.WALK_DOWN, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_LEFT, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_RIGHT, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_UP, true, false, true);

            controller.character = player;

            controller.position.x = sceneList.current.playerStartPixelX;
            controller.position.y = sceneList.current.playerStartPixelY;
            sceneList.current.startLayer.tiles.Add(player.sprite);
        }

        //The current list of scenes
        SceneList sceneList = new SceneList();


        public StateManager.StateRenderResult draw()
        {
            Video.HighLevelRenderer.draw(sceneList.current, camera);
            font.renderText("Polys", 100, 170, 240);

            return StateManager.StateRenderResult.StopDrawing;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }


        public StateManager.StateUpdateResult updateAfterInput()
        {
            controller.finishGatheringInput(sceneList.current.collisionObjects);
            player.updateUv();
            camera.centreOn(player.sprite.rect.x, player.sprite.rect.y);

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
