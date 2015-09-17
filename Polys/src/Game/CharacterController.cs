using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game
{
    class CharacterController
    {
        public Character character
        {
            get { return new Character(); }
        }

        public int movementX, movementY;

        public void begin(long timeParameter)
        {
            movementX = movementY = 0;
        }

        public void end()
        {
            
        }
           
        public void addMoveVector(int v1, int v2)
        {
            movementX += v1;
            movementY += v2;
        }
    }
}
