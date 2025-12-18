using System.Collections.Generic;

namespace M3.Core.Domain
{
    public sealed class ResolutionTrace
    {
        public BoardSnapshot Initial { get; }
        public IReadOnlyList<CascadeStep> Steps { get; }
        public BoardSnapshot Final { get; }


        public ResolutionTrace(
            BoardSnapshot initial,
            IReadOnlyList<CascadeStep> steps,
            BoardSnapshot final)
        {
            Initial = initial;
            Steps = steps;
            Final = final;
        }
    }
}