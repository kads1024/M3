using System;
using System.Collections.Generic;
using System.Linq;
using M3.Application;
using M3.Application.Input;
using M3.Core.Application;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;
using M3.Presentation;
using M3.Presentation.Board;
using M3.Presentation.Playback;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace M3.Infrastructure.DI
{
    public sealed class BoardBootstrapper : LifetimeScope
    {
        [Header("Board Config")]
        [SerializeField] private int _boardSeed = 0;
        [SerializeField] private int _gemSpawnSeed = 0;
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;

        [Header("Spatial Config")]
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private Vector2 _boardOriginOffset = Vector3.zero;

        private BoardState _board;
        private BoardSpatialMap _spatialMap; 
        
        private System.Random _random;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Domain logic
            builder.Register<IMatchDetector, LineMatchDetector>(Lifetime.Singleton);
            builder.Register<MatchClassifier>(Lifetime.Singleton);
            builder.Register<ISwapRule, AdjacentSwapRule>(Lifetime.Singleton);
            builder.Register<IBombCreationRule, BombCreationRule>(Lifetime.Singleton);
            builder.Register<IBombResolver, BombResolver>(Lifetime.Singleton);
            builder.Register<ResolutionTraceBuilder>(Lifetime.Singleton);
            // Systems
            builder.Register<IGravitySystem, GravitySystem>(Lifetime.Singleton);
            builder.Register<IGemSpawner, GemSpawner>(Lifetime.Singleton).WithParameter("seed", _gemSpawnSeed);
            builder.Register<ICascadeSystem, CascadeSystem>(Lifetime.Singleton);

            // Application layer
            builder.Register<BoardInteractionService>(Lifetime.Singleton);
            
            // Unity Interaction Layer
            // builder.RegisterComponentInHierarchy<M3.Application.DIProbe>(); // Test if DI resolves correctly
            builder.RegisterComponentInHierarchy<AssetManager>();
            builder.RegisterComponentInHierarchy<BoardView>();
            builder.RegisterComponentInHierarchy<BoardInputController>();
            builder.RegisterComponentInHierarchy<BoardResolutionPlayback>();
            
            builder.Register<BoardResolutionCoordinator>(Lifetime.Singleton);
            
            _random = new System.Random(_boardSeed);
            CreateBoard();
            CreateSpatialMap();
            
            builder.RegisterInstance(_board);
            builder.RegisterInstance(_spatialMap);
        }
        
        // Create board with no initial matches
        private void CreateBoard()
        {
            _board = new BoardState(_width, _height);

            var allColors = Enum.GetValues(typeof(GemColor))
                .Cast<GemColor>()
                .ToList();

            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var candidates = new List<GemColor>(allColors);

                // Horizontal check
                if (x >= 2)
                {
                    var c1 = _board.GetCell(x - 1, y);
                    var c2 = _board.GetCell(x - 2, y);

                    if (!c1.IsEmpty && !c2.IsEmpty && c1.Gem.Color == c2.Gem.Color)
                        candidates.Remove(c1.Gem.Color);
                }

                // Vertical check
                if (y >= 2)
                {
                    var c1 = _board.GetCell(x, y - 1);
                    var c2 = _board.GetCell(x, y - 2);

                    if (!c1.IsEmpty && !c2.IsEmpty && c1.Gem.Color == c2.Gem.Color)
                        candidates.Remove(c1.Gem.Color);
                }

                // Fallback safety (should almost never happen)
                if (candidates.Count == 0)
                    candidates.AddRange(allColors);

                var color = candidates[_random.Next(candidates.Count)];
                _board.SetGem(x, y, new GemState(color, GemType.Normal));
            }
        }

        private void CreateSpatialMap()
        {
            // World-space origin of the board
            Vector3 origin = transform.position + new Vector3(_boardOriginOffset.x, _boardOriginOffset.y, 0);

            _spatialMap = new BoardSpatialMap(
                board: _board,
                cellSize: _cellSize,
                origin: origin);
        }
    }
}
