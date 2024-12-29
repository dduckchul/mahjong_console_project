using System;
using System.Globalization;

namespace Mahjong
{
    public class Human : Player, IPlayable
    {
        public Human(string name, Game.Winds wind) : base(name, wind) { }

        // 빈칸으로 두면 입력창 받도록, 귀찮아서 이름 넘김
        public static Player SetMyAvata(string playerName)
        {
            Console.WriteLine("당신의 이름을 입력해 주세요 🤔");
            
            if (playerName == "")
            {
                playerName = Console.ReadLine();
            }

            Console.WriteLine($"안녕하세요~ {playerName}님");
            return new Human(playerName, Game.Winds.East);
        }        

        // 핸드에 temp 더하기
        public void PrintTurn()
        {
            Program.WaitUntilElapsedTime(300);
            Console.Write($"{Name}님의 순서! ");
            Console.Write("1️⃣  버리기 ");
            
            if (PlayerYaku.CanRiichi(this))
            {
                Console.Write("2️⃣  리치 ");
            }
            
            if (PlayerYaku.CanTsumo(this))
            {
                Console.Write("3️⃣  쯔모 ");
            }

            if (PlayerYaku.CanKang(this))
            {
                Console.Write("4️⃣  깡 ");
            }

            Console.Write("0️⃣  종료");
            Console.WriteLine("");            
        }

        public bool PrintTurn(Player other)
        {
            bool canAction = false;
            Console.WriteLine("");
            if (PlayerYaku.CanRon(this, other))
            {
                Console.Write("1️⃣  론 ");
                canAction = true;
            } 
            
            if (PlayerYaku.CanPong(other))
            {
                Console.Write("2️⃣  퐁 ");
                canAction = true;
            }
            
            if (PlayerYaku.CanChi(other))
            {
                Console.Write("3️⃣  치 ");
                canAction = true;                
            }
            
            if (canAction)
            {
                Console.Write("0️⃣  스킵 ");
                Console.WriteLine();
                Console.Write("상대의 버림패 :\t");
                other.LastDiscardTile.PrintTile();
                Console.WriteLine();
            }

            return canAction;
        }

        public void AddTemp(Tiles.Tile tile)
        {
            tile.IsShowingFront = true;
            Hands.Temp = tile;
        }
        
        public void AddHand()
        {
            Hands.MyTiles.Add(Hands.Temp);
        }
        
        public void DiscardTile(int tileNum, bool isRiichi)
        {
            DiscardMyHand(tileNum, isRiichi);
            Hands.SortMyHand();
        }

