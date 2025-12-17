using M3.Core.Application;
using M3.Core.Domain;
using M3.Presentation.Board;
using M3.Presentation.Playback;
using M3.UnityAdapter;
using M3.UnityAdapter.Bootstrap;
using UnityEngine;

namespace M3.Application
{
    public sealed class BoardResolutionCoordinator
    {
        private readonly BoardInteractionService _service;
        private readonly BoardResolutionPlayback _playback;
        private readonly BoardView _boardView;


        public BoardResolutionCoordinator(
            BoardInteractionService service,
            BoardResolutionPlayback playback,
            BoardView boardView)
        {
            _service = service;
            _playback = playback;
            _boardView = boardView;
        }

            public void TrySwap(BoardState board, Position a, Position b)
            {
                var before = new BoardSnapshot(board);

                var result = _service.TrySwap(board, a, b);

                var after = new BoardSnapshot(board);

                _boardView.StartCoroutine(
                    _playback.PlaySwap(
                        result == SwapResult.Accepted,
                        a,
                        b,
                        _boardView,
                        after));
            }
    }

}