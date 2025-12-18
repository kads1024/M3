namespace M3.UnityAdapter
{
    public sealed class CascadeIterationResult
    {
        public bool Resolved { get; }
        public BombPlacement? BombPlacement { get; }

        public CascadeIterationResult(
            bool resolved,
            BombPlacement? bombPlacement)
        {
            Resolved = resolved;
            BombPlacement = bombPlacement;
        }
    }

}