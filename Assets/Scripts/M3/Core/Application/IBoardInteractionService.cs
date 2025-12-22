using M3.Core.Domain;

namespace M3.Core.Application
{
    /// <summary>
    /// Responsible for accepting swap input from Input layer
    /// </summary>
    public interface IBoardInteractionService
    {
        SwapResult TrySwap(
            BoardState board,
            Position a,
            Position b);
    }
}