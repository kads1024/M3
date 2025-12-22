using UnityEngine;
using M3.Core.Domain;

namespace M3.Presentation.Board
{
    /// <summary>
    /// Visual Representation of a logical Cell
    /// </summary>
    public sealed class CellView : MonoBehaviour
    {
        public Position Position { get; private set; }

        public void Initialize(Position position)
        {
            Position = position;
        }
    }
}