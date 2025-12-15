using NUnit.Framework;
using System;
using M3.Core.Domain;

namespace M3.Tests.Core.Domain
{
    public class BoardStateTests
    {
        [Test]
        public void Board_CreatesCorrectDimensions()
        {
            var board = new BoardState(8, 8);

            Assert.AreEqual(8, board.Width);
            Assert.AreEqual(8, board.Height);
        }

        [Test]
        public void Board_InitializesAllCells()
        {
            var board = new BoardState(4, 4);

            for (int x = 0; x < 4; x++)
            for (int y = 0; y < 4; y++)
            {
                Assert.NotNull(board.GetCell(x, y));
                Assert.IsTrue(board.GetCell(x, y).IsEmpty);
            }
        }

        [Test]
        public void SetGem_PlacesGemCorrectly()
        {
            var board = new BoardState(5, 5);
            var gem = new GemState(GemColor.Red, GemType.Normal);

            board.SetGem(2, 3, gem);

            Assert.AreEqual(gem, board.GetCell(2, 3).Gem);
        }

        [Test]
        public void ClearGem_RemovesGem()
        {
            var board = new BoardState(5, 5);
            board.SetGem(1, 1, new GemState(GemColor.Blue, GemType.Normal));

            board.ClearGem(1, 1);

            Assert.IsTrue(board.GetCell(1, 1).IsEmpty);
        }

        [Test]
        public void AccessingOutOfBoundsThrows()
        {
            var board = new BoardState(3, 3);

            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetCell(-1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetCell(3, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => board.GetCell(0, 3));
        }

        [Test]
        public void Clone_CreatesDeepCopy()
        {
            var board = new BoardState(3, 3);
            board.SetGem(1, 1, new GemState(GemColor.Green, GemType.Normal));

            var clone = board.Clone();

            Assert.AreNotSame(board, clone);
            Assert.AreEqual(board.Width, clone.Width);
            Assert.AreEqual(board.Height, clone.Height);

            var originalGem = board.GetCell(1, 1).Gem;
            var clonedGem = clone.GetCell(1, 1).Gem;

            Assert.AreNotSame(originalGem, clonedGem);
            Assert.AreEqual(originalGem.Color, clonedGem.Color);
        }
    }
}
