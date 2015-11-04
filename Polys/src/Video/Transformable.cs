﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    public class Transformable : IComparable<Transformable>
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

        public int CompareTo(Transformable other)
        {
            if (rect.x == other.rect.x && rect.y == other.rect.y)
                return 0;
            if (rect.y > other.rect.y)
                return -1;
            else if (rect.y == other.rect.y)
            {
                if (rect.x < other.rect.x)
                    return -1;
                else
                    return 1;
            }
            else
                return 1;
        }

        public bool overlaps(Util.Rect r)
        {
            return r.x < (rect.x + rect.w) && (r.x + r.w) > rect.x &&
                r.y < (rect.y + rect.h) && (r.y + r.h) > rect.y;
        }
    }
}
