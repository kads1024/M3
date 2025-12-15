using NUnit.Framework;
using M3.Core.Domain;

namespace M3.Tests.Core.Domain
{
    public class PositionTests
    {
        [Test]
        public void Position_StoresCoordinatesCorrectly()
        {
            var position = new Position(2, 5);

            Assert.AreEqual(2, position.X);
            Assert.AreEqual(5, position.Y);
        }

        [Test]
        public void Position_WithSameValues_IsEqualByValue()
        {
            var a = new Position(1, 1);
            var b = new Position(1, 1);

            Assert.AreEqual(a.X, b.X);
            Assert.AreEqual(a.Y, b.Y);
        }
    }
}