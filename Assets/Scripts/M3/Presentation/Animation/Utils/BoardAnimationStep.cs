
using M3.Presentation.Animation;
using UnityEngine;

namespace M3.Presentation.Animation.Utils
{
    public abstract class BoardAnimationStep : ScriptableObject
    {
        public abstract IBoardAnimation CreateRuntime(BoardAnimationContext context);
    }

}
