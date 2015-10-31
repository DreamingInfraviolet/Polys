using MoonSharp.Interpreter;
using System;
using TiledSharp;

namespace Polys.Game
{
    /** Represents a scene, containing data unique to the scene */
    class Scene : IScriptInitialisable
    {
        public TmxMap map { get; private set; }
        public Video.Tileset[] tilesets { get; private set; }
        public Video.TileLayer[] layers { get; private set; }
        public Video.TileLayer collisionLayer { get; private set; }
        public int gridTileWidth { get; private set; }
        public int gridTileHeight { get; private set; }

        /** Initialises the scene from a script */
        public Scene(String path)
        {
            ScriptManager.initialiseFromScript(path, this);
        }

        /** Disposes of the resources associated with the scene */
        public void Dispose()
        {
            foreach (Video.Tileset set in tilesets)
                set.Dispose();
        }

        /** Returns the script representation of the scene */
        public string ScriptName()
        {
            return "scene";
        }

        /** Initialises the scene from script, loading the required data */
        public void InitialiseFromScript(Table table)
        {
            //Load map
            String tilemapPath = System.IO.Path.Combine("assets/tilemaps/", ScriptManager.retrieveValue(table, "tilemap", "TILEMAP_NOT_SET"));
            if (!System.IO.File.Exists(tilemapPath))
                throw new Exception(String.Format("Error loading \"{0}\". (is the path relative to assets/tilemaps/?)", tilemapPath));
            map = new TmxMap(tilemapPath);
            gridTileWidth = map.TileWidth;
            gridTileHeight = map.TileHeight;


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
            for (int iLayer = 0; iLayer < map.Layers.Count; ++iLayer)
            {
                //Note that keeping empty collision tiles currently ignores emptyness.
                if (map.Layers[iLayer].Name == "collision")
                    collisionLayer = new Video.TileLayer(map.Layers[iLayer], tilesets, map.Height);
               
                layers[iLayer] = new Video.TileLayer(map.Layers[iLayer], tilesets, map.Height);
            }
        }
    }
}
