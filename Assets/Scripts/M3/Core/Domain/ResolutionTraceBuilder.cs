using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain.Swap;
using M3.Core.System;

namespace M3.Core.Domain
{
    /// <summary>
    /// Responsible for building the Resolution Trace by observing the ICascadeSystem without interfering with it directly
    /// ResolutionTraceBuilder is just an observer here.
    /// </summary>
    public sealed class ResolutionTraceBuilder
    {
        private readonly ICascadeSystem _cascadeSystem;


        public ResolutionTraceBuilder(ICascadeSystem cascadeSystem)
        {
            _cascadeSystem = cascadeSystem;
        }


        public ResolutionTrace Build(
            BoardState board,
            SwapContext context)
        {
            var steps = new List<CascadeStep>();
            
            // Take a snapshot of the board before resolving
            var initial = new BoardSnapshot(board);


            bool isPlayerMove = context.IsPlayerMove;


            while (true) // Loop through the cascade steps until board is stable
            {
                // Take a snapshot of the board before each cascade step
                var before = new BoardSnapshot(board);

                // Run one iteration of cascade
                var cascadeIterationResult = _cascadeSystem.ResolveOneIteration(
                    board,
                    context,ref isPlayerMove);
                
                if (!cascadeIterationResult.Unresolved) // if board is stable, stop the loop
                    break;
                
                // if board is not stable, snapshot the board after one resolution
                var after = new BoardSnapshot(board);

                // gather data about the cleared gems and gravitated gems
                var cleared = ComputeClearedGemIds(before, after);
                var gravity = ComputeGravity(before, after);
                
                // record one step of the cascade
                steps.Add(
                    new CascadeStep(
                        before,
                        after,
                        cleared,
                        gravity,
                        bombTriggers: cascadeIterationResult.BombTriggers,
                        cascadeIterationResult.BombPlacement)
                    );
            }

            // Once board is resolved, snapshot the final state of the board
            var final = new BoardSnapshot(board);


            return new ResolutionTrace(initial, steps, final);
        }

        /// <summary>
        /// Compute all gem IDs that were cleared from a cascade step.
        /// This basically computes what cells are missing from the after when compared to the before snapshot 
        /// </summary>
        /// <param name="before">Snapshot before the clear</param>
        /// <param name="after">Snapshot after the clear</param>
        /// <returns></returns>
        private static HashSet<int> ComputeClearedGemIds(
            BoardSnapshot before,
            BoardSnapshot after)
        {
                var afterIds = after.Cells.Values
                .Select(g => g.Id)
                .ToHashSet();


            return before.Cells.Values
                .Where(g => !afterIds.Contains(g.Id))
                .Select(g => g.Id)
                .ToHashSet();
        }

        /// <summary>
        /// Compute all gem IDs that were affevted by gravity from a cascade step.
        /// This basically computes what cells Y coordinate changed from the after when compared to the before snapshot 
        /// </summary>
        /// <param name="before">Snapshot before the gravity applied</param>
        /// <param name="after">Snapshot before the gravity applied</param>
        /// <returns></returns>
        private static List<ColumnGravityResolution> ComputeGravity(
            BoardSnapshot before,
            BoardSnapshot after)
        {
            var result = new List<ColumnGravityResolution>();
            
            // loop through each column
            for (int x = 0; x < before.Width; x++)
            {
                var falls = new List<GemFall>();
                
                // get a snapshot of the before column
                var beforeColumn = before.Cells
                    .Where(kv => kv.Key.X == x)
                    .ToDictionary(kv => kv.Value.Id, kv => kv.Key);
                
                // get a snapshot of the before column
                var afterColumn = after.Cells
                    .Where(kv => kv.Key.X == x)
                    .ToDictionary(kv => kv.Value.Id, kv => kv.Key);

                // Loop through each gem in the column
                foreach (var kv in afterColumn)
                {
                    int gemId = kv.Key;
                    Position to = kv.Value;
                    
                    if (!beforeColumn.TryGetValue(gemId, out var from))
                    {
                        from = new Position(x, after.Height);
                    }

                    if (!from.Equals(to)) // if Y value is not the same, it means that it was affected by gravity
                    {
                        falls.Add(new GemFall(gemId, from, to));
                    }
                }

                if (falls.Count == 0)
                    continue;

                falls.Sort((a, b) => a.To.Y.CompareTo(b.To.Y));

                result.Add(new ColumnGravityResolution(x, falls));
            }

            return result;
        }
    }
}