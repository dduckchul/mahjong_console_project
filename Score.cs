using System;
using System.Collections.Generic;

namespace Mahjong
{
    public class Score
    {
        private Dictionary<ScoreOne, int> _scoreOneDict;
        private Dictionary<ScoreTwo, int> _scoreTwoDict;
        private Dictionary<ScoreThree, int> _scoreThreeDict;
        private Dictionary<ScoreSix, int> _scoreSixDict;
        private Player _winner;

        private int DefaultPan = 1000; 

        public Score(Player winner, bool isTsumo)
        {
            _scoreOneDict = new Dictionary<ScoreOne, int>();
            _scoreTwoDict = new Dictionary<ScoreTwo, int>();
            _scoreThreeDict = new Dictionary<ScoreThree, int>();
            _scoreSixDict = new Dictionary<ScoreSix, int>();
            _winner = winner;
            IsTsumo = isTsumo;
        }

        public Dictionary<ScoreOne, int> ScoreOneDict
        {
            get { return _scoreOneDict; }
        }
        public Dictionary<ScoreTwo, int> ScoreTwoDict
        {
            get { return _scoreTwoDict; }
        }
        public Dictionary<ScoreThree, int> ScoreThreeDict
        {
            get { return _scoreThreeDict; }
        }
        public Dictionary<ScoreSix, int> ScoreSixDict
        {
            get { return _scoreSixDict; }
        }

        public Player Winner
        {
            get { return _winner; }
        }

        public bool IsTsumo
        {
            get;
            private set;
        }
        
        public enum ScoreOne
        {
            // 리치 (멘젠), 탕야오, 멘젠쯔모 (멘젠), 
            리치, 
            탕야오, 
            멘젠, 
            // 자풍패, 장풍패, 삼원패 (백발중 3개)
            자풍패, 
            장풍패, 
            삼원패,
            // 이페코(멘젠, 123 123 한쌍), 영상개화, 해저로월
            이페코, 
            영상개화,
            해저로월,
            // 일발, 도라, 적도라, 역은 아님
            일발,
            도라, 
            적도라,
            // 만들기 까다로운 애들 핑후(멘젠), 창깡, 
            // PINGHU,
            // CHANGKKANG
        }

        public enum ScoreTwo
        {
            // 더블리치(멘젠), 삼색동각, 산깡쯔(깡만 3개)
            DOUBLERIICHI, 
            TRIPLESAMENUMBER, 
            TRIPLEFOURBODIES,
            // 또이또이(커쯔나 깡쯔 4개와 머리), 산안커(울지않은 커쯔 3개), 소삼원(백발중을 몸통2개 머리 하나로)
            DDOIDDOI, 
            SANANKOU, 
            SHOUSANGEN,
            // 혼노두(요구패만 있다), 치또이츠, 
            HONNODU, 
            SEVENHEADS, 
            // 여기서부터 울면 -1점
            // 찬타(몸4개와 머리1개 모두 요구패가 있음), 일기통관, 삼색동순
            CHANTA, 
            STRAIGHT, 
            THREECOLORSTRAIGHT
        }

        public enum ScoreThree
        {
            // 량페코(멘젠, 이페코가 두개),
            DOUBLEIPPECO, 
            // 준찬타(자패 없이 노두패만)
            JUNCHANTA, 
            // 혼일색(한종류의 수패와 자패) 울면 -1점
            HONIL
        }

        public enum ScoreSix
        {
            // 청일색 숫자패 하나만으로 모든 몸통과 머리
            CHUNGIL
        }
        
        // 나머지는~~~~~ 시간되면 하자자ㅏ자자자자
        public void CalculateScore(Game game)
        {
            FindScoreOne(game);

            int score = 0;
            
            foreach (ScoreOne key in ScoreOneDict.Keys)
            {
                score += ScoreOneDict[key];
            }

            score = score * DefaultPan;
            
            PrintScore(game);
        }

