using System;

namespace PotionPopQuest.Core
{
    public static class EconomyManager
    {
        public const int MaxLives = 5;
        public const int LifeRegenDurationSeconds = 1800; // 30 minutes
        
        public static void ProcessLifeRegeneration(SaveData data)
        {
            if (data == null || data.currentLives >= MaxLives)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            
            // If the next regen time was 0 or invalid, start it now
            if (data.nextLifeRegenTime <= 0)
            {
                data.nextLifeRegenTime = now + LifeRegenDurationSeconds;
                return;
            }

            while (now >= data.nextLifeRegenTime && data.currentLives < MaxLives)
            {
                data.currentLives++;
                if (data.currentLives < MaxLives)
                {
                    data.nextLifeRegenTime += LifeRegenDurationSeconds;
                }
                else
                {
                    data.nextLifeRegenTime = 0;
                    break;
                }
            }
        }

        public static bool TryConsumeLife(SaveData data)
        {
            if (data.currentLives <= 0)
            {
                return false;
            }

            if (data.currentLives == MaxLives)
            {
                data.nextLifeRegenTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + LifeRegenDurationSeconds;
            }

            data.currentLives--;
            return true;
        }

        public static long GetSecondsUntilNextLife(SaveData data)
        {
            if (data.currentLives >= MaxLives || data.nextLifeRegenTime <= 0)
            {
                return 0;
            }
            
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var remaining = data.nextLifeRegenTime - now;
            return remaining > 0 ? remaining : 0;
        }

        public static void RewardLevelCompletion(SaveData data, int score, int stars, bool isFirstTimeClear)
        {
            var coinsRewarded = 10; // Base completion
            coinsRewarded += stars * 5; // Up to 15 bonus
            
            if (isFirstTimeClear)
            {
                coinsRewarded += 25; // First time clear bonus
            }

            data.coins += coinsRewarded;
        }

        public static bool TryPurchaseLives(SaveData data)
        {
            const int cost = 50;
            if (data.coins < cost || data.currentLives >= MaxLives)
            {
                return false;
            }

            data.coins -= cost;
            data.currentLives = MaxLives;
            data.nextLifeRegenTime = 0;
            return true;
        }

        public static bool TryPurchaseBooster(SaveData data, BoosterType type)
        {
            const int cost = 100;
            if (data.coins < cost)
            {
                return false;
            }

            data.coins -= cost;
            if (type == BoosterType.Hammer)
            {
                data.hammerBoosters++;
            }
            else if (type == BoosterType.Shuffle)
            {
                data.shuffleBoosters++;
            }
            return true;
        }

        public static bool CheckDailyRewardAvailable(SaveData data)
        {
            if (data == null) return false;

            var now = DateTimeOffset.UtcNow;
            var lastReward = DateTimeOffset.FromUnixTimeSeconds(data.lastDailyRewardTime);
            
            // If the last reward was not collected today (UTC)
            return now.Date > lastReward.Date;
        }

        public static int ClaimDailyReward(SaveData data)
        {
            if (data == null || !CheckDailyRewardAvailable(data)) return 0;

            var now = DateTimeOffset.UtcNow;
            var lastReward = DateTimeOffset.FromUnixTimeSeconds(data.lastDailyRewardTime);

            // If more than 1 day has passed, reset streak
            if ((now.Date - lastReward.Date).TotalDays > 1)
            {
                data.dailyRewardStreak = 0;
            }

            data.dailyRewardStreak++;
            data.lastDailyRewardTime = now.ToUnixTimeSeconds();

            // Reward scales with streak, up to 7 days (50 -> 200 max)
            var streakCap = Math.Min(data.dailyRewardStreak, 7);
            var rewardCoins = 50 + ((streakCap - 1) * 25);
            
            data.coins += rewardCoins;
            return rewardCoins;
        }

        public static void PurchaseCoinPackage(SaveData data, int amount)
        {
            if (data == null || amount <= 0) return;
            data.coins += amount;
        }
    }
}
