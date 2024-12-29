using System;
using System.Collections.Generic;

namespace Mahjong
{
    public class Game
    {
        public enum Winds
        {
            East,South,West,North
        }
        
        public const int GameEndScore = 30000;
        
        private int _gameSize;        
        // 현재 N번째 국인지 확인
        private int _num;
        // 현재 N번째 장인지 저장할때
        private int _set;        

        // 현재 바람 확인 (동,남,서,북)        
        private Winds _currentWinds;
        private Player[] _players;
        private Deck.PublicDeck _publicDeck;
        // 턴에 관련된 데이터 / 변수들 저장
        private Turn _turns;

        // 게임 종료 조건인지 확인하는 플래그
        private bool _isGameContinue;
        private bool _isSetContinue;
        
        // 나를 저장하는 임시 변수
        private Player _me;
        
        // 생성자에 턴과 유저 초기화, 유저 초기화 다르게 해야하면 수정필요
        public Game(int maxGameSize)
        {
            GameSize = maxGameSize;
            Wind = Winds.East;
            // 편하게 생성하면서 Me에 나를 넣어줌
            Players = Player.InitPlayers(this);
            Turns = new Turn(Players);
            IsGameContinue = true;
            IsSetContinue = true;
        }
        
        public int GameSize
        {
            get { return _gameSize; }
            set { _gameSize = value; }
        }

        public int Num
        {
            get { return _num; }
            set { _num = value; }
        }

        public int Set
        {
            get { return _set; }
            set { _set = value; }
        }        
        public Player[] Players
        {
            get { return _players; }
            private set { _players = value; }
        }

        public Deck.PublicDeck PublicDeck
        {
            get { return _publicDeck; }
            private set { _publicDeck = value; }
        }

        public Winds Wind
        {
            get { return _currentWinds; }
            set { _currentWinds = value; }
        }

        public Turn Turns
        {
            get { return _turns; }
            private set { _turns = value; }
        }

        public bool IsGameContinue
        {
            get { return _isGameContinue; }
            set { _isGameContinue = value; }
        }

        public bool IsSetContinue
        {
            get { return _isSetContinue; }
            set { _isSetContinue = value; }
        }

        public Player Me
        {
            get { return _me; }
            set { _me = value; }
        }
        
        public void StartGame()
        {
            Num++;
            IsGameContinue = true;
        }

        public void StartSet()
        {
            Set++;
            IsSetContinue = true;

            // 게임 초기화
            Turns.InitCurrentPlayer(this);
            InitSet(false, true);
        }

        public void EndGame()
        {
            foreach (Player pl in Players)
            {
                pl.InitPlayerFlags();
            }
            Set = 0;
            IsGameContinue = false;
            IsSetContinue = false;
        }

        public void EndSet()
        {
            foreach (Player pl in Players)
            {
                pl.InitPlayerFlags();
            }
            IsSetContinue = false;            
        }
        
        // 유국 될때 때의 조건 4개만 하자...
        // 1. 패를 다 쓴다
        // 2. 사풍연타
        // 3. 구종구패 -> 플레이어가 선택해야되기 때문에 나중에 구현하자
        // 4. 한 세트에서 깡이 네번 나왔을때 (깡 구현시 구현)
        private bool IsDrawGame()
        {
            // 1. 패를 다 쓴다.
            if (PublicDeck.PublicStack.Count == 0)
            {
                Program.WaitUntilElapsedTime(500);
                Console.WriteLine("\n🚫패가 소진 되었습니다.. 유국!!🚫");
                Program.WaitUntilElapsedTime(500);
                return true;
            }

            // 2. 사풍연타. 4번째 턴에만 나오는 무승부, 4턴째인지 확인
            if (PublicDeck.CurrentTileIndex == 4)
            {
                // 1번은 뛰어넘고 비교
                Tiles.TileType tempType = Players[0].Hands.Discards[0].Type;
                for (int i = 1; i < Players.Length; i++)
                {
                    // 바람 타입 아니면 사풍연타 아님
                    if (Players[i].Hands.Discards[0].Type != Tiles.TileType.Wind)
                    {
                        break;
                    }
                    // 이전과 지금 둘다 비교해서 바람 아니면 break;
                    if (Players[i].Hands.Discards[0].Type != tempType)
                    {
                        break;
                    }
                    // 임시 변수에 이전 타일 기억해둔다
                    tempType = Players[i].Hands.Discards[0].Type;

                    // 끝까지 비교 (넷다 바람타일이다) -> 무승부
                    if (i == 3)
                    {
                        Program.WaitUntilElapsedTime(500);
                        Console.WriteLine("🀀 🀁 🀂 🀃 사 풍 연 타 유 국!! 🀀 🀁 🀂 🀃");
                        Program.WaitUntilElapsedTime(500);
                        return true;
                    }
                }
            }
            // To-Do : 나머지 추후 구현
            return false;
        }        
        
