namespace M3.Core.Domain
{
    /// <summary>
    /// Current state of a single cell and what gem it contains
    /// </summary>
    public sealed class CellState
    {
        public Position Position { get; }
        public GemState Gem { get; private set; }

        public CellState(Position position)
        {
            Position = position;
            Gem = null;
        }

        public bool IsEmpty => Gem == null;
        
        public void SetGem(GemState gem)
        {
            Gem = gem;
        }

        public void ClearGem()
        {
            Gem = null;
        }

        public CellState Clone()
        {
            var clone = new CellState(Position);
            if (Gem != null)
                clone.SetGem(Gem.Clone());

            return clone;
        }
    }
}