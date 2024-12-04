namespace Mahjong
{
    public class Games
    {
        // 동장전만 구현, 더하고싶으면 MaxGameSize를 늘리자.
        public const int MaxGameSize = 4;
        public const int GameEndScore = 30000;
        
        public struct Game
        {
            // 현재 바람 확인 (동,남,서,북)
            public Winds currentWinds;
            // 현재 N번째 국인지 확인
            public int game;
            // 현재 N번째 장인지 저장할때
            public int set;
            // 현재 게임에서 플레이중인 유저를 기억하는 인덱스
            public int turn;
        }
        public enum Winds
        {
            East,South,West,North
        }
        
        public bool IsGameContinue(Players.Player[] players, Game currentGame)
        {
            int playerMaxScore = 0;
            int currentGamePassed = ((int)currentGame.currentWinds + 1) * currentGame.game;
            
            // 플레이어 점수 조건 탐색.
            // 1. 가장 점수 많은 사람 찾기
            // 2. 플레이어중 0점 아래인 사람 발견하면 즉시 게임 종료
            for (int i = 0; i < players.Length; i++)
            {
                int playerScore = players[i].score;
                if (playerScore > playerMaxScore)
                {
                    playerMaxScore = playerScore;
                }
                if (playerScore < 0)
                {
                    return false;
                }
            }

            // 1. 게임이 4 만큼 진행되고 (maxGameSize)
            // 2. 유저중 누군가 30000점이 넘었을 경우 게임 종료
            // 아니면 계속 연장전 처럼 진행~~
            if (playerMaxScore >= GameEndScore && currentGamePassed > MaxGameSize)
            {
                return false;
            }

            return true;
        }

        // 게임마다 첫 번째 턴인 유저에게 턴을 준다.
        public void FindFirstUser(Players.Player[] players, ref Game game)
        {
            
        }

        // 현재 플레이중인 유저의 인덱스를 반환한다.
        public int FindPlayingUserInx(Players.Player[] players)
        {
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].isPlaying == true)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}