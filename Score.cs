using System.Collections.Generic;

namespace Mahjong
{
    public class Score
    {
        public static class ScoreExtensions
        {
            public static List<ScoreOne> FindScoreOne()
            {
                List<ScoreOne> scoreList = new List<ScoreOne>();
                return scoreList;
            }

            public static List<ScoreTwo> FindScoreTwo()
            {
                List<ScoreTwo> scoreList = new List<ScoreTwo>();
                return scoreList;
            }

            public static List<ScoreThree> FindScoreThree()
            {
                List<ScoreThree> scoreList = new List<ScoreThree>();
                return scoreList;
            }

            public static List<ScoreSix> FindScoreSix()
            {
                List<ScoreSix> scoreList = new List<ScoreSix>();
                return scoreList;
            }
        }
        
        public enum ScoreOne
        {
            // 리치 (멘젠), 탕야오, 멘젠쯔모 (멘젠), 
            RIICHI, TANYAO, MENZENTSUMO, 
            // 자풍패, 장풍패, 삼원패 (백발중 3개)
            MYWIND, GAMEWIND, WORDS,
            // 이페코(멘젠, 123 123 한쌍), 영상개화, 해저로월
            IPPECO, LASTTSUMO, LASTLON,
            // 일발, 도라, 적도라, 역은 아님
            ILBAL, DORA, AKADORA
            // 만들기 까다로운 애들 핑후(멘젠), 창깡, 
            // PINGHU, CHANGKKANG
        }

        public enum ScoreTwo
        {
            // 더블리치(멘젠), 삼색동각, 산깡쯔(깡만 3개)
            DOUBLERIICHI, TRIPLESAMENUMBER, TRIPLEFOURBODIES,
            // 또이또이(커쯔나 깡쯔 4개와 머리), 산안커(울지않은 커쯔 3개), 소삼원(백발중을 몸통2개 머리 하나로)
            DDOIDDOI, SANANKOU, SHOUSANGEN,
            // 혼노두(요구패만 있다), 치또이츠, 
            HONNODU, SEVENHEADS, 
            // 여기서부터 울면 -1점
            // 찬타(몸4개와 머리1개 모두 요구패가 있음), 일기통관, 삼색동순
            CHANTA, STRAIGHT, THREECOLORSTRAIGHT
        }

        public enum ScoreThree
        {
            // 량페코(멘젠, 이페코가 두개),
            // 준찬타(자패 없이 노두패만), 혼일색(한종류의 수패와 자패) 울면 -1점
            DOUBLEIPPECO, JUNCHANTA, HONIL
        }

        public enum ScoreSix
        {
            // 청일색 숫자패 하나만으로 모든 몸통과 머리
            CHUNGIL
        }
        
        // 나머지는~~~~~ 시간되면 하자자ㅏ자자자자
        public static int AddScore(Player p)
        {
            return 0;
        }
    }
}