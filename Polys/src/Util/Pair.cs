using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Util
{
    class Pair<First, Second>
    {
        public First first;
        public Second second;

        public Pair()
        {

        }

        public Pair(First f, Second s)
        {
            first = f;
            second = s;
        }
    }
}
