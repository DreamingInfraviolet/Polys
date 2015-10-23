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
            : base(tile, tileset)
        { }

        /** Constructs the tile */
        public DrawableSprite(int posX, int posY, int sizeX, int sizeY, bool visible = true,
            int uvX = 0, int uvY = 0, int uvSizeX = 0, int uvSizeY = 0)
            : base(posX, posY, sizeX, sizeY, visible) {}

        /** Constructs the tile and loads a tileset */
        public DrawableSprite(string spritePath, int posX=0, int posY=0, int sizeX = 0, int sizeY = 0, bool originIsCentre = true,
            bool visible = true, int uvX = 0, int uvY = 0)
            : base(posX, posY, sizeX, sizeY, visible, uvX, uvY)
        {
            tileset = new Tileset(spritePath, "tileset");
        }
    }
}
