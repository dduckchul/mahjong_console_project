using System;
using System.Diagnostics;

namespace Mahjong
{
    public class Games
    {
        // 동장전만 구현, 더하고싶으면 MaxGameSize를 늘리자.
        public const int MaxGameSize = 4;
        public const int GameEndScore = 30000;
        
        public struct Game
        {
            public Deck.PublicDeck publicDeck;
            // 현재 바람 확인 (동,남,서,북)
            public Winds currentWinds;
            // 현재 N번째 국인지 확인
            public int game;
            // 현재 N번째 장인지 저장할때
            public int set;
            // 게임 종료 조건인지 확인하는 플래그
            public bool isGameContinue;
            public bool isSetContinue;
        }
        public enum Winds
        {
            East,South,West,North
        }
        
        // 유국 될때 때의 조건 4개만 하자...
        // 1. 패를 다 쓴다
        // 2. 사풍연타
        // 3. 구종구패 -> 플레이어가 선택해야되기 때문에 나중에 구현하자
        // 4. 한 세트에서 깡이 네번 나왔을때 (깡 구현시 구현)
        public static bool IsDrawGame(Deck.PublicDeck publicDeck, Players.Player[] players)
        {
            Stopwatch watch = new Stopwatch();
            // 1. 패를 다 쓴다.
            if (publicDeck.currentTileIndex == Deck.PublicTiles)
            {
                Program.WaitUntilElapsedTime(watch, 1000);
                Console.WriteLine("\n🚫패가 소진 되었습니다.. 유국!!🚫");
                return true;
            }

            // 2. 사풍연타. 4번째 턴에만 나오는 무승부, 4턴째인지 확인
            if (publicDeck.currentTileIndex == 3)
            {
                // 1번은 뛰어넘고 비교
                Tiles.TileType tempType = players[0].discards[0].type;
                for (int i = 1; i < players.Length; i++)
                {
                    // 바람 타입 아니면 사풍연타 아님
                    if (players[i].discards[0].type != Tiles.TileType.Wind)
                    {
                        break;
                    }
                    // 이전과 지금 둘다 비교해서 바람 아니면 break;
                    if (players[i].discards[0].type != tempType)
                    {
                        break;
                    }
                    // 임시 변수에 이전 타일 기억해둔다
                    tempType = players[i].discards[0].type;

                    // 끝까지 비교 (넷다 바람타일이다) -> 무승부
                    if (i == 3)
                    {
                        Program.WaitUntilElapsedTime(watch, 1000);
                        Console.WriteLine("🀀 🀁 🀂 🀃 사 풍 연 타 유 국!! 🀀 🀁 🀂 🀃");
                        return true;
                    }
                }
            }
            // To-Do : 나머지 추후 구현
            return false;
        }        
        
        public bool validateWindContinue(Players.Player[] players, Game currentGame)
        {
            int playerMaxScore = 0;
            int currentGamePassed = ((int)currentGame.currentWinds + 1) * currentGame.game;
            
            // 플레이어 점수 조건 탐색.
            // 1. 가장 점수 많은 사람 찾기
            // 2. 플레이어중 0점 아래인 사람 발견하면 즉시 게임 종료
            for (int i = 0; i < players.Length; i++)
            {
                int playerScore = players[i].score;
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
        public static void FindFirstUser(ref Players.Player[] players, Game game)
        {
            Winds wind = game.currentWinds;
            int currentGame = game.game;

            int currentPlayerIndex = (int)(wind + currentGame - 1) % 4;
            players[currentPlayerIndex].isPlaying = true;
        }

        // 현재 플레이중인 유저의 인덱스를 반환한다.
        public static int FindPlayingUserInx(Players.Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].isPlaying)
                {
                    return i;
                }
            }
            Console.WriteLine("여기까지 가믄 안되요!!!!");
            return -1;
        }

        // 다음 턴 유저 인덱스 반환
        public static int FindNextUserInx(Players.Player[] players)
        {
            int playingUserInx = FindPlayingUserInx(players);
            playingUserInx++;
            
            // 유저 인덱스가 4 (마지막 유저 + 1 일때) 처음 유저 0으로 반환한다.
            if (playingUserInx == players.Length)
            {
                return 0;
            }

            return playingUserInx;
        }
        
