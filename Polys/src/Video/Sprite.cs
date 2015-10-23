using System;
using OpenGL;

namespace Polys.Video
{
    class Sprite : Transformable
    {
        /**  whether it is visible. */
        public bool  visible;
        
        /** The tile source texture coordinates in the tileset (in pixels) */
        public int uvX, uvY, width, height;

        /** Constructs the tile */
        public Sprite(TiledSharp.TmxLayerTile tile, Tileset tileset)
            : base(tile.X, tile.Y)
        {
            visible = tile.Gid != 0;
            setUV(tile.Gid < 1 ? 0 : ((tile.Gid - 1) % tileset.tileCountX)* tileset.tileWidth,
                tile.Gid < 1 ? 0 : ((tile.Gid - 1) / tileset.tileCountY)* tileset.tileHeight,
                tileset.tileWidth, tileset.tileHeight);
        }

        /** Constructs the tile */
        public Sprite(int posX, int posY, int sizeX, int sizeY, bool visible = true,
            int uvX = 0, int uvY = 0)
            : base(posX, posY)
        {
            //Copy in properties
            this.visible = visible;
            setUV(uvX, uvY, sizeX, sizeY);
        }

        void setUV(int uvX, int uvY, int uvSizeX, int uvSizeY)
        {
            this.uvX = uvX;
            this.uvY = uvY;
            this.width = uvSizeX;
            this.height = uvSizeY;
        }

        public void setTilesetIndex(int x, int y)
        {
            uvX = x*width;
            uvY = y*height;
        }

        public Matrix4 uvMatrix(float tilesetWidth, float tilesetHeight)
        {
            return new Matrix4(new float[] { width/tilesetWidth, 0, 0, 0,
                               0, height/tilesetHeight, 0, 0,
                               0, 0, 0, 0,
                               uvX/tilesetWidth, uvY/tilesetHeight, 0, 1 });
        }
    }
}
