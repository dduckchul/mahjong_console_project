using System;
using System.Collections.Generic;

namespace Mahjong
{
    public class Deck
    {
        public const int DistributedTiles = 52;
        public const int PublicTiles = 70;
        public const int MaxDoraTiles = 5;
        public const int MaxYungsangTiles = 4;
        public const int MaxMahjongTiles = 136;

        public class PublicDeck
        {
            private Stack<Tiles.Tile> _publicStack;
            private Tiles.Tile[] _doraTiles;
            private Tiles.Tile[] _uraDoraTiles;
            private Tiles.Tile[] _yungsangTiles;
            private int _currentTileIndex;
            private int _currentDoraIndex;

            // 신규 덱 초기 생성자
            public PublicDeck(Tiles.Tile[] pileOfTiles)
            {
                PublicStack = new Stack<Tiles.Tile>();
                DoraTiles = new Tiles.Tile[MaxDoraTiles];
                UraDoraTiles = new Tiles.Tile[MaxDoraTiles];
                YungSangTiles = new Tiles.Tile[MaxYungsangTiles];

                if (pileOfTiles != null && pileOfTiles.Length > 0)
                {
                    foreach (Tiles.Tile t in pileOfTiles)
                    {
                        if (Tiles.IsValidTile(t))
                        {
                            PublicStack.Push(t);                           
                        }
                    }
                }
            }
            
            public Stack<Tiles.Tile> PublicStack
            {
                get { return _publicStack; }
                private set { _publicStack = value; }
            }

            public Tiles.Tile[] DoraTiles
            {
                get { return _doraTiles; }
                private set { _doraTiles = value; }
            }
            
            public Tiles.Tile[] UraDoraTiles
            {
                get { return _uraDoraTiles; }
                private set { _uraDoraTiles = value; }
            }

            public Tiles.Tile[] YungSangTiles
            {
                get { return _yungsangTiles; }
                private set { _yungsangTiles = value; }
            }

            public int CurrentTileIndex
            {
                get { return _currentTileIndex; }
                private set { _currentTileIndex = value; }
            }

            public int CurrentDoraIndex
            {
                get { return _currentDoraIndex; }
                private set { _currentDoraIndex = value; }
            }
            
            // 초기 덱 생성
            public void MakePublicDeck()
            {
                for (int i = 0; i < DoraTiles.Length; i++)
                {
                    DoraTiles[i] = PublicStack.Pop();
                }

                for (int j = 0; j < UraDoraTiles.Length; j++)
                {
                    UraDoraTiles[j] = PublicStack.Pop();
                }

                for (int k = 0; k < YungSangTiles.Length; k++)
                {
                    YungSangTiles[k] = PublicStack.Pop();
                }
            }            
            
            // 덱의 도라를 0번으로 초기화하고, 나머지 도라들을 뒤집은 상태로 둔다
            public void InitDora()
            {
                CurrentDoraIndex = 0;
                int doraInx = CurrentDoraIndex;

                for (int i = 0; i < DoraTiles.Length; i++)
                {
                    if (doraInx == i)
                    {
                        DoraTiles[i].isShowingFront = true;                    
                    }
                    DoraTiles[i].isVisible = true;
                }
            }
        
            // 도라 타일 더 열어야 할때
            public void OpenDora()
            {
                int doraInx = CurrentDoraIndex++;
                DoraTiles[doraInx].isVisible = true;
                DoraTiles[doraInx].isShowingFront = true;
            }            
        }

        public class Hands
        {
            private Tiles.Tile[] _myTiles;
            private Tiles.Tile[] _discards;
            private Tiles.Tile _temp;
            private List<Tiles.Tile[]> _openedBodies;
            
            public Tiles.Tile[] MyTiles
            {
                get { return _myTiles; }
                set { _myTiles = value; }
            }

            public Tiles.Tile[] Discards
            {
                get { return _discards; }
                set { _discards = value; }
            }

            public Tiles.Tile Temp
            {
                get { return _temp; }
                set { _temp = value; }
            }

            public List<Tiles.Tile[]> OpenedBodies
            {
                get { return _openedBodies; }
                set { _openedBodies = value; }
            }
            
            // 1. Sort By Type,
            // 2. Sort By Number
            public void SortMyHand()
            {
                // 단순하게 하면.. Type 으로 정렬, Number 로 정렬 이중포문 두번
                for (int i = 0; i < MyTiles.Length-1; i++)
                {
                    for (int j = i + 1; j < MyTiles.Length-1; j++)
                    {
                        int myTilesType = (int)MyTiles[i].type;
                        int nextTilesType = (int)MyTiles[j].type;
                        if (myTilesType > nextTilesType)
                        {
                            Tiles.Tile temp = MyTiles[j];
                            MyTiles[j] = MyTiles[i];
                            MyTiles[i] = temp;
                        }
                    }
                }
            
                // myTile 변경될때 인덱스를 기억해뒀다가 다시 정렬
                for (int i = 0; i < MyTiles.Length-1; i++)
                {
                    for (int j = i + 1; j < MyTiles.Length-1; j++)
                    {
                        int myTileNumber = MyTiles[i].tileNumber;
                        int nextTileNumber = MyTiles[j].tileNumber;
                        int myTilesType = (int)MyTiles[i].type;
                        int nextTilesType = (int)MyTiles[j].type;

                        if (myTilesType != nextTilesType)
                        {
                            continue;
                        }

                        if (myTileNumber > nextTileNumber)
                        {
                            Tiles.Tile temp = MyTiles[j];
                            MyTiles[j] = MyTiles[i];
                            MyTiles[i] = temp;                        
                        }
                    }
                }
            }            
        }
        
        // 마작 타일 생성 후 초기화 (136개)
        public static Tiles.Tile[] MakeInitDeck()
        {
            Tiles.Tile[] tiles = new Tiles.Tile[MaxMahjongTiles];

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
                    
                    if (i == 0 && Tiles.IsNumberTile(type))
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

        private static Tiles.Tile[] MakeTypeTiles(Tiles.TileType type, bool makeRedDora)
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
            
            if (Tiles.IsNumberTile(type))
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
                if (Tiles.IsNumberTile(type))
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
        public static void ShuffleDeck(Tiles.Tile[] tiles)
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