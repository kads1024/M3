using M3.Core.Domain;
using M3.Core.Domain.Swap;

namespace M3.Core.Application
{
    public sealed class BoardInteractionService : IBoardInteractionService
    {
        private readonly ISwapRule _swapRule;

        public BoardInteractionService(ISwapRule swapRule)
        {
            _swapRule = swapRule;
        }

        public SwapResult TrySwap(BoardState board, Position a, Position b)
        {
            if (!_swapRule.IsSwapValid(board, a, b))
                return SwapResult.Rejected;

            board.Swap(a, b);
            return SwapResult.Accepted;
        }
    }
}