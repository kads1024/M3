using NUnit.Framework;
using M3.Core.Domain;

namespace M3.Tests.Core.Domain
{
    public class CellStateTests
    {
        [Test]
        public void Cell_StartsEmpty()
        {
            var cell = new CellState(new Position(0, 0));

            Assert.IsTrue(cell.IsEmpty);
            Assert.IsNull(cell.Gem);
        }

        [Test]
        public void SetGem_AssignsGem()
        {
            var cell = new CellState(new Position(1, 2));
            var gem = new GemState(GemColor.Green, GemType.Normal);

            cell.SetGem(gem);

            Assert.IsFalse(cell.IsEmpty);
            Assert.AreEqual(gem, cell.Gem);
        }

        [Test]
        public void ClearGem_RemovesGem()
        {
            var cell = new CellState(new Position(1, 2));
            cell.SetGem(new GemState(GemColor.Yellow, GemType.Normal));

            cell.ClearGem();

            Assert.IsTrue(cell.IsEmpty);
            Assert.IsNull(cell.Gem);
        }

        [Test]
        public void Clone_CopiesGemButNotReference()
        {
            var cell = new CellState(new Position(3, 4));
            cell.SetGem(new GemState(GemColor.Purple, GemType.Bomb));

            var clone = cell.Clone();

            Assert.AreNotSame(cell, clone);
            Assert.AreEqual(cell.Position.X, clone.Position.X);
            Assert.AreEqual(cell.Position.Y, clone.Position.Y);
            Assert.AreNotSame(cell.Gem, clone.Gem);
            Assert.AreEqual(cell.Gem.Color, clone.Gem.Color);
        }
    }
}