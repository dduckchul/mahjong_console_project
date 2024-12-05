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

            // 유저 키 입력 변수
            ConsoleKeyInfo keyInfo;
            
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
            Players.PrintPlayers(mahjongPlayers);

            Deck.PublicDeck publicDeck = deck.MakePublicDeck(pilesOfTile);
            // publicDeck 구성 잘 되었는지 확인
            // Console.WriteLine(publicDeck);
            
            // 각 플레이어 손패 정렬
            foreach (Players.Player pl in mahjongPlayers)
            {
                Deck.SortMyHand(pl);                
            }

            // 정렬 후 프린트 화면
            WaitUntilElapsedTime(watch, 500);
            Players.PrintPlayers(mahjongPlayers);            
            
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
                
                Players.PrintPlayers(mahjongPlayers);
                PrintTurnAndAction(mahjongPlayers[userInx]);
                PressKeyAndAction(ref mahjongPlayers[userInx] ,watch);
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
                        Players.PrintPlayers(mahjongPlayers);
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
                        Players.PrintPlayers(mahjongPlayers);
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

        public static void PrintTurnAndAction(Players.Player player)
        {
            Console.Write($"{player.name}님의 차례입니다 ");
            Console.Write("1️⃣ 버리기 ");
            Console.Write("2️⃣ 리치 ");
            Console.Write("3️⃣ 쯔모 ");
            Console.Write("0️⃣ 종료");
            Console.WriteLine();
        }

        public static void PressKeyAndAction(ref Players.Player player, Stopwatch watch)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.D1)
            {
                Console.WriteLine("버릴 타일을 선택해 주세요");
                Players.UserAddTempAndDiscardTile(ref player);
                WaitUntilElapsedTime(watch, 200);
            }
            
            if (keyInfo.Key == ConsoleKey.D2)
            {
                Console.WriteLine("🚜👷리치 구현중....⛏️");
                WaitUntilElapsedTime(watch, 200);
            }

            if (keyInfo.Key == ConsoleKey.D3)
            {
                Console.WriteLine("🚜👷쯔모 구현중....⛏️");
                WaitUntilElapsedTime(watch, 200);
            }

            if (keyInfo.Key == ConsoleKey.D0)
            {
                isRunning = false;
                Console.WriteLine("게임을 종료합니다..👋");
                WaitUntilElapsedTime(watch, 200);
            }
        }
    }
}