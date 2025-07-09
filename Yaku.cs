using System;
using System.Collections.Generic;

namespace Mahjong
{
    public class Yaku
    {
        // 비교를 위한 임시 핸드 생성
        private Deck.Hands _tempHands;
        private Dictionary<Tiles.Tile, int> _dictDeck;

        public Deck.Hands TempHands
        {
            get { return _tempHands; }
            private set { _tempHands = value; }
        }
        
        public Dictionary<Tiles.Tile, int> DictDeck
        {
            get { return _dictDeck; }
            private set { _dictDeck = value; }
        }

        // 무거운 연산은 자신의 턴일 때 한번만 (임시 핸드 만들어 역 비교)
        public void InitYaku(Deck.Hands hands)
        {
            MakeTempHands(hands);
            CountDeckByDict(TempHands.MyTiles);
        }
        
        // 비교하기 위해 임시 핸드를 만든다.
        private void MakeTempHands(Deck.Hands hands)
        {
            TempHands = new Deck.Hands();
            TempHands.MyTiles = new List<Tiles.Tile>(hands.MyTiles);
            TempHands.OpenedBodies = new List<Tiles.Tile[]>(hands.OpenedBodies);
            TempHands.SortMyHand();
            
            // 커쯔와 슌쯔를 확인하면서 TempHand에 임시 몸통으로 추가한다.
            DivideTriple(TempHands.MyTiles, 0, TempHands.OpenedBodies);
            DivideStraight(TempHands.MyTiles, 0, 1, TempHands.OpenedBodies);
        }
        
        private void CountDeckByDict(List<Tiles.Tile> deck)
        {
            DictDeck = new Dictionary<Tiles.Tile, int>();
            for (int i = 0; i < deck.Count; i++)
            {
                // 키가 있다면 count+1, 없다면 키 넣고 초기화
                if (DictDeck.ContainsKey(deck[i]))
                {
                    DictDeck[deck[i]]++;
                }
                else
                {
                    DictDeck.Add(deck[i], 1);
                }
            }
        }        
        
        public bool IsDeckHasHead(List<Tiles.Tile> deck)
        {
            for (int i = 0; i < deck.Count-1; i++)
            {
                if (deck[i].Equals(deck[i+1]))
                {
                    return true;
                }
            }
            return false;
        }

        public int CountHead(Dictionary<Tiles.Tile, int> deckDict)
        {
            int counter = 0;
            foreach (Tiles.Tile t in deckDict.Keys)
            {
                if (deckDict[t] == 2)
                {
                    counter++;
                }
            }
            return counter;
        }
        
        public bool CanRiichi(Player p)
        {
            if (p.IsCrying)
            {
                return false;
            }

            if (p.IsRiichi)
            {
                Console.Write("🎉 😻 리 치 😻 🎉 ");
                return false;
            }

            return IsTenpai(p);
        }

        // 텐파이 조건..? 헤드가 0개이거나, 몸통이 4개 or 헤드가 1개, 몸통이 3개
        // 14개 한번 정렬
        public bool IsTenpai(Player p)
        {
            bool hasHead = IsDeckHasHead(TempHands.MyTiles);
            int bodies = TempHands.OpenedBodies.Count;

            // 몸통만 완성인 경우
            if (hasHead == false && bodies == 4)
            {
                return true;
            }
            
            // 몸통 하나만 더 완성이면 되는 경우
            if (hasHead && bodies == 3)
            {
                // 머리가 2개이다
                if (CountHead(DictDeck) > 1)
                {
                    return true;                    
                }

                // 연속되는 몸통이 있다
                if (FindPossibleStraight(TempHands.MyTiles))
                {
                    return true;
                }
            }
            
            // 그외 다른 역들도 여기에 다 추가해줘~~~~~
            return false;
        }
        
        public bool CanTsumo(Player p)
        {
            bool hasHead = IsDeckHasHead(TempHands.MyTiles);
            int bodies = TempHands.OpenedBodies.Count;

            // 쯔모 할 수 있는 경우
            if (hasHead && bodies == 4)
            {
                return true;
            }

            return false;
        }

        // 내가(플레이어?) 방금 상대방이 버린 타일로 론 할수 있는지 확인
        public bool CanRon(Player me, Player target)
        {
            if (!IsTenpai(me))
            {
                return false;
            }

            List<Tiles.Tile> tempStore = TempHands.MyTiles;
            TempHands.MyTiles = new List<Tiles.Tile>(tempStore);
            
            List<Tiles.Tile[]> ronBodies = new List<Tiles.Tile[]>();
            TempHands.MyTiles.Add(target.LastDiscardTile);
            TempHands.SortMyHand();
            
            DivideTriple(TempHands.MyTiles, 0, ronBodies);
            DivideStraight(TempHands.MyTiles, 0, 1, ronBodies);

            bool hasHead = IsDeckHasHead(TempHands.MyTiles);
            int bodies = TempHands.OpenedBodies.Count + ronBodies.Count;
            
            // 임시 계산 후에 다시 원래대로~ 임시 리스트들도 다 클리어 해준다
            // 론 할 수 있는 경우
            if (hasHead && bodies == 4)
            {
                ronBodies.Clear();
                TempHands.MyTiles.Clear();
                TempHands.MyTiles = tempStore;
                return true;
            }
            
            ronBodies.Clear();
            TempHands.MyTiles.Clear();            
            TempHands.MyTiles = tempStore;
            return false;
        }
        
