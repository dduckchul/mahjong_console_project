using System;
using System.Diagnostics;

namespace Mahjong
{
    public class Game
    {
        public enum Winds
        {
            East,South,West,North
        }        
        
        // 동장전만 구현, 더하고싶으면 MaxGameSize를 늘리자.
        public const int MaxGameSize = 4;
        public const int GameEndScore = 30000;
        
        private Player[] _players;
        private Deck.PublicDeck _publicDeck;
        // 현재 바람 확인 (동,남,서,북)
        private Winds _currentWinds;
        // 현재 N번째 국인지 확인
        private int _num;
        // 현재 N번째 장인지 저장할때
        private int _set;
        
        // 게임 종료 조건인지 확인하는 플래그
        private bool _isGameContinue;
        private bool _isSetContinue;        
        
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

        
        // 유국 될때 때의 조건 4개만 하자...
        // 1. 패를 다 쓴다
        // 2. 사풍연타
        // 3. 구종구패 -> 플레이어가 선택해야되기 때문에 나중에 구현하자
        // 4. 한 세트에서 깡이 네번 나왔을때 (깡 구현시 구현)
        public bool IsDrawGame()
        {
            Stopwatch watch = new Stopwatch();
            // 1. 패를 다 쓴다.
            if (PublicDeck.currentTileIndex == Deck.PublicTiles)
            {
                Program.WaitUntilElapsedTime(1000);
                Console.WriteLine("\n🚫패가 소진 되었습니다.. 유국!!🚫");
                return true;
            }

            // 2. 사풍연타. 4번째 턴에만 나오는 무승부, 4턴째인지 확인
            if (PublicDeck.currentTileIndex == 3)
            {
                // 1번은 뛰어넘고 비교
                Tiles.TileType tempType = Players[0].Discards[0].type;
                for (int i = 1; i < Players.Length; i++)
                {
                    // 바람 타입 아니면 사풍연타 아님
                    if (Players[i].Discards[0].type != Tiles.TileType.Wind)
                    {
                        break;
                    }
                    // 이전과 지금 둘다 비교해서 바람 아니면 break;
                    if (Players[i].Discards[0].type != tempType)
                    {
                        break;
                    }
                    // 임시 변수에 이전 타일 기억해둔다
                    tempType = Players[i].Discards[0].type;

                    // 끝까지 비교 (넷다 바람타일이다) -> 무승부
                    if (i == 3)
                    {
                        Program.WaitUntilElapsedTime(1000);
                        Console.WriteLine("🀀 🀁 🀂 🀃 사 풍 연 타 유 국!! 🀀 🀁 🀂 🀃");
                        return true;
                    }
                }
            }
            // To-Do : 나머지 추후 구현
            return false;
        }        
        
        public bool validateWindContinue()
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
            if (playerMaxScore >= GameEndScore && currentGamePassed > MaxGameSize)
            {
                return false;
            }

            return true;
        }

        // 게임마다 첫 번째 턴인 유저에게 턴을 준다.
        // 동 1국 = wind + 1 = (0+1) - 1 % 4 = 0 (동)
        // 동 2국 = wind + 2 = (0+2) - 1 % 4 = 1 (남)
        // 남 3국 = wind + 3 = (1+3) - 1 % 4 = 3 (북)
        // 북 4국 = wind + 4 = (3+4) - 1 % 4 = 6 % 4 = 2 (서)
        public void FindFirstUser()
        {
            Winds wind = Wind;
            int currentGame = Num;

            int currentPlayerIndex = (int)(wind + currentGame - 1) % 4;
            Players[currentPlayerIndex].IsPlaying = true;
        }

