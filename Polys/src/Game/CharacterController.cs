namespace Polys.Game
{
    /** A character controller can control any entity. */
    public class CharacterController : IIntentHandler
    {
        //These should be floats, as otherwise small deltas have o effect.
        public OpenGL.Vector2 position = new OpenGL.Vector2();
        public OpenGL.Vector2 velocity = new OpenGL.Vector2();
        float speed;

        public Character character;

        public CharacterController(float speed)
        {
            this.speed = speed;
        }

        public void finishGatheringInput(Video.TileLayer collisionLayer)
        {
            velocity = velocity.Normalize() * speed * Time.deltaTime;
            OpenGL.Vector2 newPosition = position + velocity;
            int newPosX = (int)newPosition.x;
            int newPosY = (int)newPosition.y;
            int colliderWidth = 10, colliderHeight = 4;
            int colliderX = newPosX + character.sprite.rect.w / 2 - colliderWidth / 2;
            int colliderY = newPosY + 3 - colliderHeight / 2;

            //Check if we are colliding with something.
            bool colliding = false;
            if (collisionLayer != null)
                colliding = collisionLayer.intersects(
                    new Util.Rect(colliderX, colliderY, colliderWidth, colliderHeight));

            if (!colliding)
            {
                position = newPosition;
                character.sprite.rect.x = newPosX;
                character.sprite.rect.y = newPosY;
            }
            else
            {
                character.sprite.rect.x = (int)position.x;
                character.sprite.rect.y = (int)position.y;
                System.Console.WriteLine("Overlapping");
            }

            character.orientation = orientationFromVelocity(character.orientation, colliding);
            velocity.x = 0;
            velocity.y = 0;
        }

        Character.Orientation orientationFromVelocity(Character.Orientation @default, bool walkingIsBlocked)
        {
            character.walkState = walkingIsBlocked ? Character.WalkState.Standing : Character.WalkState.Walking;

            if (velocity.x > 0) //Going right
                if (velocity.y > 0) //Going up
                    return Character.Orientation.UpRight;
                else if (velocity.y < 0) //Going down
                    return Character.Orientation.DownRight;
                else //Not moving on y axis
                    return Character.Orientation.Right;
            else if (velocity.x < 0) //Going left
                if (velocity.y > 0) //Going up
                    return Character.Orientation.UpLeft;
                else if (velocity.y < 0) //Going down
                    return Character.Orientation.DownLeft;
                else //Not moving on y axis
                    return Character.Orientation.Left;
            //Not moving on x axis
            else if (velocity.y > 0) //Going up
                return Character.Orientation.Up;
            else if (velocity.y < 0) //Going down
                return Character.Orientation.Down;
            else //Not moving on y axis
            {
                character.walkState = Character.WalkState.Standing;
                return @default;
            }
        }

        public void addMoveVector(int v1, int v2)
        {

            velocity.x += v1;
            velocity.y += v2;
        }

        public void handleIntent(IntentManager.IntentType intentCode, bool isKeyDown, bool isKeyUp, bool isKeyHeld)
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
    }
}