        public ConsoleKey ReadActionKey()
        {
            // 기능 구현중 or 잘못된 키 판별 하는 변수
            bool isFalseKey = true;
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            
            while (isFalseKey)
            {
                if (keyInfo.Key == ConsoleKey.D1)
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D2 && PlayerYaku.CanRiichi(this))
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D3 && PlayerYaku.CanTsumo(this))
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D4 && PlayerYaku.CanKang(this))
                {
                    Console.WriteLine("🚜👷깡 구현중....⛏️");
                } else if (keyInfo.Key == ConsoleKey.D0)
                {
                    isFalseKey = false;
                    Program.IsRunning = false;
                    Console.WriteLine("게임을 종료합니다..👋");
                }
                else
                {
                    Console.WriteLine("잘못된 키입니다.");
                }
                // 틀린 키일 때 한번 더
                if (isFalseKey)
                {
                    keyInfo = Console.ReadKey(true);
                }
            }

            return keyInfo.Key;
        }
        
        public ConsoleKey ReadActionKey(Player other)
        {
            // 기능 구현중 or 잘못된 키 판별 하는 변수
            bool isFalseKey = true;
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            while (isFalseKey)
            {
                if (keyInfo.Key == ConsoleKey.D1 && PlayerYaku.CanRon(this, other))
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D2 && PlayerYaku.CanPong(other))
                {
                    isFalseKey = false;
                } else if (keyInfo.Key == ConsoleKey.D3 && PlayerYaku.CanChi(other))
                {
                    Console.WriteLine("🚜👷치 구현중....⛏️");   
                }
                else if (keyInfo.Key == ConsoleKey.D0)
                {
                    isFalseKey = false;
                    Console.WriteLine("스킵 합니다");
                }
                else
                {
                    Console.WriteLine("잘못된 키입니다.");
                }
                // 틀린 키일 때 한번 더
                if (isFalseKey)
                {
                    keyInfo = Console.ReadKey(true);
                }
            }
            return keyInfo.Key;
        }

        public void PrintDiscard()
        {
            Program.PrintClear();
            Console.WriteLine("버릴 타일을 선택 해 주세요\n");
            Tiles.PrintDeck(Hands.MyTiles);
            Console.Write("\n0 1 2 3 4 5 6 7 8 9 A B C D\n");            
        }
        
        public int ReadDiscardKey()
        {
            ConsoleKeyInfo keyInfo;
            bool parseResult = false;
            int keyInt = 0;
            
            // 스트링 -> 16진수 변환하기, 0~13 까지 체크하고 넘으면 다시 입력할수 있도록
            // https://stackoverflow.com/questions/98559/how-to-parse-hex-values-into-a-uint            
            while (!parseResult || keyInt > Hands.MyTiles.Count-1) 
            {
                keyInfo = Console.ReadKey();
                parseResult = int.TryParse(keyInfo.KeyChar.ToString(), 
                    NumberStyles.HexNumber, CultureInfo.CurrentCulture, out keyInt);
                if (parseResult && keyInt < MaxHandTiles)
                {
                    Program.WaitUntilElapsedTime(100);
                    Console.WriteLine(" 선택한 숫자 : " + keyInt);
                }
                else
                {
                    Console.WriteLine("잘못된 키를 입력하셨습니다");
                }                
            }

            return keyInt;
        }

        // 핸드에 임시 타일 추가, 역 계산, 행동 표시, 입력키에 따른 행동
        public void Action(Game game)
        {
            AddHand();
            PlayerYaku.InitYaku(Hands);
            PrintTurn();
            switch (ReadActionKey())
            {
                case ConsoleKey.D0 : break;
                case ConsoleKey.D1 :
                {
                    UserDiscardAction();
                    break;
                }
                case ConsoleKey.D2:
                {
                    Riichi();
                    break;
                }
                case ConsoleKey.D3:
                {
                    Tsumo(game);
                    break;
                }                
            }
        }

        public void Action(Game game, Player other)
        {
            bool canAction = PrintTurn(other);
            if (canAction)
            {
                switch (ReadActionKey(other))
                {
                    case ConsoleKey.D0 : break;
                    case ConsoleKey.D1 :
                    {
                        Ron(game, other);
                        break;
                    }
                    case ConsoleKey.D2 :
                    {
                        Pong(game, other);
                        break;
                    }                    
                }                
            }
        }

        public void UserDiscardAction()
        {
            PrintDiscard();
            DiscardTile(ReadDiscardKey(), false);
        }

        public void Riichi()
        {
            Score -= 1000;
            IsRiichi = true;
            PrintDiscard();
            DiscardTile(ReadDiscardKey(), true);
        }
        
        public void Tsumo(Game game)
        {
            Program.PrintTsumo();
            
            Score score = new Score(this, true);
            score.CalculateScore(game);

            // 부모가 났을 경우, 세트만 종료
            if (game.Turns.FindInitPlayer(game).Equals(this))
            {
                game.EndSet();
            }
            // 자식이 났을경우, 다음 게임으로
            else
            {
                game.EndGame();
            }
         }

        public void Ron(Game game, Player other)
        {
            Program.PrintRon();

            // 론 했으면 마지막 타일을 출력 & 계산 위해 내 핸드의 Temp와 Yaku에 담아줌
            Hands.Temp = other.LastDiscardTile;
            PlayerYaku.TempHands.MyTiles.Add(other.LastDiscardTile);
            PlayerYaku.InitYaku(PlayerYaku.TempHands);
            
            Score score = new Score(this, false);
            score.CalculateScore(game, other);

            // 부모가 났을 경우, 세트만 종료
            if (game.Turns.FindInitPlayer(game).Equals(this))
            {
                game.EndSet();
            }
            // 자식이 났을경우, 다음 게임으로
            else
            {
                game.EndGame();
            }
        }

        public void Pong(Game game, Player other)
        {
            Tiles.Tile otherTile = other.LastDiscardTile;
            other.Hands.Discards.Remove(otherTile);

            int tempInx = 0;
            Tiles.Tile[] body = new Tiles.Tile[3];

            // 2개까지 담고 지우기
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < Hands.MyTiles.Count; j++)
                {
                    if (otherTile.Equals(Hands.MyTiles[j]))
                    {
                        body[tempInx] = Hands.MyTiles[j];
                        tempInx++;
                        Hands.MyTiles.RemoveAt(j);
                    }
                }                
            }

            // 공개 패에 선언
            body[tempInx] = otherTile;
            Hands.OpenedBodies.Add(body);
            
            // 내 손패 버리고 업데이트
            UserDiscardAction();
            PlayerYaku.InitYaku(Hands);

            // 울기 표시 & 턴 뺏어버리기
            IsCrying = true;
            game.Turns.FindAndSetCurrent(this);
        }

        public void Chi(Game game, Player other)
        {
            // 턴 뺏어버리기 예제
            // Turns.FindAndSetCurrent(me);
        }
    }
}