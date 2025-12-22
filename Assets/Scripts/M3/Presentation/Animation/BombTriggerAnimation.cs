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
        private readonly MonoBehaviour _host;
        
        public BombTriggerAnimation(
            MonoBehaviour host,
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
            _host = host;
        }

        public IEnumerator Play()
        {
            Coroutine flashRoutine = null;

            var bombView = _boardView.GetGemViewById(_trigger.BombGemId);
            if (bombView)
            {
                // Start flashing in PARALLEL 
                flashRoutine = _host.StartCoroutine(
                    bombView.Flashing(
                        duration: float.MaxValue,   
                        pulseScale: 1.2f));
            }
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
            
            // Stop flashing BEFORE clearing bomb
            if (flashRoutine != null && bombView)
            {
                bombView.StopCoroutine(flashRoutine);
            }
            
            if (bombView)
            {
                yield return bombView.ClearGem(0.1f);
                _boardView.DestroyGemViewAt(bombView.Position);
            }
            
        }
    }
}