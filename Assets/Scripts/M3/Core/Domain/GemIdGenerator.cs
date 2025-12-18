namespace M3.Core.Domain
{
    public static class GemIdGenerator
    {
        private static int _nextId = 1;

        public static int Next()
        {
            return _nextId++;
        }
    }
}