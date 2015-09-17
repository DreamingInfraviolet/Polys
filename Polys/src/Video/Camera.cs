using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class Camera
    {
        //Top right
        int mCornerX, mCornerY;


        public void worldToScreen(ref int x, ref int y)
        {
            x -= mCornerX;
            y -= mCornerY;
        }

        public void move(int dx, int dy)
        {
            mCornerX += dx;
            mCornerY += dy;
        }
    }
}
