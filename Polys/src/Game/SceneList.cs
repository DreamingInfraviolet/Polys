using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/** A class that stores the list of scenes that have been loaded during the playthrough.
  * Note that only one disctinct copy of a scene may exist during one time. */
namespace Polys.Game
{ 
    class SceneList
    {
        Dictionary<String, Scene> mScenes = new Dictionary<string, Scene>();

        public Scene current{ get; set; }
        
        public SceneList()
        {
            current = load("startup.lua");
        }

        /** Loads the scene the first time, but returns the scene first loaded in subsequent calls. Preferable to load(). */
        public Scene get(String path)
        {
            Scene scene;
            if(mScenes.TryGetValue(path, out scene))
                return scene;
            else
                return load(path);
        }

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

        public void Dispose()
        {
            foreach (Scene scene in mScenes.Values)
                scene.Dispose();
        }
    }
}
