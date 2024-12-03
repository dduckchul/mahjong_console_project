using System;

namespace Mahjong
{
    public class Players
    {
        public const int Score = 25000;
        public struct Player
        {
            public string name;
            public int score;
            public Tiles.Tile[] hands;
            public Tiles.Tile[] discards;            
        }

        // 나는 초기화 했다고 가정, cpu 플레이어 생성해주기
        public Player[] InitPlayers(ref Player me)
        {
            string[] cpuName = { "알파고", "오픈AI", "코파일럿" };
            Player[] players = new Player[4]
            {
                new Player(), new Player(), new Player(), me
            };

            for (int i = 0; i < 3; i++)
            {
                players[i].name = cpuName[i];
                players[i].score = Score;
            }
            
            return players;
        }
        
        public Player SetMyAvata()
        {
            Console.WriteLine("당신의 이름을 입력해 주세요 🤔");
            
            Players.Player me = new Players.Player();
            me.name = Console.ReadLine();
            me.score = Players.Score;
            
            return me;
        }
    }
}