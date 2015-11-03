using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    public class Transformable
    {
        public Util.Rect rect;

        public Transformable()
        {
            rect = new Util.Rect();
        }

        public Transformable(Util.Rect r)
        {
            rect = r;
        }
        
        public int centreX() { return rect.x / rect.w; }
        public int centreY() { return rect.y / rect.h; }

        public bool overlaps(Util.Rect r)
        {
            return r.x < (rect.x + rect.w) && (r.x + r.w) > rect.x &&
                r.y < (rect.y + rect.h) && (r.y + r.h) > rect.y;
        }
    }
}
