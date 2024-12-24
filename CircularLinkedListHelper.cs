using System.Collections.Generic;

namespace Mahjong
{
    public static class CircularLinkedListHelper
    {
        public static LinkedListNode<T> NextOrFirst<T>(this LinkedListNode<T> current)
        {
            return current.Next == null ? current.List.First : current.Next;
        }
        
        public static LinkedListNode<T> PrevOrLast<T>(this LinkedListNode<T> current)
        {
            return current.Previous == null ? current.List.Last : current.Previous;
        }        
    }
}