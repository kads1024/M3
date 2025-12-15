using NUnit.Framework;
using M3.Core.Domain;

namespace M3.Tests.Core.Domain
{
    public class GemStateTests
    {
        [Test]
        public void GemState_StoresColorAndType()
        {
            var gem = new GemState(GemColor.Red, GemType.Normal);

            Assert.AreEqual(GemColor.Red, gem.Color);
            Assert.AreEqual(GemType.Normal, gem.Type);
        }

        [Test]
        public void Clone_CreatesIndependentCopy()
        {
            var original = new GemState(GemColor.Blue, GemType.Bomb);
            var clone = original.Clone();

            Assert.AreNotSame(original, clone);
            Assert.AreEqual(original.Color, clone.Color);
            Assert.AreEqual(original.Type, clone.Type);
        }
    }
}