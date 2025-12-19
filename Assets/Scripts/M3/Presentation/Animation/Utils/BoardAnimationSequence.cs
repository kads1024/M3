using System.Collections.Generic;
using UnityEngine;

namespace M3.Presentation.Animation.Utils
{
    [CreateAssetMenu(menuName = "Board Animations/Animation Sequence")]
    public sealed class BoardAnimationSequence : ScriptableObject
    {
        public BoardAnimationStep SwapAcceptedAnimationStep;
        public BoardAnimationStep SwapRejectedAnimationStep;
        public List<BoardAnimationStep> ResolutionAnimationSteps;
    }
}