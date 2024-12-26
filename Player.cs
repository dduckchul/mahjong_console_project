using System;
using System.Collections.Generic;
using System.Globalization;

namespace Mahjong
{
    public class Player : IEquatable<Player>
    {
        // 마작 기본 점수
        public const int DefaultScore = 25000;
        // 같이 마작 할사람 ㅠㅠ 4명이 있어야만 진행됨....
        public const int MaxPlayers = 4;
        public const int MaxHandTiles = 14;
        
        // 플레이어 정보
        private int _score;
        
        // 플레이어의 현재 진행중 게임 정보
        private bool _isPlaying;
        private bool _isRiichi;
        private bool _isCrying;
        private Game.Winds _wind;
        private Deck.Hands _hands;

        public Player(string name, Game.Winds wind)
        {
            Name = name;
            Score = DefaultScore;
            Wind = wind;
            Hands = new Deck.Hands();
        }
        public String Name
        {
            get; set;
        }

        public int Score
        {
            get { return _score; }
            protected set { _score = value; }
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
            protected set { _isRiichi = value; }
        }

        public bool IsCrying
        {
            get { return _isCrying; }
            protected set { _isCrying = value; }
        }

        public Deck.Hands Hands
        {
            get { return _hands; }
            private set { _hands = value; }
        }

        // 나는 초기화 했다고 가정, cpu 플레이어 생성해주기
        public static Player[] InitPlayers()
        {
            // TO-DO : 실제 작동할때는 이름 입력받도록
            // Players.Player me = players.SetMyAvata("");
            string[] cpuName = {"암거나", "알파고", "오픈AI", "잼민이"};            
            Player[] players = new Player[MaxPlayers];
            players[0] = Human.SetMyAvata("저에요");
            
            // 0번에 나를 넣었음
            for (int i = 1; i < MaxPlayers; i++)
            {
                Game.Winds winds = (Game.Winds)Enum.Parse(typeof(Game.Winds), i.ToString());
                players[i] = new Cpu(cpuName[i], winds);
            }
            return players;
        }
        
        // 플레이어가 number 개 만큼 타일 가져가기
        // 따로 넘버로 나눈 이유는 뿌려주는거 애니메이션처럼 할려고 ㅎㅎㅎㅎㅎㅎㅎ..
        public void TakeTiles(Stack<Tiles.Tile> publicStack, int number)
        {
            // 1개씩 더미에서 내 핸드로 가져오기            
            for (int i = 0; i < number; i++)
            {
                // 타일 잠시 저장해두는 변수
                Tiles.Tile tile = publicStack.Pop();
                tile.IsVisible = true;
                if (this is Human)
                {
                    tile.IsShowingFront = true;
                }
                Hands.MyTiles.Add(tile);
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
            
            if (this is Human)
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
            Tiles.PrintDeck(Hands.MyTiles);
        }

        private void PrintPlayerTemp()
        {
            if (Hands.Temp.IsValidTile())
            {
                Console.Write("\t\t");
                Hands.Temp.PrintTile();
                Console.ResetColor();
                Console.Write("🤏");                
            }
        }

        private void PrintPlayerDiscards()
        {
            Console.Write("🗑️\t:\t");
            Tiles.PrintDeck(Hands.Discards);
        }
        
        // 선택한 타일 Discard 핸드에 넣고 버리기
        // 정렬을 맨뒤가 하나 비어있는걸로 가정했기 때문에, 강제로 빈걸로 맨 뒤로 넣어준다.
        // 버림패는 무조건 공개
        public void DiscardMyHand(int keyInt, bool isRiichi)
        {
            Tiles.Tile discard = Hands.MyTiles[keyInt];
            discard.IsShowingFront = true;
            if (isRiichi)
            {
                discard.IsRiichi = true;                
            }
            Hands.MyTiles.RemoveAt(keyInt);
            Hands.Discards.Add(discard);
            Hands.Temp = new Tiles.Tile();
        }

        // C# equals 재정의
        // https://learn.microsoft.com/ko-kr/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
        public bool Equals(Player other)
        {
            // 널 & 타입 비교
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }

            // 주소값 비교
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // 이름 같으면 같은 사람으로 치자
            return Name == other.Name;
        }
    }

    public class Human : Player, IPlayable
    {
        public Human(string name, Game.Winds wind) : base(name, wind) { }

        // 빈칸으로 두면 입력창 받도록, 귀찮아서 이름 넘김
        public static Player SetMyAvata(string playerName)
        {
            Console.WriteLine("당신의 이름을 입력해 주세요 🤔");
            
            if (playerName == "")
            {
                playerName = Console.ReadLine();
            }

            Console.WriteLine($"안녕하세요~ {playerName}님");
            return new Human(playerName, Game.Winds.East);
        }        

        // 핸드에 temp 더하기
        public void PrintTurn()
        {
            Program.WaitUntilElapsedTime(300);
            Console.Write($"{Name}님의 순서! ");
            Console.Write("1️⃣  버리기 ");
            
            if (Yaku.CanRiichi(this))
            {
                Console.Write("2️⃣  리치 ");
            }
            
            if (Yaku.CanTsumo(this))
            {
                Console.Write("3️⃣  쯔모 ");
            }

            if (Yaku.CanRon(this))
            {
                Console.Write("4️⃣  론 ");
            }

            if (Yaku.CanKang(this))
            {
                Console.Write("5️⃣  깡 ");
            }

            Console.Write("0️⃣  종료");
            Console.WriteLine("");            
        }

