using NUnit.Framework;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;

namespace PotionPopQuest.Unity.Tests
{
    public sealed class LevelCatalogLoaderTests
    {
        [Test]
        public void LoadLevels_LoadsJsonCatalogWithExpandedLevels()
        {
            var levels = new LevelCatalogLoader(new NullGameLogger()).LoadLevels(null);

            Assert.That(levels.Count, Is.GreaterThanOrEqualTo(20));
            Assert.That(levels[0].LevelNumber, Is.EqualTo(1));
            Assert.That(levels[levels.Count - 1].LevelNumber, Is.EqualTo(20));
        }
    }
}
