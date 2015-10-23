using System;

namespace Polys.Video
{
    /** Represents a bitmap font in memory. A font is nearly identical to a tileset,
      * mainly differing in that each tile is a letter. */
    class Font : Tileset
    {
        public Font(string path, int characterWidth, int characterHeight, string name = "font")
          : base(path, name, characterWidth, characterHeight, 1)
        {

        }

        public void renderText(string text, int positionX, int positionY)
        {

        }
    }
}
