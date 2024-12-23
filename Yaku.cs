namespace Mahjong
{
    public class Yaku
    {

        public bool IsDeckHasHead(Tiles.Tile[] tiles)
        {
            for (int i = 0; i < tiles.Length -1 ; i++)
            {
                if (tiles[i].Number == tiles[i + 1].Number)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsDeckHasBody(Tiles.Tile[] tiles)
        {
            return false;
        }
        
        public int CountBodies(Tiles.Tile[] tiles)
        {
            return 0;
        }

        // 텐파이 조건..? 헤드가 0개이거나, 몸통이 4개 or 헤드가 1개, 몸통이 3개
        public bool IsTenpai(Player p)
        {
            bool hasHead = IsDeckHasHead(p.Hands.MyTiles);
            int bodies = CountBodies(p.Hands.MyTiles);

            if (hasHead && bodies == 3 || hasHead == false && bodies == 4)
            {
                return true;
            }

            // 그외 다른 역들도 여기에 다 추가해줘~~~~~
            return false;
        }
    }
}