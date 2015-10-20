using System;
using OpenGL;

namespace Polys.Video
{
    /** A class to represent a tileset image, based on a subset of the tmx format. It does not support a border, or spacing
      * between the tiles. Internally it uses a palette to store colours. */
    class Tileset : SpriteImage
    {
        /** A constructor that loads the tileset and initialises the internal fields */
        public Tileset(String path, String name, int tilewidth, int tileheight, int firstGid)
            : base(path)
        {
            this.palette = image.palette;
            this.name = name;
            this.tileWidth = tilewidth;
            this.tileHeight = tileheight;
            this.firstGid = firstGid;
            this.tileCountX = image.width / tileWidth;
            this.tileCountY = image.height / tileHeight;
        }


        /** The name of this tileset. */
        public String name { get; private set; }
        
        /** The starting GID of the tileset (as defined in the tmx format) */
        public int firstGid { get; private set; }

        /** The width of the tiles in this tileset */
        public int tileWidth { get; private set; }

        /** The height of the tiles in this tileset */
        public int tileHeight { get; private set; }

        /** The number of tiles along the x axis */
        public int tileCountX { get; private set; }

        /** The number of tiles along the y axis */
        public int tileCountY { get; private set; }
    }
}
