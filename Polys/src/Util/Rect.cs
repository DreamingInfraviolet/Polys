using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Util
{
    public struct Rect
    {
        public int x, y, w, h;

        public Rect(int x=0,int y=0, int w=0, int h=0)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
        }

        public int bottom { get { return y; } set { y = value; } }
        public int left { get { return x; } set { x = value; } }
        public int top
        {
            get { return y + h; }
            set { if (value < y) { y = value; h = 0; } else h = value - y; }
        }
        public int right
        {
            get { return x + w; }
            set { if (value < x) { x = value; w = 0; } else w = value - x; }
        }

        public int area { get { return w * h; } }


        public bool overlaps(Rect r)
        {
            return r.x < (x + w) && (r.x + r.w) >= x &&
                r.y < (y + h) && (r.y + r.h) >= y;
        }
    }
}
