using System;

namespace Mahjong
{
    public class Tiles
    {
        public enum TileType
        {
            Man, Sak, Tong, Wind, Word, End
        }
        public enum Winds
        {
            East=1,South,West,North,End
        }
        public enum Words
        {
            Blank=1,Start,Middle,End
        }
        public struct Tile
        {
            public TileType type;
            public int tileNumber;
            public bool isDora;
            public bool isShowing;
        }

        public static void PrintTile(Tile tile)
        {
            if (!tile.isShowing)
            {
                Console.Write("🀫 ");
                return;
            }
            
            if (tile.isDora)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            if (tile.type == TileType.Man)
            {
                switch (tile.tileNumber)
                {
                    case 1 : Console.Write("🀇"); break;
                    case 2 : Console.Write("🀈"); break;
                    case 3 : Console.Write("🀉"); break;
                    case 4 : Console.Write("🀊"); break;
                    case 5 : Console.Write("🀋"); break;
                    case 6 : Console.Write("🀌"); break;
                    case 7 : Console.Write("🀍"); break;
                    case 8 : Console.Write("🀎"); break;
                    case 9 : Console.Write("🀏"); break;
                    default: Console.Write("😱"); break;
                } 
            }
            if (tile.type == TileType.Sak)
            {
                switch (tile.tileNumber)
                {
                    case 1 : Console.Write("🀐"); break;
                    case 2 : Console.Write("🀑"); break;
                    case 3 : Console.Write("🀒"); break;
                    case 4 : Console.Write("🀓"); break;
                    case 5 : Console.Write("🀔"); break;
                    case 6 : Console.Write("🀕"); break;
                    case 7 : Console.Write("🀖"); break;
                    case 8 : Console.Write("🀗"); break;
                    case 9 : Console.Write("🀘"); break;
                    default: Console.Write("😱"); break;
                } 
            }
            if (tile.type == TileType.Tong)
            {
                switch (tile.tileNumber)
                {
                    case 1 : Console.Write("🀙"); break;
                    case 2 : Console.Write("🀚"); break;
                    case 3 : Console.Write("🀛"); break;
                    case 4 : Console.Write("🀜"); break;
                    case 5 : Console.Write("🀝"); break;
                    case 6 : Console.Write("🀞"); break;
                    case 7 : Console.Write("🀟"); break;
                    case 8 : Console.Write("🀠"); break;
                    case 9 : Console.Write("🀡"); break;
                    default: Console.Write("😱"); break;
                } 
            }
            if (tile.type == TileType.Wind)
            {
                switch (tile.tileNumber)
                {
                    case (int)Winds.East : Console.Write("🀀"); break;
                    case (int)Winds.South : Console.Write("🀁"); break;
                    case (int)Winds.West : Console.Write("🀂"); break;
                    case (int)Winds.North : Console.Write("🀃"); break;
                    default: Console.Write("😱"); break;                    
                }
            }
            if (tile.type == TileType.Word)
            {
                switch (tile.tileNumber)
                {
                    case (int)Words.Blank : Console.Write("🀆"); break;
                    case (int)Words.Start : Console.Write("🀅"); break;
                    case (int)Words.Middle : Console.Write("🀄︎"); break;
                    default: Console.Write("😱"); break;
                }
            }

            Console.Write(" ");
            
            if (tile.isDora)
            {
                Console.ResetColor();
            }  
        }

        public static void PrintDeck(Tile[] tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                PrintTile(tiles[i]);
            }
        }

        public static bool IsNumberTiles(TileType type)
        {
            return (int)type < (int)TileType.Wind;
        }
    }
}