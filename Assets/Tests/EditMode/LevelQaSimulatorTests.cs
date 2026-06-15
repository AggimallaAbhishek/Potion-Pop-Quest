using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class LevelQaSimulatorTests
    {
        [Test]
        public void RunLevel_FirstLevelReportsNoStuckBoards()
        {
            var level = MvpLevelCatalog.CreateLevels().First();
            var result = new LevelQaSimulator(logger: new NullGameLogger()).RunLevel(level, attempts: 10, seed: 1234);

            Assert.That(result.Attempts, Is.EqualTo(10));
            Assert.That(result.StuckBoards, Is.EqualTo(0));
            Assert.That(result.Wins + result.Losses, Is.EqualTo(10));
        }
    }
}
