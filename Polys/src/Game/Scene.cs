using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoonSharp.Interpreter;
using TiledSharp;

namespace Polys.Game
{
    class Scene
    {
        Script mScript;
        public TmxMap map { get; private set; }
        public Video.Tileset[] tilesets { get; private set; }
        public Video.TileLayer[] layers { get; private set; }

        public Scene(String path)
        {
            //Run scene script
            mScript = new Script();
            mScript.DoFile(path);

            //Load map
            String tilemapPath = System.IO.Path.Combine("assets/tilemaps/", tryGetString("tilemap"));
            if (!System.IO.File.Exists(tilemapPath))
                throw new Exception(String.Format("Error loading \"{0}\". (is the path relative to assets/tilemaps/?)", tilemapPath));
            map = new TmxMap(tilemapPath);

            //Load tilesets
            tilesets = new Video.Tileset[map.Tilesets.Count];
            for (int iTileset = 0; iTileset < tilesets.Length; ++iTileset)
            {
                TmxTileset set = map.Tilesets[iTileset];
                if (set.Margin != 0 || set.Spacing != 0)
                    throw new Exception(String.Format("Tileset {0} must have margin and spacing equal to zero.", set.Name));
                tilesets[iTileset] = new Video.Tileset(set.Image.Source, set.Name, set.TileWidth, set.TileHeight, set.FirstGid);
            }

            //Initialise layers
            layers = new Video.TileLayer[map.Layers.Count];
            for(int iLayer = 0; iLayer < map.Layers.Count; ++iLayer)
                layers[iLayer] = new Video.TileLayer(map.Layers[iLayer], tilesets);
        }

        public void Dispose()
        {
            foreach (Video.Tileset set in tilesets)
                set.Dispose();
        }

        String tryGetString(String variableName)
        {
            DynValue tilemapPathVal = mScript.Globals.Get(variableName);
            if (tilemapPathVal.Type != DataType.String)
                throw new Exception(String.Format("Error parsing script: \"{0}\" path must be a string.", variableName));
            return tilemapPathVal.String;
        }
    }
}
