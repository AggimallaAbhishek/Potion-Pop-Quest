using System;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class EconomyManagerTests
    {
        [Test]
        public void ProcessLifeRegeneration_StartsTimerWhenLivesAreMissing()
        {
            var saveData = new SaveData
            {
                currentLives = 4,
                nextLifeRegenTime = 0
            };

            var changed = EconomyManager.ProcessLifeRegeneration(saveData);

            Assert.That(changed, Is.True);
            Assert.That(saveData.currentLives, Is.EqualTo(4));
            Assert.That(saveData.nextLifeRegenTime, Is.GreaterThan(0));
        }

        [Test]
        public void ProcessLifeRegeneration_RestoresElapsedLives()
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var saveData = new SaveData
            {
                currentLives = 3,
                nextLifeRegenTime = now - EconomyManager.LifeRegenDurationSeconds - 1
            };

            var changed = EconomyManager.ProcessLifeRegeneration(saveData);

            Assert.That(changed, Is.True);
            Assert.That(saveData.currentLives, Is.EqualTo(5));
            Assert.That(saveData.nextLifeRegenTime, Is.EqualTo(0));
        }

        [Test]
        public void TryConsumeLife_StartsRegenerationWhenDroppingFromMax()
        {
            var saveData = new SaveData
            {
                currentLives = EconomyManager.MaxLives,
                nextLifeRegenTime = 0
            };

            var consumed = EconomyManager.TryConsumeLife(saveData);

            Assert.That(consumed, Is.True);
            Assert.That(saveData.currentLives, Is.EqualTo(EconomyManager.MaxLives - 1));
            Assert.That(saveData.nextLifeRegenTime, Is.GreaterThan(0));
        }

        [Test]
        public void BoosterPurchaseConsumesCoinsAndAddsInventory()
        {
            var saveData = new SaveData
            {
                coins = 150,
                hammerBoosters = 0,
                shuffleBoosters = 0
            };

            var purchased = EconomyManager.TryPurchaseBooster(saveData, BoosterType.Hammer);

            Assert.That(purchased, Is.True);
            Assert.That(saveData.coins, Is.EqualTo(50));
            Assert.That(saveData.hammerBoosters, Is.EqualTo(1));
            Assert.That(saveData.shuffleBoosters, Is.EqualTo(0));
        }

        [Test]
        public void ClaimDailyRewardOnlyPaysOncePerUtcDay()
        {
            var saveData = new SaveData
            {
                coins = 0,
                lastDailyRewardTime = 0,
                dailyRewardStreak = 0
            };

            var firstReward = EconomyManager.ClaimDailyReward(saveData);
            var secondReward = EconomyManager.ClaimDailyReward(saveData);

            Assert.That(firstReward, Is.EqualTo(50));
            Assert.That(secondReward, Is.EqualTo(0));
            Assert.That(saveData.coins, Is.EqualTo(50));
            Assert.That(saveData.dailyRewardStreak, Is.EqualTo(1));
        }
    }
}
