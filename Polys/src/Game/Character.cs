namespace Polys.Game
{
    public class Character
    {
        public enum Orientation { Up, Down, Left, Right, UpRight, UpLeft, DownLeft, DownRight };
        public enum WalkState { Standing, Walking };
        
        public WalkState walkState = WalkState.Standing;

        public Orientation orientation { get; set; }


        public Character(string name, Video.DrawableSprite sprite)
        {
            this.name = name;
            this.sprite = sprite;
        }

        public string name { get; set; }
        public Video.DrawableSprite sprite { get; private set; }
        public void updateUv()
        {
            int xSpriteIndex, ySpriteIndex;
            
            switch (orientation)
            {
                case Orientation.Down:
                case Orientation.DownLeft:
                case Orientation.DownRight:
                    xSpriteIndex = 0;
                    break;
                case Orientation.Up:
                case Orientation.UpLeft:
                case Orientation.UpRight:
                    xSpriteIndex = 1;
                    break;
                case Orientation.Left:
                    xSpriteIndex = 2;
                    break;
                case Orientation.Right:
                    xSpriteIndex = 3;
                    break;
                default:
                    xSpriteIndex = 0;
                    break;
            }

            if (walkState == WalkState.Standing)
                ySpriteIndex = 0;
            else
                ySpriteIndex = ((Time.currentTime%800000)<400000) ? 1 : 2;

            sprite.setTilesetIndex(xSpriteIndex, ySpriteIndex);
        }
    }
}
