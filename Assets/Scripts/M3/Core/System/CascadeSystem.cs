using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;

namespace M3.Core.System
{
    /// <summary>
    /// Default ICascadeSystem with incorporated bombs. (You can create a new child of ICascadeSystem without bombs)
    /// </summary>
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

        /// <summary>
        /// Resolve cascade until board is stable
        /// </summary>
        /// <param name="board">board to be mutated and stabilized</param>
        /// <param name="context">swap information</param>
        public void Resolve(BoardState board, SwapContext context)
        {
            bool isPlayerMove = context.IsPlayerMove;

            while (ResolveIteration(board, context, ref isPlayerMove).Unresolved)
            {
                // loop until stable
            }
        }
        
        /// <summary>
        /// Resolve one iteration of a cascade regardless of board stability
        /// </summary>
        /// <param name="board">board to be mutated and resolve one iteration</param>
        /// <param name="context">swap information</param>
        /// <param name="isPlayerMove">whether the swap was initiated by the player</param>
        /// <returns></returns>
        public CascadeIterationResult ResolveOneIteration(
            BoardState board,
            SwapContext context,
            ref bool isPlayerMove)
        {
            return ResolveIteration(board, context, ref isPlayerMove);
        }
        
        /// <summary>
        /// Resolve one iteration of a cascade regardless of board stability
        /// </summary>
        /// <param name="board">board to be mutated and resolve one iteration</param>
        /// <param name="context">swap information</param>
        /// <param name="isPlayerMove">whether the swap was initiated by the player</param>
        /// <returns></returns>
        private CascadeIterationResult ResolveIteration(
            BoardState board,
            SwapContext context,
            ref bool isPlayerMove)
        {
            // Detect all matches on the board. If there are no matches, exit
            var rawMatches = _matchDetector.Detect(board);
            if (rawMatches.Count == 0)
                return new CascadeIterationResult(false, null, new List<BombTrigger>());
            
            // if there are matches, classify them
            var classifiedMatches = _matchClassifier.Classify(rawMatches);

            //-----------------------------------------------------
            // determine if bomb creation is possible from a match. You can comment this out if you dont want a
            // bomb system, and it wouldn't break the game since bomb is a separate subsystem and not invasive to other 
            // domain Logic
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
                            isPlayerMove,
                            GemIdGenerator.Next());

                        if (bombToCreate != null)
                        {
                            bombPlacement = new BombPlacement(
                                bombToCreate.Id,
                                bombToCreate.Position);
                            break; 
                        }
                    }
                }
            }
            //---------------------------------------------------
            
            var toRemove = new HashSet<Position>();
            // Once classified, remove matches
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
            
            var queue = new Queue<Position>(toRemove);
            var bombTriggers = new List<BombTrigger>();
            //-------------------------------------------------
            // Non invasive way of destroying gems is to add the exploded gems to the toRemove Queue instead of 
            // incorporating bomb logic in the match logic. If you comment this line, it will act as if the bomb is a 
            // normal gem without breaking the game.
            
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
            //-------------------------------------------------
            
            // Proceed to removing all gems that were matched
            foreach (var pos in toRemove)
            {
                board.ClearGem(pos.X, pos.Y);
            }
            
            // Spawn the bomb in the correct place (commentable)
            if (bombToCreate != null)
            {
                board.SetGem(
                    bombToCreate.Position.X,
                    bombToCreate.Position.Y,
                    new GemState(bombToCreate.Color, GemType.Bomb, bombToCreate.Id));
            }
            
            _gravitySystem.Apply(board); 
            _gemSpawner.Spawn(board);

            isPlayerMove = false; // Set player move to false since future iterations after this (if there are any)
                                  // is considered cascade and not a player move anymore

            return new CascadeIterationResult(
                unresolved: true,
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
