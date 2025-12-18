using M3.Core.Application;
using M3.Core.Domain;
using M3.Core.Domain.Swap;
using M3.Presentation.Board;
using M3.Presentation.Playback;
using M3.UnityAdapter;

namespace M3.Application
{
    public sealed class BoardResolutionCoordinator
    {
        private readonly BoardInteractionService _interactionService;
        private readonly ResolutionTraceBuilder _traceBuilder;
        private readonly BoardResolutionPlayback _playback;
        private readonly BoardView _boardView;

        public BoardResolutionCoordinator(
            BoardInteractionService interactionService,
            ResolutionTraceBuilder traceBuilder,
            BoardResolutionPlayback playback,
            BoardView boardView)
        {
            _interactionService = interactionService;
            _traceBuilder = traceBuilder;
            _playback = playback;
            _boardView = boardView;
        }

        public void TrySwap(BoardState board, Position a, Position b)
        {
            // Snapshot BEFORE swap
            var before = new BoardSnapshot(board);

            // Try swap (NO CASCADE)
            var result = _interactionService.TrySwap(board, a, b);

            // Snapshot AFTER swap only
            var after = new BoardSnapshot(board);

            // Play swap animation (accepted or rejected)
            _boardView.StartCoroutine(
                _playback.PlaySwap(
                    result == SwapResult.Accepted,
                    a,
                    b,
                    _boardView,
                    before,
                    after));

        }
    }
}
