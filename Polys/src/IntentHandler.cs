using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys
{
    /** A class wishing to receive intents must implement this interface, after which they can register with the intent system. */
    interface IIntentHandler
    {
        /** Handles an intent for which the class was registered. */
        void handleIntent(IntentManager.IntentType intentCode, bool isKeyDown, bool isKeyUp, bool isKeyHeld);
    }
}
