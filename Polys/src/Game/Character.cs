namespace Polys.Game
{
    class Character
    {
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
