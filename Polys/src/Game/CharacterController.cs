namespace Polys.Game
{
    /** A character controller can control any entity. */
    public class CharacterController : IIntentHandler
    {
        //These should be floats, as otherwise small deltas have no effect.
        public OpenGL.Vector2 position = new OpenGL.Vector2();
        public OpenGL.Vector2 velocity = new OpenGL.Vector2(); 
        float speed;

        public Character character;

        public CharacterController(float speed)
        {
            this.speed = speed;
        }

        bool canMove(OpenGL.Vector2 pos, Video.TileLayer collisionLayer)
        {
            int colliderWidth = 10, colliderHeight = 4;
            int newPosX = (int)pos.x;
            int newPosY = (int)pos.y;
            int colliderX = newPosX + character.sprite.rect.w / 2 - colliderWidth / 2;
            int colliderY = newPosY + 3 - colliderHeight / 2;

            //Check if we are colliding with something.
            bool colliding = false;
            if (collisionLayer != null)
                colliding = collisionLayer.intersects(
                    new Util.Rect(colliderX, colliderY, colliderWidth, colliderHeight));

            return !colliding;
        }

        public void finishGatheringInput(Video.TileLayer collisionLayer)
        {
            velocity = velocity.Normalize() * speed * Time.deltaTime;
            OpenGL.Vector2 newPosition = position + velocity;

            bool canMoveXY = canMove(position + velocity, collisionLayer);
            bool moved = velocity.x != 0 || velocity.y != 0;

            if (!canMoveXY)
            {
                float dx = speed * Time.deltaTime * (velocity.x > 0 ? 1 : -1);
                float dy = speed * Time.deltaTime * (velocity.y > 0 ? 1 : -1);
                bool canMoveX = canMove(new OpenGL.Vector2(position.x + dx, position.y), collisionLayer);
                bool canMoveY = canMove(new OpenGL.Vector2(position.x, position.y+dy), collisionLayer);

                //If we are moving on less than two axes 
                if (!(velocity.x != 0 & velocity.y != 0))
                    moved = false;
                else if (canMoveX)
                {
                    position.x += dx;
                    velocity.y = 0;
                }
                else if (canMoveY)
                {
                    position.y += dy;
                    velocity.x = 0;
                }
            }
            else
                position = position + velocity;
            
            character.sprite.rect.x = (int)position.x;
            character.sprite.rect.y = (int)position.y;

            character.orientation = orientationFromVelocity(character.orientation, moved);
            velocity.x = 0;
            velocity.y = 0;
        }

        Character.Orientation orientationFromVelocity(Character.Orientation @default, bool walking)
        {
            character.walkState = walking ? Character.WalkState.Walking : Character.WalkState.Standing;

            if(!walking)
            {
                character.walkState = Character.WalkState.Standing;
                return @default;
            }
            else
            {
                character.walkState = Character.WalkState.Walking;
                return 
                 (velocity.x != 0 ? (velocity.x > 0 ? Character.Orientation.Right : Character.Orientation.Left) : Character.Orientation.NA) |
                 (velocity.y != 0 ? (velocity.y > 0 ? Character.Orientation.Up : Character.Orientation.Down) : Character.Orientation.NA);
            }
        }

        public void addMoveVector(int v1, int v2)
        {

            velocity.x += v1;
            velocity.y += v2;
        }

        public void handleIntent(IntentManager.IntentType intentCode)
        {
            switch (intentCode)
            {
                case IntentManager.IntentType.WALK_DOWN:
                    addMoveVector(0, -1);
                    break;
                case IntentManager.IntentType.WALK_UP:
                    addMoveVector(0, 1);
                    break;
                case IntentManager.IntentType.WALK_LEFT:
                    addMoveVector(-1, 0);
                    break;
                case IntentManager.IntentType.WALK_RIGHT:
                    addMoveVector(1, 0);
                    break;
            }
        }

        bool wantsKeyDownIntent() { return false; }
        bool wantsKeyUpIntent() { return false; }
        bool wantsKeyHeldIntent() { return true; }
    }
}
