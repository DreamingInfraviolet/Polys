using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video.UI
{
    class Button
    {
        public Button(string text, Util.Rect rect, string fontPath, string rectPath)
        {
            font = new Font(fontPath);
            vs = new VisualRectangle(rectPath);
            this.rect = rect;
            this.text = text;
        }

        Font font;
        VisualRectangle vs;
        public string text { get; private set; }

        public Util.Rect rect;

        public void draw()
        {
            Util.Rect textRect = rect;
            textRect.w = font.maxRightPosition(text, 0, 0)+font.tileWidth;
            textRect.h = font.minDownPosition(text, 0, 0)+font.tileHeight;
            vs.renderRect(rect);
            font.renderText(text, rect.centreX - textRect.w / 2, rect.centreY - textRect.h / 2, textRect.w);
        }
    }
}