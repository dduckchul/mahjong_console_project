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
            East,South,West,North
        }
        public enum Words
        {
            Blank,Start,Middle
        }
        public struct Tile
        {
            private readonly TileType _type;
            private readonly int _number;
            private readonly bool _isDora;

            public TileType Type
            {
                get { return _type; }
            }

            public int Number
            {
                get { return _number; }
            }

            public bool IsDora
            {
                get { return _isDora; }
            }

            public bool IsShowingFront
            {
                get;
                set;
            }

            public bool IsVisible
            {
                get;
                set;
            }

            public Tile(TileType type, int number, bool isDora)
            {
                _type = type;
                _number = number;
                _isDora = isDora;
                IsShowingFront = false;
                IsVisible = false;
            }
            
            public void PrintTile()
            {
                if (!IsVisible) { return; }
                if (!IsShowingFront)
                {
                    Console.Write("🀫 ");
                    return;
                }
                
                if (IsDora)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }

                if (Type == TileType.Man) { PrintMan(); }
                if (Type == TileType.Sak) { PrintSak(); }
                if (Type == TileType.Tong) { PrintTong(); }
                if (Type == TileType.Wind) { PrintWind(); }
                if (Type == TileType.Word) { PrintWord(); }

                // 중 타일 하나 띄고 나오는거 거슬려서 예외처리
                if (!(Type == TileType.Word && Number == (int)Words.Middle))
                {
                    Console.Write(" ");
                }
                
                if (IsDora)
                {
                    Console.ResetColor();                
                }
            }

            private void PrintMan()
            {
                switch (Number)
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

            private void PrintSak()
            {
                switch (Number)
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

            private void PrintTong()
            {
                switch (Number)
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

            private void PrintWind()
            {
                switch (Number)
                {
                    case (int)Winds.East : Console.Write("🀀"); break;
                    case (int)Winds.South : Console.Write("🀁"); break;
                    case (int)Winds.West : Console.Write("🀂"); break;
                    case (int)Winds.North : Console.Write("🀃"); break;
                    default: Console.Write("😱"); break;                    
                }                
            }

            private void PrintWord()
            {
                switch (Number)
                {
                    case (int)Words.Blank : Console.Write("🀆"); break;
                    case (int)Words.Start : Console.Write("🀅"); break;
                    case (int)Words.Middle : Console.Write("🀄︎"); break;
                    default: Console.Write("😱"); break;
                }                
            }
            
            // 노두패 (숫자패인데 1이나 9)
            public bool IsNumberOneOrNine()
            {
                if (!IsNumberTile())
                {
                    return false;
                }
            
                return Number == 1 || Number == 9;
            }
            
            // 자패 (풍패나 역패)인지 확인
            public bool IsNumberTile()
            {
                return (int)Type < (int)TileType.Wind;
            }
            
            // 자패 이거나, 숫자가 1이나 9인 타일
            public bool IsYoguTile()
            {
                if (!IsNumberTile())
                {
                    return true;
                }
            
                return IsNumberOneOrNine();
            }
            
            // 유효한 타일인지 (타일이 Man, 0이면 이상하게 초기화된 타일임)
            public bool IsValidTile()
            {
                if (Type == TileType.Man && Number == 0)
                {
                    return false;
                }

                return true;
            }            
        }
        
        // 해당 타입이 숫자 타일인지 체크하는 유틸 메서드
        public static bool IsNumberType(TileType type)
        {
            return (int)type < (int)TileType.Wind;
        }
        
        // To-Do 마지막 버린 타일을 색 변경해서 보여주고싶다
        public static void PrintDeck(Tile[] tiles)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].PrintTile();
            }
            Console.ResetColor();
        }
    }
}