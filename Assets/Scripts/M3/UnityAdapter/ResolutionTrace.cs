using System.Collections.Generic;

using M3.UnityAdapter;


namespace M3.UnityAdapter
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