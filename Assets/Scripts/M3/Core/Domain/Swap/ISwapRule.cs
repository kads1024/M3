namespace M3.Core.Domain.Swap
{
    /// <summary>
    /// Rules for a valid swap
    /// </summary>
    public interface ISwapRule
    {
        bool IsSwapValid(
            BoardState board,
            Position a,
            Position b);
    }

}