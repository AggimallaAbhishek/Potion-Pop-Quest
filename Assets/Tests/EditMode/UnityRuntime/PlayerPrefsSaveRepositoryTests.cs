using NUnit.Framework;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;

namespace PotionPopQuest.Unity.Tests
{
    public sealed class PlayerPrefsSaveRepositoryTests
    {
        [Test]
        public void SaveAndLoad_PreservesUnlockedLevelStarsAndSettings()
        {
            var repository = new PlayerPrefsSaveRepository(new NullGameLogger());
            repository.Reset();

            var save = new SaveData
            {
                highestUnlockedLevel = 4,
                musicEnabled = false,
                sfxEnabled = true
            };
            SaveProgressService.ApplyLevelCompleted(save, 3, 2500, 2, hasNextLevel: true);

            repository.Save(save);
            var loaded = repository.Load();

            Assert.That(loaded.highestUnlockedLevel, Is.EqualTo(4));
            Assert.That(loaded.musicEnabled, Is.False);
            Assert.That(loaded.sfxEnabled, Is.True);
            Assert.That(loaded.GetOrCreateLevelProgress(3).stars, Is.EqualTo(2));
            Assert.That(loaded.GetOrCreateLevelProgress(3).bestScore, Is.EqualTo(2500));

            repository.Reset();
        }
    }
}

