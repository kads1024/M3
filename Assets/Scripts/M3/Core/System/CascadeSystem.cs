using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;

namespace M3.Core.System
{
    public sealed class CascadeSystem : ICascadeSystem
    {
        private readonly IMatchDetector _matchDetector;
        private readonly MatchClassifier _matchClassifier;
        private readonly IGravitySystem _gravitySystem;
        private readonly IGemSpawner _gemSpawner;
        private readonly IBombCreationRule _bombCreationRule;
        private readonly IBombResolver _bombResolver;

        public CascadeSystem(
            IMatchDetector matchDetector,
            MatchClassifier matchClassifier,
            IGravitySystem gravitySystem,
            IGemSpawner gemSpawner,
            IBombCreationRule bombCreationRule,
            IBombResolver bombResolver)
        {
            _matchDetector = matchDetector;
            _matchClassifier = matchClassifier;
            _gravitySystem = gravitySystem;
            _gemSpawner = gemSpawner;
            _bombCreationRule = bombCreationRule;
            _bombResolver = bombResolver;
        }

        private static Position? FindActualSwapOrigin(
            IReadOnlyList<ClassifiedMatch> matches,
            Position a,
            Position b)
        {
            foreach (var match in matches)
            {
                if (match.Positions.Contains(a))
                    return a;

                if (match.Positions.Contains(b))
                    return b;
            }

            return null;
        }

        public void Resolve(
            BoardState board,
            bool isPlayerMove,
            Position swapA,
            Position swapB)
        {
            while (true)
            {
                var rawMatches = _matchDetector.Detect(board);
                if (rawMatches.Count == 0)
                    break;

                var classifiedMatches = _matchClassifier.Classify(rawMatches);

                // 0. determine actual swap origin
                Position? actualSwapOrigin = null;

                if (isPlayerMove)
                {
                    actualSwapOrigin = FindActualSwapOrigin(
                        classifiedMatches,
                        swapA,
                        swapB);
                }

                // 1. Bomb creation (unchanged rule, better input)
                BombCreationResult? bombToCreate = null;

                if (actualSwapOrigin.HasValue)
                {
                    foreach (var match in classifiedMatches)
                    {
                        bombToCreate = _bombCreationRule.TryCreate(
                            match,
                            actualSwapOrigin.Value,
                            isPlayerMove);

                        if (bombToCreate != null)
                            break; // one bomb per move
                    }
                }

                // 2. Build initial removal set
                var toRemove = new HashSet<Position>();

                foreach (var match in classifiedMatches)
                {
                    foreach (var pos in match.Positions)
                    {
                        var cell = board.GetCell(pos.X, pos.Y);

                        // Color-aware bomb triggering
                        if (!cell.IsEmpty &&
                            cell.Gem.Type == GemType.Bomb &&
                            cell.Gem.Color != match.Color)
                        {
                            continue; // bomb not triggered by wrong color
                        }

                        toRemove.Add(pos);
                    }
                }

                // 3. Expand bomb explosions 
                var queue = new Queue<Position>(toRemove);

                while (queue.Count > 0)
                {
                    var pos = queue.Dequeue();
                    var cell = board.GetCell(pos.X, pos.Y);

                    if (cell.IsEmpty || cell.Gem.Type != GemType.Bomb)
                        continue;

                    var blast = _bombResolver.Resolve(board, pos);

                    foreach (var blastPos in blast)
                    {
                        if (toRemove.Add(blastPos))
                        {
                            queue.Enqueue(blastPos);
                        }
                    }
                }

                // 4. Remove gems (except bomb replacement)
                foreach (var pos in toRemove)
                {
                    board.ClearGem(pos.X, pos.Y);
                }

                // 5. Place bomb if created
                if (bombToCreate != null)
                {
                    board.SetGem(
                        bombToCreate.Position.X,
                        bombToCreate.Position.Y,
                        new GemState(bombToCreate.Color, GemType.Bomb));
                }

                // 6. Gravity + spawn
                _gravitySystem.Apply(board);
                _gemSpawner.Spawn(board);

                // 7. After first iteration, everything is cascade
                isPlayerMove = false;
            }
        }
    }
}