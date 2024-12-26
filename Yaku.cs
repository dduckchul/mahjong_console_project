using System;
using System.Collections.Generic;

namespace Mahjong
{
    public static class Yaku
    {

        public static bool IsDeckHasHead(Deck.Hands hands)
        {
            for (int i = 0; i < hands.MyTiles.Count -1 ; i++)
            {
                if (hands.MyTiles[i].Number == hands.MyTiles[i + 1].Number)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDeckHasBody(Deck.Hands hands)
        {
            return false;
        }
        
        public static int CountBodies(Deck.Hands hands)
        {
            return 0;
        }

        public static bool CanRiichi(Player p)
        {
            if (p.IsCrying)
            {
                Console.WriteLine("울기 상태에서는 리치 할 수 없습니다");
                return false;
            }

            if (p.IsRiichi)
            {
                Console.WriteLine("이미 리치입니다");
                return false;
            }

            return IsTenpai(p);
        }

        // 텐파이 조건..? 헤드가 0개이거나, 몸통이 4개 or 헤드가 1개, 몸통이 3개
        public static bool IsTenpai(Player p)
        {
            bool hasHead = IsDeckHasHead(p.Hands);
            int bodies = CountBodies(p.Hands);

            if (hasHead && bodies == 3 || hasHead == false && bodies == 4)
            {
                return true;
            }
            
            // 14개 한번 정렬
            p.Hands.SortMyHand(Player.MaxHandTiles);

            //
            List<Tiles.Tile> tempList = new List<Tiles.Tile>();
            
            
            // 그외 다른 역들도 여기에 다 추가해줘~~~~~
            return false;
        }

        public static bool hasTriple(List<Tiles.Tile> tempList)
        {
            return false;
        }

        public static bool hasSequential(List<Tiles.Tile> tempList)
        {
            return false;
        }
    }
}