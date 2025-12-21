using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;

namespace M3.Presentation.Animation
{
    public static class SpawnDiff
    {
        public static IReadOnlyList<ColumnSpawn> Compute(
            BoardSnapshot before,
            BoardSnapshot after)
        {
            // Find gems that exist ONLY in 'after'
            var spawned = after.Cells
                .Where(kv => !before.Cells.Values.Any(b => b.Id == kv.Value.Id))
                .Select(kv => new
                {
                    Position = kv.Key,
                    Gem = kv.Value
                })
                .ToList();

            // Group by column (X)
            var byColumn = spawned
                .GroupBy(x => x.Position.X)
                .OrderBy(g => g.Key);

            var result = new List<ColumnSpawn>();

            foreach (var columnGroup in byColumn)
            {
                // Sort bottom to top by final Y
                var ordered = columnGroup
                    .OrderBy(x => x.Position.Y)
                    .ToList();

                var gems = new List<SpawnedGem>();

                for (int i = 0; i < ordered.Count; i++)
                {
                    gems.Add(
                        new SpawnedGem(
                            ordered[i].Gem.Clone(),
                            ordered[i].Position,
                            stackIndex: i));
                }

                result.Add(new ColumnSpawn(columnGroup.Key, gems));
            }

            return result;
        }
    }
}