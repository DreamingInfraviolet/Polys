using System;

namespace Polys.Game
{
    /** Represents a single inventory item */
    class InventoryItem
    {
        public InventoryItem(String name)
        {

        }

        public String name { get; private set; }

        public String description { get; private set; }

        public int value { get; private set; }

        public float lootFrequency { get; private set; }

        public bool wearable { get; private set; }

    }
}