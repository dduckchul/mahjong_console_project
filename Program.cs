using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Mahjong
{
    public class Program
    {
        // 다른곳과 같이 쓸 StopWatch 클래스
        private static Stopwatch _stopwatch;
        private static bool _isRunning;
        
        public static bool IsRunning
        {
            get { return _isRunning;}
            set { _isRunning = value; }
        }

        public static Stopwatch Watch
        {
            get { return _stopwatch; }
            private set { _stopwatch = value; }
        }

        public static void Main(string[] args)
        {
            // 게임 초기화. 동풍전 1국 1번장부터 시작, 유저와 덱 모두 초기화
            Game game = new Game();
            IsRunning = true;
            
            // 게임 기억하기
            Stats stats = new Stats();
            stats.Games.Add(game);
            
            // 게임 진짜 시작
            while (IsRunning)
            {
                game.Num++;
                game.IsGameContinue = true;
                while (IsRunning && game.IsGameContinue)
                {
                    // 게임 초기화
                    game.InitGame();
                    game.FindFirstUser();
                    game.Set++;
                    game.IsSetContinue = true;
                    while (IsRunning && game.IsSetContinue)
                    {
                        game.KeepPlayingSet();
                    }                    
                }
            }
        }

        // Thread.sleep 대신 변경, 디버그용으로 멈추는 애니메이션 없에고싶으면 다 주석처리
        public static void WaitUntilElapsedTime(long waitTime)
        {
            if (Watch == null)
            {
                Watch = new Stopwatch();
            }
            
            Watch.Reset();
            Watch.Start();
            while (waitTime > Watch.ElapsedMilliseconds)
            {
                if (waitTime <= Watch.ElapsedMilliseconds)
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
    }
}