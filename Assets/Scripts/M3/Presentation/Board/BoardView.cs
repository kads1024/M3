using System.Collections;
using UnityEngine;
using VContainer;
using System.Collections.Generic;
using M3.Core.Domain;
using M3.Presentation.Gem;
using M3.Presentation.Playback;

namespace M3.Presentation.Board
{
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Sprite _boardBackgroundSprite;
        [SerializeField] private Color _boardBackgroundColor = Color.white;
        [SerializeField] private float _backgroundPadding = 0.2f; // extra space around cells
        
        [SerializeField] private GemView _gemPrefab;
        private GemViewPool _pool; // BoardView Owns the pool. So no need to inject
        [SerializeField] private Sprite _cellSprite;

        [Inject] private BoardState _board;
        [Inject] private BoardSpatialMap _spatialMap;

        // Identity-based tracking
        private readonly List<GemView> _activeViews = new();

        private void Start()
        {
            _pool = new GemViewPool(_gemPrefab, transform);
            RenderBoardBackground();
            RenderCells();
            RenderInitialBoard();
        }

        private void RenderBoardBackground()
        {
            float width  = _board.Width  * _spatialMap.CellSize;
            float height = _board.Height * _spatialMap.CellSize;

            var bg = new GameObject("BoardBackground");
            bg.transform.SetParent(transform);

            var renderer = bg.AddComponent<SpriteRenderer>();
            renderer.sprite = _boardBackgroundSprite;
            renderer.color = _boardBackgroundColor;
            renderer.sortingOrder = -10; // behind everything

            // Center of the board
            Vector3 center = _spatialMap.Origin +
                             new Vector3(
                                 width  * 0.5f,
                                 height * 0.5f,
                                 0f);

            bg.transform.position = center;

            // Scale background to fit board
            bg.transform.localScale = new Vector3(
                width  + _backgroundPadding,
                height + _backgroundPadding,
                1f);
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
            }
        }

        public GemView CreateGemView(Position pos, GemState gem)
        {
            var view = _pool.Get();

            view.Initialize(pos, gem);

            view.transform.position = _spatialMap.GridToWorld(pos);
            view.transform.localScale = Vector3.one * _spatialMap.CellSize;
            view.transform.SetParent(transform);

            _activeViews.Add(view);
            return view;
        }
        
        private void RenderCells()
        {
            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var pos = new Position(x, y);

                var go = new GameObject($"Cell ({x},{y})");
                go.transform.SetParent(transform);

                var renderer = go.AddComponent<SpriteRenderer>();
                renderer.sprite = _cellSprite;
                renderer.sortingOrder = 0; // behind gems

                go.transform.position = _spatialMap.GridToWorld(pos);
                go.transform.localScale = Vector3.one * _spatialMap.CellSize;
            }
        }

        
        // public void SyncWithBoard()
        // {
        //     var usedViews = new HashSet<GemView>();
        //     var nextViews = new List<GemView>();
        //
        //     for (int x = 0; x < _board.Width; x++)
        //     for (int y = 0; y < _board.Height; y++)
        //     {
        //         var pos = new Position(x, y);
        //         var cell = _board.GetCell(x, y);
        //
        //         if (cell.IsEmpty)
        //             continue;
        //
        //         var view = FindReusableView(usedViews);
        //
        //         if (view == null)
        //         {
        //             view = CreateGemView(pos, cell.Gem);
        //         }
        //         else
        //         {
        //             // view.SetPosition(pos);
        //             view.UpdateVisual(cell.Gem);
        //             view.transform.position = _spatialMap.GridToWorld(pos);
        //         }
        //
        //         usedViews.Add(view);
        //         nextViews.Add(view);
        //     }
        //
        //     // Destroy unused views
        //     foreach (var view in _activeViews)
        //     {
        //         if (!usedViews.Contains(view))
        //         {
        //             Destroy(view.gameObject);
        //         }
        //     }
        //
        //     _activeViews.Clear();
        //     _activeViews.AddRange(nextViews);
        // }

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
        
        public GemView GetGemViewById(int id)
        {
            foreach (var view in _activeViews)
            {
                if (view.State.Id == id)        
                    return view;
            }
            return null;
        }
        
        public void ClearAllGemViews()
        {
            _pool.ReleaseAll(_activeViews);
            _activeViews.Clear();
        }

        public void DestroyGemViewAt(Position pos)
        {
            var view = GetGemViewAt(pos);
            if (view == null)
                return;

            _activeViews.Remove(view);
            _pool.Release(view);
        }
        
        public void RenderFromSnapshot(BoardSnapshot snapshot)
        {
            foreach (var kv in snapshot.Cells)
            {
                var view = CreateGemView(kv.Key, kv.Value);
            }
        }
    }
}