        // 일단 임시로 다형성 추가
        public static void PrintGames(Players.Player[] players)
        {
            PrintGames(new Game(), new Deck.PublicDeck(), players);
        }        
        
        // 게임의 전체 화면 보여주는 메서드
        public static void PrintGames(Game game, Deck.PublicDeck publicDeck, Players.Player[] players)
        {
            Console.Clear();
            if (game.set != 0)
            {
                PrintGameInfo(game);                
            }
            PrintHeadInfo();
            if (publicDeck.publicTiles != null)
            {
                PrintDoraTiles(publicDeck);
                PrintLeftTiles(publicDeck);
            }
            Console.WriteLine();
            foreach (Players.Player p in players)
            {
                Players.PrintPlayer(p);
            }
        }

        public static void PrintGameInfo(Game game)
        {
            string wind = "😱";
            switch (game.currentWinds)
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
            string gameStr = Program.ReturnIntToEmoji(game.game) + " 국";
            string setStr = Program.ReturnIntToEmoji(game.set) + " 번장";
            
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

        public static void InitGame(ref Players.Player[] players, ref Game game)
        {
            Stopwatch watch = new Stopwatch();
            // 덱 초기화
            Tiles.Tile[] pilesOfTile = Deck.MakeInitDeck();
            // 마작 패 초기화 잘 됐는지 출력
            // Tiles.PrintDeck(pilesOfTile);
            
            // 마작 덱 셔플
            Deck.ShuffleDeck(pilesOfTile);
            // 마작 패 셔플 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);
            
            // 플레이어에게 패 나눠주기
            InitPlayersHand(players, pilesOfTile);
            
            // 공용 덱 구성하고 도라 타일 1개 열기
            Deck.PublicDeck publicDeck = Deck.MakePublicDeck(pilesOfTile);
            game.publicDeck = publicDeck;
            Deck.initDora(ref publicDeck);
            // publicDeck 구성 잘 되었는지 확인
            // Console.WriteLine(publicDeck);
            
            // 각 플레이어 손패 정렬
            foreach (Players.Player pl in players)
            {
                Deck.SortMyHand(pl);
             }
            PrintGames(game, publicDeck, players);            
        }
        
        public static void InitPlayersHand(Players.Player[] mahjongPlayers, Tiles.Tile[] mahjongTiles)
        {
            Stopwatch watch = new Stopwatch();

            // 핸드 new 로 초기화
            for (int i = 0; i < mahjongPlayers.Length; i++)
            {
                mahjongPlayers[i].hands = new Tiles.Tile[Players.MaxHandTiles];
                mahjongPlayers[i].discards = new Tiles.Tile[Players.MaxDiscardTiles];                
            }
            
            // 현재 분배중인 덱 인덱스
            int tileIndex = 0;
            int distributeTimes = 3;
            // 처음은 핸드 최대값 -1 만큼 분배, 분배를 n번으로 쪼개고싶다
            int wantToDistribute = (Players.MaxHandTiles-1) / distributeTimes;
            // 마지막 for-loop 에서 줘야하는 타일값
            int remainderTiles = (Players.MaxHandTiles-1) % distributeTimes;
            
            // 얼마나 빨리 나눠줄지, 적을수록 순식간에 줌
            long waitTimeLong = 100;            
            
            // 반복해서 13개 타일을 n번 분배하는 기능
            for (int i = 0; i < distributeTimes + 1; i++)
            {
                // wantToDistribute 만큼 타일 분배
                if (i < distributeTimes)
                {
                    for (int j = 0; j < mahjongPlayers.Length; j++)
                    {
                        Program.WaitUntilElapsedTime(watch, waitTimeLong);
                        Players.TakeTiles(mahjongTiles, ref mahjongPlayers[j], wantToDistribute, tileIndex);
                        Games.PrintGames(mahjongPlayers);
                        tileIndex += wantToDistribute;
                    }     
                }
                // 마지막 반복에서는 나머지 타일만 준다
                if (i == distributeTimes)
                {
                    for (int j = 0; j < mahjongPlayers.Length; j++)
                    {
                        Program.WaitUntilElapsedTime(watch, waitTimeLong);
                        Players.TakeTiles(mahjongTiles, ref mahjongPlayers[j], remainderTiles, tileIndex);
                        Games.PrintGames(mahjongPlayers);
                        tileIndex += remainderTiles;
                    }
                }
            }         
        }        
    }
}