        public void AddTemp(Tiles.Tile tile)
        {
            tile.IsShowingFront = true;
            Hands.Temp = tile;
        }
        
        public void AddHand()
        {
            Hands.MyTiles.Add(Hands.Temp);
        }
        
        public void DiscardTile(int tileNum, bool isRiichi)
        {
            DiscardMyHand(tileNum, isRiichi);
            Hands.SortMyHand();
        }

        public ConsoleKey ReadActionKey()
        {
            // 기능 구현중 or 잘못된 키 판별 하는 변수
            bool isFalseKey = true;
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            
            while (isFalseKey)
            {
                if (keyInfo.Key == ConsoleKey.D1)
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D2 && Yaku.CanRiichi(this))
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D3 && Yaku.CanTsumo(this))
                {
                    Console.WriteLine("🚜👷쯔모 구현중....⛏️");
                } else if (keyInfo.Key == ConsoleKey.D4 && Yaku.CanRon(this))
                {
                    Console.WriteLine("🚜👷론 구현중....⛏️");                    
                } else if (keyInfo.Key == ConsoleKey.D5 && Yaku.CanKang(this))
                {
                    Console.WriteLine("🚜👷깡 구현중....⛏️");
                } else if (keyInfo.Key == ConsoleKey.D0)
                {
                    isFalseKey = false;
                    Program.IsRunning = false;
                    Console.WriteLine("게임을 종료합니다..👋");
                }
                else
                {
                    Console.WriteLine("잘못된 키입니다.");
                }
                // 틀린 키일 때 한번 더
                if (isFalseKey)
                {
                    keyInfo = Console.ReadKey(true);
                }
            }

            return keyInfo.Key;
        }

        public void PrintDiscard()
        {
            Console.Clear();
            Console.WriteLine("버릴 타일을 선택 해 주세요\n");
            Tiles.PrintDeck(Hands.MyTiles);
            Console.Write("\n0 1 2 3 4 5 6 7 8 9 A B C D\n");            
        }
        
        public int ReadDiscardKey()
        {
            ConsoleKeyInfo keyInfo;
            bool parseResult = false;
            int keyInt = 0;
            
            // 스트링 -> 16진수 변환하기, 0~13 까지 체크하고 넘으면 다시 입력할수 있도록
            // https://stackoverflow.com/questions/98559/how-to-parse-hex-values-into-a-uint            
            while (!parseResult || keyInt > MaxHandTiles -1) 
            {
                keyInfo = Console.ReadKey();
                parseResult = int.TryParse(keyInfo.KeyChar.ToString(), 
                    NumberStyles.HexNumber, CultureInfo.CurrentCulture, out keyInt);
                if (parseResult && keyInt < MaxHandTiles)
                {
                    Program.WaitUntilElapsedTime(100);
                    Console.WriteLine(" 선택한 숫자 : " + keyInt);
                }
                else
                {
                    Console.WriteLine("잘못된 키를 입력하셨습니다");
                }                
            }

            return keyInt;
        }

        public void Action()
        {
            AddHand();
            PrintTurn();
            switch (ReadActionKey())
            {
                case ConsoleKey.D0 : break;
                case ConsoleKey.D1 :
                {
                    UserDiscardAction();
                    break;
                }
                case ConsoleKey.D2:
                {
                    Riichi();
                    break;
                }
                case ConsoleKey.D3:
                {
                    Tsumo();
                    break;
                }                
            }
        }

        public void UserDiscardAction()
        {
            PrintDiscard();
            DiscardTile(ReadDiscardKey(), false);
        }

        public void Riichi()
        {
            // 그럴일 없겠지만 만약 리치 할 수 없다면 바로 리턴. 
            if (!Yaku.CanRiichi(this))
            {
                return;
            }
            
            Score -= 1000;
            IsRiichi = true;
            PrintDiscard();
            DiscardTile(ReadDiscardKey(), true);            
        }

        
        public void Tsumo()
        {
            // 이상하게 쯔모한다면 바로 리턴
            if (!Yaku.CanTsumo(this))
            {
                return;
            }
        }
    }

    public class Cpu : Player, IAction
    {
        public Cpu(string name, Game.Winds wind) : base(name, wind) { }
        
        // 핸드에 temp 더하기
        public void PrintTurn()
        {
            int computerThinking = 3;
            long waitTime = 300;
            Console.Write($"{Name}님의 순서! ");
            Program.WaitUntilElapsedTime(waitTime);
            Console.Write("컴퓨터 생각중... ");
            for (int i = 0; i < computerThinking; i++)
            {
                Program.WaitUntilElapsedTime(waitTime);
                Console.Write("🤔");
            }
            Program.WaitUntilElapsedTime(waitTime);
        }

        public void AddTemp(Tiles.Tile tile)
        {
            Hands.Temp = tile;
        }

        public void AddHand()
        {
            Hands.MyTiles.Add(Hands.Temp);
        }

        // 컴퓨터가 하는 행동
        // To-Do : 더 업그레이드 하면 좋겠지만 그냥 랜덤으로 뽑아서 버리자
        public void DiscardTile(int tileNum, bool isRiichi)
        {
            DiscardMyHand(tileNum, isRiichi);
            Hands.SortMyHand();
        }        
        
        public void Action()
        {
            AddHand();
            PrintTurn();
            DiscardTile(Program.Random.Next(0, MaxHandTiles), false);
        }

        public void Riichi()
        {

        }

        public void Tsumo()
        {
            
        }
    }
}