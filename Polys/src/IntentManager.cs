using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDL2;

//Issues:
//  deregistering might leave empty arrays.

namespace Polys
{
    class IntentManager
    {
        public enum KeyType { UP, DOWN, HELD };

        private struct Intent
        {
            public IntentHandler handler;
            public KeyType keyType;
        }

        public enum IntentType { WALK_UP, WALK_DOWN, WALK_LEFT, WALK_RIGHT, ESC };

        //Key bindings to intents. Each key is bound to a series of intents.
        Dictionary<IntentType, List<Intent>> handlers = new Dictionary<IntentType, List<Intent>>();

        Dictionary<SDL.SDL_Keycode, HashSet<IntentType>> bindings = new Dictionary<SDL.SDL_Keycode, HashSet<IntentType>>();

        /** Default constructor doing trivial initialisation. */
        public IntentManager()
        {
        }

        /** Adds a handler to be notified when the inputted intent occurs.
         * 
         * @param handler The handler object to be notified.
         * @param intent The intent that must occur for the handler to be notified.
         */
        public void register(IntentHandler handler, IntentType intentCode, KeyType keyType)
        {
            deregister(handler, intentCode);

            Intent intent;
            intent.handler = handler;
            intent.keyType = keyType;

            List<Intent> insertionList;
            if (!handlers.TryGetValue(intentCode, out insertionList))
            {
                insertionList = new List<Intent>();
                handlers.Add(intentCode, insertionList);
            }

            handlers[intentCode].Add(intent);
        }

        /** The handler will no longer be notified when the particular intent occurs.
         * 
         * @param handler The handler to be removed from the specific intent notification list.
         * @param intent The intent which the handler no longer wishes to receive.
         */
        public void deregister (IntentHandler handler, IntentType intentCode)
        {
            //Find the intent and deregister it
            foreach(var li in handlers)
            {
                for(int i = 0; i < li.Value.Count; ++i)
                {
                    if(li.Key == intentCode && li.Value[i].handler.Equals(handler))
                    {
                        li.Value.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        /** The handler will no longer be notified of any intents.
         * 
         * @param handler The handler to de-register from all intents.
         */
        public void deregister(IntentHandler handler)
        {
            //Find the intent and deregister it
            foreach (var li in handlers)
            {
                for (int i = 0; i < li.Value.Count; ++i)
                {
                    if (li.Value[i].handler.Equals(handler))
                    {
                        li.Value.RemoveAt(i);
                        --i;
                    }
                }
            }
        }

        /** Adds a key-intent binding, so that a particular intent will occur when a key is pressed or held.
         * 
         * @param key The key to be associated with the event.
         * @param intent
         */
        public void addBinding(SDL.SDL_Keycode key, IntentType intentCode)
        {
            //If there is no array list of intents associated with the key, create it
            HashSet<IntentType> binding;
            if (!bindings.TryGetValue(key, out binding))
            {
                binding = new HashSet<IntentType>();
                bindings.Add(key, binding);
            }

            //Add the intent to the given key
            binding.Add(intentCode);
        }

        private void processKey(SDL2.SDL.SDL_Keycode key, KeyType type)
        {
            HashSet<IntentType> binding;

            //If there is no such binding, exit.
            if(!bindings.TryGetValue(key, out binding))
                return;
            
            //Iterate through all intent types bound to the key.
            foreach(IntentType intentToHandle in binding)
            {
                List<Intent> intents;

                //If binding exists but no handlers
                if(!handlers.TryGetValue(intentToHandle, out intents))
                    return;

                foreach(Intent intent in intents)
                {
                    if (type == KeyType.DOWN && intent.keyType == type)
                        intent.handler.handleIntent(intentToHandle, type);
                    else if (type == KeyType.UP && intent.keyType == type)
                        intent.handler.handleIntent(intentToHandle, type);
                    else if (type == KeyType.HELD && intent.keyType == type)
                        intent.handler.handleIntent(intentToHandle, type);
                }
            }
        }

        public void keyDown(SDL.SDL_Keycode key)
        {
            processKey(key, KeyType.DOWN);
        }

        public void keyUp(SDL.SDL_Keycode key)
        {
            processKey(key, KeyType.UP);
        }
        public void keyHeld(SDL.SDL_Keycode key)
        {
            processKey(key, KeyType.HELD);
        }
    }
}
