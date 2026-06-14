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
        public const int CurrentSaveVersion = 2;

        public int saveVersion = CurrentSaveVersion;
        public int highestUnlockedLevel = 1;
        public bool musicEnabled = true;
        public bool sfxEnabled = true;
        public float musicVolume = 0.55f;
        public float sfxVolume = 0.85f;
        public bool vibrationEnabled = true;
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
            var migratedFromOlderSettings = saveVersion < CurrentSaveVersion;
            saveVersion = CurrentSaveVersion;
            highestUnlockedLevel = Math.Max(1, highestUnlockedLevel);
            musicVolume = Clamp01(migratedFromOlderSettings && musicVolume <= 0f ? 0.55f : musicVolume);
            sfxVolume = Clamp01(migratedFromOlderSettings && sfxVolume <= 0f ? 0.85f : sfxVolume);
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
