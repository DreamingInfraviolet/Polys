using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video.UI
{
    class GUIManager
    {
        static Font mMainFont = new Font("assets/gui/fonts/default.bmp");
        public static Font mainFont { get { return mMainFont; } }

        static VisualRectangle mButtonRectSelected = new VisualRectangle("assets/gui/misc/button_rect_selected.bmp");
        public static VisualRectangle buttonRectSelected { get { return mButtonRectSelected; } }

        static VisualRectangle mButtonRectUnselected = new VisualRectangle("assets/gui/misc/button_rect_unselected.bmp");
        public static VisualRectangle buttonRectUnselected { get { return mButtonRectUnselected; } }
    }
}
