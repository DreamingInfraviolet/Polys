using System;
using System.Collections.Generic;

namespace Polys.Video
{
    /** Represents a bitmap font in memory. A font is nearly identical to a tileset,
      * mainly differing in that each tile is a letter. */
    class Font : Tileset
    {
        int characterWidth, characterHeight;
        Dictionary<char, Util.Pair<int, int>> charPosMapping = new Dictionary<char, Util.Pair<int, int>>();

        public Font(string path, int characterWidth, int characterHeight, Dictionary<char, Util.Pair<int, int>> charPosMapping)
          : base(path, "font", characterWidth, characterHeight)
        {
            this.characterWidth = characterWidth;
            this.characterHeight = characterHeight;
            this.charPosMapping = charPosMapping;
        }

        public void renderText(string text, int positionX, int positionY)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                Util.Pair<int,int> charPos;
                if(!charPosMapping.TryGetValue(text[i], out charPos))
                    continue;

                Sprite sprite = new Sprite(positionX+characterWidth*i, positionY,
                    characterWidth, characterHeight, true,
                    charPos.first*characterWidth, charPos.second*characterHeight);

                HighLevelRenderer.draw(sprite, this, new Camera());
            }
        }
    }
}
