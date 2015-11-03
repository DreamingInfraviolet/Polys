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
        public Dictionary<Tileset, List<Sprite>> tileDict = new Dictionary<Tileset, List<Sprite>>(); 

        /** Initialises the tile layer from the given arguments.
          * @param layer The TiledSharp representation of the tile layer.
          * @param orderedTilesets An ordered array (ascending) of all the tilesets in the map 
          * @param filterEmpty If true, invisible tiles will be removed. Should be off for things like collision layers */
        public TileLayer(TiledSharp.TmxLayer layer, Tileset[] orderedTilesets, int tileCountY, int genericTileWidth, int genericTileHeight, bool filterEmpty = true)
        {
            name = layer.Name;
            visible = layer.Visible;
            properties = layer.Properties;

            //Insert tileset, breaking them up into tilesets.
            for (int i = 0; i < layer.Tiles.Count; ++i)
            {
                Tileset tileset = getCorrespondingTilest(orderedTilesets, layer.Tiles[i].Gid);
                Sprite tile = new Sprite(layer.Tiles[i], genericTileWidth, genericTileHeight, tileset, tileCountY);

               if (!tileDict.ContainsKey(tileset))
                    tileDict[tileset] = new List<Sprite>();

                tileDict[tileset].Add(tile);
            }

            //Filter empty tiles, if needed
            if (filterEmpty)
                deleteInvisibleTiles();
        }

        /** Deletes all invisible tiles in the layer */
        void deleteInvisibleTiles()
        {
            List<Sprite> tmp = new List<Sprite>();
            foreach (var p in tileDict)
            {
                //Here it seems that we must use a temporary variable, as the iteration is
                //immutable.
                tmp.Clear();
                tmp.AddRange(p.Value);
                p.Value.Clear();
                p.Value.AddRange(from tile in tmp where (tile.visible == true) select tile);
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
