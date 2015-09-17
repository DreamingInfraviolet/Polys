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

        //The tileset to which it refers
        public Tileset tileset;

        public Tile(TiledSharp.TmxLayerTile tile, Tileset[] orderedTilesets)
        {
            visible = tile.Gid != 0;
            diagonalFlip = tile.DiagonalFlip;
            horizontalFlip = tile.HorizontalFlip;
            verticalFlip = tile.VerticalFlip;

            worldX = (short)tile.X;
            worldY = (short)tile.Y;

            //Find correct tileset
            tileset = null;
            for (int i = 1; i < orderedTilesets.Length; ++i)
            {
                if (orderedTilesets[i].firstGid > tile.Gid)
                {
                    tileset = orderedTilesets[i - 1];
                    break;
                }
            }

            if (tileset == null)
                tileset = orderedTilesets.Last();

            if (visible)
            {
                tilesetX = (short)((tile.Gid - 1) % tileset.tileCountX);
                tilesetY = (short)((tile.Gid - 1) / tileset.tileCountY);
            }
            else
            {
                tilesetX = 0;
                tilesetY = 0;
            }
        }
    }
}
