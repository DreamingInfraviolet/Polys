using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    /** Similar to a sprite, but also contains an internal tileset for convenience. */
    public class DrawableSprite : Sprite
    {
        public Tileset tileset;

        /** Constructs the tile */
        public DrawableSprite(Util.Rect rect, bool visible = true,
            int uvX = 0, int uvY = 0, int uvSizeX = 0, int uvSizeY = 0)
            : base(rect, visible) {}

        /** Constructs the tile and loads a tileset */
        public DrawableSprite(string spritePath, Util.Rect rect, bool originIsCentre = true,
            bool visible = true, int uvX = 0, int uvY = 0)
            : base(rect, visible, uvX, uvY)
        {
            tileset = new Tileset(spritePath, "tileset");
        }
    }
}
