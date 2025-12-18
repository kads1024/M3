using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M3.Presentation.Animation;
using M3.Presentation.Board;
using M3.Presentation.Gem;
using M3.Presentation.Playback;
using M3.UnityAdapter;

namespace M3.Presentation.Animation
{
    public sealed class GravityGroupFallAnimation : IBoardAnimation
    {
        private readonly MonoBehaviour _host;
        private readonly BoardView _boardView;
        private readonly BoardSpatialMap _spatialMap;
        private readonly List<GravityFallInstruction> _falls;
        private readonly float _duration;

        public GravityGroupFallAnimation(
            MonoBehaviour host,
            BoardView boardView,
            BoardSpatialMap spatialMap,
            List<GravityFallInstruction> falls,
            float duration)
        {
            _host = host;
            _boardView = boardView;
            _spatialMap = spatialMap;
            _falls = falls;
            _duration = duration;
        }

        public IEnumerator Play()
        {
            var routines = new List<IEnumerator>();

            foreach (var fall in _falls)
            {
                var view = _boardView.GetGemViewAt(fall.From);
                if (view == null)
                    continue;

                var target = _spatialMap.GridToWorld(fall.To);
                routines.Add(view.AnimateMove(target, _duration));
            }

            if (routines.Count > 0)
                yield return CoroutineUtil.RunParallel(_host, routines.ToArray());
        }
    }
}