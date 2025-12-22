using M3.Core.Application;
using M3.Core.Domain;
using M3.Presentation.Board;
using M3.Presentation.Playback;

namespace M3.Application
{
    public sealed class BoardResolutionCoordinator
    {
        private readonly BoardInteractionService _interactionService;
        private readonly BoardResolutionPlayback _playback;
        private readonly BoardView _boardView;

        public BoardResolutionCoordinator(
            BoardInteractionService interactionService,
            BoardResolutionPlayback playback,
            BoardView boardView)
        {
            _interactionService = interactionService;
            _playback = playback;
            _boardView = boardView;
        }

        public void TrySwap(BoardState board, Position a, Position b)
        {
            // Try swap first before animating
            var result = _interactionService.TrySwap(board, a, b);

            // Play board animations (swap - clear - gravity - spawn)
            _boardView.StartCoroutine(
                _playback.PlaybackBoardSnapshot(
                    result == SwapResult.Accepted,
                    a,
                    b));

        }
    }
}
