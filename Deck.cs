using System;

namespace Mahjong
{
    public class Deck
    {
        public const int DistributedTiles = 52;

        public const int PublicTiles = 70;
        public const int DoraTiles = 5;
        public const int UraDoraTiles = 5;
        public const int YungsangTiles = 4;
        public const int MahjongMaxTiles = 136;

        public struct PublicDeck
        {
            // 같이 쯔모하는 패산
            public Tiles.Tile[] publicTiles;
            // 도라 계산시에 필요한 타일들
            public Tiles.Tile[] doraTiles;
            // 리치시 계산시에 필요한 뒷도라 타일들
            public Tiles.Tile[] uraDoraTiles;
            // 깡 했을때 가져오는 영상패
            public Tiles.Tile[] yungsangTiles;
            // 현재 덱에서 몇번 인덱스의 타일을 쓰는지 기록
            public int currentTileIndex;
            // 현재 덱에서 몇번 인덱스 까지 도라가 나왔는지 확인
            public int currentDoraTileIndex;
        }
        
        // 마작 타일 생성 후 초기화 (136개)
        public Tiles.Tile[] MakeInitDeck()
        {
            Tiles.Tile[] tiles = new Tiles.Tile[MahjongMaxTiles];

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
                int tileNum = i;
                if (Tiles.IsNumberTiles(type))
                {
                    tileNum = i + 1;
                }
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
        public void ShuffleDeck(Tiles.Tile[] tiles)
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

        public PublicDeck MakePublicDeck(Tiles.Tile[] tiles)
        {
            PublicDeck publicDeck = new PublicDeck();
            publicDeck.publicTiles = new Tiles.Tile[PublicTiles];
            publicDeck.doraTiles = new Tiles.Tile[DoraTiles];
            publicDeck.uraDoraTiles = new Tiles.Tile[UraDoraTiles];
            publicDeck.yungsangTiles = new Tiles.Tile[YungsangTiles];
            
            for (int i = 0; i < PublicTiles; i++)
            {
                int deckIndex = DistributedTiles + i;
                publicDeck.publicTiles[i] = tiles[deckIndex];
            }

            for (int j = 0; j < DoraTiles; j++)
            {
                int deckIndex = DistributedTiles + PublicTiles + j;
                publicDeck.doraTiles[j] = tiles[deckIndex];                
            }

            for (int k = 0; k < UraDoraTiles; k++)
            {
                int deckIndex = DistributedTiles + PublicTiles + DoraTiles + k;
                publicDeck.uraDoraTiles[k] = tiles[deckIndex];
            }

            for (int l = 0; l < YungsangTiles; l++)
            {
                int deckIndex = DistributedTiles + PublicTiles + DoraTiles + UraDoraTiles + l;
                publicDeck.yungsangTiles[l] = tiles[deckIndex];
            }
            
            return publicDeck;
        }

        // 1. Sort By Type,
        // 2. Sort By Number
        public static void SortMyHand(Players.Player player)
        {
            Tiles.Tile[] myHands = player.hands;
            // 단순하게 하면.. Type 으로 정렬, Number 로 정렬 이중포문 두번
            for (int i = 0; i < myHands.Length-1; i++)
            {
                for (int j = i + 1; j < myHands.Length-1; j++)
                {
                    int myTilesType = (int)myHands[i].type;
                    int nextTilesType = (int)myHands[j].type;
                    if (myTilesType > nextTilesType)
                    {
                        Tiles.Tile temp = myHands[j];
                        myHands[j] = myHands[i];
                        myHands[i] = temp;
                    }
                }
            }
            
            // myTile 변경될때 인덱스를 기억해뒀다가 다시 정렬
            for (int i = 0; i < myHands.Length-1; i++)
            {
                for (int j = i + 1; j < myHands.Length-1; j++)
                {
                    int myTileNumber = myHands[i].tileNumber;
                    int nextTileNumber = myHands[j].tileNumber;
                    int myTilesType = (int)myHands[i].type;
                    int nextTilesType = (int)myHands[j].type;

                    if (myTilesType != nextTilesType)
                    {
                        continue;
                    }

                    if (myTileNumber > nextTileNumber)
                    {
                        Tiles.Tile temp = myHands[j];
                        myHands[j] = myHands[i];
                        myHands[i] = temp;                        
                    }
                }
            }
        } 
    }
}