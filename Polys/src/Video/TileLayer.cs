using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class TileLayer
    {
        public String name { get; private set; }
        public bool visible { get; private set; }
        public TiledSharp.PropertyDict properties { get; private set; }

        public Dictionary<Tileset, List<Tile>> tileDict = new Dictionary<Tileset, List<Tile>>(); 

        public TileLayer(TiledSharp.TmxLayer layer, Tileset[] orderedTilesets, bool filterEmpty = true)
        {
            name = layer.Name;
            visible = layer.Visible;
            properties = layer.Properties;

            //Insert tileset, breaking them up into tilesets.
            for (int i = 0; i < layer.Tiles.Count; ++i)
            {
                Tileset tileset = getCorrespondingTilest(orderedTilesets, layer.Tiles[i].Gid);
                Tile tile = new Tile(layer.Tiles[i], tileset.tileCountX, tileset.tileCountY);

                if (!tileDict.ContainsKey(tileset))
                    tileDict[tileset] = new List<Tile>();

                tileDict[tileset].Add(tile);
            }

            if (filterEmpty)
            {
                List<Tile> tmp = new List<Tile>();
                foreach(var p in tileDict)
                {
                    //Here it seems that we must use a temporary variable, as the iteration is
                    //immutable.
                    tmp.Clear();
                    tmp.AddRange(p.Value);
                    p.Value.Clear();
                    p.Value.AddRange(from tile in tmp where (tile.visible == true) select tile);
                }
            }
        }

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
