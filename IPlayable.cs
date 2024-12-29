using System;

namespace Mahjong
{
    public interface IPlayable : IAction
    {
        ConsoleKey ReadActionKey();
        ConsoleKey ReadActionKey(Player other);
        void PrintDiscard();
        int ReadDiscardKey();
        bool PrintTurn(Player other);
    }
}