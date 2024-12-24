namespace Mahjong
{
    public class Yaku
    {

        public bool IsDeckHasHead(Deck.Hands hands)
        {
            for (int i = 0; i < hands.MyTiles.Length -1 ; i++)
            {
                if (hands.MyTiles[i].Number == hands.MyTiles[i + 1].Number)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDeckHasBody(Deck.Hands hands)
        {
            return false;
        }
        
        public int CountBodies(Deck.Hands hands)
        {
            return 0;
        }

        // 텐파이 조건..? 헤드가 0개이거나, 몸통이 4개 or 헤드가 1개, 몸통이 3개
        public bool IsTenpai(Player p)
        {
            bool hasHead = IsDeckHasHead(p.Hands);
            int bodies = CountBodies(p.Hands);

            if (hasHead && bodies == 3 || hasHead == false && bodies == 4)
            {
                return true;
            }

            // 그외 다른 역들도 여기에 다 추가해줘~~~~~
            return false;
        }
    }
}