using M3.Core.Domain;

namespace M3.Core.Domain.Swap
{
    public readonly struct SwapContext
    {
        public bool IsPlayerMove { get; }
        public Position SwapA { get; }
        public Position SwapB { get; }

        private SwapContext(bool isPlayerMove, Position a, Position b)
        {
            IsPlayerMove = isPlayerMove;
            SwapA = a;
            SwapB = b;
        }

        public static SwapContext PlayerMove(Position a, Position b)
            => new SwapContext(true, a, b);

        public static SwapContext Cascade()
            => new SwapContext(false, default, default);
    }
}