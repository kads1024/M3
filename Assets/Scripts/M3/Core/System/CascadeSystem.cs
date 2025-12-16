using System.Collections.Generic;
using M3.Core.Domain;
using M3.Core.Domain.Match;

namespace M3.Core.System
{
    public sealed class CascadeSystem : ICascadeSystem
    {
        private readonly IMatchDetector _matchDetector;
        private readonly MatchClassifier _matchClassifier;
        private readonly IGravitySystem _gravitySystem;
        private readonly IGemSpawner _gemSpawner;

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
            while (true)
            {
                // 1. Detect matches on the current board
                var rawMatches = _matchDetector.Detect(board);
                if (rawMatches.Count == 0)
                    break;

                // 2. Classify matches (Line, L, T, overlapping, etc.)
                var classifiedMatches = _matchClassifier.Classify(rawMatches);

                // 3. Remove matched gems
                RemoveMatches(board, classifiedMatches);

                // 4. Apply gravity
                _gravitySystem.Apply(board);

                // 5. Spawn new gems safely
                _gemSpawner.Spawn(board);
            }
        }

        private static void RemoveMatches(
            BoardState board,
            IReadOnlyList<ClassifiedMatch> matches)
        {
            foreach (var match in matches)
            {
                foreach (var position in match.Positions)
                {
                    board.ClearGem(position.X, position.Y);
                }
            }
        }
    }
}