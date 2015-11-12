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
            IntentManager.register(this, IntentManager.IntentType.MOVE_SELECTION_UP, true, false, false);
            IntentManager.register(this, IntentManager.IntentType.MOVE_SELECTION_DOWN, true, false, false);
            IntentManager.register(this, IntentManager.IntentType.CONFIRM_SELECTION, true, false, false);
        }

        public void setStateManager(StateManager m)
        {
            sm = m;
        }

        public void Dispose()
        {
        }

        public void handleIntent(IntentManager.IntentType intentCode, bool isKeyDown, bool isKeyUp, bool isKeyHeld)
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
                    IntentManager.keyDown(SDL2.SDL.SDL_Keycode.SDLK_ESCAPE); //temporary solution until I can trigger intents directly
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
