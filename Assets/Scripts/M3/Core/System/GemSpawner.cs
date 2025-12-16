using System;
using System.Collections.Generic;
using M3.Core.Domain;

namespace M3.Core.System
{
    public sealed class GemSpawner : IGemSpawner
    {
        private readonly Random _random;
        private readonly GemColor[] _availableColors;
        
        public GemSpawner(int seed)
        {
            _random = new Random(seed);
            _availableColors = (GemColor[])Enum.GetValues(typeof(GemColor));

            // Needs at least 3 available colors or else it would create unintentional matches
            if (_availableColors.Length < 3)
                throw new InvalidOperationException(
                    "Default GemSpawner requires at least 3 gem colors.");
        }

        public void Spawn(BoardState board)
        {
           // Implement after tests
        }

    }
}
