using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    [Serializable]
    public sealed class LevelSaveData
    {
        public int levelNumber;
        public int bestScore;
        public int stars;
    }

    [Serializable]
    public sealed class SaveData
    {
        public const int CurrentSaveVersion = 4;

        public int saveVersion = CurrentSaveVersion;
        public int highestUnlockedLevel = 1;
        public bool musicEnabled = true;
        public bool sfxEnabled = true;
        public float musicVolume = 0.55f;
        public float sfxVolume = 0.85f;
        public bool vibrationEnabled = true;
        
        // Economy & Boosters
        public int coins = 100;
        public int currentLives = 5;
        public long nextLifeRegenTime = 0;
        public int hammerBoosters = 2;
        public int shuffleBoosters = 2;

        // Daily Rewards
        public long lastDailyRewardTime = 0;
        public int dailyRewardStreak = 0;

        public List<LevelSaveData> levelProgress = new List<LevelSaveData>();

        public LevelSaveData GetOrCreateLevelProgress(int levelNumber)
        {
            var progress = levelProgress.FirstOrDefault(item => item.levelNumber == levelNumber);
            if (progress != null)
            {
                return progress;
            }

            progress = new LevelSaveData { levelNumber = levelNumber };
            levelProgress.Add(progress);
            return progress;
        }

        public void Normalize()
        {
            var migratedFromOlderSettings = saveVersion < 2;
            var migratedFromNoEconomy = saveVersion < 3;
            var migratedFromNoDaily = saveVersion < 4;
            
            saveVersion = CurrentSaveVersion;
            highestUnlockedLevel = Math.Max(1, highestUnlockedLevel);
            musicVolume = Clamp01(migratedFromOlderSettings && musicVolume <= 0f ? 0.55f : musicVolume);
            sfxVolume = Clamp01(migratedFromOlderSettings && sfxVolume <= 0f ? 0.85f : sfxVolume);
            
            if (migratedFromNoEconomy)
            {
                coins = 100;
                currentLives = 5;
                nextLifeRegenTime = 0;
                hammerBoosters = 2;
                shuffleBoosters = 2;
            }

            if (migratedFromNoDaily)
            {
                lastDailyRewardTime = 0;
                dailyRewardStreak = 0;
            }
            
            currentLives = Math.Max(0, Math.Min(5, currentLives));
            levelProgress = levelProgress ?? new List<LevelSaveData>();
        }

        private static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            return value > 1f ? 1f : value;
        }
    }
}

