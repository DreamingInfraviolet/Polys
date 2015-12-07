using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video.UI
{
    public class Button
    {
        public Button(string text, Util.Rect rect)
        {
            this.font = GUIManager.mainFont;
            this.vsSelected = GUIManager.buttonRectSelected;
            this.vsUnselected = GUIManager.buttonRectUnselected;
            this.rect = rect;
            this.text = text;
        }

        public Button(string text, Util.Rect rect, Font font, VisualRectangle vsSelected, VisualRectangle vsUnselected)
        {
            this.font = font;
            this.vsSelected = vsSelected;
            this.vsUnselected = vsUnselected;
            this.rect = rect;
            this.text = text;
        }

        Font font;
        VisualRectangle vsSelected, vsUnselected;
        public string text;

        public Util.Rect rect;

        public void draw(bool selected=false)
        {
            Util.Rect textRect = rect;
            textRect.w = font.maxRightPosition(text, 0, 0)+font.tileWidth;
            textRect.h = font.minDownPosition(text, 0, 0)+font.tileHeight;

            if (selected)
                vsSelected.renderRect(rect);
            else
                vsUnselected.renderRect(rect);

            font.renderText(text, rect.centreX - textRect.w / 2, rect.centreY - textRect.h / 2, textRect.w);
        }
    }
}