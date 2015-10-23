namespace Polys.Game
{
    class Character
    {
        public enum Orientation { Up, Down, Left, Right, UpRight, UpLeft, DownLeft, DownRight };

        Orientation mOrientation;

        public Orientation orientation
        {
            get { return mOrientation; }
            set
            {
                mOrientation = value;
                switch (value)
                {
                    case Orientation.Down:
                    case Orientation.DownLeft:
                    case Orientation.DownRight:
                        sprite.setUV(sprite.tileset, 0, 0);
                        break;
                    case Orientation.Up:
                    case Orientation.UpLeft:
                    case Orientation.UpRight:
                        sprite.setUV(sprite.tileset, 1, 0);
                        break;
                    case Orientation.Left:
                        sprite.setUV(sprite.tileset, 2, 0);
                        break;
                    case Orientation.Right:
                        sprite.setUV(sprite.tileset, 3, 0);
                        break;
                }
            }
        }


        public Character(string name, Video.DrawableSprite sprite)
        {
            this.name = name;
            this.sprite = sprite;
            sprite.setOriginAsSpriteCentre(sprite.tileset.width, sprite.tileset.height);
        }

        public string name { get; set; }
        public Video.DrawableSprite sprite { get; private set; }
    }
}
