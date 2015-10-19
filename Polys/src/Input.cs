using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using SDL2;

namespace Polys
{
    /** The input class is a low-level class that keeps track of the keys currently active. */
    class Input : IScriptInitialisable
    {
        //The intent manager to be notified of any key presses
        IntentManager intentManager;
        
        //This keytable is used to handle continuous key presses.
        //Once a key is pressed, it is stored until it is unpressed.
        //Maps SDL_Keycode value to bool, as it's a non-linear enum.
        public Dictionary<SDL.SDL_Keycode, bool> mKeyTable = new Dictionary<SDL.SDL_Keycode, bool>();

        /** Initialises the key table, mapping all possible keys to false. */
        private void initKeyDictionary()
        {
            //Get all the possible key enims
            string[] entries = Enum.GetNames(typeof(SDL.SDL_Keycode));

            //Add them to the dictionary, setting them to false
            for (int i = 0; i < entries.Length; ++i)
                mKeyTable.Add((SDL.SDL_Keycode)Enum.Parse(typeof(SDL.SDL_Keycode), entries[i]), false);
        }

        /** Basic constructor performing essential initialisation. */
        public Input(IntentManager handler)
        {
            this.intentManager = handler;
            initKeyDictionary();
        }

        /** Call this to signal that a key is being pressed down. */
        public void keyDown(SDL.SDL_Keycode key)
        {
            mKeyTable[key] = true;
            intentManager.keyDown(key);
        }

        /** Call this to signal that a key is being held up. */
        public void keyUp(SDL.SDL_Keycode key)
        {
            mKeyTable[key] = false;
            intentManager.keyUp(key);
        }

        /** Sends out any pending keys and sends out intents to registered classes. */
        public void finalise()
        {
            foreach(var v in mKeyTable)
                if (v.Value)
                    intentManager.keyHeld(v.Key);

            intentManager.dispatchRequestsAndClear();
        }
        
        /** Returns the script representation of the class. */
        public string ScriptName()
        {
            return "keyBindings";
        }

        /** Initialises the key bindings from script. */
        public void InitialiseFromScript(Table table)
        {
            foreach (var entry in table.Pairs)
            {
                try
                {
                    string keyStr = ("SDLK_" + entry.Key.CastToString());
                    string valueStr = (entry.Value.CastToString());

                    SDL.SDL_Keycode key = (SDL.SDL_Keycode)Enum.Parse(typeof(SDL.SDL_Keycode), keyStr, true);
                    IntentManager.IntentType intent = (IntentManager.IntentType)Enum.Parse(typeof(IntentManager.IntentType), valueStr, true);

                    intentManager.addBinding(key, intent);

                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Invalid binding: \"{0}\"->\"{1}\". Internal reason: {2}", entry.Key, entry.Value, e.Message));
                }
            }
        }
    }
}
