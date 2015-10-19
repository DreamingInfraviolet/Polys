using System;

namespace Polys.Video
{
    /** A class to represent a tileset image, based on a subset of the tmx format. It does not support a border, or spacing
      * between the tiles. Internally it uses a palette to store colours. */
    class Tileset
    {
        //A global cache of images to avoid duplication.
        static Util.ObjectCache<String, IndexedBitmap> sImageCache = new Util.ObjectCache<string, IndexedBitmap>();

        //The image reference for the tileset
        IndexedBitmap image;

        //The path of the image to be used for this tileset
        String mImagePath = "";

        /** The currently selected palette for the tileset */
        public Palette palette { get; set; }

        /** The name of this tileset. */
        public String name { get; private set; }

        /** The width of the tileset */
        public int width { get { return image.width; } }

        /** The height of the tileset */
        public int height { get { return image.height; } }

        /** The width of the tiles in this tileset */
        public int tileWidth { get; private set; }

        /** The height of the tiles in this tileset */
        public int tileHeight { get; private set; }

        /** The number of tiles along the x axis */
        public int tileCountX { get; private set; }

        /** The number of tiles along the y axis */
        public int tileCountY { get; private set; }

        /** The starting GID of the tileset (as defined in the tmx format) */
        public int firstGid { get; private set; }

        /** A constructor that loads the tileset and initialises the internal fields */
        public Tileset(String path, String name, int tilewidth, int tileheight, int firstGid)
        {
            this.image = sImageCache.register(path, x => new IndexedBitmap(x));
            this.mImagePath = path;
            this.palette = image.palette;
            this.name = name;
            this.tileWidth = tilewidth;
            this.tileHeight = tileheight;
            this.firstGid = firstGid;
            this.tileCountX = image.width / tileWidth;
            this.tileCountY = image.height / tileHeight;
        }

        /** Disposes of the object, marking it for removal from the cache. */
        public void Dispose()
        {
            sImageCache.deregister(mImagePath);
        }

        /** Binds the tileset and its palette */
        public void bind()
        {
            image.bind();
            palette.bind();
        }
    }
}
