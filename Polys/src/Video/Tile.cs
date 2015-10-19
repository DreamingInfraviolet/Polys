namespace Polys.Video
{
    /** Represents a single tile. */
    struct Tile
    {
        /** Whether the tile should be flipped, and whether it is visible. */
        public bool diagonalFlip, horizontalFlip, verticalFlip, visible;

        /** The world position of the tile (in tile coordinates) */
        public short worldX, worldY;

        /** The tile source texture coordinates in the tileset (in pixels) */
        public short tilesetX, tilesetY;

        /** Constructs the tile */
        public Tile(TiledSharp.TmxLayerTile tile, int tileCountX, int tileCountY)
        {
            //Copy in properties
            visible = tile.Gid != 0;
            diagonalFlip = tile.DiagonalFlip;
            horizontalFlip = tile.HorizontalFlip;
            verticalFlip = tile.VerticalFlip;

            worldX = (short)tile.X;
            worldY = (short)tile.Y;

            //If it is not visible, then its GID is 0, which is illegal.

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