        public bool ValidateWindContinue()
        {
            int playerMaxScore = 0;
            int currentGamePassed = ((int)Wind + 1) * Num;
            
            // 플레이어 점수 조건 탐색.
            // 1. 가장 점수 많은 사람 찾기
            // 2. 플레이어중 0점 아래인 사람 발견하면 즉시 게임 종료
            for (int i = 0; i < Players.Length; i++)
            {
                int playerScore = Players[i].Score;
                if (playerScore > playerMaxScore)
                {
                    playerMaxScore = playerScore;
                }
                if (playerScore < 0)
                {
                    return false;
                }
            }
            
            // 1. 게임이 4 만큼 진행되고 (maxGameSize)
            // 2. 유저중 누군가 30000점이 넘었을 경우 게임 종료
            // 아니면 계속 연장전 처럼 진행~~
            if (playerMaxScore >= GameEndScore && currentGamePassed >= GameSize)
            {
                return false;
            }

            return true;
        }

        // 게임 1국에 필요한 것들 모두 초기화
        private void InitSet(bool isDebug, bool isJoojak)
        {
            // 마작 덱 셔플 & 공용 덱 초기화
            Tiles.Tile[] pilesOfTile = Deck.MakeInitDeck();
            Deck.ShuffleDeck(pilesOfTile);
            PublicDeck = new Deck.PublicDeck(pilesOfTile);

            // 플레이어에게 패 나눠주기
            InitPlayersHand(PublicDeck.PublicStack);
            
            // 공용 덱 구성하고 도라 타일 1개 열기
            PublicDeck.MakePublicDeck();

            // 퍼블릭 덱 검증 출력, 디버그 모드 true 이면 출력
            DebugGame(isDebug, pilesOfTile);
            // 테스트용 게임 조작하기
            MakeJooJakHand(isJoojak);
            MakeJooJakDora(isJoojak);
            
            // 각 플레이어 손패 정렬 & 플래그 초기화
            foreach (Player pl in Players)
            {
                pl.Hands.SortMyHand();
            }
            
            PrintGames();
        }

        private void InitPlayersHand(Stack<Tiles.Tile> publicStack)
        {
            // 핸드 초기화 수정
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].Hands.MyTiles.Clear();
                Players[i].Hands.Discards.Clear();
                Players[i].Hands.OpenedBodies.Clear();
            }
            
            int distributeTimes = 4;
            // 처음은 핸드 최대값 -1 만큼 분배, 분배를 n번으로 쪼개고싶다
            int wantToDistribute = (Player.MaxHandTiles-1) / distributeTimes;
            // 마지막 for-loop 에서 줘야하는 타일값
            int remainderTiles = (Player.MaxHandTiles-1) % distributeTimes;
            
            // 얼마나 빨리 나눠줄지, 적을수록 순식간에 줌
            long waitTimeLong = 100;            
            
