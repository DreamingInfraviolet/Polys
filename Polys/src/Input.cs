using MoonSharp.Interpreter;
using SDL2;
using System;
using System.Collections.Generic;

namespace Polys
{
    /** The input class is a low-level class that keeps track of the keys currently active. */
    [System.Runtime.InteropServices.ComVisible(true)]
    public class Input : IScriptInitialisable
    {
        struct Binding
        {
            public enum HandlerType { Down = 0x0000000f, Up = 0x000000f0, Held = 0x00000f00 };
            public HandlerType htype;
            public IntentManager.IntentType itype;
        }

        Dictionary<SDL.SDL_Keycode, HashSet<Binding>> intentBindigs = new Dictionary<SDL.SDL_Keycode, HashSet<Binding>>();


        public struct Key
        {
            public bool isDown;
            public bool justChanged;
        }

        //This keytable is used to handle continuous key presses.
        //Once a key is pressed, it is stored until it is unpressed.
        //Maps SDL_Keycode value to bool, as it's a non-linear enum.
        Dictionary<SDL.SDL_Keycode, Key> mKeyTable = new Dictionary<SDL.SDL_Keycode, Key>();

        /** Initialises the key table, mapping all possible keys to false. */
        private void initKeyDictionary()
        {
            //Get all the possible key enims
            string[] entries = Enum.GetNames(typeof(SDL.SDL_Keycode));

            //Add them to the dictionary, setting them to false
            for (int i = 0; i < entries.Length; ++i)
            {
                SDL.SDL_Keycode code = (SDL.SDL_Keycode)Enum.Parse(typeof(SDL.SDL_Keycode), entries[i]);
                Key key;
                key.isDown = false;
                key.justChanged = false;
                mKeyTable.Add(code, key);
            }
        }

        /** Basic constructor performing essential initialisation. */
        public Input()
        {
            initKeyDictionary();
        }

        /** Call this to signal that a key is being pressed down. */
        public void keyDown(SDL.SDL_Keycode key)
        {
            Key k = mKeyTable[key];
            k.justChanged = !k.isDown;
            k.isDown = true;
            mKeyTable[key] = k;
        }

        /** Call this to signal that a key is being held up. */
        public void keyUp(SDL.SDL_Keycode key)
        {
            Key k = mKeyTable[key];
            k.justChanged = k.isDown;
            k.isDown = false;
            mKeyTable[key] = k;
        }

        public bool isKeyDown(SDL.SDL_Keycode key)
        {
            return mKeyTable[key].isDown && mKeyTable[key].justChanged;
        }

        public bool isKeyUp(SDL.SDL_Keycode key)
        {
            return !mKeyTable[key].isDown;
        }

        public bool isKeyHeld(SDL.SDL_Keycode key)
        {
            return mKeyTable[key].isDown && !mKeyTable[key].justChanged;
        }

        public void endFrame()
        {
            IntentManager.clearIntents();
        }

        public void startFrame()
        {
            foreach (var pair in intentBindigs)
            {
                Key k = mKeyTable[pair.Key];
                foreach (var binding in pair.Value)
                {
                    if (binding.htype == Binding.HandlerType.Down && k.isDown && k.justChanged)
                        IntentManager.markForSending(binding.itype);
                    else if (binding.htype == Binding.HandlerType.Held && k.isDown && !k.justChanged)
                         IntentManager.markForSending(binding.itype);
                    else if (binding.htype == Binding.HandlerType.Up && !k.isDown && k.justChanged)
                        IntentManager.markForSending(binding.itype);
                }
            }

            //Update keys
            var copy = new Dictionary<SDL.SDL_Keycode, Key>(mKeyTable);
            foreach (var pair in copy)
            {
                var key = pair.Value;
                key.justChanged = false;
                mKeyTable[pair.Key] = key;
            }
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
                    //Parse
                    string keyStr = ("SDLK_" + entry.Key.CastToString());

                    SDL.SDL_Keycode key = (SDL.SDL_Keycode)Enum.Parse(typeof(SDL.SDL_Keycode), keyStr, true);

                    //Each key may be associated with multiple actions using  ',' as a separator.
                    string[] actions = (entry.Value.CastToString()).Replace(" ", string.Empty).Split(',');
                    foreach (string action in actions)
                    {
                        string[] intent_type_pair = action.Split(':');
                        if (intent_type_pair.Length != 2)
                            throw new Exception("Invalid attribute format.");
                        IntentManager.IntentType intent = (IntentManager.IntentType)Enum.Parse(typeof(IntentManager.IntentType), intent_type_pair[0], true);

                        //Add binding
                        //If there is no array list of intents associated with the key, create it
                        HashSet<Binding> binding;
                        if (!intentBindigs.TryGetValue(key, out binding))
                        {
                            binding = new HashSet<Binding>();
                            intentBindigs.Add(key, binding);
                        }

                        //Add the intent to the given key
                        Binding b;
                        b.itype = intent;
                        if (intent_type_pair[1].Equals("pressed"))
                            b.htype = Binding.HandlerType.Down;
                        else if (intent_type_pair[1].Equals("released"))
                            b.htype = Binding.HandlerType.Up;
                        else if (intent_type_pair[1].Equals("held"))
                            b.htype = Binding.HandlerType.Held;
                        else
                            throw new Exception("Invalid attribute: must be 'held', 'pressed' or 'released'.");

                        binding.Add(b);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(String.Format("Invalid binding: \"{0}\"->\"{1}\". Reason: {2}", entry.Key, entry.Value, e.Message));
                }
            }
        }
    }
}
