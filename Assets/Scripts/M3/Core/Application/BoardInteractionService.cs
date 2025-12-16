using M3.Core.Domain;
using M3.Core.Domain.Swap;
using M3.Core.System;

namespace M3.Core.Application
{
    public sealed class BoardInteractionService : IBoardInteractionService
    {
        private readonly ISwapRule _swapRule;
        private readonly ICascadeSystem _cascadeSystem;

        public BoardInteractionService(
            ISwapRule swapRule,
            ICascadeSystem cascadeSystem)
        {
            _swapRule = swapRule;
            _cascadeSystem = cascadeSystem;
        }

        public SwapResult TrySwap(BoardState board, Position a, Position b)
        {
            if (!_swapRule.IsSwapValid(board, a, b))
                return SwapResult.Rejected;

            board.Swap(a, b);

            _cascadeSystem.Resolve(
                board,
                SwapContext.PlayerMove(a, b));

            return SwapResult.Accepted;
        }
    }
}