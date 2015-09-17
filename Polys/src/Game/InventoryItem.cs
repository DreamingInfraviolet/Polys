using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game
{
    class InventoryItem
    {
        public InventoryItem(String name)
        {

        }

        public String name
        {
            get { return "";  }
        }

        public String cescription
        {
            get { return ""; }
            set {}
        }

        public int value
        {
            get { return -1; }
            set { }
        }

        public float lootFrequency
        {
            get { return 0.0f; }
            set { }
        }

        public bool wearable
        {
            get { return false; }
            set { }
        }

    }
}
