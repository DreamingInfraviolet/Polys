using System;
using System.Collections.Generic;
using SDL2;

namespace Polys
{
    class Input
    {
        IntentManager intentManager;
        
        //This keytable is used to handle continuous key presses.
        //Once a key is pressed, it is stored until it is unpressed.
        //Maps SDL_Keycode value to bool, as it's a non-linear enum.
        public Dictionary<SDL.SDL_Keycode, bool> mKeyTable;

        private void initKeyDictionary()
        {
            if (mKeyTable != null)
                return;

            mKeyTable = new Dictionary<SDL.SDL_Keycode, bool>();
            string[] entries = Enum.GetNames(typeof(SDL.SDL_Keycode));

            for (int i = 0; i < entries.Length; ++i)
                mKeyTable.Add((SDL.SDL_Keycode)Enum.Parse(typeof(SDL.SDL_Keycode), entries[i]), false);
        }

        public Input(IntentManager handler)
        {
            this.intentManager = handler;
            initKeyDictionary();
        }

        public void keyDown(SDL.SDL_Keycode key)
        {
            mKeyTable[key] = true;
            intentManager.keyDown(key);
        }

        public void keyUp(SDL.SDL_Keycode key)
        {
            mKeyTable[key] = false;
            intentManager.keyUp(key);
        }

        public void process()
        {
            foreach(var v in mKeyTable)
            {
                if (v.Value)
                    intentManager.keyHeld(v.Key);
            }
        }
    }
}
