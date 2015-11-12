using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video.UI
{
    public class VisualRectangle : Tileset
    {
        int objectWidth, objectHeight;

        public VisualRectangle(string path)
          : base(path, "rect")
        {
            this.objectWidth = width/3;
            this.objectHeight = height/3;
            this.tileCountX = width / objectWidth;
            this.tileCountY = height / objectHeight;
            if (this.tileCountX < 3 || this.tileCountY < 3)
                throw new Exception("Not enough rectangle elements. Must be at least a 3x3");

        }
        
        /** Draws a visual rectangle enclosing the rect parameter. Note that the actual rectangle drawn may be larger than the
          * inputted rectangle. */
        public void renderRect(Util.Rect desiredRect)
        {
            //Find the actual rectangle to enclose.
            Util.Rect actualRect = new Util.Rect();
            actualRect.w = desiredRect.w + objectWidth * 2;
            if (actualRect.w % objectWidth != 0)
                actualRect.w += (objectWidth - actualRect.w % objectWidth);

            actualRect.h = desiredRect.h + objectHeight * 2;
            if (actualRect.h % objectHeight != 0)
                actualRect.h = actualRect.h + (objectHeight - actualRect.h % objectHeight);


            actualRect.centreX = desiredRect.centreX;
            actualRect.centreY = desiredRect.centreY;


            int nWidthEdges = (actualRect.w - 2 * objectWidth) / objectWidth;
            int nHeightEdges = (actualRect.h - 2 * objectHeight) / objectHeight;

            int lowerLeftX = actualRect.x, lowerLeftY = actualRect.y;
            int upperRightX = actualRect.x+objectWidth*(1+nWidthEdges), upperRightY = actualRect.y + objectHeight * (1 + nHeightEdges);


            //Draw the 4 corners
            Sprite sprite = new Sprite(new Util.Rect(lowerLeftX, lowerLeftY, objectWidth, objectHeight), this); ;
            
            sprite.setTilesetIndex(0, 2);
            HighLevelRenderer.draw(sprite);
            
            sprite.rect.y = upperRightY;
            sprite.setTilesetIndex(0, 0);
            HighLevelRenderer.draw(sprite);

            sprite.rect.x = upperRightX;
            sprite.setTilesetIndex(2, 0);
            HighLevelRenderer.draw(sprite);

            sprite.rect.y = lowerLeftY;
            sprite.setTilesetIndex(2, 2);
            HighLevelRenderer.draw(sprite);

            //Draw the vertical edges.
            for (int i = lowerLeftY + objectHeight, j = 0; j < nHeightEdges; ++j, i += objectHeight)
            {
                sprite.rect.y = i;
                sprite.rect.x = lowerLeftX;
                sprite.setTilesetIndex(0, 1);
                HighLevelRenderer.draw(sprite);
                sprite.setTilesetIndex(2, 1);
                sprite.rect.x = upperRightX;
                HighLevelRenderer.draw(sprite);
            }

            //Draw the horizontal edges.
            for (int i = lowerLeftX + objectWidth, j = 0; j < nWidthEdges; ++j, i += objectWidth)
            {
                sprite.rect.x = i;
                sprite.rect.y = lowerLeftY;
                sprite.setTilesetIndex(1, 2);
                HighLevelRenderer.draw(sprite);
                sprite.setTilesetIndex(1, 0);
                sprite.rect.y = upperRightY;
                HighLevelRenderer.draw(sprite);
            }

            //Fill the insides
            sprite.setTilesetIndex(1, 1);
            for (int x = lowerLeftX + objectWidth, j = 0; j < nWidthEdges; ++j, x += objectWidth)
            { 
                for (int y = lowerLeftY + objectHeight, k = 0; k < nHeightEdges; ++k, y += objectHeight)
                {
                    sprite.rect.x = x;
                    sprite.rect.y = y;
                    HighLevelRenderer.draw(sprite);
                }
            }

            int minWidth = this.width;
            int minHeight = this.height;
        }
    }
}