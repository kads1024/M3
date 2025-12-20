using UnityEngine;
using M3.Core.Domain;

namespace M3.Presentation.Board
{
    public sealed class CellView : MonoBehaviour
    {
        public Position Position { get; private set; }

        public void Initialize(Position position)
        {
            Position = position;
        }
    }
}