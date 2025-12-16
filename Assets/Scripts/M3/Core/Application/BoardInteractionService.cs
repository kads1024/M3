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

        public SwapResult TrySwap(
            BoardState board,
            Position a,
            Position b)
        {
            // 1. Validate swap
            if (!_swapRule.IsSwapValid(board, a, b))
                return SwapResult.Rejected;

            // 2. Apply swap to the REAL board
            board.Swap(a, b);

            // 3. Resolve cascades (SYSTEM LOGIC)
            _cascadeSystem.Resolve(board, isPlayerMove: true, a, b);

            return SwapResult.Accepted;
        }
    }
}