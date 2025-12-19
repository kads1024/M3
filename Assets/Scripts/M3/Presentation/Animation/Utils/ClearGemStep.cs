using System.Collections.Generic;
using M3.Core.Domain;
using M3.Presentation.Animation;
using UnityEngine;

namespace M3.Presentation.Animation.Utils
{
    [CreateAssetMenu(menuName = "Board Animations/Steps/Clear Gems Step")]
    public sealed class ClearGemsStep : BoardAnimationStep
    {
        [SerializeField] private float duration = 0.2f;

        public override IBoardAnimation CreateRuntime(BoardAnimationContext context)
        {
            return new ClearGemsAnimation(
                context.Host,
                context.BoardView,
                new List<Position>(),
                duration);
        }
    }

}