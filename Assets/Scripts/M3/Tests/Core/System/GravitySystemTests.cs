using NUnit.Framework;
using M3.Core.System;
using M3.Core.Domain;


namespace M3.Tests.Core.System
{
    public class GravitySystemTests
    {
        [Test]
        public void Gravity_EmptyBoard_NoChanges()
        {
            var board = new BoardState(3, 3);
            IGravitySystem gravity = new GravitySystem();

            gravity.Apply(board);

            for (int x = 0; x < board.Width; x++)
            for (int y = 0; y < board.Height; y++)
            {
                Assert.IsTrue(board.GetCell(x, y).IsEmpty);
            }
        }
        
        [Test]
        public void Gravity_FullColumn_NoMovement()
        {
            var board = new BoardState(1, 3);

            board.SetGem(0, 0, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(0, 1, new GemState(GemColor.Green, GemType.Normal));
            board.SetGem(0, 2, new GemState(GemColor.Blue, GemType.Normal));

            IGravitySystem gravity = new GravitySystem();
            gravity.Apply(board);

            Assert.AreEqual(GemColor.Red, board.GetCell(0, 0).Gem.Color);
            Assert.AreEqual(GemColor.Green, board.GetCell(0, 1).Gem.Color);
            Assert.AreEqual(GemColor.Blue, board.GetCell(0, 2).Gem.Color);
        }
        
        [Test]
        public void Gravity_SingleGem_FallsToBottom()
        {
            var board = new BoardState(1, 3);

            board.SetGem(0, 2, new GemState(GemColor.Red, GemType.Normal));

            IGravitySystem gravity = new GravitySystem();
            gravity.Apply(board);

            Assert.IsTrue(board.GetCell(0, 2).IsEmpty);
            Assert.IsTrue(board.GetCell(0, 1).IsEmpty);
            Assert.AreEqual(GemColor.Red, board.GetCell(0, 0).Gem.Color);
        }

        [Test]
        public void Gravity_MultipleGems_OrderPreserved()
        {
            var board = new BoardState(1, 5);

            board.SetGem(0, 4, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(0, 2, new GemState(GemColor.Green, GemType.Normal));
            board.SetGem(0, 0, new GemState(GemColor.Blue, GemType.Normal));

            IGravitySystem gravity = new GravitySystem();
            gravity.Apply(board);

            Assert.AreEqual(GemColor.Blue, board.GetCell(0, 0).Gem.Color);
            Assert.AreEqual(GemColor.Green, board.GetCell(0, 1).Gem.Color);
            Assert.AreEqual(GemColor.Red, board.GetCell(0, 2).Gem.Color);

            Assert.IsTrue(board.GetCell(0, 3).IsEmpty);
            Assert.IsTrue(board.GetCell(0, 4).IsEmpty);
        }
        
        [Test]
        public void Gravity_MultipleColumns_IndependentResolution()
        {
            var board = new BoardState(2, 3);

            board.SetGem(0, 2, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(1, 1, new GemState(GemColor.Green, GemType.Normal));

            IGravitySystem gravity = new GravitySystem();
            gravity.Apply(board);

            Assert.AreEqual(GemColor.Red, board.GetCell(0, 0).Gem.Color);
            Assert.AreEqual(GemColor.Green, board.GetCell(1, 0).Gem.Color);

            Assert.IsTrue(board.GetCell(0, 1).IsEmpty);
            Assert.IsTrue(board.GetCell(0, 2).IsEmpty);
            Assert.IsTrue(board.GetCell(1, 1).IsEmpty);
            Assert.IsTrue(board.GetCell(1, 2).IsEmpty);
        }
        
        [Test]
        public void Gravity_IsIdempotent()
        {
            var board = new BoardState(1, 4);

            board.SetGem(0, 3, new GemState(GemColor.Red, GemType.Normal));
            board.SetGem(0, 1, new GemState(GemColor.Blue, GemType.Normal));

            IGravitySystem gravity = new GravitySystem();

            gravity.Apply(board);
            var firstBottom = board.GetCell(0, 0).Gem.Color;
            var secondBottom = board.GetCell(0, 1).Gem.Color;

            gravity.Apply(board);

            Assert.AreEqual(firstBottom, board.GetCell(0, 0).Gem.Color);
            Assert.AreEqual(secondBottom, board.GetCell(0, 1).Gem.Color);
        }

    }
}
