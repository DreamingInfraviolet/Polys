using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Video
{
    class Transformable
    {
        public int mPosX, mPosY;
        public int originX, originY;

        public int posX
        {
            get { return mPosX - originX; }
            set { mPosX = value; }
        }

        public int posY
        {
            get { return mPosY - originY; }
            set { mPosY = value; }
        }

        public Transformable()
        {
            posX = posY = originX = originY = 0;
        }

        public Transformable(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
        }

        public void setOriginAsSpriteCentre(int spriteWidth, int spriteHeight)
        {
            originX = spriteWidth / 2;
            originY = spriteHeight / 2;
        }
    }
}
