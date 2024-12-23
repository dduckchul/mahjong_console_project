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
            East,South,West,North,End
        }
        public enum Words
        {
            Blank,Start,Middle,End
        }
        public struct Tile
        {
            public TileType type;
            public int tileNumber;
            public bool isDora;
            public bool isShowingFront;
            public bool isVisible;
        }

        public static void PrintTile(Tile tile)
        {
            if (!tile.isVisible)
            {
                return;
            }
            
            if (!tile.isShowingFront)
            {
                Console.Write("🀫");
                Console.Write(" ");
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

            // 중 타일 하나 띄고 나오는거 거슬려서 예외처리
            if (!(tile.type == TileType.Word && tile.tileNumber == (int)Words.Middle))
            {
                Console.Write(" ");
            }
            
            // 도라 색상 변경 후 리셋 버그 예외 처리
            if (Console.BackgroundColor == ConsoleColor.DarkGreen && tile.isDora)
            {
                Console.ResetColor();
                // Console.BackgroundColor = ConsoleColor.DarkGreen;
            }
            else if (tile.isDora)
            {
                Console.ResetColor();                
            }
        }

        // To-Do 마지막 버린 타일을 색 변경해서 보여주고싶다
        public static void PrintDeck(Tile[] tiles)
        {
            // Console.BackgroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < tiles.Length; i++)
            {
                PrintTile(tiles[i]);
            }
            Console.ResetColor();
        }

        // 자패 (풍패나 역패)인지 확인
        public static bool IsNumberTile(TileType type)
        {
            return (int)type < (int)TileType.Wind;
        }

        // 노두패 (숫자패인데 1이나 9)
        public static bool IsNumberOneOrNine(Tile tile)
        {
            if (!IsNumberTile(tile.type))
            {
                return false;
            }
            
            if (tile.tileNumber > 1 && tile.tileNumber < 9)
            {
                return false;
            }

            return true;
        }

        // 자패 이거나, 숫자가 1이나 9인 타일
        public static bool IsYoguTile(Tile tile)
        {
            if (!IsNumberTile(tile.type))
            {
                return true;
            }
            
            if(IsNumberOneOrNine(tile))
            {
                return true;
            }
            
            return false;
        }

        // 유효한 타일인지 (타일이 Man, 0이면 이상하게 초기화된 타일임)
        public static bool IsValidTile(Tile tile)
        {
            if (tile.type == TileType.Man && tile.tileNumber == 0)
            {
                return false;
            }

            return true;
        }
    }
}