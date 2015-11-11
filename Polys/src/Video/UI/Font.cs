using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Polys.Video.UI
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

        public void renderText(string text, int positionX, int positionY, int boxWidth=0)
        {
            int pixelPosX = positionX, pixelPosY = positionY;
            string[] words = text.Split(' ', '\t');

            foreach(var word in words)
            {
                if (boxWidth > 0)
                {
                    //If the word is out of bounds  and not super big, insert a newline
                    if ((pixelPosX+word.Length * characterWidth >= boxWidth) && word.Length * characterWidth < boxWidth)
                    {

                        pixelPosX = positionX;
                        pixelPosY -= characterHeight;
                    }
                }

                //Draw the word
                foreach(var character in word)
                {
                    if (character == '\n' || (boxWidth > 0 && pixelPosX +characterWidth>=boxWidth))
                    {
                        pixelPosX = positionX;
                        pixelPosY -= characterHeight;
                    }
                    else
                    {
                        Util.Pair<int, int> charPos;
                        if (!charPosMapping.TryGetValue(character, out charPos))
                            continue;

                        pixelPosX += characterWidth;

                        Sprite sprite = new Sprite(new Util.Rect(pixelPosX, pixelPosY,
                            characterWidth, characterHeight), this, true,
                            charPos.first * characterWidth, charPos.second * characterHeight);

                        HighLevelRenderer.draw(sprite, null);
                    }
                }

                //Add space after the word
                pixelPosX += characterWidth;
            }
       } 
        public string ScriptName()
        {
            return "map";
        }
    }
}
