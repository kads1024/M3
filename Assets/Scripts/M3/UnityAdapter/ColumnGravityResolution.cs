using System.Collections.Generic;


namespace M3.UnityAdapter
{
    public sealed class ColumnGravityResolution
    {
        public int ColumnX { get; }
        public IReadOnlyList<int> FinalGemOrder { get; }
        public IReadOnlyList<int> SpawnedGemIds { get; }


        public ColumnGravityResolution(
            int columnX,
            IReadOnlyList<int> finalGemOrder,
            IReadOnlyList<int> spawnedGemIds)
        {
            ColumnX = columnX;
            FinalGemOrder = finalGemOrder;
            SpawnedGemIds = spawnedGemIds;
        }
    }
}