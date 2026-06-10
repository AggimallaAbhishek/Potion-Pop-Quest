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
        public int saveVersion = 1;
        public int highestUnlockedLevel = 1;
        public bool musicEnabled = true;
        public bool sfxEnabled = true;
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
    }
}

