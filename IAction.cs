namespace Mahjong
{
    public interface IAction
    {
        void PrintTurn();
        void AddTemp(Tiles.Tile tile);
        void AddHand();        
        void DiscardTile(int tileNum, bool isRiichi);
        void Action(Game game);
        void Action(Game game, Player other);

        // 아래는 마작 역을 판단하는 액션
        void Riichi();
        void Tsumo(Game game);
        void Ron(Game game, Player other);
    }
}