            // 반복해서 13개 타일을 n번 분배하는 기능
            for (int i = 0; i < distributeTimes + 1; i++)
            {
                // wantToDistribute 만큼 타일 분배
                if (i < distributeTimes)
                {
                    for (int j = 0; j < Players.Length; j++)
                    {
                        Program.WaitUntilElapsedTime(waitTimeLong);
                        Players[j].TakeTiles(publicStack,wantToDistribute);
                        PrintGames();
                    }     
                }
                // 마지막 반복에서는 나머지 타일만 준다
                if (i == distributeTimes)
                {
                    for (int j = 0; j < Players.Length; j++)
                    {
                        Program.WaitUntilElapsedTime(waitTimeLong);
                        Players[j].TakeTiles(publicStack,remainderTiles);
                        PrintGames();
                    }
                }
            }         
        }
        
        // 한턴마다 진행하는 플레이어의 액션들
        // 1. 공용 덱에서 파일 뽑기 -> 출력 -> 플레이어 액션 -> 턴종료
        public void PlayingSet()
        {
            Player player = Turns.StartCurrentTurn();
            Tiles.Tile tile = PublicDeck.Tsumo();

            (player as IAction)?.AddTemp(tile);
            PrintGames();
            (player as IAction)?.Action(this);
            
            Turns.EndCurrentTurn();
            
            // 게임 유국 조건이면 무승부를 띄우고 게임 초기화, 세트는 0번으로
            if (IsDrawGame())
            {
                EndGame();
            }
        }
        
        // 게임의 전체 화면 보여주는 메서드
        public void PrintGames()
        {
            Program.PrintClear();
            if (Set != 0)
            {
                PrintGameInfo();                
            }
            PrintHeadInfo();

            if (PublicDeck.DoraTiles[0].IsValidTile())
            {
                PrintDoraTiles();
            }
            
            if (PublicDeck.PublicStack != null)
            {
                PrintLeftTiles();
            }
            Console.WriteLine();
            foreach (Player p in Players)
            {
                p.PrintPlayer();
            }
        }

        private void PrintGameInfo()
        {
            string wind = "😱";
            switch (Wind)
            {
                case Winds.East :
                    wind = "🀀"; break;
                case Winds.South :
                    wind = "🀁"; break;
                case Winds.West :
                    wind = "🀂"; break;                    
                case Winds.North :
                    wind = "🀃"; break;
                default : Console.Write("😱\t"); break;
            }

            string windStr = wind + "  ";
            string gameStr = Program.ReturnIntToEmoji(Num) + " 국";
            string setStr = Program.ReturnIntToEmoji(Set) + " 번장";
            
            string title = windStr + gameStr + setStr;
            int startPos = (Console.WindowWidth - title.Length) / 2;
            Console.SetCursorPosition(startPos, Console.CursorTop);
            Console.WriteLine(title);
        }
        
        // 게임 위 정보 화면
        public void PrintHeadInfo()
        {
            Console.Write("👦\t");
            Console.Write("💨\t");
            Console.Write("💯\t");
            Console.Write("🙋\t");
            Console.Write("💭\t");
        }

        public void PrintDoraTiles()
        {
            Console.Write("도라 : ");
            Tiles.PrintDeck(PublicDeck.DoraTiles);
        }
        
        // 뒷 도라는 리치시에만 공개 
        public void PrintUraDoraTiles()
        {
            Console.Write("뒷 도라 : ");
            for(int i = 0; i < PublicDeck.UraDoraTiles.Length; i++)
            {
                PublicDeck.UraDoraTiles[i].IsVisible = true;
                if (i < PublicDeck.CurrentDoraIndex)
                {
                    PublicDeck.UraDoraTiles[i].IsShowingFront = true;
                }
            }
            Tiles.PrintDeck(PublicDeck.UraDoraTiles);
        }

        private void PrintLeftTiles()
        {
            int leftTiles = PublicDeck.PublicStack.Count;
            int hund = leftTiles / 100;
            int ten = leftTiles / 10 % 10;
            int one = leftTiles % 10;

            Console.Write("  🀫 ✖️ ");
            
            if (hund > 0)
            {
                Console.Write(Program.ReturnIntToEmoji(hund) + " ");
            }
            
            Console.Write(Program.ReturnIntToEmoji(ten) + " ");
            Console.Write(Program.ReturnIntToEmoji(one));
        }
        
        private void DebugGame(bool isDebug, Tiles.Tile[] pilesOfTile)
        {
            // 디버그 아니면 탈출
            if (!isDebug) { return; }
            // 초기화 덱 나오는지 검증
            Tiles.PrintDeck(pilesOfTile);
            // 마작 패 셔플 잘 됐는지 출력
            Tiles.PrintDeck(PublicDeck.PublicStack.ToArray());
            
            if (PublicDeck.IsValidPublicDeck())
            {
                Console.WriteLine("정상적으로 생성");
            }
            else
            {
                Console.WriteLine("이상한 덱 생성 확인해 주세요");
            }
        }
        
        // 테스트용 주작 핸드 만들기
        private void MakeJooJakHand(bool isJoojak)
        {
            if (!isJoojak) { return; }
            
            Human human = null;
            foreach (Player p in Players)
            {
                if (p is Human)
                {
                    human = p as Human;
                } 
            }
            
            // 뭔가 잘못되었음, 사람이 없다 ㄷㄷ
            if (human == null)
            {
                Console.WriteLine("주작 할수가 없습니다..");
                return;
            }

            Tiles.Tile[] joojakTiles =
            {
                // new Tiles.Tile(Tiles.TileType.Man, 2, false),
                // new Tiles.Tile(Tiles.TileType.Man, 2, false),
                // new Tiles.Tile(Tiles.TileType.Man, 2, false),
                // new Tiles.Tile(Tiles.TileType.Man, 2, false),
                // new Tiles.Tile(Tiles.TileType.Man, 3, false),
                // new Tiles.Tile(Tiles.TileType.Man, 4, false),
                // new Tiles.Tile(Tiles.TileType.Tong, 4, false),
                // new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                // new Tiles.Tile(Tiles.TileType.Tong, 8, false),
                // new Tiles.Tile(Tiles.TileType.Tong, 8, false),
                // new Tiles.Tile(Tiles.TileType.Man, 5, true),
                // new Tiles.Tile(Tiles.TileType.Man, 6, false),
                // new Tiles.Tile(Tiles.TileType.Man, 7, false),
                
                new Tiles.Tile(Tiles.TileType.Man, 1, false),
                new Tiles.Tile(Tiles.TileType.Man, 1, false),
                new Tiles.Tile(Tiles.TileType.Man, 3, false),
                new Tiles.Tile(Tiles.TileType.Man, 3, false),
                new Tiles.Tile(Tiles.TileType.Man, 4, false),
                new Tiles.Tile(Tiles.TileType.Man, 4, false),
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                new Tiles.Tile(Tiles.TileType.Tong, 6, false),
                new Tiles.Tile(Tiles.TileType.Tong, 6, false),
                new Tiles.Tile(Tiles.TileType.Man, 7, true),
                new Tiles.Tile(Tiles.TileType.Man, 7, false),
                new Tiles.Tile(Tiles.TileType.Man, 8, false),                
            };

            for (int i = 0; i < joojakTiles.Length; i++)
            {
                if (joojakTiles[i].IsValidTile())
                {
                    joojakTiles[i].IsVisible = true;
                    joojakTiles[i].IsShowingFront = true;
                }
            }

            human.Hands.MyTiles = new List<Tiles.Tile>(joojakTiles);
        }
        
        // 테스트용 주작 핸드 만들기
        private void MakeJooJakDora(bool isJoojak)
        {
            if (!isJoojak) { return; }
            Tiles.Tile[] joojakDoras = new Tiles.Tile[]
            {
                new Tiles.Tile(Tiles.TileType.Tong, 2, false),
                new Tiles.Tile(Tiles.TileType.Tong, 2, false),
                new Tiles.Tile(Tiles.TileType.Tong, 2, false),
                new Tiles.Tile(Tiles.TileType.Tong, 2, false),
                new Tiles.Tile(Tiles.TileType.Tong, 2, false),
            };
            
            Tiles.Tile[] joojakDoras2 = new Tiles.Tile[]
            {
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
                new Tiles.Tile(Tiles.TileType.Tong, 5, false),
            };
            
            joojakDoras[0].IsShowingFront = true;

            for (int i = 0; i < 5; i++)
            {
                if (joojakDoras[i].IsValidTile())
                {
                    joojakDoras[i].IsVisible = true;
                }
            }

            PublicDeck.DoraTiles = joojakDoras;
            PublicDeck.UraDoraTiles = joojakDoras2;
        }
    }
}