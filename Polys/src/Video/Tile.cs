using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    struct Tile
    {
        public bool diagonalFlip, horizontalFlip, verticalFlip, visible;

        //The world position of the tile
        public short worldX, worldY;

        //The tile coordinates in the source tileset
        public short tilesetX, tilesetY;

        public Tile(TiledSharp.TmxLayerTile tile, int tileCountX, int tileCountY)
        {
            visible = tile.Gid != 0;
            diagonalFlip = tile.DiagonalFlip;
            horizontalFlip = tile.HorizontalFlip;
            verticalFlip = tile.VerticalFlip;

            worldX = (short)tile.X;
            worldY = (short)tile.Y;

            if (visible)
            {
                tilesetX = (short)((tile.Gid - 1) % tileCountX);
                tilesetY = (short)((tile.Gid - 1) / tileCountY);
            }
            else
            {
                tilesetX = 0;
                tilesetY = 0;
            }
        }
    }
}
