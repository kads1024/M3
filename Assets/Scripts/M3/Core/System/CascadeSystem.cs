using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;

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

        public void Resolve(BoardState board, SwapContext context)
        {
            bool isPlayerMove = context.IsPlayerMove;

            while (ResolveIteration(board, context, ref isPlayerMove).Resolved)
            {
                // loop until stable
            }
        }

        public CascadeIterationResult ResolveOneIteration(
            BoardState board,
            SwapContext context,
            ref bool isPlayerMove)
        {
            return ResolveIteration(board, context, ref isPlayerMove);
        }

        private CascadeIterationResult ResolveIteration(
            BoardState board,
            SwapContext context,
            ref bool isPlayerMove)
        {
            var rawMatches = _matchDetector.Detect(board);
            if (rawMatches.Count == 0)
                return new CascadeIterationResult(false, null, new List<BombTrigger>());

            var classifiedMatches = _matchClassifier.Classify(rawMatches);

            // -------------------------------------------------
            // 1️⃣ Bomb creation (ONLY ON PLAYER MOVE)
            // -------------------------------------------------
            BombCreationResult? bombToCreate = null;
            BombPlacement? bombPlacement = null;

            if (isPlayerMove)
            {
                var swapOrigin = FindActualSwapOrigin(classifiedMatches, context);

                if (swapOrigin.HasValue)
                {
                    foreach (var match in classifiedMatches)
                    {
                        bombToCreate = _bombCreationRule.TryCreate(
                            match,
                            swapOrigin.Value,
                            isPlayerMove);

                        if (bombToCreate != null)
                        {
                            bombPlacement = new BombPlacement(
                                bombToCreate.Id,
                                bombToCreate.Position);
                            break; // ✅ ONLY ONE BOMB
                        }
                    }
                }
            }

            // -------------------------------------------------
            // 2️⃣ Collect removals from matches
            // -------------------------------------------------
            var toRemove = new HashSet<Position>();

            foreach (var match in classifiedMatches)
            {
                foreach (var pos in match.Positions)
                {
                    var cell = board.GetCell(pos.X, pos.Y);

                    if (!cell.IsEmpty &&
                        cell.Gem.Type == GemType.Bomb &&
                        cell.Gem.Color != match.Color)
                    {
                        continue;
                    }

                    toRemove.Add(pos);
                }
            }

            // -------------------------------------------------
            // 3️⃣ Resolve bomb explosions (chain-aware)
            // -------------------------------------------------
            var bombTriggers = new List<BombTrigger>();
            var queue = new Queue<Position>(toRemove);

            while (queue.Count > 0)
            {
                var pos = queue.Dequeue();
                var cell = board.GetCell(pos.X, pos.Y);

                if (cell.IsEmpty || cell.Gem.Type != GemType.Bomb)
                    continue;

                int bombId = cell.Gem.Id;

                var blast = _bombResolver.Resolve(board, pos);
                var affectedIds = new List<int>();

                foreach (var blastPos in blast)
                {
                    var blastCell = board.GetCell(blastPos.X, blastPos.Y);
                    if (!blastCell.IsEmpty)
                        affectedIds.Add(blastCell.Gem.Id);

                    if (toRemove.Add(blastPos))
                        queue.Enqueue(blastPos);
                }

                bombTriggers.Add(
                    new BombTrigger(
                        bombId,
                        pos,
                        affectedIds));
            }

            // -------------------------------------------------
            // 4️⃣ Remove gems
            // -------------------------------------------------
            foreach (var pos in toRemove)
            {
                board.ClearGem(pos.X, pos.Y);
            }

            // -------------------------------------------------
            // 5️⃣ Place bomb (replacement, NOT spawn)
            // -------------------------------------------------
            if (bombToCreate != null)
            {
                board.SetGem(
                    bombToCreate.Position.X,
                    bombToCreate.Position.Y,
                    new GemState(bombToCreate.Color, GemType.Bomb));
            }

            // -------------------------------------------------
            // 6️⃣ Gravity + spawn
            // -------------------------------------------------
            _gravitySystem.Apply(board);
            _gemSpawner.Spawn(board);

            isPlayerMove = false;

            return new CascadeIterationResult(
                resolved: true,
                bombPlacement: bombPlacement,
                bombTrigger: bombTriggers);
        }

        private static Position? FindActualSwapOrigin(
            IReadOnlyList<ClassifiedMatch> matches,
            SwapContext context)
        {
            if (!context.IsPlayerMove)
                return null;

            foreach (var match in matches)
            {
                if (match.Positions.Contains(context.SwapA))
                    return context.SwapA;

                if (match.Positions.Contains(context.SwapB))
                    return context.SwapB;
            }

            return null;
        }
    }
}
