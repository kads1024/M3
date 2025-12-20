using System.Collections;
using M3.Core.Domain.Bomb;
using UnityEngine;
using M3.Presentation.Board;

namespace M3.Presentation.Animation
{
    public sealed class BombTriggerAnimation : IBoardAnimation
    {
        private readonly BoardView _boardView;
        private readonly BombTrigger _trigger;
        private readonly float _neighborClearDelay;
        private readonly float _bombClearDelay;

        public BombTriggerAnimation(
            BoardView boardView,
            BombTrigger trigger,
            float neighborClearDelay,
            float bombClearDelay)
        {
            _boardView = boardView;
            _trigger = trigger;
            _neighborClearDelay = neighborClearDelay;
            _bombClearDelay = bombClearDelay;
        }

        public IEnumerator Play()
        {
            // Clear neighbors FIRST
            foreach (var gemId in _trigger.AffectedGemIds)
            {
                var view = _boardView.GetGemViewById(gemId);
                if (view != null)
                {
                    _boardView.DestroyGemViewAt(view.Position);
                }
            }

            yield return new WaitForSeconds(_neighborClearDelay);
//  Clear the bomb LAST
            var bombView = _boardView.GetGemViewById(_trigger.BombGemId);
            if (bombView != null)
            {
                _boardView.DestroyGemViewAt(bombView.Position);
            }

            yield return new WaitForSeconds(_bombClearDelay);
            
        }
    }
}