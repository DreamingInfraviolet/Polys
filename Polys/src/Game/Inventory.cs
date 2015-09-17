using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game
{
    class Inventory
    {
        private List<InventoryItem> mItems;

        public List<InventoryItem> items
        {
            get { return mItems; }
        }
    }
}
