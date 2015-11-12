using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    class MainMenuState : State
    {
        Video.Sprite background = new Video.Sprite("assets/gui/misc/main_menu_background.bmp", new Util.Rect(0, 0, 256, 192));

        Video.UI.Button playButton = new Video.UI.Button("Start", new Util.Rect(50,100,156,30),
            "assets/gui/fonts/default.bmp", "assets/gui/misc/rect.bmp");

        public void Dispose()
        {
        }

        public StateManager.StateRenderResult draw()
        {
            Video.HighLevelRenderer.draw(background);
            playButton.draw();
            return StateManager.StateRenderResult.StopDrawing;
        }

        public StateManager.StateUpdateResult updateAfterFrame()
        {
            return StateManager.StateUpdateResult.Finish;
        }

        public StateManager.StateUpdateResult updateAfterInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }
    }
}
