using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Polys.Video.UI
{
    /** Represents a bitmap font in memory. A font is nearly identical to a tileset,
      * mainly differing in that each tile is a letter. */
    public class Font : Tileset, IScriptInitialisable
    {
        Dictionary<char, Util.Pair<int, int>> charPosMapping = new Dictionary<char, Util.Pair<int, int>>();

        public Font(string path)
          : base(path, "font")
        {
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


            tileWidth = ScriptManager.retrieveValue(table, "characterWidth", 16);
            tileHeight = ScriptManager.retrieveValue(table, "characterHeight", 16);
        }

        List<Util.Pair<int,int>> getCharacterPositions(string text, int positionX, int positionY, int boxWidth)
        {
            List<Util.Pair<int, int>> result = new List<Util.Pair<int, int>>();

            int pixelPosX = positionX, pixelPosY = positionY;
            string[] words = text.Split(' ', '\t');

            foreach (var word in words)
            {
                if (boxWidth > 0)
                {
                    //If the word is out of bounds  and not super big, insert a newline
                    if ((pixelPosX + word.Length * tileWidth >= boxWidth+ positionX) && word.Length * tileWidth < boxWidth)
                    {

                        pixelPosX = positionX;
                        pixelPosY -= tileHeight;
                    }
                }

                //Draw the word
                foreach (var character in word)
                {
                    if (character == '\n' || (boxWidth > 0 && pixelPosX + tileWidth >= boxWidth+ positionX))
                    {
                        pixelPosX = positionX;
                        pixelPosY -= tileHeight;
                    }
                    {
                        Util.Pair<int, int> charPos;
                        if (!charPosMapping.TryGetValue(character, out charPos))
                            continue;
                        pixelPosX += tileWidth;
                        result.Add(new Util.Pair<int, int>(pixelPosX, pixelPosY));
                    }
                }

                //Add space after the word
                pixelPosX += tileWidth;
            }

            return result;
        }

        public int maxRightPosition(string text, int startX, int boxWidth)
        {
            var charPositions = getCharacterPositions(text, startX, 0, boxWidth);
            if (charPositions.Count == 0)
                return 0;
            int max = charPositions[0].first;
            for (int i = 1; i < charPositions.Count; ++i)
                if (max < charPositions[i].first)
                    max = charPositions[i].first;
            return max;
        }

        public int minDownPosition(string text, int startY, int boxWidth)
        {
            var charPositions = getCharacterPositions(text, 0, startY, boxWidth);
            if (charPositions.Count == 0)
                return 0;
            int min = charPositions[0].second;
            for (int i = 1; i < charPositions.Count; ++i)
                if (min < charPositions[i].second)
                    min = charPositions[i].second;
            return min;
        }


        public void renderText(string text, int positionX, int positionY, int boxWidth=0)
        {
            if (text.Length == 0)
                return;

            var charPositions = getCharacterPositions(text, positionX, positionY, boxWidth);

            if (charPositions.Count == 0)
                return;

            int index = 0;
            foreach(var c in text)
            {
                Util.Pair<int, int> charPos;
                if (c != '\n' && charPosMapping.TryGetValue(c, out charPos))
                {
                    Sprite sprite = new Sprite(new Util.Rect(charPositions[index].first, charPositions[index].second    ,
                        tileWidth, tileHeight), this, true,
                        charPos.first * tileWidth, charPos.second * tileHeight);

                    HighLevelRenderer.draw(sprite);
                }
            ++index;
            }
       } 
        public string ScriptName()
        {
            return "map";
        }
    }
}
