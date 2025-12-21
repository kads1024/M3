using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Core.Domain;
using M3.Core.Domain.Swap;
using M3.Presentation.Animation;
using M3.Presentation.Board;

using VContainer;


namespace M3.Presentation.Playback
{
    public sealed class BoardResolutionPlayback : MonoBehaviour
    {
        [Header("Swap Config")]
        [SerializeField] private float _swapDuration = 0.25f;
        [SerializeField] private float _postSwapDelay = 0.15f;
        
        [Header("Clear Config")]
        [SerializeField] private float _clearDuration = 0.25f;
        [SerializeField] private float _postClearDelay = 0.15f;
        
        [Header("Bomb Config")]
        [SerializeField] private float _preNeighborExplodeDelay = 0.25f;
        [SerializeField] private float _neighborExplodeDuration = 0.25f;
        [SerializeField] private float _preBombClearDelay = 0.15f;
        [SerializeField] private float _postBombPlacementDelay = 0.15f;
        
        [Header("Gravity Config")]
        [SerializeField] private float _fallDuration = 0.25f;
        [SerializeField] private float _delayBetweenGem = 0.1f;
 
        [Inject] private ResolutionTraceBuilder _traceBuilder;
        [Inject] private BoardView _boardView;
        [Inject] private BoardSpatialMap _spatialMap;
        [Inject] private BoardState _board;
        

        
        public IEnumerator PlaybackBoardSnapshot(
            bool accepted,
            Position a,
            Position b)
        {
            var gemA = _boardView.GetGemViewAt(a);
            var gemB = _boardView.GetGemViewAt(b);

            Vector3 aTarget = _spatialMap.GridToWorld(b);
            Vector3 bTarget = _spatialMap.GridToWorld(a);

            IBoardAnimation anim = new SwapAnimation(this, gemA, gemB, aTarget, bTarget, _swapDuration, accepted);

            yield return anim.Play();

            yield return new WaitForSeconds(_postSwapDelay);

            if (!accepted)
                yield break;

            // Update logical positions immediately
            gemA.SetLogicalPosition(b);
            gemB.SetLogicalPosition(a);


            var trace = _traceBuilder.Build(
                _board,
                SwapContext.PlayerMove(a, b));


            foreach (var step in trace.Steps)
            {
                // Bomb triggers 
                foreach (var trigger in step.BombTriggers)
                {
                    var bombAnim = new BombTriggerAnimation(
                        _boardView,
                        trigger,
                        neighborClearDelay: _neighborExplodeDuration,
                        bombClearDelay: _preBombClearDelay,
                        preNeighborClearDelay:_preNeighborExplodeDelay);

                    yield return bombAnim.Play();
                }
                
                var clearedPositions = ResolveClearedPositions(step);
                var clearAnim = new ClearGemsAnimation(
                    this,
                    _boardView,
                    clearedPositions,
                    _clearDuration);

                yield return clearAnim.Play();

                yield return new WaitForSeconds(_postClearDelay);
                
           

                if (step.BombPlacement != null)
                {
                    var bp = step.BombPlacement;

                    // Create bomb view IN PLACE
                    var bombGem = step.After.Cells[step.After.CellsById[bp.BombGemId]];

                    var view = _boardView.CreateGemView(
                        bp.Position,
                        bombGem);

                    view.SetLogicalPosition(bp.Position);

                    view.transform.position =
                        _spatialMap.GridToWorld(bp.Position);
                }
                
                yield return new WaitForSeconds(_postBombPlacementDelay);
                var spawns = SpawnDiff.Compute(step.Before, step.After);

                
                yield return new SpawnGemsAnimation(
                        this,
                        _boardView,
                        _spatialMap,
                        spawns,
                        spawnDuration: 0.01f,
                        step.After)
                    .Play();
                
                var gravityAnim = new GravityParallelColumnsSequentialAnimation(
                    this,
                    _boardView,
                    _spatialMap,
                    step.Gravity,
                    fallDuration: _fallDuration,
                    delayBetween: _delayBetweenGem);

                yield return gravityAnim.Play();
            }

// FINAL VISUAL SYNC

            _boardView.ClearAllGemViews();
            _boardView.RenderFromSnapshot(trace.Final);
        }

        
        private static List<Position> ResolveClearedPositions(
            CascadeStep step)
        {
            var positions = new List<Position>();

            foreach (var gemId in step.ClearedGemIds)
            {
                if (step.Before.TryFindById(gemId, out var pos))
                {
                    positions.Add(pos);
                }
            }

            return positions;
        }
    }
}