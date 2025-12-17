using M3.Core.Application;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;
using VContainer;
using VContainer.Unity;

namespace M3.Infrastructure.DI
{
    public sealed class GameLifetimeScope : LifetimeScope
    {
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
            builder.Register<IGemSpawner, GemSpawner>(Lifetime.Singleton).WithParameter("seed", 12345);
            builder.Register<ICascadeSystem, CascadeSystem>(Lifetime.Singleton);

            // Application layer
            builder.Register<BoardInteractionService>(Lifetime.Singleton);
            
            // Unity Interaction Layer
            builder.RegisterComponentInHierarchy<M3.Application.DIProbe>();
            
        }
    }
}
