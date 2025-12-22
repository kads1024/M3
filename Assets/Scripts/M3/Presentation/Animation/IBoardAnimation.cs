using System.Collections;

namespace M3.Presentation.Animation
{
    /// <summary>
    /// Defines a single cascade step animation
    /// </summary>
    public interface IBoardAnimation
    {
        IEnumerator Play();
    }
}