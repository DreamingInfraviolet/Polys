using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class Sprite : Transformable
    {
        /** Whether the tile should be flipped, and whether it is visible. */
        public bool diagonalFlip, horizontalFlip, verticalFlip, visible;
        
        /** The tile source texture coordinates in the tileset (in pixels) */
        public float uvX, uvY, uvSizeX, uvSizeY;

        Sprite()
        {
            diagonalFlip = horizontalFlip = verticalFlip = false;
            visible = true;
        }

        /** Constructs the tile */
        public Sprite(int posX, int posY, bool visible = true,
            bool diagonalFlip = false, bool horizontalFlip = false,
            bool verticalFlip = false, float uvX = 0, float uvY = 0, float uvSizeX = 1, float uvSizeY = 1)
            : base(posX, posY)
        {
            //Copy in properties
            this.visible = visible;
            this.diagonalFlip = diagonalFlip;
            this.horizontalFlip = horizontalFlip;
            this.verticalFlip = verticalFlip;
            
            this.uvX = uvX;
            this.uvY = uvY;
            this.uvSizeX = uvSizeX;
            this.uvSizeY = uvSizeY;
        }
    }
}
