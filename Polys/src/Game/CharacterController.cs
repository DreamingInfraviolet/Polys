namespace Polys.Game
{
    /** A character controller can control any entity. */
    class CharacterController : IIntentHandler
    {
        public Character character
        {
            get { return new Character(); }
        }

        public int positionX, positionY;
        public OpenGL.Vector2 velocity = new OpenGL.Vector2();
        float speed;

        public CharacterController(float speed)
        {
            this.speed = speed;
        }

        public void finishGatheringInput()
        {
            velocity = velocity.Normalize()*speed*Time.deltaTime;
            positionX += (int)velocity.x;
            positionY += (int)velocity.y;
            velocity.x = 0;
            velocity.y = 0;
        }
           
        public void addMoveVector(int v1, int v2)
        {

            velocity.x += v1;
            velocity.y += v2;
        }

        public void handleIntent(IntentManager.IntentType intentCode, bool isKeyDown, bool isKeyUp, bool isKeyHeld)
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
