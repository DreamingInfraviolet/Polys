using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    /** Similar to a sprite, but also contains an internal tileset for convenience. */
    class DrawableSprite : Sprite
    {
        public Tileset tileset;

        public DrawableSprite(TiledSharp.TmxLayerTile tile, Tileset tileset)
            : base(tile, tileset) {}

        /** Constructs the tile */
        public DrawableSprite(int posX, int posY, bool visible = true,
            bool diagonalFlip = false, bool horizontalFlip = false,
            bool verticalFlip = false, float uvX = 0, float uvY = 0, float uvSizeX = 1, float uvSizeY = 1)
            : base(posX, posY, visible, diagonalFlip, horizontalFlip, verticalFlip, uvX, uvY, uvSizeX, uvSizeY) {}

        /** Constructs the tile and loads a tileset */
        public DrawableSprite(string spritePath, int posX=0, int posY=0, bool originIsCentre = true,
            bool visible = true,
            bool diagonalFlip = false, bool horizontalFlip = false,
            bool verticalFlip = false, float uvX = 0, float uvY = 0, float uvSizeX = 1, float uvSizeY = 1)
            : base(posX, posY, visible, diagonalFlip, horizontalFlip, verticalFlip, uvX, uvY, uvSizeX, uvSizeY)
        {
            tileset = new Tileset(spritePath, "tileset");
        }
    }
}
