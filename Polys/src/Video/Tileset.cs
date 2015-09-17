using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class Tileset
    {
        //A global list of images to avoid duplication.
        static Util.UniqueList<String, IndexedBitmap> sImageList = new Util.UniqueList<string, IndexedBitmap>();

        //The image for the tileset
        public IndexedBitmap image;

        //The currently selected palette for the tileset.
        public Palette mPalette;

        public byte invisibilityColourIndex = 0;

        //The path of the image to be used for this tileset.
        String mImagePath = "";

        //The name of this tileset.
        public String name { get; private set; }

        //The (maximum) width of the tiles in this tileset. 
        public int tileWidth { get; private set; }

        //The (maximum) height of the tiles in this tileset.
        public int tileHeight { get; private set; }

        public int tileCountX { get; private set; }
        public int tileCountY { get; private set; }

        //The starting GID of the tileset
        public int firstGid { get; private set; }

        public Tileset(String path, String name, int tilewidth, int tileheight, int firstGid)
        {
            mImagePath = path;
            image = sImageList.register(path, x => new IndexedBitmap(x));
            mPalette = image.palette;
            this.name = name;
            this.tileWidth = tilewidth;
            this.tileHeight = tileheight;
            this.firstGid = firstGid;
            this.tileCountX = image.width / tileWidth;
            this.tileCountY = image.height / tileHeight; 
        }

        public void Dispose()
        {
            sImageList.deregister(mImagePath);
        }
    }
}
