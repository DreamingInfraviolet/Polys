using SDL2;
using System.Collections.Generic;

namespace Polys
{
    /** This class facilitates an intent system. 
      * Keys are mapped to intents dynamically. An IntentHandler can register for an intent, in which case they will be notified of it.
      * Issues:
      * deregistering might leave empty arrays.
      */
    public class IntentManager
    {
        public enum IntentType { WALK_UP, WALK_DOWN, WALK_LEFT, WALK_RIGHT, ESC, MOVE_SELECTION_UP, MOVE_SELECTION_DOWN, CONFIRM_SELECTION };

        static HashSet<IntentType> activeIntents = new HashSet<IntentType>();

        /** Puts the intent into a queue to be sent during the next frame. Note that an intent is sent only once,
          * no matter how many times it has been marked. */
        public static void markForSending(IntentType t)
        {
            activeIntents.Add(t);
        }

        public static void clearIntents()
        {
            activeIntents.Clear();
        }

        public static bool isActive(IntentType t)
        {
            return activeIntents.Contains(t);
        }
    }
}
