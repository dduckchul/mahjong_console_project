using System;

namespace Mahjong
{
    public class Players
    {
        // 마작 기본 점수
        public const int Score = 25000;
        // 같이 마작 할사람 ㅠㅠ 4명이 있어야만 진행됨....
        public const int MaxPlayers = 4;
        public const int MaxHandTiles = 14;
        public const int DiscardTiles = 20;
        public struct Player
        {
            public string name;
            public int score;
            public bool isHuman;
            public Games.Winds wind;
            public Tiles.Tile[] hands;
            public Tiles.Tile[] discards;
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
                // cpu는 남(=2) 부터 적용
                int nextEnum = i + 1;
                players[i].wind = (Games.Winds)Enum.Parse(typeof(Games.Winds), nextEnum.ToString());
                players[i].name = cpuName[i];
                players[i].score = Score;
                players[i].hands = new Tiles.Tile[MaxHandTiles];
                players[i].discards = new Tiles.Tile[DiscardTiles];
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
            me.name = playerName;
            me.score = Score;
            me.wind = Games.Winds.East;
            me.hands = new Tiles.Tile[MaxHandTiles];
            me.discards = new Tiles.Tile[DiscardTiles];
            me.isHuman = true;
            
            return me;
        }
        
        // 플레이어가 number 개 만큼 타일 가져가기
        // 따로 넘버로 나눈 이유는 뿌려주는거 애니메이션처럼 할려고 ㅎㅎㅎㅎㅎㅎㅎ..
        public static void TakeTiles(Tiles.Tile[] pileOfTiles, ref Player player, int number, int index)
        {
            if (player.hands == null)
            {
                player.hands = new Tiles.Tile[MaxHandTiles];
            }

            // 1개씩 더미에서 내 핸드로 가져오기            
            for (int i = 0; i < number; i++)
            {
                int ind = index + i;

                // 타일 잠시 저장해두는 변수
                Tiles.Tile tile = pileOfTiles[ind];
                for (int j = 0; j < player.hands.Length; j++)
                {
                    tile.isVisible = true;
                    
                    if (player.isHuman)
                    {
                        tile.isShowingFront = true;
                    }
                    
                    if (player.hands[j].tileNumber == 0)
                    {
                        player.hands[j] = tile;
                        break;
                    }
                }            
            }
        }
        public static void PrintPlayers(Player[] players)
        {
            foreach (Player p in players)
            {
                PrintPlayer(p);
            }
        }

        // C# 콘솔창 오른쪽 텍스트 출력
        // https://stackoverflow.com/questions/6913563/c-sharp-align-text-right-in-console
        public static void PrintPlayer(Player p)
        {
            PrintPlayerInfo(p);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine();
            PrintPlayerHand(p);
            Console.WriteLine();
            Console.ResetColor();
        }

        private static void PrintPlayerInfo(Player p)
        {
            Console.Write("👦\t");
            if (p.isHuman)
            {
                Console.Write("🙋\t");
            }
            else
            {
                Console.Write("\t");
            }
            Console.Write("💯\t");
            Console.Write("💨");
            Console.WriteLine();
            
            Console.Write(p.name+"\t\t");
            Console.Write(p.score+"\t");

            switch (p.wind)
            {
                case Games.Winds.East :
                    Console.Write("🀀"); break;
                case Games.Winds.South :
                    Console.Write("🀁"); break;
                case Games.Winds.West :
                    Console.Write("🀂"); break;
                case Games.Winds.North :
                    Console.Write("🀃"); break;
                default : Console.Write("😱"); break;
            }
            Console.WriteLine("");
        }
        private static void PrintPlayerHand(Player p)
        {
            Tiles.PrintDeck(p.hands);
        }

        private static void PrintPlayerDiscards(Player p)
        {
            Tiles.PrintDeck(p.discards);
        }
    }
}