        private void PrintScore(Game game)
        {
            Program.PrintClear();
            game.PrintDoraTiles();
            if (Winner.IsRiichi)
            {
                Console.WriteLine();
                game.PrintUraDoraTiles();                
            }
            Program.WaitUntilElapsedTime(500);            
            
            Console.WriteLine("\n");
            game.PrintHeadInfo();
            Console.WriteLine();
            Winner.PrintPlayer();
            Program.WaitUntilElapsedTime(500);

            Console.WriteLine();
            if (ScoreOneDict.Count > 0)
            {
                foreach (ScoreOne key in ScoreOneDict.Keys)
                {
                    Console.Write(key + ":\t\t" + ScoreOneDict[key]);
                    Program.WaitUntilElapsedTime(1000);
                    Console.WriteLine();
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("계속하려면 아무키나 눌러주세요");
            Console.ReadKey();            
        }

        private void FindScoreOne(Game game)
        {
            FindRiichi();
            FindTanyao();
            FindMenzenTsumo();
            FindMyWind();
            FindGameWind(game);
            FindWords();
            FindLastScore(game);
            FindDora(game);
            FindAkaDora();
        }

        public void FindRiichi()
        {
            if (Winner.IsRiichi)
            {
                ScoreOneDict.Add(ScoreOne.리치, 1);
            }
        }

        public void FindTanyao()
        {
            foreach (Tiles.Tile t in Winner.PlayerYaku.TempHands.MyTiles)
            {
                if (t.IsYoguTile())
                {
                    return;
                }
            }

            foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
            {
                foreach (Tiles.Tile t in body)
                {
                    if (t.IsYoguTile())
                    {
                        return;
                    }                    
                }
            }
            
            ScoreOneDict.Add(ScoreOne.탕야오, 1);
        }

        // 쯔모 계산?
        // 1. 지금 난 유저가 울지 않았다.
        // 2. 스코어 계산시 게임의 현재 턴의 유저가 스코어 계산 유저와 같다면? 쯔모로 났다.
        // 변수로 쯔모 플래그 선언 해줘도 되는데, 유저에 플래그 변수 더이상 늘리기 싫다.
        public void FindMenzenTsumo()
        {
            if (!Winner.IsCrying && IsTsumo)
            {
                ScoreOneDict.Add(ScoreOne.멘젠, 1);
            }
        }

        // 자풍패일 경우
        public void FindMyWind()
        {
            foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
            {
                if (body[0].Type == Tiles.TileType.Wind && body[0].Number == (int)Winner.Wind)
                {
                    ScoreOneDict.Add(ScoreOne.자풍패, 1);
                }
            }
        }

        // 장풍패인경우
        public void FindGameWind(Game game)
        {
            foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
            {
                if (body[0].Type == Tiles.TileType.Wind && body[0].Number == (int)game.Wind)
                {
                    ScoreOneDict.Add(ScoreOne.장풍패, 1);
                }
            }            
        }

        // 삼원패인경우
        public void FindWords()
        {
            foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
            {
                if (body[0].Type == Tiles.TileType.Word)
                {
                    ScoreOneDict.Add(ScoreOne.삼원패, 1);
                }
            }
        }

        // 마지막 턴일때, 내가 났으면 영상개화
        // 마지막 턴일때, 론이면 하저로월
        public void FindLastScore(Game game)
        {
            if (game.PublicDeck.PublicStack.Count == 0 && IsTsumo)
            {
                ScoreOneDict.Add(ScoreOne.영상개화, 1);
            } else if (game.PublicDeck.PublicStack.Count == 0)
            {
                ScoreOneDict.Add(ScoreOne.해저로월, 1);                
            }
        }

        // 도라 타일의 +1 패가 도라 점수 1점
        public void FindDora(Game game)
        {
            int doraIndex = game.PublicDeck.CurrentDoraIndex;
            
            Tiles.Tile[] doraTiles = game.PublicDeck.DoraTiles;
            Tiles.Tile[] uraDaraTiles = game.PublicDeck.UraDoraTiles;

            for (int i = 0; i < doraIndex; i++)
            {
                Tiles.Tile bonusDora = GetBonusDora(doraTiles[i]);
                foreach (Tiles.Tile t in Winner.PlayerYaku.TempHands.MyTiles)
                {
                    // 키 있는, 없는 케이스
                    if (t.Equals(bonusDora) && ScoreOneDict.ContainsKey(ScoreOne.도라))
                    {
                        ScoreOneDict[ScoreOne.도라] += 1;
                    } else if (t.Equals(bonusDora))
                    {
                        ScoreOneDict.Add(ScoreOne.도라, 1);
                    }
                }
                
                foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
                {
                    foreach (Tiles.Tile t in body)
                    {
                        // 키 있는, 없는 케이스
                        if (t.Equals(bonusDora) && ScoreOneDict.ContainsKey(ScoreOne.도라))
                        {
                            ScoreOneDict[ScoreOne.도라] += 1;
                        } else if (t.Equals(bonusDora))
                        {
                            ScoreOneDict.Add(ScoreOne.도라, 1);
                        }
                    }
                }
            }

            // 리치인경우 뒷도라도 오픈
            if (Winner.IsRiichi)
            {
                for (int i = 0; i < doraIndex; i++)
                {
                    Tiles.Tile bonusDora = GetBonusDora(uraDaraTiles[i]);
                    foreach (Tiles.Tile t in Winner.PlayerYaku.TempHands.MyTiles)
                    {
                        // 키 있는, 없는 케이스
                        if (t.Equals(bonusDora) && ScoreOneDict.ContainsKey(ScoreOne.도라))
                        {
                            ScoreOneDict[ScoreOne.도라] += 1;
                        } else if (t.Equals(bonusDora))
                        {
                            ScoreOneDict.Add(ScoreOne.도라, 1);
                        }
                    }
                    
                    foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
                    {
                        foreach (Tiles.Tile t in body)
                        {
                            // 키 있는, 없는 케이스
                            if (t.Equals(bonusDora) && ScoreOneDict.ContainsKey(ScoreOne.도라))
                            {
                                ScoreOneDict[ScoreOne.도라] += 1;
                            } else if (t.Equals(bonusDora))
                            {
                                ScoreOneDict.Add(ScoreOne.도라, 1);
                            }
                        }
                    }
                }
            }
        }

        private Tiles.Tile GetBonusDora(Tiles.Tile dora)
        {
            Tiles.TileType type = dora.Type;
            int bonusNum = dora.Number + 1;

            // 보너스 번호가 일반적인 타일 값을 넘어갈때 예외처리들
            if (Tiles.TileType.Wind == type && bonusNum == (int)Tiles.Winds.End)
            {
                bonusNum = (int)Tiles.Winds.East;
            }
                
            if (Tiles.TileType.Word == type && bonusNum == (int)Tiles.Words.End)
            {
                bonusNum = (int)Tiles.Words.Blank;
            }

            if (Tiles.IsNumberType(type) && bonusNum > 9)
            {
                bonusNum = 1;                    
            }
            
            return new Tiles.Tile(type, bonusNum, false);
        }

        public void FindAkaDora()
        {
            foreach (Tiles.Tile t in Winner.PlayerYaku.TempHands.MyTiles)
            {
                if (t.IsDora && ScoreOneDict.ContainsKey(ScoreOne.적도라))
                {
                    ScoreOneDict[ScoreOne.적도라] += 1;
                } else if (t.IsDora)
                {
                    ScoreOneDict.Add(ScoreOne.적도라, 1);
                }
            }

            foreach (Tiles.Tile[] body in Winner.PlayerYaku.TempHands.OpenedBodies)
            {
                foreach (Tiles.Tile t in body)
                {
                    if (t.IsDora && ScoreOneDict.ContainsKey(ScoreOne.적도라))
                    {
                        ScoreOneDict[ScoreOne.적도라] += 1;
                    } else if (t.IsDora)
                    {
                        ScoreOneDict.Add(ScoreOne.적도라, 1);
                    }
                }
            }
        }
    }
}