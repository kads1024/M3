using System.Collections;
using UnityEngine;
using M3.Core.Domain;
using M3.Presentation.Animation;
using M3.Presentation.Board;
using M3.UnityAdapter.Bootstrap;
using VContainer;


namespace M3.Presentation.Playback
{
    public sealed class BoardResolutionPlayback : MonoBehaviour
    {
        [SerializeField] private float _swapDuration = 0.25f;
        [SerializeField] private float _postSwapDelay = 0.15f;
        
        [Inject] private BoardBootstrapper _bootstrapper;
        
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

            // STEP 1.5 — Clear animation (NEW)
            var cleared = ClearDiff.Compute(before, after);

            var clearAnim = new ClearGemsAnimation(
                this,
                boardView,
                cleared,
                2f);

            yield return clearAnim.Play();

            yield return new WaitForSeconds(2f);

            // STEP 2 — gravity animation
            var falls = GravityDiff.Compute(before, after);

            var gravityAnim = new GravitySequentialFallAnimation(
                this,
                boardView,
                spatialMap,
                falls,
                fallDuration: 0.25f,
                delayBetween: 0.05f);

            yield return gravityAnim.Play();


            // VISUAL CLEANUP ONLY
            boardView.ClearAllGemViews();
            boardView.RenderFromSnapshot(after);
        }
    }
}