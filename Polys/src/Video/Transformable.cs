using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class Transformable
    {
        public int posX, posY;

        public Transformable()
        {
            posX = posY = 0;
        }

        public Transformable(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
        }
    }
}
