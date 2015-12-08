namespace Polys.Game
{
    public class Character
    {
        public enum Orientation { Up=0x0000000f, Down=0x000000f0,
                                  Left=0x00000f00, Right= 0x0000f000,
                                  UpRight = 0x0000f00f, UpLeft= 0x00000f0f,
                                  DownRight = 0x0000f0f0, DownLeft=0x00000ff0, NA=0x00000000 };
        public enum WalkState { Standing, Walking };
        
        public WalkState walkState = WalkState.Standing;

        public Orientation orientation { get; set; }

        int idleFrames;

        public Character(string name, Video.Sprite sprite, int idleFrames=1)
        {
            this.name = name;
            this.sprite = sprite;
            this.idleFrames = idleFrames;
        }

        public string name { get; set; }
        public Video.Sprite sprite { get; private set; }
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
            {
                int msToChange = 400000;
                ySpriteIndex = (((int)Time.currentTime / msToChange)%idleFrames);
                System.Console.WriteLine(ySpriteIndex);
            }
            else
                ySpriteIndex = idleFrames + (((Time.currentTime % 800000) < 400000) ? 0 : 1);

            sprite.setTilesetIndex(xSpriteIndex, ySpriteIndex);
        }
    }
}
