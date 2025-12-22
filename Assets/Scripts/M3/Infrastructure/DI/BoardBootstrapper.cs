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
            _random = new System.Random(_boardSeed);
            CreateBoard();
            CreateSpatialMap();
            
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
            
            builder.RegisterInstance(_board);
            builder.RegisterInstance(_spatialMap);
        }
        
        
        private void CreateBoard()
        {
            _board = new BoardState(_width, _height);

            // Temporary initialization
            for (int x = 0; x < _board.Width; x++)
            for (int y = 0; y < _board.Height; y++)
            {
                var color = (GemColor)_random.Next(5);
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
