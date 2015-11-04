using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Polys.Video
{
    /** Represents a bitmap font in memory. A font is nearly identical to a tileset,
      * mainly differing in that each tile is a letter. */
    public class Font : Tileset, IScriptInitialisable
    {
        int characterWidth, characterHeight;
        Dictionary<char, Util.Pair<int, int>> charPosMapping = new Dictionary<char, Util.Pair<int, int>>();

        public Font(string path, int characterWidth, int characterHeight)
          : base(path, "font", characterWidth, characterHeight)
        {
            this.characterWidth = characterWidth;
            this.characterHeight = characterHeight;
            ScriptManager.initialiseFromScript(System.IO.Path.ChangeExtension(path, ".mapping"), this);
        }

        public void InitialiseFromScript(Table table)
        {
            foreach(var entry in table.Pairs)
            {
                //Test that the value is a valid table.
                if (entry.Key.Type!=DataType.String ||
                    entry.Key.String.Length != 1 ||
                    entry.Value.Type!=DataType.Table ||
                    entry.Value.Table.Length!=2 ||
                    entry.Value.Table.Get(DynValue.NewNumber(1)).Type != DataType.Number ||
                    entry.Value.Table.Get(DynValue.NewNumber(2)).Type != DataType.Number)
                    continue;

                int x = (int)entry.Value.Table.Get(DynValue.NewNumber(1)).Number;
                int y = (int)entry.Value.Table.Get(DynValue.NewNumber(2)).Number;

                charPosMapping.Add(entry.Key.String[0], new Util.Pair<int, int>(x, y));
            }
        }

        public void renderText(string text, int positionX, int positionY)
        {
            for (int i = 0; i < text.Length; ++i)
            {
                Util.Pair<int,int> charPos;
                if(!charPosMapping.TryGetValue(text[i], out charPos))
                    continue;

                Sprite sprite = new Sprite(new Util.Rect(positionX+characterWidth*i, positionY,
                    characterWidth, characterHeight), this, true,
                    charPos.first*characterWidth, charPos.second*characterHeight);

                HighLevelRenderer.draw(sprite, new Camera());
            }
        }

        public string ScriptName()
        {
            return "map";
        }
    }
}
