using System.Collections.Generic;
using M3.Core.Domain;
using M3.Core.Domain.Match;

namespace M3.Core.System
{
    public sealed class CascadeSystem : ICascadeSystem
    {
        private readonly IMatchDetector _matchDetector;
        private readonly IGravitySystem _gravitySystem;
        private readonly IGemSpawner _gemSpawner;

        private readonly MatchClassifier _matchClassifier;
        
        public CascadeSystem(
            IMatchDetector matchDetector,
            MatchClassifier matchClassifier,
            IGravitySystem gravitySystem,
            IGemSpawner gemSpawner)
        {
            _matchDetector = matchDetector;
            _matchClassifier = matchClassifier;
            _gravitySystem = gravitySystem;
            _gemSpawner = gemSpawner;
        }

        public void Resolve(BoardState board)
        {
            
        }
    }
}