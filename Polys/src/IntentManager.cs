using SDL2;
using System.Collections.Generic;

namespace Polys
{
    /** This class facilitates an intent system. 
      * Keys are mapped to intents dynamically. An IntentHandler can register for an intent, in which case they will be notified of it.
      * Issues:
      * deregistering might leave empty arrays.
      */
    class IntentManager
    {
        public enum KeyType { UP, DOWN, HELD };

        private class Intent
        {
            public IIntentHandler handler;

            //Defines when the intent should be triggered
            public bool wantsKeyDown, wantsKeyUp, wantsKeyHeld;
            
            //Defines the conditions met for triggering
            public bool getsKeyDown, getsKeyUp, getsKeyHeld;

            public void resetExecution()
            {
                getsKeyDown = getsKeyUp = getsKeyHeld = false;
            }

            public void markKey(KeyType type)
            {
                if (type == KeyType.DOWN)
                    getsKeyDown = true;
                else if (type == KeyType.UP)
                    getsKeyUp = true;
                else if (type == KeyType.HELD)
                    getsKeyHeld = true;
            }
        }

        public enum IntentType { WALK_UP, WALK_DOWN, WALK_LEFT, WALK_RIGHT, ESC };

        //Key bindings to intents. Each key is bound to a series of intents.
        Dictionary<IntentType, List<Intent>> handlers = new Dictionary<IntentType, List<Intent>>();

        Dictionary<SDL.SDL_Keycode, HashSet<IntentType>> bindings = new Dictionary<SDL.SDL_Keycode, HashSet<IntentType>>();

        /** Adds a handler to be notified when the inputted intent occurs.
         * 
         * @param handler The handler object to be notified.
         * @param intent The intent that must occur for the handler to be notified.
         * @param down Whether the intent should be fired if the key is down.
         * @param up Whether the intent should be fired if the key is up.
         * @param held Whether the event should be fired if the key is held.
         */
        public void register(IIntentHandler handler, IntentType intentCode, bool down, bool up, bool held)
        {
            deregister(handler, intentCode);

            Intent intent = new Intent();
            intent.handler = handler;
            intent.getsKeyDown = intent.getsKeyUp = intent.getsKeyHeld = false;
            intent.wantsKeyDown = down;
            intent.wantsKeyUp = up;
            intent.wantsKeyHeld = held;
            

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
        public void deregister (IIntentHandler handler, IntentType intentCode)
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
        public void deregister(IIntentHandler handler)
        {
            //Find the intent and deregister it
            foreach (var li in handlers)
                for (int i = 0; i < li.Value.Count; ++i)
                    if (li.Value[i].handler.Equals(handler))
                    {
                        li.Value.RemoveAt(i);
                        --i;
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
        
        private void markIntentExecution(SDL2.SDL.SDL_Keycode key, KeyType type)
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

                for(int i = 0; i < intents.Count; ++i)
                    if (type == KeyType.DOWN && intents[i].wantsKeyDown)
                        intents[i].markKey(type);
                    else if (type == KeyType.UP && intents[i].wantsKeyUp)
                        intents[i].markKey(type);
                    else if (type == KeyType.HELD && intents[i].wantsKeyHeld)
                        intents[i].markKey(type);
            }
        }

        public void dispatchRequestsAndClear()
        {
            foreach(var keyIntentPair in handlers)
                foreach(Intent intent in keyIntentPair.Value)
                    if((intent.getsKeyDown && intent.wantsKeyDown) ||
                        (intent.getsKeyUp && intent.wantsKeyUp) ||
                        (intent.getsKeyHeld && intent.wantsKeyHeld))
                    {
                        intent.handler.handleIntent(keyIntentPair.Key, intent.getsKeyDown, intent.getsKeyUp, intent.getsKeyHeld);
                        intent.resetExecution();
                    }
        }

        public void keyDown(SDL.SDL_Keycode key)
        {
            markIntentExecution(key, KeyType.DOWN);
        }

        public void keyUp(SDL.SDL_Keycode key)
        {
            markIntentExecution(key, KeyType.UP);
        }
        public void keyHeld(SDL.SDL_Keycode key)
        {
            markIntentExecution(key, KeyType.HELD);
        }
    }
}
