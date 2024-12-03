using System;

namespace Mahjong
{
    public class Deck
    {
        struct PublicDeck
        {
            // 같이 쯔모하는 패산
            private Tiles[] openTiles;
            // 도라 계산시에 필요한 타일들
            private Tiles[] doraTiles;
            // 리치시 계산시에 필요한 뒷도라 타일들
            private Tiles[] uraDoraTiles;
            // 깡 했을때 가져오는 영상패
            private Tiles[] yungsangTiles;
            // 현재 덱에서 몇번 인덱스의 타일을 쓰는지 기록
            private int currentTileIndex;
            // 현재 덱에서 몇번 인덱스 까지 도라가 나왔는지 확인
            private int currentDoraTileIndex;
        }
        
        // 마작 타일 생성 후 초기화 (136개)
        public Tiles.Tile[] MakeInitDeck()
        {
            Tiles.Tile[] tiles = new Tiles.Tile[136];

            int tileMultiplyNum = 4;
            int numberToMakeType = (int)Tiles.TileType.End;
            int prevTilesNum = 0;
            
            for (int i = 0; i < tileMultiplyNum; i++)
            {
                for (int j = 0; j < numberToMakeType; j++)
                {
                    Tiles.Tile[] newTiles;
                    Tiles.TileType type;
                    
                    bool isSomethingParsed = Enum.TryParse(j.ToString(), out type);
                    
                    if (!isSomethingParsed)
                    {
                        Console.WriteLine("뭔가가 잘못되었습니다.. 확인해주세요");
                        Console.WriteLine($"j : {j}, type:{type}");
                        continue;
                    }
                    
                    if (i == 0 && Tiles.IsNumberTiles(type))
                    {
                        newTiles = MakeTypeTiles(type, true);                        
                    }
                    else
                    {
                        newTiles = MakeTypeTiles(type, false);
                    }
                    
                    for (int k = 0; k < newTiles.Length; k++)
                    {
                        int tilesIndex = k + prevTilesNum;
                        tiles[tilesIndex] = newTiles[k];
                    }
                    
                    prevTilesNum += newTiles.Length;
                }
            }
            
            return tiles;
        }

        private Tiles.Tile[] MakeTypeTiles(Tiles.TileType type, bool makeRedDora)
        {
            int tilesMaxInt = 0;
            
            if (type == Tiles.TileType.Wind)
            {
                tilesMaxInt = 4;
            } 
            
            if (type == Tiles.TileType.Word)
            {
                tilesMaxInt = 3;
            }
            
            if (Tiles.IsNumberTiles(type))
            {
                tilesMaxInt = 9;
            }

            if (tilesMaxInt == 0)
            {
                Console.WriteLine($"{type} 타일 생성이 제대로 되지 않았습니다");
            }
            
            Tiles.Tile[] tiles = new Tiles.Tile[tilesMaxInt];
            
            for (int i = 0; i < tilesMaxInt; i++)
            {
                int tileNum = i + 1;
                Tiles.Tile tile = new Tiles.Tile();
                tile.type = type;
                tile.tileNumber = tileNum;
                if (tileNum == 5 && makeRedDora)
                {
                    tile.isDora = true;
                }
                tiles[i] = tile;
            }

            return tiles;
        }

        // Fisher–Yates shuffle
        // https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
        public void ShuffleDeck(ref Tiles.Tile[] tiles)
        {
            int n = tiles.Length;
            Random random = new Random();
            while (n > 1)
            {
                int k = random.Next(n--);
                Tiles.Tile temp = tiles[n];
                tiles[n] = tiles[k];
                tiles[k] = temp;
            }
        }
    }
}