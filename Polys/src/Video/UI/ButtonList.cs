using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video.UI
{
    public class ButtonList
    {
        public int activeButton { get; private set; }
        Button[] buttons;
        bool cycle;

        public ButtonList(Button[] buttons, bool cycle = true)
        {
            this.buttons = buttons;
            this.cycle = cycle;
        }

        public void draw()
        {
            for (int i = 0; i < buttons.Length; ++i)
                if (i == activeButton)
                    buttons[i].draw(true);
                else
                    buttons[i].draw(false);
        }

        public void selectNext()
        {
            if (cycle)
                activeButton = (activeButton + 1) % buttons.Length;
            else
                activeButton = Util.Util.Clamp(activeButton + 1, 0, buttons.Length - 1);
        }

        public void selectPrevious()
        {
            if (cycle)
                activeButton = (activeButton + buttons.Length - 1) % buttons.Length;
            else
                activeButton = Util.Util.Clamp(activeButton - 1, 0, buttons.Length - 1);
        }
    }
}
