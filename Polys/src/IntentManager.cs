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
        
        //A dictinary of intent types matched with a list of handlers for each.
        static Dictionary<IntentType, HashSet<IIntentHandler>> handlers =
            new Dictionary<IntentType, HashSet<IIntentHandler>>();


        /** Adds a handler to be notified when the inputted intent occurs.
         * 
         * @param handler The handler object to be notified.
         * @param intent The intent that must occur for the handler to be notified.
         */
        public static void register(IIntentHandler handler, IntentType intentCode)
        {
            HashSet<IIntentHandler> set;
            if (!handlers.TryGetValue(intentCode, out set))
            {
                set = new HashSet<IIntentHandler>();
                handlers.Add(intentCode, set);
            }
            set.Add(handler);
        }

        /** The handler will no longer be notified when the particular intent occurs.
         * 
         * @param handler The handler to be removed from the specific intent notification list.
         * @param intent The intent which the handler no longer wishes to receive.
         */
        public static void deregister (IIntentHandler handler, IntentType intentCode)
        {
            HashSet<IIntentHandler> set;
            if(handlers.TryGetValue(intentCode, out set))
                set.Remove(handler);
        }

        /** The handler will no longer be notified of any intents.
         * @param handler The handler to de-register from all intents. */
        public static void deregister(IIntentHandler handler)
        {
            foreach (var set in handlers)
                set.Value.Remove(handler);
        }

        
        static HashSet<IntentType> sendBuffer = new HashSet<IntentType>();

        /** Puts the intent into a queue to be sent during the next frame. Note that an intent is sent only once,
          * no matter how many times it has been marked. */
        public static void markForSending(IntentType t)
        {
            sendBuffer.Add(t);
        }

        /** Sends all events, clearing the buffer. Events are allowed to be sent from handlers. */
        public static void sendAll()
        {
            HashSet<IntentType> set = new HashSet<IntentType>(sendBuffer);
            sendBuffer.Clear();
            foreach (IntentType t in set)
                if(handlers.ContainsKey(t))
                    foreach (var handler in handlers[t])
                        handler.handleIntent(t);
        }
    }
}
