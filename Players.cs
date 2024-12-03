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
        public struct Player
        {
            public string name;
            public int score;
            public bool isHuman;
            public Tiles.Tile[] hands;
            public Tiles.Tile[] discards;
        }

        // 나는 초기화 했다고 가정, cpu 플레이어 생성해주기
        public Player[] InitPlayers(ref Player me)
        {
            string[] cpuName = {"암거나", "알파고", "오픈AI", "코파일럿" };
            Player[] players = new Player[MaxPlayers]
            {
                me, new Player(), new Player(), new Player()
            };

            for (int i = 1; i < MaxPlayers; i++)
            {
                players[i].name = cpuName[1];
                players[i].score = Score;
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
            me.isHuman = true;
            
            return me;
        }
        
        // 플레이어가 number 개 만큼 타일 가져가기
        // 따로 넘버로 나눈 이유는 뿌려주는거 애니메이션처럼 할려고 ㅎㅎㅎㅎㅎㅎㅎ..
        public void TakeTiles(Tiles.Tile[] pileOfTiles, ref Player player, int number, int index)
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
                    if (player.isHuman)
                    {
                        tile.isShowing = true;
                    }
                    
                    if (player.hands[j].tileNumber == 0)
                    {
                        player.hands[j] = tile;
                        break;
                    }
                }            
            }
            
            // 디버깅용 출력
            if (player.isHuman)
            {
                Console.Clear();
                Tiles.PrintDeck(player.hands);
            }
        }        
    }
}