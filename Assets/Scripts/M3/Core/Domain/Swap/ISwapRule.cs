namespace M3.Core.Domain.Swap
{
    public interface ISwapRule
    {
        bool IsSwapValid(
            BoardState board,
            Position a,
            Position b);
    }

}