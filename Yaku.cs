using System;
using System.Collections.Generic;

namespace Mahjong
{
    public static class Yaku
    {
        public static bool IsDeckHasHead(Deck.Hands hands)
        {
            for (int i = 0; i < hands.MyTiles.Count -1 ; i++)
            {
                if (hands.MyTiles[i].Equals(hands.MyTiles[i+1]))
                {
                    return true;
                }
            }
            return false;
        }
        
        public static int CountBodies(Deck.Hands hands, Deck.Hands tempHands)
        {
            return hands.OpenedBodies.Count + tempHands.OpenedBodies.Count;
        }

        // To-Do 리치 판단에 0.5초걸림 ㄷㄷ 성능 개선?
        public static bool CanRiichi(Player p)
        {
            if (p.IsCrying)
            {
                Console.WriteLine("이미 울었어요 😭");
                return false;
            }

            if (p.IsRiichi)
            {
                Console.Write("🎉 리치!! 🎉 ");
                return false;
            }

            return IsTenpai(p);
        }

        // 텐파이 조건..? 헤드가 0개이거나, 몸통이 4개 or 헤드가 1개, 몸통이 3개
        // 14개 한번 정렬
        public static bool IsTenpai(Player p)
        {
            // 비교하기 위해 임시 핸드를 만든다.
            Deck.Hands tempHands = new Deck.Hands();
            tempHands.MyTiles = new List<Tiles.Tile>(p.Hands.MyTiles);
            tempHands.OpenedBodies = new List<Tiles.Tile[]>();
            tempHands.SortMyHand();
            
            // 커쯔와 슌쯔를 확인하면서 TempHand에 임시 몸통으로 추가한다.
            DivideTriple(tempHands, 0);
            DivideStraight(tempHands, 0, 1);
            
            // 머리 판별은 위에 tempHands에서만 해도 됨
            bool hasHead = IsDeckHasHead(tempHands);
            int bodies = CountBodies(p.Hands, tempHands);

            // 몸통만 완성인 경우
            if (hasHead == false && bodies == 4)
            {
                return true;
            }
            
            // 몸통 하나만 더 완성이면 되는 경우
            if (hasHead && bodies == 3)
            {
                return true;
            }
            
            // 그외 다른 역들도 여기에 다 추가해줘~~~~~
            return false;
        }
        
        public static bool CanTsumo(Player p)
        {
            return false;
        }

        public static bool CanKang(Player p)
        {
            return false;
        }

        public static bool CanRon(Player other)
        {
            return false;
        }
        
        public static bool CanPong(Player other)
        {
            return false;
        }

        public static bool CanChi(Player other)
        {
            return false;
        }
        
        // 커쯔 있는지 확인, 재귀로 호출
        public static void DivideTriple(Deck.Hands hands, int loop)
        {
            // 재귀 종료조건, 임시 핸드 타일수가 3보다 작거나, loop가 타일 갯수 보다 클때
            if (hands.MyTiles.Count < 3) { return; }
            if (loop > hands.MyTiles.Count - 3) { return; }
            
            if (!hands.MyTiles[loop].Equals(hands.MyTiles[loop + 1]))
            {
                loop++;
                DivideTriple(hands, loop);
                return;
            }

            if (!hands.MyTiles[loop].Equals(hands.MyTiles[loop + 2]))
            {
                loop++;
                DivideTriple(hands, loop);
                return;
            }
            
            // 1,2와 비교, 1,3과 비교해서 다 같다면? 인덱스에서 빼고 body로 추가
            Tiles.Tile[] body = 
            {
                hands.MyTiles[loop], 
                hands.MyTiles[loop + 1], 
                hands.MyTiles[loop + 2]
            };
            
            hands.OpenedBodies.Add(body);
            hands.MyTiles.RemoveRange(loop, 3);
            loop = 0;
            DivideTriple(hands, loop);
        }

        // 슌쯔 있는지 확인
        public static void DivideStraight(Deck.Hands hands, int start, int ind)
        {
            if (hands.MyTiles.Count < 3)
            {
                return;
            }

            // 출발점이 리스트 인덱스보다 크다
            if (start > hands.MyTiles.Count-1)
            {
                return;
            }
            
            if (!hands.MyTiles[start].IsNumberTile())
            {
                return;
            }

            int tempInx = FindSequencial(hands, start, ind);
            int tempInx2 = -1;
            
            if (tempInx > 0)
            {
                tempInx2 = FindSequencial(hands, tempInx, tempInx+1);
            }
            
            // 시퀀셜한 인덱스가 2개다 -> 슌쯔임
            if (tempInx > 0 && tempInx2 > 0)
            {
                Tiles.Tile[] body = 
                {
                    hands.MyTiles[start], 
                    hands.MyTiles[tempInx], 
                    hands.MyTiles[tempInx2]
                };
                hands.OpenedBodies.Add(body);

                // 이해하기 쉽게 제일 큰 인덱스부터 하나씩 빼자 (-1씩 인덱스가 변함)
                hands.MyTiles.RemoveAt(tempInx2);
                hands.MyTiles.RemoveAt(tempInx);
                hands.MyTiles.RemoveAt(start);

                start = 0;
                ind = 1;
                DivideStraight(hands, start, ind);
                return;
            }

            start++;
            ind = start + 1;
            DivideStraight(hands, start, ind);
        }

        public static int FindSequencial(Deck.Hands hands, int start, int ind)
        {
            // 다음 패가 현재 패에서 +1 한 숫자면 true, 같은 숫자면 한번 더 넘어감
            for (int i = ind; i < hands.MyTiles.Count-1; i++)
            {
                if (hands.MyTiles[start].Type != hands.MyTiles[i].Type)
                {
                    return -1;
                }

                if (hands.MyTiles[start].Equals(hands.MyTiles[i]))
                {
                    continue;
                }
                
                if (hands.MyTiles[start].Number + 1 == hands.MyTiles[i].Number)
                {
                    return i;
                }

                // 끝까지 왔으면 false 리턴
                return -1;
            }
            // 포문 끝나버리면 다음패가 없다, false
            return -1;
        }
    }
}