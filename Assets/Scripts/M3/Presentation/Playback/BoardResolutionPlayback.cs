using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Core.Domain;
using M3.Presentation.Animation;
using M3.Presentation.Board;
using M3.Presentation.Gem;
using M3.UnityAdapter;
using M3.UnityAdapter.Bootstrap;
using VContainer;

namespace M3.Presentation.Playback
{
    public sealed class BoardResolutionPlayback : MonoBehaviour
    {
        [SerializeField] private float _swapDuration = 0.25f;
        [SerializeField] private float _postSwapDelay = 0.15f;
        
        public IEnumerator PlaySwap(
            bool accepted,
            Position a,
            Position b,
            BoardView boardView,
            BoardBootstrapper bootstrapper,
            BoardSnapshot finalState)
        {
            var spatialMap = bootstrapper.SpatialMap;
            
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

            // VISUAL CLEANUP ONLY
            boardView.ClearAllGemViews();
            boardView.RenderFromSnapshot(finalState);
        }
    }
}