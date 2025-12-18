namespace M3.Core.Domain
{
    public sealed class GemState
    {
        public int Id { get; }
        public GemColor Color { get; }
        public GemType Type { get; }

        public GemState(GemColor color, GemType type)
        {
            Id = GemIdGenerator.Next();
            Color = color;
            Type = type;
        }

        private GemState(int id, GemColor color, GemType type)
        {
            Id = id;
            Color = color;
            Type = type;
        }
        
        public GemState Clone()
        {
            return new GemState(Id, Color, Type);
        }
    }
}
