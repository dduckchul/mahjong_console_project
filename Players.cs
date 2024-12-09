using System;
using System.Diagnostics;
using System.Globalization;

namespace Mahjong
{
    public class Players
    {
        // 마작 기본 점수
        public const int Score = 25000;
        // 같이 마작 할사람 ㅠㅠ 4명이 있어야만 진행됨....
        public const int MaxPlayers = 4;
        public const int MaxHandTiles = 14;
        public const int MaxDiscardTiles = 30;
        public struct Player
        {
            public string name;
            public int score;
            public bool isHuman;
            // 지금 나의 턴인지 확인하는 플래그
            public bool isPlaying;
            // 내가 리치 선언했는지 확인하는 플래그
            public bool isRiichi;
            // 내가 울었는지 확인하는 플래그
            public bool isCrying;
            public Games.Winds wind;
            public Tiles.Tile[] hands;
            public Tiles.Tile[] discards;
            public Tiles.Tile[,] openedBodies;
            public Tiles.Tile temp;
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
                // cpu는 남(=1) 부터 적용
                players[i].wind = (Games.Winds)Enum.Parse(typeof(Games.Winds), i.ToString());
                players[i].name = cpuName[i];
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
            me.wind = Games.Winds.East;
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
                    
                    // 비어있는거 확인하려고 숫자 비교했는데 여기서 이상해짐
                    if (Tiles.IsValidTile(player.hands[j]) == false)
                    {
                        player.hands[j] = tile;
                        break;
                    }
                }            
            }
        }

        // C# 콘솔창 오른쪽 텍스트 출력
        // https://stackoverflow.com/questions/6913563/c-sharp-align-text-right-in-console
        public static void PrintPlayer(Player p)
        {
            PrintPlayerInfo(p);
            PrintPlayerHand(p);
            PrintPlayerTemp(p);
            Console.WriteLine();
            PrintPlayerDiscards(p);
            Console.WriteLine("\n");
        }

        private static void PrintPlayerInfo(Player p)
        {
            Console.Write(p.name+"\t");
            
            if (p.isHuman)
            {
                Console.Write("👤");
            }
            else
            {
                Console.Write("💻");
            }

            switch (p.wind)
            {
                case Games.Winds.East :
                    Console.Write("🀀\t"); break;
                case Games.Winds.South :
                    Console.Write("🀁\t"); break;
                case Games.Winds.West :
                    Console.Write("🀂\t"); break;
                case Games.Winds.North :
                    Console.Write("🀃\t"); break;
                default : Console.Write("😱\t"); break;
            }            
            
            Console.Write(p.score+"\t");
            
            if (p.isRiichi)
            {
                Console.Write("🛑\t");                
            }
            else
            {
                Console.Write("\t");
            }

            if (p.isPlaying)
            {
                Console.Write("🤯");
            }
            
            Console.WriteLine("");
        }
        private static void PrintPlayerHand(Player p)
        {
            Console.Write("덱\t:\t");
            Tiles.PrintDeck(p.hands);
        }

        private static void PrintPlayerTemp(Player p)
        {
            if (Tiles.IsValidTile(p.temp))
            {
                Console.Write("\t\t");
                // Console.BackgroundColor = ConsoleColor.DarkGreen;
                Tiles.PrintTile(p.temp);
                Console.ResetColor();
                Console.Write("🤏");                
            }
        }

        private static void PrintPlayerDiscards(Player p)
        {
            Console.Write("🗑️\t:\t");
            Tiles.PrintDeck(p.discards);
        }
        
        // 공용 덱에서 하나 타일을 뽑는다.
        public static Tiles.Tile Tsumo(ref Deck.PublicDeck deck)
        {
            if (deck.currentTileIndex < 0 || deck.currentTileIndex > Deck.MahjongMaxTiles)
            {
                Console.WriteLine("쯔모시 뭔가 잘못되었습니다 😱");
            }
            
            return deck.publicTiles[deck.currentTileIndex++];
        }

        public static void UserAddTempAndDiscardTile(ref Player p)
        {
            Stopwatch watch = new Stopwatch();
            Console.Clear();
            
            // 핸드에 temp 더하기
            p.hands[MaxHandTiles - 1] = p.temp;

            // 핸드 정렬
            Console.WriteLine("버릴 타일을 선택 해 주세요\n");
            Tiles.PrintDeck(p.hands);
            Console.Write("\n0 1 2 3 4 5 6 7 8 9 A B C D\n");
            
            ConsoleKeyInfo keyInfo;
            bool parseResult = false;
            int keyInt = 0;
            
            // 스트링 -> 16진수 변환하기
            // https://stackoverflow.com/questions/98559/how-to-parse-hex-values-into-a-uint            
            while (!parseResult)
            {
                keyInfo = Console.ReadKey();
                char key = keyInfo.KeyChar;
                parseResult = int.TryParse(key.ToString(), 
                    NumberStyles.HexNumber, CultureInfo.CurrentCulture, out keyInt);
                if (parseResult)
                {
                    Program.WaitUntilElapsedTime(watch, 200);
                    Console.WriteLine(" 선택한 숫자 : " + keyInt);
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 키를 입력하셨습니다");
                }                
            }
            DiscardTile(ref p, keyInt);
        }
        
        // 컴퓨터가 하는 행동
        // To-Do : 더 업그레이드 하면 좋겠지만 그냥 랜덤으로 뽑아서 버리자
        public static void AiAddTempAndDiscardTile(ref Player p)
        {
            // 핸드에 temp 더하기
            p.hands[MaxHandTiles - 1] = p.temp;            
            Random rand = new Random();
            DiscardTile(ref p, rand.Next(0,MaxHandTiles));
        }
        
        // 선택한 타일 Discard 핸드에 넣고 버리기
        // 정렬을 맨뒤가 하나 비어있는걸로 가정했기 때문에, 강제로 빈걸로 맨 뒤로 넣어준다.
        // 버림패는 무조건 공개
        public static void DiscardTile(ref Player p, int keyInt)
        {
            Tiles.Tile discard = p.hands[keyInt];
            discard.isShowingFront = true;
            p.hands[keyInt] = p.temp;
            p.hands[MaxHandTiles - 1] = new Tiles.Tile();

            int lastDiscard = FindLastDiscardInx(p);
            p.discards[lastDiscard] = discard;
            p.temp = new Tiles.Tile();
            
            Deck.SortMyHand(p);
        }

        // 비어있는 공간 찾기
        public static int FindLastDiscardInx(Player p)
        {
            for (int i = 0; i < p.discards.Length; i++)
            {
                if (!Tiles.IsValidTile(p.discards[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}