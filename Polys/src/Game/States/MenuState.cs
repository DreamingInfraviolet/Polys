using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    public class MainMenuState : State, IIntentHandler
    {
        StateManager sm;

        Video.Sprite background = new Video.Sprite("assets/gui/misc/main_menu_background.bmp", new Util.Rect(0, 0, 256, 192));

        Video.UI.ButtonList buttons = new Video.UI.ButtonList(new Video.UI.Button[] {
            new Video.UI.Button("Start", new Util.Rect(50, 45, 156, 20)),
            new Video.UI.Button("Quit",  new Util.Rect(50, 10, 156, 20))});

        public MainMenuState()
        {
            IntentManager.register(this, IntentManager.IntentType.MOVE_SELECTION_UP);
            IntentManager.register(this, IntentManager.IntentType.MOVE_SELECTION_DOWN);
            IntentManager.register(this, IntentManager.IntentType.CONFIRM_SELECTION);
        }

        public void setStateManager(StateManager m)
        {
            sm = m;
        }

        public void Dispose()
        {
        }

        public void handleIntent(IntentManager.IntentType intentCode)
        {
            if (intentCode == IntentManager.IntentType.MOVE_SELECTION_DOWN)
                buttons.selectNext();
            else if(intentCode==IntentManager.IntentType.MOVE_SELECTION_UP)
                buttons.selectPrevious();
            else if (intentCode==IntentManager.IntentType.CONFIRM_SELECTION)
            {
                if (buttons.activeButton == 0)
                    sm.push(new PlayState());
                else if (buttons.activeButton == 1)
                    IntentManager.markForSending(IntentManager.IntentType.ESC);
            }
        }

        public StateManager.StateRenderResult draw()
        {
            Video.HighLevelRenderer.draw(background);
            buttons.draw();
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
