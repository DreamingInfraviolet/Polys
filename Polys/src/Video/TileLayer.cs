using System;
using System.Collections.Generic;
using System.Linq;
using TiledSharp;

namespace Polys.Video
{
    /** The tile layer stores a series of tiles. */
    public class TileLayer
    {
        /** The name of the tile layer */
        public String name { get; private set; }

        /** Whether the tile layer should be visible */
        public bool visible { get; private set; }

        /** A dictionary of properties for the tile layer, as defined in the tmx file */
        public TiledSharp.PropertyDict properties { get; private set; }

        public List<Sprite> objects;
        public Sprite[] tileCache;
        public int[,] tiles;
        public int tileCountX, tileCountY;
        public int genericTileWidth, genericTileHeight;
        public int maxTileWidth, maxTileHeight;

        /** Initialises the tile layer from the given arguments.
          * @param layer The TiledSharp representation of the tile layer.
          * @param orderedTilesets An ordered array (ascending) of all the tilesets in the map 
          * @param filterEmpty If true, invisible tiles will be removed. Should be off for things like collision layers */
        public TileLayer(TiledSharp.TmxLayer layer, Tileset[] orderedTilesets, int tileCountX, int tileCountY, int genericTileWidth, int genericTileHeight)
        {
            name = layer.Name;
            visible = layer.Visible;
            properties = layer.Properties;

            //When constructing the quadtree to store the tiles, it must be of a power-of-two size.
            //Find this size.
            int quadTreeWidth =  Util.Maths.biggerPowerOfTwo(((tileCountX + 2) * genericTileWidth));
            int quadTreeHeight = Util.Maths.biggerPowerOfTwo(((tileCountY + 2) * genericTileHeight));
            objects = new List<Sprite>();
            tiles = new int[tileCountX, tileCountY];
            this.tileCountX = tileCountX;
            this.tileCountY = tileCountY;
            this.genericTileWidth = genericTileWidth;
            this.genericTileHeight = genericTileHeight;

            //Find the maximum tile dimensions
            if (orderedTilesets.Length != 0)
            {
                maxTileWidth = orderedTilesets[0].tileWidth;
                maxTileHeight = orderedTilesets[0].tileHeight;
                for(int i = 1; i < orderedTilesets.Length; ++i)
                {
                    if (orderedTilesets[i].tileWidth > maxTileWidth)
                        maxTileWidth = orderedTilesets[i].tileWidth;
                    if (orderedTilesets[i].tileHeight > maxTileHeight)
                        maxTileHeight = orderedTilesets[i].tileHeight;
                }
            }

            //Insert tiles
            //A dictionary mapping a tmp tile to a pair of an id to put into the grid and the corresponding tile.
            Dictionary<TiledSharp.TmxLayerTile, Util.Pair<int,Sprite>> tileHash =
                new Dictionary<TiledSharp.TmxLayerTile, Util.Pair<int, Sprite>>(new TmxTileComparer());
            foreach (var tmxTile in layer.Tiles)
            {
                //Ignore invisible tiles.
                if (tmxTile.Gid == 0)
                {
                    tiles[tmxTile.X, tileCountY - tmxTile.Y - 1] = -1;
                    continue;
                }
                
                Tileset tileset = getCorrespondingTilest(orderedTilesets, tmxTile.Gid);
                

                Util.Pair<int, Sprite> pair;
                if(!tileHash.TryGetValue(tmxTile, out pair))
                {
                    //If we can't find the value, add a new tile.
                    int id = tileHash.Count;
                    Sprite tile = new Sprite(tmxTile, tileset, genericTileWidth, genericTileHeight, tileCountY);
                    pair = new Util.Pair<int, Sprite>(id, tile);
                    tileHash.Add(tmxTile, pair);
                }

                tiles[tmxTile.X, tileCountY - tmxTile.Y - 1] = pair.first;
            }

            //When this is done, our grid should be filled with references to various tiles.
            //Now convert the set to the vector user internally.
            tileCache = new Sprite[tileHash.Count];
            foreach (var p in tileHash)
                tileCache[p.Value.first] = p.Value.second;
        }

        class TmxTileComparer : IEqualityComparer<TiledSharp.TmxLayerTile>
        {
            public bool Equals(TmxLayerTile x, TmxLayerTile y)
            {
                return x.VerticalFlip == y.VerticalFlip & x.HorizontalFlip == y.HorizontalFlip &&
                    x.DiagonalFlip == y.DiagonalFlip &&
                    x.Gid == y.Gid;
            }

            public int GetHashCode(TmxLayerTile obj)
            {
                return (obj.VerticalFlip?0:0x000000ff) ^
                    (obj.HorizontalFlip ? 0 : 0x0000ff00) ^
                    (obj.DiagonalFlip ? 0 : 0x00ff0000) ^ obj.Gid;
            }
        }

        /** Returns the tileset corresponding to the gid from the ordered tileset list.
          * This is required because of how the tmx format works. */
        Tileset getCorrespondingTilest(Tileset[] orderedTilesets, int gid)
        {
            //Find correct tileset
            Tileset tileset = null;
            for (int i = 1; i < orderedTilesets.Length; ++i)
            {
                if (orderedTilesets[i].firstGid > gid)
                {
                    tileset = orderedTilesets[i - 1];
                    break;
                }
            }

            if (tileset == null)
                tileset = orderedTilesets.Last();

            return tileset;
        }

        public bool intersects(Util.Rect rect)
        {
            int tileMinX = (rect.x - maxTileWidth) / genericTileWidth;
            int tileMinY = (rect.y - maxTileHeight) / genericTileHeight;
            int tileMaxX = tileMinX + (rect.w + maxTileWidth) / genericTileWidth + 2;
            int tileMaxY = tileMinY + (rect.h + maxTileHeight) / genericTileHeight + 2;
            int endX = Math.Min(tileCountX, tileMaxX);
            int endY = Math.Min(tileCountY, tileMaxY);

            for (int yid = Math.Max(tileMinY, 0); yid < endY; ++yid)
            {
                int y = yid * genericTileHeight;

                for (int xid = Math.Max(tileMinX, 0); xid < endX; ++xid)
                {
                    int x = xid * genericTileWidth;

                    int tileId = tiles[xid, yid];
                    if (tileId < 0)
                        continue;
                    else
                    {
                        Sprite tile = tileCache[tileId];
                        Util.Rect spriteRect = new Util.Rect(x, y, tile.rect.w, tile.rect.h);
                        if (rect.overlaps(spriteRect))
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
