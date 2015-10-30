using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    class PlayState : State
    {
        CharacterController controller = new CharacterController(30);
        Video.Camera camera = new Video.Camera();
        Player player = new Player("Anima", new Video.DrawableSprite("assets/sprites/player.bmp",0,0, 16, 32));

        Video.Font font = new Video.Font("assets/fonts/default.bmp", 8, 16,
            new Dictionary<char, Util.Pair<int, int>>()
            {
                { 'a', new Util.Pair<int, int>(1, 3) },
                { 'b', new Util.Pair<int, int>(1, 1) },
                { 'c', new Util.Pair<int, int>(1, 1) },
                { 'd', new Util.Pair<int, int>(1, 1) },
                { 'e', new Util.Pair<int, int>(1, 1) },
                { 'f', new Util.Pair<int, int>(1, 1) },
                { 'g', new Util.Pair<int, int>(1, 1) },
                { 'h', new Util.Pair<int, int>(1, 1) },
                { 'i', new Util.Pair<int, int>(1, 1) },
                { 'j', new Util.Pair<int, int>(1, 1) },
                { 'k', new Util.Pair<int, int>(1, 1) },
                { 'l', new Util.Pair<int, int>(1, 1) },
                { 'm', new Util.Pair<int, int>(1, 1) },
                { 'n', new Util.Pair<int, int>(1, 1) },
                { 'o', new Util.Pair<int, int>(1, 1) },
                { 'p', new Util.Pair<int, int>(1, 1) },
                { 'q', new Util.Pair<int, int>(1, 1) },
                { 'r', new Util.Pair<int, int>(1, 1) },
                { 's', new Util.Pair<int, int>(1, 1) },
                { 't', new Util.Pair<int, int>(1, 1) },
                { 'u', new Util.Pair<int, int>(1, 1) },
                { 'v', new Util.Pair<int, int>(1, 1) },
                { 'A', new Util.Pair<int, int>(5, 1) },
                { 'B', new Util.Pair<int, int>(6, 1) },
                { 'C', new Util.Pair<int, int>(7, 1) },
                { 'D', new Util.Pair<int, int>(8, 1) },
                { 'E', new Util.Pair<int, int>(9, 1) },
                { 'F', new Util.Pair<int, int>(10, 1) },
                { 'G', new Util.Pair<int, int>(11, 1) },
                { 'H', new Util.Pair<int, int>(12, 1) },
                { 'I', new Util.Pair<int, int>(13, 1) },
                { 'J', new Util.Pair<int, int>(14, 1) },
                { 'K', new Util.Pair<int, int>(15, 1) },
                { 'L', new Util.Pair<int, int>(16, 1) },
                { 'M', new Util.Pair<int, int>(17, 1) },
                { 'N', new Util.Pair<int, int>(18, 1) },
                { 'O', new Util.Pair<int, int>(19, 1) },
                { 'P', new Util.Pair<int, int>(20, 1) },
                { 'Q', new Util.Pair<int, int>(21, 1) },
                { 'R', new Util.Pair<int, int>(22, 1) },
                { 'S', new Util.Pair<int, int>(23, 1) },
                { 'T', new Util.Pair<int, int>(24, 1) },
                { 'U', new Util.Pair<int, int>(25, 1) },
                { 'V', new Util.Pair<int, int>(26, 1) },
                { 'W', new Util.Pair<int, int>(27, 1) },
                { 'X', new Util.Pair<int, int>(28, 1) },
                { 'Y', new Util.Pair<int, int>(29, 1) },
                { 'Z', new Util.Pair<int, int>(30, 1) },

            });

        public PlayState()
        {
            IntentManager.register(controller, IntentManager.IntentType.WALK_DOWN, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_LEFT, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_RIGHT, true, false, true);
            IntentManager.register(controller, IntentManager.IntentType.WALK_UP, true, false, true);

            controller.character = player;
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

            font.renderText("HELLO WORLD", 2, 5);

            return StateManager.StateRenderResult.StopDrawing;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }


        public StateManager.StateUpdateResult updateAfterInput()
        {
            controller.finishGatheringInput(sceneList.current.collisionLayer);
            player.updateUv();
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
