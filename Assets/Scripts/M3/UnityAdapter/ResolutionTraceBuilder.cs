using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using M3.Core.Domain.Bomb;
using M3.Core.Domain.Swap;
using M3.Core.System;


namespace M3.UnityAdapter
{
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


            var initial = new BoardSnapshot(board);


            bool isPlayerMove = context.IsPlayerMove;


            while (true)
            {
                var before = new BoardSnapshot(board);


                var cascadeIterationResult = _cascadeSystem.ResolveOneIteration(
                    board,
                    context,ref isPlayerMove);


                if (!cascadeIterationResult.Resolved)
                    break;


                var after = new BoardSnapshot(board);


                var cleared = ComputeClearedGemIds(before, after);
                var gravity = ComputeGravity(before, after);
                
                
                

                
                steps.Add(
                    new CascadeStep(
                        before,
                        after,
                        cleared,
                        gravity,
                        bombTriggers: new List<BombTrigger>(),
                        cascadeIterationResult.BombPlacement)
                    );
            }


            var final = new BoardSnapshot(board);


            return new ResolutionTrace(initial, steps, final);
        }


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
                .ToHashSet();}


        private static List<ColumnGravityResolution> ComputeGravity(
            BoardSnapshot before,
            BoardSnapshot after)
        {
            var result = new List<ColumnGravityResolution>();

            for (int x = 0; x < before.Width; x++)
            {
                var falls = new List<GemFall>();

                // Map gemId -> position before
                var beforeColumn = before.Cells
                    .Where(kv => kv.Key.X == x)
                    .ToDictionary(kv => kv.Value.Id, kv => kv.Key);

                // Map gemId -> position after
                var afterColumn = after.Cells
                    .Where(kv => kv.Key.X == x)
                    .ToDictionary(kv => kv.Value.Id, kv => kv.Key);

                foreach (var kv in afterColumn)
                {
                    int gemId = kv.Key;
                    Position to = kv.Value;

                    // If gem did not exist before, it spawned above the board
                    if (!beforeColumn.TryGetValue(gemId, out var from))
                    {
                        from = new Position(x, after.Height); // spawn row
                    }

                    // If position changed, it fell
                    if (!from.Equals(to))
                    {
                        falls.Add(new GemFall(gemId, from, to));
                    }
                }

                if (falls.Count == 0)
                    continue;

                // IMPORTANT: bottom → top order
                falls.Sort((a, b) => a.To.Y.CompareTo(b.To.Y));

                result.Add(
                    new ColumnGravityResolution(x, falls));
            }

            return result;
        }
    }
}