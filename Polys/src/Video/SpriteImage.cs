using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class SpriteImage
    {
        //A global cache of images to avoid duplication.
        static Util.ObjectCache<String, IndexedTexture> sImageCache = new Util.ObjectCache<string, IndexedTexture>();

        //The image reference for the tileset
        protected IndexedTexture image;

        //The path of the image to be used for this tileset
        String mImagePath = "";

        /** The currently selected palette for the tileset */
        public Palette palette { get; set; }

        /** The width of the tileset */
        public int width { get { return image.width; } }

        /** The height of the tileset */
        public int height { get { return image.height; } }
        
        /** A constructor that loads the tileset and initialises the internal fields */
        public SpriteImage(String path)
        {
            this.image = sImageCache.register(path, x => new IndexedTexture(x));
            this.mImagePath = path;
            this.palette = image.palette;
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
