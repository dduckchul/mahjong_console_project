using System;

namespace Mahjong
{
    public interface IPlayable : IAction
    {
        ConsoleKey ReadActionKey();
        ConsoleKey ReadActionKey(Player other);
        void PrintDiscard();
        int ReadDiscardKey();
        void PrintTurn(Player other);
    }
}