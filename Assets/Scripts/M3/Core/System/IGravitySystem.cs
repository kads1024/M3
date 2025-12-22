using M3.Core.Domain;

namespace M3.Core.System
{
    /// <summary>
    /// Responsible for taking care of how gems are affected by gravity logically
    /// </summary>
    public interface IGravitySystem
    {
        void Apply(BoardState board);
    }
}