        public bool CanKang(Player p)
        {
            return false;
        }        
        
        public bool CanPong(Player other)
        {
            if (DictDeck.ContainsKey(other.LastDiscardTile) && DictDeck[other.LastDiscardTile] == 2)
            {
                return true;
            }

            return false;
        }

        public bool CanChi(Player other)
        {
            return false;
        }
        
        // 커쯔 있는지 확인, 재귀로 호출
        public void DivideTriple(List<Tiles.Tile> deck, int loop, List<Tiles.Tile[]> bodies)
        {
            // 재귀 종료조건, 임시 핸드 타일수가 3보다 작거나, loop가 타일 갯수 보다 클때
            if (deck.Count < 3) { return; }
            if (loop > deck.Count - 3) { return; }
            
            if (!deck[loop].Equals(deck[loop + 1]))
            {
                loop++;
                DivideTriple(deck, loop, bodies);
                return;
            }

            if (!deck[loop].Equals(deck[loop + 2]))
            {
                loop++;
                DivideTriple(deck, loop, bodies);
                return;
            }
            
            // 1,2와 비교, 1,3과 비교해서 다 같다면? 인덱스에서 빼고 body로 추가
            Tiles.Tile[] body = {deck[loop], deck[loop + 1], deck[loop + 2]};
            bodies.Add(body);
            deck.RemoveRange(loop, 3);
            
            // 0번 부터 다시 루프
            loop = 0;
            DivideTriple(deck, loop, bodies);
        }

        // 슌쯔 있는지 확인
        public void DivideStraight(List<Tiles.Tile> deck, int start, int ind, List<Tiles.Tile[]> bodies)
        {
            if (deck.Count < 3) { return; }
            // 출발점이 리스트 인덱스보다 크다
            if (start > deck.Count-1) { return; }
            // 슌쯔는 숫자패 에서만 통함
            if (!deck[start].IsNumberTile()) { return; }

            int tempInx = FindSequencial(deck, start, ind);
            int tempInx2 = -1;
            
            if (tempInx > 0)
            {
                tempInx2 = FindSequencial(deck, tempInx, tempInx+1);
            }
            
            // 시퀀셜한 인덱스가 2개다 -> 슌쯔임
            if (tempInx > 0 && tempInx2 > 0)
            {
                Tiles.Tile[] body = {deck[start], deck[tempInx], deck[tempInx2]};
                bodies.Add(body);

                // 인덱스 변동이 없도록 제일 큰 인덱스부터 하나씩 빼자 (-1씩 인덱스가 변함)
                deck.RemoveAt(tempInx2);
                deck.RemoveAt(tempInx);
                deck.RemoveAt(start);

                start = 0;
                ind = 1;
                DivideStraight(deck, start, ind, bodies);
                return;
            }

            start++;
            ind = start + 1;
            DivideStraight(deck, start, ind, bodies);
        }

        public int FindSequencial(List<Tiles.Tile> deck, int start, int ind)
        {
            // 다음 패가 현재 패에서 +1 한 숫자면 true, 같은 숫자면 한번 더 넘어감
            for (int i = ind; i < deck.Count; i++)
            {
                if (deck[start].Equals(deck[i]))
                {
                    continue;
                }
                
                // 같은 타입의 다음 숫자라면? 인덱스 반환
                if (deck[start].Type == deck[i].Type
                    && deck[start].Number + 1 == deck[i].Number)
                {
                    return i;
                }

                // 끝까지 왔으면 없는 케이스. 음수 인덱스를 리턴
                return -1;
            }
            // 포문 끝나버리면 없는 케이스. 음수 인덱스를 리턴
            return -1;
        }

        // 3개의 마작패에서 1개 빼면 슌쯔가 되는 패 찾아보기
        public bool FindPossibleStraight(List<Tiles.Tile> deck)
        {
            for (int i = 0; i < deck.Count - 1; i++)
            {
                if (!deck[i].IsNumberTile())
                {
                    continue; 
                }
                // 9면 스트레이트 안됨
                if (deck[i].Number >= 9)
                {
                    continue; 
                }

                if (deck[i].Type != deck[i + 1].Type)
                {
                    continue; 
                }

                if (deck[i].Equals(deck[i + 1]))
                {
                    continue; 
                }

                // 현재 타일과 다음 타일이 1차이 혹은 2차이가 난다면 리치 선언 할 수 있다.
                if (deck[i].Type == deck[i + 1].Type && deck[i].Number + 1 == deck[i + 1].Number)
                {
                    return true;
                }

                if (deck[i].Type == deck[i + 1].Type && deck[i].Number + 2 == deck[i + 1].Number)
                {
                    return true;
                }
            }

            return false;
        }
    }
}