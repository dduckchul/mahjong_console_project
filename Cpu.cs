using System;

namespace Mahjong
{
    public class Cpu : Player, IAction
    {
        public Cpu(string name, Game.Winds wind) : base(name, wind) { }
        
        // 핸드에 temp 더하기
        public void PrintTurn()
        {
            int computerThinking = 3;
            long waitTime = 300;
            Console.Write($"{Name}님의 순서! ");
            Program.WaitUntilElapsedTime(waitTime);
            Console.Write("컴퓨터 생각중... ");
            for (int i = 0; i < computerThinking; i++)
            {
                Program.WaitUntilElapsedTime(waitTime);
                Console.Write("🤔");
            }
            Program.WaitUntilElapsedTime(waitTime);
        }

        public void AddTemp(Tiles.Tile tile)
        {
            Hands.Temp = tile;
        }

        public void AddHand()
        {
            Hands.MyTiles.Add(Hands.Temp);
        }

        // 컴퓨터가 하는 행동
        // To-Do : 더 업그레이드 하면 좋겠지만 그냥 랜덤으로 뽑아서 버리자
        public void DiscardTile(int tileNum, bool isRiichi)
        {
            DiscardMyHand(tileNum, isRiichi);
            Hands.SortMyHand();
        }        
        
        public void Action(Game game)
        {
            AddHand();
            PlayerYaku.InitYaku(Hands);
            PrintTurn();
            DiscardTile(Program.Random.Next(0, MaxHandTiles), false);

            // 내 덱이 론 할수 있을지 확인
            Player me = game.Me;
            if (me.PlayerYaku.CanRon(me, this))
            {
                (me as IAction)?.Action(game,this);
            }
        }

        // To-Do 아래는 컴퓨터가 자아를 가질 때 하는 행동들
        public void Action(Game game, Player other)
        {
            
        }

        // To-Do 컴퓨터 리치, 쯔모, 론 가능하도록..? 언제하니~~
        public void Riichi()
        {

        }

        public void Tsumo(Game game)
        {
            
        }

        public void Ron(Game game, Player other)
        {
            
        }
    }
}