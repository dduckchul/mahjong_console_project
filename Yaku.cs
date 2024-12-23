namespace Mahjong
{
    public class Yaku
    {

        public bool isDeckHasHead(Tiles.Tile[] tiles)
        {
            for (int i = 0; i < tiles.Length -1 ; i++)
            {
                if (tiles[i].tileNumber == tiles[i + 1].tileNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isDeckHasBody(Tiles.Tile[] tiles)
        {
            return false;
        }
        
        public int countBodies(Tiles.Tile[] tiles)
        {
            return 0;
        }

        // 텐파이 조건..? 헤드가 0개이거나, 몸통이 4개 or 헤드가 1개, 몸통이 3개
        public bool isTenpai(Player p)
        {
            bool hasHead = isDeckHasHead(p.Hands.MyTiles);
            int bodies = countBodies(p.Hands.MyTiles);

            if (hasHead && bodies == 3 || hasHead == false && bodies == 4)
            {
                return true;
            }

            // 그외 다른 역들도 여기에 다 추가해줘~~~~~
            
            return false;
        }
    }
}