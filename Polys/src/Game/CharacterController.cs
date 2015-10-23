﻿namespace Polys.Game
{
    /** A character controller can control any entity. */
    class CharacterController : IIntentHandler
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

        public void finishGatheringInput()
        {
            velocity = velocity.Normalize()*speed*Time.deltaTime;
            position += velocity;

            character.sprite.posX = (int)position.x;
            character.sprite.posY = (int)position.y;
            character.orientation = orientationFromVelocity(character.orientation);

            velocity.x = 0;
            velocity.y = 0;
        }

        Character.Orientation orientationFromVelocity(Character.Orientation @default)
        {
            character.walkState = Character.WalkState.Walking;

            if (velocity.x > 0) //Going right
                if (velocity.y > 0) //Going down
                    return Character.Orientation.DownRight;
                else if (velocity.y < 0) //Going up
                    return Character.Orientation.UpRight;
                else //Not moving on y axis
                    return Character.Orientation.Right;
            else if (velocity.x < 0) //Going left
                if (velocity.y > 0) //Going down
                    return Character.Orientation.DownLeft;
                else if (velocity.y < 0) //Going up
                    return Character.Orientation.UpLeft;
                else //Not moving on y axis
                    return Character.Orientation.Left;
            else //Not moving on x axis
                if (velocity.y > 0) //Going down
                return Character.Orientation.Down;
            else if (velocity.y < 0) //Going up
                return Character.Orientation.Up;
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
