using System;
using OpenGL;

namespace Polys.Video
{
    class Sprite : Transformable
    {
        /**  whether it is visible. */
        public bool  visible;
        
        /** The tile source texture coordinates in the tileset (in pixels) */
        public int uvX, uvY, sizeX, sizeY;

        /** Constructs the tile */
        public Sprite(TiledSharp.TmxLayerTile tile, Tileset tileset)
            : base(tile.X, tile.Y)
        {
            visible = tile.Gid != 0;
            setUV(tileset, tile.Gid < 1 ? 0 : ((tile.Gid - 1) % tileset.tileCountX), tile.Gid < 1 ? 0 : ((tile.Gid - 1) / tileset.tileCountY));
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

        public void setUV(int uvX, int uvY, int uvSizeX, int uvSizeY)
        {
            this.uvX = uvX;
            this.uvY = uvY;
            this.sizeX = uvSizeX;
            this.sizeY = uvSizeY;
        }

        public void setUV(Tileset tileset, int xTile, int yTile)
        {
            uvX = ((xTile) * tileset.tileWidth);
            uvY = ((yTile) * tileset.tileHeight);
            sizeX = tileset.tileWidth;
            sizeY = tileset.tileHeight;

        }

        /** Transform matrix using the position as coordinates. */
        public Matrix4 transformMatrix()
        {
            return new Matrix4(new float[] { sizeX, 0, 0, 0,
                               0, sizeY, 0, 0,
                               0, 0, 0, 0,
                               posX, posY, 0, 1 });
        }

        public Matrix4 uvMatrix(float tilesetWidth, float tilesetHeight)
        {
            return new Matrix4(new float[] { sizeX/tilesetWidth, 0, 0, 0,
                               0, sizeY/tilesetHeight, 0, 0,
                               0, 0, 0, 0,
                               uvX/tilesetWidth, uvY/tilesetHeight, 0, 1 });
        }
    }
}
