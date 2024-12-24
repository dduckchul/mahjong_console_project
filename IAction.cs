using System;

namespace Mahjong
{
    public interface IAction
    {
        void PrintTurn();
        void AddTemp(Tiles.Tile tile);
        void AddHand();        
        void DiscardTile(int tileNum);
        void Action(Tiles.Tile tile);
    }
}