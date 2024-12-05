using System;
using System.Diagnostics;
using System.Threading;

namespace Mahjong
{
    internal class Program
    {
        static bool isRunning = true;
        public static void Main(string[] args)
        {
            // 시간 계산 위한 스톱워치 초기화
            Stopwatch watch = new Stopwatch();
            Deck deck = new Deck();
            Players players = new Players();

            // 덱 초기화
            Tiles.Tile[] pilesOfTile = deck.MakeInitDeck();
            // 마작 패 초기화 잘 됐는지 출력
            // Tiles.PrintDeck(pilesOfTile);
            
            // 마작 덱 셔플
            deck.ShuffleDeck(pilesOfTile);
            // 마작 패 셔플 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);
            
            Players.Player[] mahjongPlayers = players.InitPlayers();
            InitPlayersHand(mahjongPlayers, pilesOfTile);
            Games.PrintGames(mahjongPlayers);
            
            // 공용 덱 구성하고 도라 타일 1개 열기
            Deck.PublicDeck publicDeck = deck.MakePublicDeck(pilesOfTile);
            Deck.initDora(ref publicDeck);
            // publicDeck 구성 잘 되었는지 확인
            // Console.WriteLine(publicDeck);
            
            // 각 플레이어 손패 정렬
            foreach (Players.Player pl in mahjongPlayers)
            {
                Deck.SortMyHand(pl);                
            }

            // 정렬 후 프린트 화면
            WaitUntilElapsedTime(watch, 500);
            Games.PrintGames(mahjongPlayers);            
            
            // 동풍전 1국 1번장부터 시작
            Games.Game game = new Games.Game();
            game.game = 1;
            game.set = 1;
            game.currentWinds = Games.Winds.East;
            
            // 정렬 후 프린트 화면
            WaitUntilElapsedTime(watch, 500);
            Games.FindFirstUser(mahjongPlayers, ref game);
            
            while (isRunning)
            {
                // 첫 화면 출력
                int userInx = Games.FindPlayingUserInx(mahjongPlayers);
                int nextUserInx = Games.FindNextUserInx(mahjongPlayers);
                // 나부터 하나씩 뽑자
                Tiles.Tile tile = Players.Tsumo(ref publicDeck);

                // 뽑은 타일은 보이게끔
                tile.isVisible = true;
                
                // 내가 뽑았으면 보이게끔
                if (mahjongPlayers[userInx].isHuman)
                {
                    tile.isShowingFront = true;
                }
                mahjongPlayers[userInx].temp = tile;
                Games.PrintGames(game, publicDeck, mahjongPlayers);
                
                PrintTurnAndAction(watch, mahjongPlayers[userInx]);
                if (mahjongPlayers[userInx].isHuman)
                {
                    PressKeyAndAction(ref mahjongPlayers[userInx], watch);                    
                }
                else
                {
                    ComputerAction(ref mahjongPlayers[userInx]);
                }

                // 키 입력후에는 한턴 넘어가는것으로 판단한다.
                mahjongPlayers[userInx].isPlaying = false;
                mahjongPlayers[nextUserInx].isPlaying = true;
            }
        }

        public static void InitPlayersHand(Players.Player[] mahjongPlayers, Tiles.Tile[] mahjongTiles)
        {
            Stopwatch watch2 = new Stopwatch();
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
                        WaitUntilElapsedTime(watch2, waitTimeLong);
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
                        WaitUntilElapsedTime(watch2, waitTimeLong);
                        Players.TakeTiles(mahjongTiles, ref mahjongPlayers[j], remainderTiles, tileIndex);
                        Games.PrintGames(mahjongPlayers);
                        tileIndex += remainderTiles;
                    }
                }
            }         
        }

        // Thread.sleep 대신 변경
        public static void WaitUntilElapsedTime(Stopwatch watch, long waitTime)
        {
            watch.Reset();
            watch.Start();
            while (waitTime > watch.ElapsedMilliseconds)
            {
                if (waitTime <= watch.ElapsedMilliseconds)
                {
                    // 여기서 리셋 해주니까 시간이 계속 흐름 // watch.Reset();
                    break;
                }
            }
        }
        
        public static string ReturnIntToEmoji(int value)
        {
            switch (value)
            {
                case 0: return "0️⃣";
                case 1: return "1️⃣";
                case 2: return "2️⃣";
                case 3: return "3️⃣";
                case 4: return "4️⃣";
                case 5: return "5️⃣";
                case 6: return "6️⃣";
                case 7: return "7️⃣";
                case 8: return "8️⃣";
                case 9: return "9️⃣";                
                default: return "😱";
            }
        }        

        public static void PrintTurnAndAction(Stopwatch watch, Players.Player player)
        {
            WaitUntilElapsedTime(watch, 1000);
            Console.Write($"{player.name}님의 순서! ");
            if (player.isHuman)
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
                long waitTime = 1000;
                
                Console.Write("컴퓨터 생각중... ");
                for (int i = 0; i < computerThinking; i++)
                {
                    WaitUntilElapsedTime(watch, waitTime);
                    Console.Write("🤔");
                }
            }
        }

        public static void PressKeyAndAction(ref Players.Player player, Stopwatch watch)
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
                    Players.UserAddTempAndDiscardTile(ref player);
                    WaitUntilElapsedTime(watch, 200);
                } else if (keyInfo.Key == ConsoleKey.D2)
                {
                    Console.WriteLine("🚜👷리치 구현중....⛏️");
                    WaitUntilElapsedTime(watch, 200);
                } else if (keyInfo.Key == ConsoleKey.D3)
                {
                    Console.WriteLine("🚜👷쯔모 구현중....⛏️");
                    WaitUntilElapsedTime(watch, 200);
                } else if (keyInfo.Key == ConsoleKey.D4)
                {
                    Console.WriteLine("🚜👷깡 구현중....⛏️");
                    WaitUntilElapsedTime(watch, 200);                    
                } else if (keyInfo.Key == ConsoleKey.D0)
                {
                    isRunning = false;
                    isFalseKey = false;
                    Console.WriteLine("게임을 종료합니다..👋");
                    WaitUntilElapsedTime(watch, 200);
                }
                else
                {
                    Console.WriteLine("잘못된 키입니다.");
                }                
            }
        }

        public static void ComputerAction(ref Players.Player player)
        {
            Players.AiAddTempAndDiscardTile(ref player);
        }
    }
}