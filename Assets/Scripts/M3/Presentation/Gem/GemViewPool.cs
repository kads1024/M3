using System.Collections.Generic;
using UnityEngine;

namespace M3.Presentation.Gem
{
    /// <summary>
    /// Object pooling for GemView (specific for now since we don't need pooling for other game objects for now)
    /// </summary>
    public sealed class GemViewPool
    {
        private readonly GemView _prefab;
        private readonly Transform _root;

        private readonly Stack<GemView> _inactive = new();
        private readonly HashSet<GemView> _active = new();

        public int ActiveCount => _active.Count;
        public int InactiveCount => _inactive.Count;

        public GemViewPool(GemView prefab, Transform root)
        {
            _prefab = prefab;
            _root = root;
        }

        public GemView Get()
        {
            GemView view;

            if (_inactive.Count > 0)
            {
                view = _inactive.Pop();
            }
            else
            {
                view = Object.Instantiate(_prefab, _root);
            }

            view.gameObject.SetActive(true);
            _active.Add(view);

            return view;
        }

        public void Release(GemView view)
        {
            if (view == null)
                return;

            if (!_active.Remove(view))
                return;

            view.ResetView(); // IMPORTANT: clears state & visuals
            view.gameObject.SetActive(false);
            view.transform.SetParent(_root);

            _inactive.Push(view);
        }

        public void ReleaseAll(IEnumerable<GemView> views)
        {
            foreach (var view in views)
            {
                Release(view);
            }
        }
    }
}