using System;
using OpenGL;

namespace Polys.Video
{
    class Sprite : Transformable
    {
        /** Whether the tile should be flipped, and whether it is visible. */
        public bool diagonalFlip, horizontalFlip, verticalFlip, visible;
        
        /** The tile source texture coordinates in the tileset (in pixels) */
        public float uvX, uvY, uvSizeX, uvSizeY;

        /** Constructs the tile */
        public Sprite(TiledSharp.TmxLayerTile tile, Tileset tileset)
            : base(tile.X, tile.Y)
        {
            visible = tile.Gid != 0;
            diagonalFlip = tile.DiagonalFlip;
            horizontalFlip = tile.HorizontalFlip;
            verticalFlip = tile.VerticalFlip;

            uvX = (float)((tile.Gid == 0 ? 0 : ((tile.Gid - 1) % tileset.tileCountX)) * tileset.tileWidth + 0.5f) / tileset.width;

            uvY = (float)((tile.Gid == 0 ? 0 : ((tile.Gid - 1) / tileset.tileCountY)) * tileset.tileHeight + 0.5f) / tileset.height;

            uvSizeX = (float)tileset.tileWidth / tileset.width;
            uvSizeY = (float)tileset.tileHeight / tileset.height;
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


        public Matrix4 uvMatrix()
        {
            return new Matrix4(new float[] { uvSizeX, 0, 0, 0,
                               0, uvSizeY, 0, 0,
                               0, 0, 0, 0,
                               uvX, uvY, 0, 1 });
        }
    }
}
