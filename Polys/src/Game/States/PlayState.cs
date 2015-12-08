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
        Player player = new Player("Anima", new Video.Sprite("assets/sprites/zeta.bmp", new Util.Rect(0, 0, 16, 32)),2);
        StateManager sm = null;


        public PlayState()
        {
            controller.character = player;

            controller.position.x = sceneList.current.playerStartPixelX;
            controller.position.y = sceneList.current.playerStartPixelY;
            sceneList.current.startLayer.objects.Add(player.sprite);
        }

        //The current list of scenes
        SceneList sceneList = new SceneList();


        public StateManager.StateRenderResult draw()
        {
            Video.HighLevelRenderer.draw(sceneList.current, camera);
            return StateManager.StateRenderResult.Continue;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }


        public StateManager.StateUpdateResult updateAfterInput()
        {
            controller.update(sceneList.current.collisionLayer);
            player.updateUv();
            camera.centreOn(player.sprite.rect.x, player.sprite.rect.y);

            if (IntentManager.isActive(IntentManager.IntentType.ESC))
                sm.push(new MainMenuState(false));
            return StateManager.StateUpdateResult.Finish;
        }

        public StateManager.StateUpdateResult updateAfterFrame()
        {
            return StateManager.StateUpdateResult.Finish;
        }

        public void setStateManager(StateManager m)
        {
            sm = m;
        }

        public void Dispose()
        {
            sceneList.Dispose();
        }
    }
}
