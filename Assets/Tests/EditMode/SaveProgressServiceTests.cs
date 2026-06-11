using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class SaveProgressServiceTests
    {
        [Test]
        public void ApplyLevelCompleted_UnlocksNextLevelAndStoresBestResult()
        {
            var saveData = new SaveData();

            SaveProgressService.ApplyLevelCompleted(saveData, 1, 1200, 2, hasNextLevel: true);

            Assert.That(saveData.highestUnlockedLevel, Is.EqualTo(2));
            var progress = saveData.GetOrCreateLevelProgress(1);
            Assert.That(progress.bestScore, Is.EqualTo(1200));
            Assert.That(progress.stars, Is.EqualTo(2));
        }

        [Test]
        public void ApplyLevelCompleted_DoesNotOverwriteBetterScoreOrStars()
        {
            var saveData = new SaveData();
            SaveProgressService.ApplyLevelCompleted(saveData, 1, 2200, 3, hasNextLevel: true);

            SaveProgressService.ApplyLevelCompleted(saveData, 1, 800, 1, hasNextLevel: true);

            var progress = saveData.GetOrCreateLevelProgress(1);
            Assert.That(progress.bestScore, Is.EqualTo(2200));
            Assert.That(progress.stars, Is.EqualTo(3));
        }
    }
}

