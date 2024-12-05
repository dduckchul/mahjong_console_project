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
            Players players = new Players();

            // 게임 시작, 플레이어 초기화
            Players.Player[] mahjongPlayers = players.InitPlayers();    
            
            // 게임 초기화. 동풍전 1국 1번장부터 시작, 유저와 덱 모두 초기화
            Games.Game game = new Games.Game();
            game.currentWinds = Games.Winds.East;
            // 게임 초기화시 세트처럼 반복문에 넘길것
            game.isGameContinue = true;
            game.isSetContinue = true;            

            // 게임 진짜 시작
            while (isRunning)
            {
                game.game++;
                game.isGameContinue = true;
                while (isRunning && game.isGameContinue)
                {
                    // 게임 초기화
                    Games.InitGame(ref mahjongPlayers, ref game);
                    Games.FindFirstUser(ref mahjongPlayers, game);
                    game.set++;
                    game.isSetContinue = true;
                    while (isRunning && game.isSetContinue)
                    {
                        KeepPlayingSet(ref mahjongPlayers, ref game);
                    }                    
                }
            }
        }

        // Thread.sleep 대신 변경, 디버그용으로 멈추는 애니메이션 없에고싶으면 다 주석처리
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
        
        public static void KeepPlayingSet(ref Players.Player[] mahjongPlayers, ref Games.Game game)
        {
            Stopwatch watch = new Stopwatch();            
            int userInx = Games.FindPlayingUserInx(mahjongPlayers);
            int nextUserInx = Games.FindNextUserInx(mahjongPlayers);

            // 나부터 하나씩 뽑자
            Tiles.Tile tile = Players.Tsumo(ref game.publicDeck);
            // 뽑은 타일은 보이게끔
            tile.isVisible = true;
            // 내가 뽑았으면 보이게끔
            if (mahjongPlayers[userInx].isHuman)
            {
                tile.isShowingFront = true;
            }
            mahjongPlayers[userInx].temp = tile;

            WaitUntilElapsedTime(watch, 500);
            Games.PrintGames(game, game.publicDeck, mahjongPlayers);
                
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
            
            // 게임 유국 조건이면 무승부를 띄우고 게임 초기화, 세트는 0번으로
            bool isDrawGame = Games.IsDrawGame(game.publicDeck, mahjongPlayers);
            if (isDrawGame)
            {
                game.set = 0;
                game.isGameContinue = false;
                game.isSetContinue = false;
                mahjongPlayers[nextUserInx].isPlaying = false;
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
                long waitTime = 300;
                
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