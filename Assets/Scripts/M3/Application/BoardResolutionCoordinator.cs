using M3.Core.Application;
using M3.Core.Domain;
using M3.Presentation.Board;
using UnityEngine;

namespace M3.Application
{
    public sealed class BoardResolutionCoordinator
    {
        private readonly BoardInteractionService _interactionService;
        private readonly BoardView _boardView;

        public BoardResolutionCoordinator(
            BoardInteractionService interactionService,
            BoardView boardView)
        {
            _interactionService = interactionService;
            _boardView = boardView;
        }

        public void TrySwap(BoardState board, Position a, Position b)
        {
            var result = _interactionService.TrySwap(board, a, b);
            Debug.Log($"Swap ({a.X}, {a.Y}) -> ({b.X}, {b.Y}): {result}");
            
            if (result == SwapResult.Accepted)
            {
                _boardView.SyncWithBoard();
            }
        }
    }
}