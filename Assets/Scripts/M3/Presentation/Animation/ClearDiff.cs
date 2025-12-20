using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using M3.Presentation.Playback;

namespace M3.Presentation.Animation
{
    public static class ClearDiff
    {
        public static List<Position> Compute(
            BoardSnapshot before,
            BoardSnapshot after)
        {
            return before.Cells
                .Where(kv => after.Cells.Values.All(g => g.Id != kv.Value.Id))
                .Select(kv => kv.Key)
                .ToList();
        }

    }
}