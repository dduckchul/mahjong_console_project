using System;
using System.Threading;

namespace Mahjong
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Deck deck = new Deck();
            Tiles tiles = new Tiles();
            Players players = new Players();
            
            // 덱 초기화
            Tiles.Tile[] pilesOfTile = deck.MakeInitDeck();
            
            // 마작 패 초기화 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);
            
            // 마작 덱 셔플
            deck.ShuffleDeck(ref pilesOfTile);

            // 마작 패 셔플 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);
            
            // TO-DO : 실제 작동할때는 이름 입력받도록
            // Players.Player me = players.SetMyAvata("");
            Players.Player me = players.SetMyAvata("😅입력하기 귀찮아");
            Players.Player[] mahjongPlayers = players.InitPlayers(ref me);
            
            // 현재 분배중인 덱 인덱스
            int tileIndex = 0;
            int distributeTimes = 3;
            // 처음은 핸드 최대값 -1 만큼 분배, 분배를 n번으로 쪼개고싶다
            int wantToDistribute = (Players.MaxHandTiles-1) / distributeTimes;
            // 마지막 for-loop 에서 줘야하는 타일값
            int remainderTiles = (Players.MaxHandTiles-1) % distributeTimes;
            
            // 반복해서 13개 타일을 n번 분배하는 기능
            for (int i = 0; i < distributeTimes + 1; i++)
            {
                // wantToDistribute 만큼 타일 분배
                if (i < distributeTimes)
                {
                    for (int j = 0; j < mahjongPlayers.Length; j++)
                    {
                        Thread.Sleep(150);
                        players.TakeTiles(pilesOfTile, ref mahjongPlayers[j], wantToDistribute, tileIndex);
                        tileIndex += wantToDistribute;
                    }     
                }
                // 마지막 반복에서는 나머지 타일만 준다
                if (i == distributeTimes)
                {
                    for (int j = 0; j < mahjongPlayers.Length; j++)
                    {
                        Thread.Sleep(150);
                        players.TakeTiles(pilesOfTile, ref mahjongPlayers[j], remainderTiles, tileIndex);
                        tileIndex += remainderTiles;
                    }                    
                }
            }
        }
    }
}