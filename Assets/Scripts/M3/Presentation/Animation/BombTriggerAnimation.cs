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
        private readonly float _preBombClearDelay;
        private readonly float _preNeighborClearDelay;
        
        public BombTriggerAnimation(
            BoardView boardView,
            BombTrigger trigger,
            float neighborClearDelay,
            float bombClearDelay,
            float preNeighborClearDelay)
        {
            _boardView = boardView;
            _trigger = trigger;
            _neighborClearDelay = neighborClearDelay;
            _preBombClearDelay = bombClearDelay;
            _preNeighborClearDelay = preNeighborClearDelay;
        }

        public IEnumerator Play()
        {
            yield return new WaitForSeconds(_preNeighborClearDelay);
            
            // Clear neighbors FIRST
            foreach (var gemId in _trigger.AffectedGemIds)
            {
                if (gemId == _trigger.BombGemId) continue;
                
                var view = _boardView.GetGemViewById(gemId);
                
                if (!view) continue;
                
                yield return view.ClearGem(_neighborClearDelay);
                _boardView.DestroyGemViewAt(view.Position);
            }
            
            //  Clear the bomb LAST
            yield return new WaitForSeconds(_preBombClearDelay);
            
            var bombView = _boardView.GetGemViewById(_trigger.BombGemId);
            if (bombView)
            {
                yield return bombView.ClearGem(0.1f);
                _boardView.DestroyGemViewAt(bombView.Position);
            }
            
        }
    }
}