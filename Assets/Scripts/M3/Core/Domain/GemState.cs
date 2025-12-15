namespace M3.Core.Domain
{
    public sealed class GemState
    {
        public GemColor Color { get; }
        public GemType Type { get; }

        public GemState(GemColor color, GemType type)
        {
            Color = color;
            Type = type;
        }

        public GemState Clone()
        {
            return new GemState(Color, Type);
        }
    }
}