        // 현재 플레이중인 유저의 인덱스를 반환한다.
        public int FindPlayingUserInx(Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].IsPlaying)
                {
                    return i;
                }
            }
            Console.WriteLine("여기까지 가믄 안되요!!!!");
            return -1;
        }

        // 다음 턴 유저 인덱스 반환
        public int FindNextUserInx()
        {
            int playingUserInx = FindPlayingUserInx(Players);
            playingUserInx++;
            
            // 유저 인덱스가 4 (마지막 유저 + 1 일때) 처음 유저 0으로 반환한다.
            if (playingUserInx == Players.Length)
            {
                return 0;
            }

            return playingUserInx;
        }
        
        // 게임의 전체 화면 보여주는 메서드
        public void PrintGames()
        {
            Console.Clear();
            if (Set != 0)
            {
                PrintGameInfo();                
            }
            PrintHeadInfo();
            if (PublicDeck.publicTiles != null)
            {
                PrintDoraTiles(PublicDeck);
                PrintLeftTiles(PublicDeck);
            }
            Console.WriteLine();
            foreach (Player p in Players)
            {
                p.PrintPlayer();
            }
        }

        public void PrintGameInfo()
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
        public static void PrintHeadInfo()
        {
            Console.Write("👦\t");
            Console.Write("💨\t");
            Console.Write("💯\t");
            Console.Write("🙋\t");
            Console.Write("💭");
        }

        public static void PrintDoraTiles(Deck.PublicDeck publicDeck)
        {
            Console.Write("  도라 : ");
            Tiles.PrintDeck(publicDeck.doraTiles);
        }

        public static void PrintLeftTiles(Deck.PublicDeck publicDeck)
        {
            int leftTiles = Deck.PublicTiles - publicDeck.currentTileIndex;

            int ten = leftTiles / 10 % 10;
            int one = leftTiles % 10;

            string leftStr = "  🀫 ✖️ " + Program.ReturnIntToEmoji(ten) + " " + Program.ReturnIntToEmoji(one);
            Console.Write(leftStr);            
        }

        public void InitGame()
        {
            Wind = Winds.East;
            // 게임 초기화시 세트처럼 반복문에 넘길것
            IsGameContinue = true;
            IsSetContinue = true;
            
            Player player = new Player();
            // 게임 시작, 플레이어 초기화
            Players = player.InitPlayers();
            
            // 덱 초기화
            Tiles.Tile[] pilesOfTile = Deck.MakeInitDeck();
            // 마작 패 초기화 잘 됐는지 출력
            // Tiles.PrintDeck(pilesOfTile);
            
            // 마작 덱 셔플
            Deck.ShuffleDeck(pilesOfTile);
            // 마작 패 셔플 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);
            
            // 플레이어에게 패 나눠주기
            InitPlayersHand(pilesOfTile);
            
            // 공용 덱 구성하고 도라 타일 1개 열기
            Deck.PublicDeck publicDeck = Deck.MakePublicDeck(pilesOfTile);
            PublicDeck = publicDeck;
            Deck.initDora(ref publicDeck);
            // publicDeck 구성 잘 되었는지 확인
            // Console.WriteLine(publicDeck);
            
            // 각 플레이어 손패 정렬
            foreach (Player pl in Players)
            {
                Deck.SortMyHand(pl);
             }
            PrintGames();            
        }
        
        public void InitPlayersHand(Tiles.Tile[] mahjongTiles)
        {
            // 핸드 new 로 초기화
            for (int i = 0; i < Players.Length; i++)
            {
                Players[i].Hands = new Tiles.Tile[Mahjong.Player.MaxHandTiles];
                Players[i].Discards = new Tiles.Tile[Mahjong.Player.MaxDiscardTiles];                
            }
            
            // 현재 분배중인 덱 인덱스
            int tileIndex = 0;
            int distributeTimes = 3;
            // 처음은 핸드 최대값 -1 만큼 분배, 분배를 n번으로 쪼개고싶다
            int wantToDistribute = (Mahjong.Player.MaxHandTiles-1) / distributeTimes;
            // 마지막 for-loop 에서 줘야하는 타일값
            int remainderTiles = (Mahjong.Player.MaxHandTiles-1) % distributeTimes;
            
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
                        Players[i].TakeTiles(mahjongTiles,wantToDistribute, tileIndex);
                        PrintGames();
                        tileIndex += wantToDistribute;
                    }     
                }
                // 마지막 반복에서는 나머지 타일만 준다
                if (i == distributeTimes)
                {
                    for (int j = 0; j < Players.Length; j++)
                    {
                        Program.WaitUntilElapsedTime(waitTimeLong);
                        Players[i].TakeTiles(mahjongTiles,remainderTiles, tileIndex);
                        PrintGames();
                        tileIndex += remainderTiles;
                    }
                }
            }         
        }
        
        public void KeepPlayingSet()
        {
            int userInx = FindPlayingUserInx(Players);
            int nextUserInx = FindNextUserInx();
            
            Player currentPlayer = Players[userInx];

            // 나부터 하나씩 뽑자
            Tiles.Tile tile = currentPlayer.Tsumo(PublicDeck);
            // 뽑은 타일은 보이게끔
            tile.isVisible = true;
            // 내가 뽑았으면 보이게끔
            if (currentPlayer.IsHuman)
            {
                tile.isShowingFront = true;
            }
            currentPlayer.Temp = tile;

            Program.WaitUntilElapsedTime(500);
            PrintGames();
                
            PrintTurnAndAction(currentPlayer);
            if (currentPlayer.IsHuman)
            {
                currentPlayer.PressKeyAndAction();                    
            }
            else
            {
                currentPlayer.ComputerAction();
            }

            // 키 입력후에는 한턴 넘어가는것으로 판단한다.
            currentPlayer.IsPlaying = false;
            Players[nextUserInx].IsPlaying = true;
            
            // 게임 유국 조건이면 무승부를 띄우고 게임 초기화, 세트는 0번으로
            bool isDrawGame = IsDrawGame();
            if (isDrawGame)
            {
                Set = 0;
                IsGameContinue = false;
                IsSetContinue = false;
                Players[nextUserInx].IsPlaying = false;
            }
        }        

        public void PrintTurnAndAction(Player player)
        {
            Program.WaitUntilElapsedTime(1000);
            Console.Write($"{player.Name}님의 순서! ");
            if (player.IsHuman)
            {
                Console.Write("1️⃣  버리기 ");
                Console.Write("2️⃣  리치 ");
                Console.Write("3️⃣  쯔모 ");
                Console.Write("4️⃣  깡 ");
                Console.Write("0️⃣  종료");
                Console.WriteLine("");
            }
            else
            {
                int computerThinking = 3;
                long waitTime = 300;
                
                Console.Write("컴퓨터 생각중... ");
                for (int i = 0; i < computerThinking; i++)
                {
                    Program.WaitUntilElapsedTime(waitTime);
                    Console.Write("🤔");
                }
            }
        }
    }
}