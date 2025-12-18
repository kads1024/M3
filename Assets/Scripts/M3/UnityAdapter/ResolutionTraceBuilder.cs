using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
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


                bool resolved = _cascadeSystem.ResolveOneIteration(
                    board,
                    context,ref isPlayerMove);


                if (!resolved)
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
                        bombTriggers: new List<BombTrigger>()));
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
                var beforeIds = before.GetColumnGemIds(x);
                var afterIds = after.GetColumnGemIds(x);


                if (beforeIds.SequenceEqual(afterIds))
                    continue;


                var spawned = afterIds
                    .Except(beforeIds)
                    .ToList();


                //result.Add(
                  //  new ColumnGravityResolution(afterIds, spawned));
            }


            return result;
        }
    }
}