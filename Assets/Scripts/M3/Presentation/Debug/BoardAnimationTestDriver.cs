using UnityEngine;
using M3.Core.Domain;
using M3.Presentation.Gem;
using M3.Presentation.Animation;
using M3.Presentation.Board;
using M3.Presentation.Playback;
using M3.UnityAdapter;

namespace M3.Presentation.Debug
{
    public sealed class BoardAnimationTestDriver : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoardView _boardView;
        [SerializeField] private BoardSpatialMap _spatialMap;

        [Header("Swap Test Input")]
        [SerializeField] private int _fromX;
        [SerializeField] private int _fromY;
        [SerializeField] private int _toX;
        [SerializeField] private int _toY;

        [Header("Timing")]
        [SerializeField] private float _swapDuration = 0.25f;

        // =========================
        // Test Buttons
        // =========================

        [ContextMenu("Test Swap Accepted")]
        public void TestSwapAccepted()
        {
            if (!TryGetGems(out var a, out var b))
                return;

            var anim = new SwapAcceptedAnimation(
                this,
                a,
                b,
                _spatialMap.GridToWorld(new Position(_toX, _toY)),
                _spatialMap.GridToWorld(new Position(_fromX, _fromY)),
                _swapDuration);

            StartCoroutine(anim.Play());
        }

        [ContextMenu("Test Swap Rejected")]
        public void TestSwapRejected()
        {
            if (!TryGetGems(out var a, out var b))
                return;

            var anim = new SwapRejectedAnimation(
                this,
                a,
                b,
                _spatialMap.GridToWorld(new Position(_toX, _toY)),
                _spatialMap.GridToWorld(new Position(_fromX, _fromY)),
                _swapDuration);

            StartCoroutine(anim.Play());
        }

        // [ContextMenu("Test Gravity Group Fall")]
        // public void TestGravityGroupFall()
        // {
        //     var before = new BoardSnapshot(_boardView);
        //     // Manually modify board visually or via test setup
        //     var after = new BoardSnapshot(_boardView.Board);
        //
        //     var falls = GravityDiff.Compute(before, after);
        //
        //     var anim = new GravityGroupFallAnimation(
        //         this,
        //         _boardView,
        //         _spatialMap,
        //         falls,
        //         0.3f);
        //
        //     StartCoroutine(anim.Play());
        // }
        
        // =========================
        // Helpers
        // =========================

        private bool TryGetGems(out GemView a, out GemView b)
        {
            a = _boardView.GetGemViewAt(new Position(_fromX, _fromY));
            b = _boardView.GetGemViewAt(new Position(_toX, _toY));

            if (a == null || b == null)
            {
                UnityEngine.Debug.LogWarning("Invalid positions: GemView not found.");
                return false;
            }

            return true;
        }
        
    }
}
