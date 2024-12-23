using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mahjong
{
    public class Player
    {
        // 마작 기본 점수
        public const int DefaultScore = 25000;
        // 같이 마작 할사람 ㅠㅠ 4명이 있어야만 진행됨....
        public const int MaxPlayers = 4;
        public const int MaxHandTiles = 14;
        public const int MaxDiscardTiles = 30;
        
        // 플레이어 정보
        private int _score;
        private bool _isHuman;
        
        // 진행중인 게임 정보
        private bool _isPlaying;
        private bool _isRiichi;
        private bool _isCrying;
        
        private Game.Winds _wind;
        private Tiles.Tile[] _hands;
        private Tiles.Tile[] _discards;
        private Tiles.Tile _temp;
        private List<Tiles.Tile[]> _openedBodies;       

        public String Name
        {
            get; set;
        }

        public int Score
        {
            get { return _score; }
            private set { _score = value; }
        }

        public bool IsHuman
        {
            get { return _isHuman; }
            set { _isHuman = value; }
        }
        
        public Game.Winds Wind
        {
            get { return _wind; }
            set { _wind = value; }
        }        

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; }
        }

        public bool IsRiichi
        {
            get { return _isRiichi; }
            private set { _isRiichi = value; }
        }

        public bool IsCrying
        {
            get { return _isCrying; }
            private set { _isCrying = value; }
        }

        public Tiles.Tile[] Hands
        {
            get { return _hands; }
            set { _hands = value; }
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

        // 나는 초기화 했다고 가정, cpu 플레이어 생성해주기
        public Player[] InitPlayers()
        {
            // TO-DO : 실제 작동할때는 이름 입력받도록
            // Players.Player me = players.SetMyAvata("");
            Player me = SetMyAvata("저에요");            
            Player[] players = new Player[MaxPlayers]
            {
                me, new Player(), new Player(), new Player()
            };

            string[] cpuName = {"암거나", "알파고", "오픈AI", "잼민이"};
            
            // 0번에 나를 넣었음
            for (int i = 1; i < MaxPlayers; i++)
            {
                // cpu는 남(=1) 부터 적용
                players[i].Wind = (Game.Winds)Enum.Parse(typeof(Game.Winds), i.ToString());
                players[i].Name = cpuName[i];
                players[i].Score = DefaultScore;
            }
            return players;
        }
        
        // 빈칸으로 두면 입력창 받도록, 귀찮아서 이름 넘김
        public Player SetMyAvata(string playerName)
        {
            Console.WriteLine("당신의 이름을 입력해 주세요 🤔");
            
            if (playerName == "")
            {
                playerName = Console.ReadLine();
            }

            Console.WriteLine($"안녕하세요~ {playerName}님");

            Player me = new Player();
            me.Name = playerName;
            me.Score = DefaultScore;
            me.Wind = Game.Winds.East;
            me.IsHuman = true;
            
            return me;
        }
        
        // 플레이어가 number 개 만큼 타일 가져가기
        // 따로 넘버로 나눈 이유는 뿌려주는거 애니메이션처럼 할려고 ㅎㅎㅎㅎㅎㅎㅎ..
        public void TakeTiles(Tiles.Tile[] pileOfTiles, int number, int index)
        {
            if (Hands == null)
            {
                Hands = new Tiles.Tile[MaxHandTiles];
            }

            // 1개씩 더미에서 내 핸드로 가져오기            
            for (int i = 0; i < number; i++)
            {
                int ind = index + i;

                // 타일 잠시 저장해두는 변수
                Tiles.Tile tile = pileOfTiles[ind];
                for (int j = 0; j < Hands.Length; j++)
                {
                    tile.isVisible = true;
                    
                    if (IsHuman)
                    {
                        tile.isShowingFront = true;
                    }
                    
                    // 비어있는거 확인하려고 숫자 비교했는데 여기서 이상해짐
                    if (Tiles.IsValidTile(Hands[j]) == false)
                    {
                        Hands[j] = tile;
                        break;
                    }
                }            
            }
        }

        // C# 콘솔창 오른쪽 텍스트 출력
        // https://stackoverflow.com/questions/6913563/c-sharp-align-text-right-in-console
        public void PrintPlayer()
        {
            PrintPlayerInfo();
            PrintPlayerHand();
            PrintPlayerTemp();
            Console.WriteLine();
            PrintPlayerDiscards();
            Console.WriteLine("\n");
        }

        private void PrintPlayerInfo()
        {
            Console.Write(Name+"\t");
            
            if (IsHuman)
            {
                Console.Write("👤");
            }
            else
            {
                Console.Write("💻");
            }

            switch (Wind)
            {
                case Game.Winds.East :
                    Console.Write("🀀\t"); break;
                case Game.Winds.South :
                    Console.Write("🀁\t"); break;
                case Game.Winds.West :
                    Console.Write("🀂\t"); break;
                case Game.Winds.North :
                    Console.Write("🀃\t"); break;
                default : Console.Write("😱\t"); break;
            }            
            
            Console.Write(Score+"\t");
            
            if (IsRiichi)
            {
                Console.Write("🛑\t");                
            }
            else
            {
                Console.Write("\t");
            }

            if (IsPlaying)
            {
                Console.Write("🤯");
            }
            
            Console.WriteLine("");
        }
        private void PrintPlayerHand()
        {
            Console.Write("덱\t:\t");
            Tiles.PrintDeck(Hands);
        }

        private void PrintPlayerTemp()
        {
            if (Tiles.IsValidTile(Temp))
            {
                Console.Write("\t\t");
                // Console.BackgroundColor = ConsoleColor.DarkGreen;
                Tiles.PrintTile(Temp);
                Console.ResetColor();
                Console.Write("🤏");                
            }
        }

        private void PrintPlayerDiscards()
        {
            Console.Write("🗑️\t:\t");
            Tiles.PrintDeck(Discards);
        }
        
        // 공용 덱에서 하나 타일을 뽑는다.
        public Tiles.Tile Tsumo(Deck.PublicDeck publicDeck)
        {
            if (publicDeck.currentTileIndex < 0 || publicDeck.currentTileIndex > Deck.MahjongMaxTiles)
            {
                Console.WriteLine("쯔모시 뭔가 잘못되었습니다 😱");
            }
            
            return publicDeck.publicTiles[publicDeck.currentTileIndex++];
        }

        public void UserAddTempAndDiscardTile()
        {
            Console.Clear();
            
            // 핸드에 temp 더하기
            Hands[MaxHandTiles - 1] = Temp;

            // 핸드 정렬
            Console.WriteLine("버릴 타일을 선택 해 주세요\n");
            Tiles.PrintDeck(Hands);
            Console.Write("\n0 1 2 3 4 5 6 7 8 9 A B C D\n");
            
            ConsoleKeyInfo keyInfo;
            bool parseResult = false;
            int keyInt = 0;
            
            // 스트링 -> 16진수 변환하기
            // https://stackoverflow.com/questions/98559/how-to-parse-hex-values-into-a-uint            
            while (!parseResult)
            {
                keyInfo = Console.ReadKey();
                char key = keyInfo.KeyChar;
                parseResult = int.TryParse(key.ToString(), 
                    NumberStyles.HexNumber, CultureInfo.CurrentCulture, out keyInt);
                if (parseResult)
                {
                    Program.WaitUntilElapsedTime(200);
                    Console.WriteLine(" 선택한 숫자 : " + keyInt);
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 키를 입력하셨습니다");
                }                
            }
            DiscardTile(keyInt);
        }
        
        // 컴퓨터가 하는 행동
        // To-Do : 더 업그레이드 하면 좋겠지만 그냥 랜덤으로 뽑아서 버리자
        public void AiAddTempAndDiscardTile()
        {
            // 핸드에 temp 더하기
            Hands[MaxHandTiles - 1] = Temp;            
            Random rand = new Random();
            DiscardTile(rand.Next(0,MaxHandTiles));
        }
        
        // 선택한 타일 Discard 핸드에 넣고 버리기
        // 정렬을 맨뒤가 하나 비어있는걸로 가정했기 때문에, 강제로 빈걸로 맨 뒤로 넣어준다.
        // 버림패는 무조건 공개
        public void DiscardTile(int keyInt)
        {
            Tiles.Tile discard = Hands[keyInt];
            discard.isShowingFront = true;
            Hands[keyInt] = Temp;
            Hands[MaxHandTiles - 1] = new Tiles.Tile();

            int lastDiscard = FindLastDiscardInx();
            Discards[lastDiscard] = discard;
            Temp = new Tiles.Tile();
            
            Deck.SortMyHand(this);
        }

        // 비어있는 공간 찾기
        public int FindLastDiscardInx()
        {
            for (int i = 0; i < Discards.Length; i++)
            {
                if (!Tiles.IsValidTile(Discards[i]))
                {
                    return i;
                }
            }
            return -1;
        }
        
        public void PressKeyAndAction()
        {
            // 기능 구현중 or 잘못된 키 판별 하는 변수
            bool isFalseKey = true;
            ConsoleKeyInfo keyInfo;
            
            while (isFalseKey)
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.D1)
                {
                    isFalseKey = false;
                    UserAddTempAndDiscardTile();
                    Program.WaitUntilElapsedTime(200);
                } else if (keyInfo.Key == ConsoleKey.D2)
                {
                    Console.WriteLine("🚜👷리치 구현중....⛏️");
                    Program.WaitUntilElapsedTime(200);
                } else if (keyInfo.Key == ConsoleKey.D3)
                {
                    Console.WriteLine("🚜👷쯔모 구현중....⛏️");
                    Program.WaitUntilElapsedTime(200);
                } else if (keyInfo.Key == ConsoleKey.D4)
                {
                    Console.WriteLine("🚜👷깡 구현중....⛏️");
                    Program.WaitUntilElapsedTime(200);                    
                } else if (keyInfo.Key == ConsoleKey.D0)
                {
                    Program.IsRunning = false;
                    isFalseKey = false;
                    Console.WriteLine("게임을 종료합니다..👋");
                    Program.WaitUntilElapsedTime(200);
                }
                else
                {
                    Console.WriteLine("잘못된 키입니다.");
                }                
            }
        }        
        
        public void ComputerAction()
        {
            AiAddTempAndDiscardTile();
        }                
    }
}