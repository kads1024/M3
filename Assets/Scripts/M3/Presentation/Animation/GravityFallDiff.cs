using System.Collections.Generic;
using System.Linq;
using M3.Core.Domain;
using M3.Presentation.Playback;
using M3.UnityAdapter;

namespace M3.Presentation.Animation
{
    public static class GravityDiff
    {
        public static List<GravityFallInstruction> Compute(
            BoardSnapshot before,
            BoardSnapshot after)
        {
            var result = new List<GravityFallInstruction>();

            foreach (var afterCell in after.Cells)
            {
                var to = afterCell.Key;
                var gem = afterCell.Value;

                if (!before.TryFindById(gem.Id, out var from))
                    continue;

                if (from.Y != to.Y)
                {
                    result.Add(new GravityFallInstruction(from, to));
                }
            }

            return result;
        }
        
    }
}