using System;
using System.Collections.Generic;

/** A class that stores the list of scenes that have been loaded during the playthrough.
  * Note that only one distinct copy of a scene may exist during one time. */
namespace Polys.Game
{
    public class SceneList
    {
        //A dictionary of currently loaded scenes
        Dictionary<String, Scene> mScenes = new Dictionary<string, Scene>();

        /** The current scene */
        public Scene current{ get; set; }
        
        /** Loads the default scene */
        public SceneList()
        {
            current = load("startup.lua");
        }
        
        /** If a scene has already been loaded, it returns it. Otherwise, it loads it from a file and returns. Preferable to load(). */
        public Scene get(String path)
        {
            Scene scene;
            if(mScenes.TryGetValue(path, out scene))
                return scene;
            else
                return load(path);
        }

        /** Ennumerates all scenes */
        IEnumerable<Scene> scenes()
        {
            return mScenes.Values;
        }
        

        /** Loads a scene from the designated path, and adds it to the internal list, overwriting any previous scenes. */
        public Scene load(String path)
        {
            path = System.IO.Path.Combine("scripts\\scenes\\", path);
            if (!System.IO.File.Exists(path))
                throw new Exception("Unable to load scene: \"" + path + "\" not found.");
            Scene scene = new Scene(path);
            mScenes[path] = scene;
            return scene;
        }

        /** Disposes of all scenes */
        public void Dispose()
        {
            foreach (Scene scene in mScenes.Values)
                scene.Dispose();
        }
    }
}
