using System;
using System.Collections.Generic;
using System.Linq;

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
        
        /** A dictionary of tiles, organised by the tilesets to which they belong. */
        public Dictionary<Tileset, Util.Quadtree> tileDict = new Dictionary<Tileset, Util.Quadtree>();

        public Util.Quadtree tiles;

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
            tiles = new Util.Quadtree(new Util.Rect(-genericTileWidth, -genericTileHeight, quadTreeWidth, quadTreeHeight));

            //Insert tiles
            for (int i = 0; i < layer.Tiles.Count; ++i)
            {
                //Ignore invisible tiles.
                if (layer.Tiles[i].Gid == 0)
                    continue;
                
                Tileset tileset = getCorrespondingTilest(orderedTilesets, layer.Tiles[i].Gid);
                Sprite tile = new Sprite(layer.Tiles[i], tileset, genericTileWidth, genericTileHeight, tileCountY);

                tiles.insert(tile);
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
    }
}
