using System.Collections.Generic;


namespace M3.UnityAdapter
{
    public sealed class BombTrigger
    {
        public int BombGemId { get; }
        public IReadOnlySet<int> AffectedGemIds { get; }


        public BombTrigger(int bombGemId, IReadOnlySet<int> affectedGemIds)
        {
            BombGemId = bombGemId;
            AffectedGemIds = affectedGemIds;
        }
    }

    public interface IReadOnlySet<T>
    {
    }
}