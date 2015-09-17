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
        public Tile[] tiles { get; private set; }

        public TileLayer(TiledSharp.TmxLayer layer, Tileset[] orderedTilesets, bool filterEmpty = true)
        {
            name = layer.Name;
            visible = layer.Visible;
            properties = layer.Properties;
            tiles = new Tile[layer.Tiles.Count];
            for (int i = 0; i < layer.Tiles.Count; ++i)
                tiles[i] = new Tile(layer.Tiles[i], orderedTilesets);
            if(filterEmpty)
                tiles = (from tile in tiles where (tile.visible == true) select tile).ToArray();
        }
    }
}
