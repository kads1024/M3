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
        
        [Test]
        public void TrySwap_InvalidSwap_IsRejected_AndBoardUnchanged()
        {
            var board = new BoardState(2, 1);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Blue, GemType.Normal));

            var originalLeft = board.GetCell(0, 0).Gem.Color;
            var originalRight = board.GetCell(1, 0).Gem.Color;

            var service = CreateService(seed: 1);

            var result = service.TrySwap(
                board,
                new Position(0, 0),
                new Position(1, 0));

            Assert.AreEqual(SwapResult.Rejected, result);
            Assert.AreEqual(originalLeft, board.GetCell(0, 0).Gem.Color);
            Assert.AreEqual(originalRight, board.GetCell(1, 0).Gem.Color);
        }

        [Test]
        public void TrySwap_ValidSwap_IsAccepted_AndBoardMutated()
        {
            var board = new BoardState(3, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(2, 0, new GemState(GemColor.Green, GemType.Normal));

            board.SetGem(2, 1, new GemState(GemColor.Red, GemType.Normal));

            var service = CreateService(seed: 2);

            var result = service.TrySwap(
                board,
                new Position(2, 0),
                new Position(2, 1));

            Assert.AreEqual(SwapResult.Accepted, result);

            // Board should be stable after cascade
            AssertNoMatches(board);
        }   
    }
    
    
}