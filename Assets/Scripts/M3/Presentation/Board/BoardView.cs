using UnityEngine;
using VContainer;
using System.Collections.Generic;
using M3.Core.Domain;
using M3.UnityAdapter.Bootstrap;
using M3.UnityAdapter;
using M3.Presentation.Gem;
using M3.Presentation.Playback;

namespace M3.Presentation.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [Inject] private BoardBootstrapper _bootstrapper;

        private BoardState _board;
        private BoardSpatialMap _spatialMap;

        // Identity-based tracking
        private readonly List<GemView> _activeViews = new();

        private void Start()
        {
            _board = _bootstrapper.Board;
            _spatialMap = _bootstrapper.SpatialMap;

            RenderInitialBoard();
        }

        private void RenderInitialBoard()
        {
            _activeViews.Clear();

            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var cell = _board.GetCell(x, y);
                if (cell.IsEmpty)
                    continue;

                var pos = new Position(x, y);
                var view = CreateGemView(pos, cell.Gem);
                _activeViews.Add(view);
            }
        }

        private GemView CreateGemView(Position pos, GemState gem)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
            go.name = $"GemView ({pos.X},{pos.Y})";
            go.transform.SetParent(transform);

            var view = go.AddComponent<GemView>();
            view.Initialize(pos, gem);

            go.transform.position = _spatialMap.GridToWorld(pos);
            go.transform.localScale *= _spatialMap.CellSize;

            return view;
        }
        
        public void SyncWithBoard()
        {
            var usedViews = new HashSet<GemView>();
            var nextViews = new List<GemView>();

            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var pos = new Position(x, y);
                var cell = _board.GetCell(x, y);

                if (cell.IsEmpty)
                    continue;

                var view = FindReusableView(usedViews);

                if (view == null)
                {
                    view = CreateGemView(pos, cell.Gem);
                }
                else
                {
                    // view.SetPosition(pos);
                    view.UpdateVisual(cell.Gem);
                    view.transform.position = _spatialMap.GridToWorld(pos);
                }

                usedViews.Add(view);
                nextViews.Add(view);
            }

            // Destroy unused views
            foreach (var view in _activeViews)
            {
                if (!usedViews.Contains(view))
                {
                    Destroy(view.gameObject);
                }
            }

            _activeViews.Clear();
            _activeViews.AddRange(nextViews);
        }

        private GemView FindReusableView(HashSet<GemView> used)
        {
            foreach (var view in _activeViews)
            {
                if (!used.Contains(view))
                    return view;
            }

            return null;
        }
        
        
        public GemView GetGemViewAt(Position pos)
        {
            foreach (var view in _activeViews)
            {
                if (view.Position.Equals(pos))
                    return view;
            }
            return null;
        }
        
        public void ClearAllGemViews()
        {
            foreach (var view in _activeViews)
                Destroy(view.gameObject);

            _activeViews.Clear();
        }

        public void DestroyGemViewAt(Position pos)
        {
            var view = GetGemViewAt(pos);
            if (view == null)
                return;

            _activeViews.Remove(view);
            Destroy(view.gameObject);
        }
        
        public void RenderFromSnapshot(BoardSnapshot snapshot)
        {
            foreach (var kv in snapshot.Cells)
            {
                var view = CreateGemView(kv.Key, kv.Value);
                _activeViews.Add(view);
            }
        }


    }
}
