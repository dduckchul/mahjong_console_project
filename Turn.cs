using System.Collections.Generic;

namespace Mahjong
{
    public class Turn
    {
        // 무르기시 이용할 히스토리, 추후 구현
        // private Stack<History> _playerHistory;
        private LinkedList<Player> _playerList;
        private LinkedListNode<Player> _currentPlayer;

        private LinkedList<Player> PlayerList
        {
            get { return _playerList; }
            set { _playerList = value; }
        }

        public LinkedListNode<Player> CurrentPlayer
        {
            get { return _currentPlayer; }
            private set { _currentPlayer = value; }
        }
        
        // 플레이어 링크드 리스트에 넣기, Player[] 동 남 서 북 순서대로 그대로 넣는다
        public Turn(Player[] players)
        {
            PlayerList = new LinkedList<Player>();
            foreach (Player p in players)
            {
                PlayerList.AddFirst(p);
            }
        }

        // 현재 턴의 플레이어 번환
        public Player StartCurrentTurn()
        {
            CurrentPlayer.Value.IsPlaying = true;
            return CurrentPlayer.Value;
        }

        // 플레이어 턴 종료시
        public void EndCurrentTurn()
        {
            CurrentPlayer.Value.IsPlaying = false;
            CurrentPlayer = CurrentPlayer.NextOrFirst();
        }
        
        // 게임마다 첫 번째 턴인 유저에게 턴을 준다. 링크드 리스트 만들어주고 현재 노드로 설정
        // 동 1국 = wind + 1 = (0+1) - 1 % 4 = 0 (동)
        // 동 2국 = wind + 2 = (0+2) - 1 % 4 = 1 (남)
        // 남 3국 = wind + 3 = (1+3) - 1 % 4 = 3 (북)
        // 북 4국 = wind + 4 = (3+4) - 1 % 4 = 6 % 4 = 2 (서)
        public void InitCurrentPlayer(Game game)
        {
            Game.Winds wind = game.Wind;
            int currentGame = game.Num;
            
            int moveTo = (int)(wind + currentGame - 1) % 4;
            CurrentPlayer = PlayerList.First;

            for (int i = 0; i < moveTo; i++)
            {
                CurrentPlayer = CurrentPlayer.NextOrFirst();
            }
        }
    }
}