using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Polys.Game
{
    class CharacterController : IntentHandler
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

        public void handleIntent(IntentManager.IntentType intentCode, IntentManager.KeyType type)
        {
            switch(intentCode)
            {
                case IntentManager.IntentType.WALK_DOWN:
                    addMoveVector(0, 1);
                    break;
                case IntentManager.IntentType.WALK_UP:
                    addMoveVector(0, -1);
                    break;
                case IntentManager.IntentType.WALK_LEFT:
                    addMoveVector(-1, 0);
                    break;
                case IntentManager.IntentType.WALK_RIGHT:
                    addMoveVector(1, 0);
                    break;
            }
        }
    }
}
