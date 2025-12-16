using M3.Core.Domain;

namespace M3.Core.Application
{
    public interface IBoardInteractionService
    {
        SwapResult TrySwap(
            BoardState board,
            Position a,
            Position b);
    }
}