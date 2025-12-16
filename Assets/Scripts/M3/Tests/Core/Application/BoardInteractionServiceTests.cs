using NUnit.Framework;
using M3.Core.Application;
using M3.Core.Domain;
using M3.Core.Domain.Match;
using M3.Core.Domain.Swap;
using M3.Core.System;


namespace M3.Tests.Core.Application
{
    public class BoardInteractionServiceTests
    {
        private static BoardInteractionService CreateService(int seed)
        {
            var detector = new LineMatchDetector();
            var classifier = new MatchClassifier();
            var gravity = new GravitySystem();
            var spawner = new GemSpawner(seed);
            var cascade = new CascadeSystem(
                detector,
                classifier,
                gravity,
                spawner);

            var swapRule = new AdjacentSwapRule(detector);

            return new BoardInteractionService(
                swapRule,
                cascade);
        }
        
        private static void AssertNoMatches(BoardState board)
        {
            var detector = new LineMatchDetector();
            var matches = detector.Detect(board);

            Assert.AreEqual(0, matches.Count, "Board still contains matches.");
        }

    }
}