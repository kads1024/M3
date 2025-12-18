using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Core.Domain;
using M3.Core.Domain.Swap;
using M3.Presentation.Animation;
using M3.Presentation.Board;
using M3.UnityAdapter;
using M3.UnityAdapter.Bootstrap;
using VContainer;


namespace M3.Presentation.Playback
{
    public sealed class BoardResolutionPlayback : MonoBehaviour
    {
        [SerializeField] private float _swapDuration = 0.25f;
        [SerializeField] private float _postSwapDelay = 0.15f;

        [Inject] private BoardBootstrapper _bootstrapper;
        [Inject] private ResolutionTraceBuilder _traceBuilder;

        public IEnumerator PlaySwap(
            bool accepted,
            Position a,
            Position b,
            BoardView boardView,
            BoardSnapshot before,
            BoardSnapshot after)
        {
            var spatialMap = _bootstrapper.SpatialMap;

            var gemA = boardView.GetGemViewAt(a);
            var gemB = boardView.GetGemViewAt(b);

            Vector3 aTarget = spatialMap.GridToWorld(b);
            Vector3 bTarget = spatialMap.GridToWorld(a);

            IBoardAnimation anim =
                accepted
                    ? new SwapAcceptedAnimation(this, gemA, gemB, aTarget, bTarget, _swapDuration)
                    : new SwapRejectedAnimation(this, gemA, gemB, aTarget, bTarget, _swapDuration);

            yield return anim.Play();

            yield return new WaitForSeconds(_postSwapDelay);

            if (!accepted)
                yield break;

            // Update logical positions immediately
            gemA.SetLogicalPosition(b);
            gemB.SetLogicalPosition(a);


            var trace = _traceBuilder.Build(
                _bootstrapper.Board,
                SwapContext.PlayerMove(a, b));


            foreach (var step in trace.Steps)
            {
                var clearedPositions = ResolveClearedPositions(step);
                var clearAnim = new ClearGemsAnimation(
                    this,
                    boardView,
                    clearedPositions,
                    2f);

                yield return clearAnim.Play();

                yield return new WaitForSeconds(0.1f);

                var spawns = SpawnDiff.Compute(step.Before, step.After);

                yield return new SpawnGemsAnimation(
                        this,
                        boardView,
                        _bootstrapper.SpatialMap,
                        spawns,
                        spawnDuration: 2f,
                        step.After)
                    .Play();
                
                yield return new WaitForSeconds(2f);
               
                var gravityAnim = new GravityWithSequentialFallAnimation(
                    this,
                    boardView,
                    _bootstrapper.SpatialMap,
                    step.Gravity,
                    fallDuration: 0.25f,
                    delayBetween: 0.05f);

                yield return gravityAnim.Play();

                yield return new WaitForSeconds(0.1f);
            }

// FINAL VISUAL SYNC

            boardView.ClearAllGemViews();
            boardView.RenderFromSnapshot(trace.Final);
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