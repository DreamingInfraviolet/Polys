using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game.States
{
    public class MainMenuState : State
    {
        //If the state is starting, it should have a Start button that launches a new state.
        //Otherwise, a continue button that pops it from the stack.
        bool startingState;

        StateManager sm;

        public MainMenuState(bool starting = true)
        {
            startingState = starting;
            if (!startingState)
                buttons.buttons[0].text = "Continue";
        }

        Video.Sprite background = new Video.Sprite("assets/gui/misc/main_menu_background.bmp", new Util.Rect(0, 0, 256, 192));

        Video.UI.ButtonList buttons = new Video.UI.ButtonList(new Video.UI.Button[] {
            new Video.UI.Button("Start", new Util.Rect(50, 45, 156, 20)),
            new Video.UI.Button("Quit",  new Util.Rect(50, 10, 156, 20))});

        public void setStateManager(StateManager m)
        {
            sm = m;
        }

        public void Dispose()
        {
        }

        public StateManager.StateRenderResult draw()
        {
            Video.HighLevelRenderer.draw(background);
            buttons.draw();
            return StateManager.StateRenderResult.Continue;
        }

        public StateManager.StateUpdateResult updateAfterFrame()
        {
            return StateManager.StateUpdateResult.Finish;
        }

        public StateManager.StateUpdateResult updateAfterInput()
        {
            if (IntentManager.isActive(IntentManager.IntentType.MOVE_SELECTION_DOWN))
                buttons.selectNext();
            else if (IntentManager.isActive(IntentManager.IntentType.MOVE_SELECTION_UP))
                buttons.selectPrevious();
            else if (IntentManager.isActive(IntentManager.IntentType.CONFIRM_SELECTION))
            {
                if (buttons.activeButton == 0)
                {
                    sm.pop();
                    if (startingState)
                        sm.push(new PlayState());
                }
                else if (buttons.activeButton == 1)
                    return StateManager.StateUpdateResult.Quit;
            }
            else if (IntentManager.isActive(IntentManager.IntentType.ESC))
                if (startingState)
                    return StateManager.StateUpdateResult.Quit;
                else
                    sm.pop();

            return StateManager.StateUpdateResult.Finish;
        }

        public StateManager.StateUpdateResult updateBeforeInput()
        {
            return StateManager.StateUpdateResult.Finish;
        }
    }
}
