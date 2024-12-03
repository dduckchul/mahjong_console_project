using System;

namespace Mahjong
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Deck deck = new Deck();
            Tiles tiles = new Tiles();
            Players players = new Players();

            // 4명이 없으면 못하는 게임 ㅠㅠ
            int playerNum = 4;
            
            // 마작 기본 점수
            int score = 25000;
            
            // 초기화
            Tiles.Tile[] pilesOfTile = deck.MakeInitDeck();
            
            // 마작 패 초기화 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);
            
            // 셔플
            deck.ShuffleDeck(ref pilesOfTile);

            // 마작 패 셔플 잘 됐는지 출력
            // tiles.PrintDeck(pilesOfTile);

            Players.Player me = players.SetMyAvata();
            Players.Player[] mahjongPlayers = players.InitPlayers(ref me);
            
            
        }
    }
}