using System.Collections.Generic;

namespace M3.Core.Domain
{
    /// <summary>
    /// Records a full trace of a full cycle of a cascade
    /// One full cycle of a cascade is a loop of cascade steps [swap>clear>gravity>spawn] until board is stable
    /// </summary>
    public sealed class ResolutionTrace
    {
        public BoardSnapshot Initial { get; } // state of the board before resolving
        public IReadOnlyList<CascadeStep> Steps { get; } // list of iteration steps (swap>clear>gravity>spawn)[]
        public BoardSnapshot Final { get; } // final stabilized board state


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