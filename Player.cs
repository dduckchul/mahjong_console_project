using System;
using System.Collections.Generic;

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
        private Yaku _yaku;

        public Player(string name, Game.Winds wind)
        {
            Name = name;
            Score = DefaultScore;
            Wind = wind;
            Hands = new Deck.Hands();
            PlayerYaku = new Yaku();
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

        public Tiles.Tile LastDiscardTile
        {
            get { return Hands.Discards[Hands.Discards.Count - 1]; }
        }

        public Yaku PlayerYaku
        {
            get { return _yaku; }
            protected set { _yaku = value; }
        }

        // 나는 초기화 했다고 가정, cpu 플레이어 생성해주기
        public static Player[] InitPlayers(Game game)
        {
            // TO-DO : 실제 작동할때는 이름 입력받도록
            // Players.Player me = players.SetMyAvata("");
            string[] cpuName = {"암거나", "알파고", "오픈AI", "잼민이"};            
            Player[] players = new Player[MaxPlayers];
            players[0] = Human.SetMyAvata("저에요");
            game.Me = players[0];
            
            // 0번에 나를 넣었음
            for (int i = 1; i < MaxPlayers; i++)
            {
                Game.Winds winds = (Game.Winds)Enum.Parse(typeof(Game.Winds), i.ToString());
                players[i] = new Cpu(cpuName[i], winds);
            }
            return players;
        }

        public void InitPlayerFlags()
        {
            IsPlaying = false;
            IsRiichi = false;
            IsCrying = false;
            Hands.Temp = new Tiles.Tile();
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
        // 버림패는 무조건 공개, 임시 계산용 역에서도 뺴기
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
            PlayerYaku.TempHands.MyTiles.Remove(discard);
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
}