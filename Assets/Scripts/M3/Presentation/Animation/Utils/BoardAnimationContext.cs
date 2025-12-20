using M3.Core.Domain;
using M3.Presentation.Board;
using UnityEngine;

namespace M3.Presentation.Animation.Utils
{
    public sealed class BoardAnimationContext
    {
        public BoardView BoardView;
        public BoardSpatialMap SpatialMap;
        public ResolutionTrace Trace;
        public CascadeStep Step;
        public MonoBehaviour Host;
    }

}