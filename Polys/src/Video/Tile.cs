using OpenGL;

namespace Polys.Video
{
    /** Represents a single tile.. */
    class Tile : Sprite
    {




        /** Constructs the tile */
        public Tile(TiledSharp.TmxLayerTile tile, int tileCountX, int tileCountY,
                    int tileWidth, int tileHeight,
                    int tilesetWidth, int tilesetHeight)
            : base(tile.X, tile.Y,
                  visible : true,
                  diagonalFlip : tile.DiagonalFlip,
                  horizontalFlip : tile.HorizontalFlip,
                  verticalFlip : tile.VerticalFlip,

                  uvX :
                  //(float)(tilesetXIndex * tileWidth + 0.5f) / tilesetWidth
                  (float)((tile.Gid == 0 ? 0 : ((tile.Gid - 1) % tileCountX))*tileWidth+0.5f)/tilesetWidth,          

                  uvY : 
                  //(float)(tilesetYIndex * tileHeight + 0.5f) / tilesetHeight
                  (float)((tile.Gid == 0 ? 0 : ((tile.Gid - 1) / tileCountY))*tileHeight+0.5f)/tilesetHeight,
                  
                  uvSizeX : (float)tileWidth  / tilesetWidth,
                  uvSizeY : (float)tileHeight / tilesetHeight)
        {
            //Copy in properties
            visible = tile.Gid != 0;
            diagonalFlip = tile.DiagonalFlip;
            horizontalFlip = tile.HorizontalFlip;
            verticalFlip = tile.VerticalFlip;

            posX = tile.X;
            posY = tile.Y;
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
