using System;

namespace Mahjong
{
    public interface IPlayable : IAction
    {
        ConsoleKey ReadActionKey();
        void PrintDiscard();
        int ReadDiscardKey();

    }
}