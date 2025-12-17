using M3.Application;
using M3.Core.Application;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;
using M3.Presentation.Playback;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace M3.Infrastructure.DI
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
        [FormerlySerializedAs("_seed")] [SerializeField] private int _gameSeed = 0;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // Domain logic
            builder.Register<IMatchDetector, LineMatchDetector>(Lifetime.Singleton);
            builder.Register<MatchClassifier>(Lifetime.Singleton);
            builder.Register<ISwapRule, AdjacentSwapRule>(Lifetime.Singleton);
            builder.Register<IBombCreationRule, BombCreationRule>(Lifetime.Singleton);
            builder.Register<IBombResolver, BombResolver>(Lifetime.Singleton);

            // Systems
            builder.Register<IGravitySystem, GravitySystem>(Lifetime.Singleton);
            builder.Register<IGemSpawner, GemSpawner>(Lifetime.Singleton).WithParameter("seed", _gameSeed);
            builder.Register<ICascadeSystem, CascadeSystem>(Lifetime.Singleton);

            // Application layer
            builder.Register<BoardInteractionService>(Lifetime.Singleton);
            
            // Unity Interaction Layer
            // builder.RegisterComponentInHierarchy<M3.Application.DIProbe>(); // Test if DI resolves correctly
            builder.RegisterComponentInHierarchy<M3.UnityAdapter.Bootstrap.BoardBootstrapper>();
            builder.RegisterComponentInHierarchy<M3.Presentation.Board.BoardView>();
            builder.RegisterComponentInHierarchy<M3.Application.Input.BoardInputController>();
            builder.RegisterComponentInHierarchy<BoardResolutionPlayback>();
            builder.Register<BoardResolutionCoordinator>(Lifetime.Singleton);

        }
    }
}
