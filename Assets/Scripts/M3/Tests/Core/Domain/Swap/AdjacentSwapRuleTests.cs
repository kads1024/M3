using NUnit.Framework;
using M3.Core.Domain;
using M3.Core.Domain.Swap;
using M3.Core.Domain.Match;

namespace M3.Tests.Core.Domain.Swap
{
    public class AdjacentSwapRuleTests
    {
        private static ISwapRule CreateSwapRule()
        {
            IMatchDetector detector = new LineMatchDetector();
            return new AdjacentSwapRule(detector);
        }

        private static BoardState CreateBoard(
            int width,
            int height,
            (int x, int y, GemColor color)[] gems)
        {
            var board = new BoardState(width, height);

            foreach (var (x, y, color) in gems)
            {
                board.SetGem(x, y, new GemState(color, GemType.Normal));
            }

            return board;
        }

        [Test]
        public void AdjacentSwap_WithNoMatch_IsRejected()
        {
            var board = CreateBoard(
                2, 1,
                new[]
                {
                    (0, 0, GemColor.Red),
                    (1, 0, GemColor.Blue)
                });

            var rule = CreateSwapRule();

            bool result = rule.IsSwapValid(
                board,
                new Position(0, 0),
                new Position(1, 0));

            Assert.IsFalse(result);
        }

        [Test]
        public void DiagonalSwap_IsRejected()
        {
            var board = CreateBoard(
                2, 2,
                new[]
                {
                    (0, 0, GemColor.Red),
                    (1, 1, GemColor.Red)
                });

            var rule = CreateSwapRule();

            bool result = rule.IsSwapValid(
                board,
                new Position(0, 0),
                new Position(1, 1));

            Assert.IsFalse(result);
        }

        [Test]
        public void SwapThatProducesNoMatch_IsRejected()
        {
            var board = CreateBoard(
                3, 3,
                new[]
                {
                    (0, 0, GemColor.Red),
                    (1, 0, GemColor.Green),
                    (2, 0, GemColor.Blue),

                    (0, 1, GemColor.Blue),
                    (1, 1, GemColor.Red),
                    (2, 1, GemColor.Green),

                    (0, 2, GemColor.Green),
                    (1, 2, GemColor.Blue),
                    (2, 2, GemColor.Red)
                });

            var rule = CreateSwapRule();

            bool result = rule.IsSwapValid(
                board,
                new Position(0, 0),
                new Position(1, 0));

            Assert.IsFalse(result);
        }

        [Test]
        public void SwapThatCreatesMatch_IsAccepted()
        {
            var board = CreateBoard(
                3, 3,
                new[]
                {
                    (0, 0, GemColor.Red),
                    (1, 0, GemColor.Red),
                    (2, 0, GemColor.Green),

                    (0, 1, GemColor.Green),
                    (1, 1, GemColor.Green),
                    (2, 1, GemColor.Red),

                    (0, 2, GemColor.Blue),
                    (1, 2, GemColor.Red),
                    (2, 2, GemColor.Blue)
                });

            var rule = CreateSwapRule();

            bool result = rule.IsSwapValid(
                board,
                new Position(2, 0),
                new Position(2, 1));

            Assert.IsTrue(result);
        }

        [Test]
        public void SwapValidation_DoesNotMutateOriginalBoard()
        {
            // Arrange
            var board = new BoardState(2, 1);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 0, new GemState(GemColor.Blue, GemType.Normal));

            var originalLeft  = board.GetCell(0, 0).Gem.Color;
            var originalRight = board.GetCell(1, 0).Gem.Color;

            var detector = new LineMatchDetector();
            var rule = CreateSwapRule();

            // Act
            rule.IsSwapValid(
                board,
                new Position(0, 0),
                new Position(1, 0));

            // Assert
            Assert.AreEqual(
                originalLeft,
                board.GetCell(0, 0).Gem.Color,
                "Left cell gem was mutated during swap validation");

            Assert.AreEqual(
                originalRight,
                board.GetCell(1, 0).Gem.Color,
                "Right cell gem was mutated during swap validation");
        }

    }
}