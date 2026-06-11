using System;

namespace PotionPopQuest.Core
{
    public static class SaveProgressService
    {
        public static void ApplyLevelCompleted(
            SaveData saveData,
            int levelNumber,
            int score,
            int stars,
            bool hasNextLevel)
        {
            if (saveData == null)
            {
                throw new ArgumentNullException(nameof(saveData));
            }

            var progress = saveData.GetOrCreateLevelProgress(levelNumber);
            progress.bestScore = Math.Max(progress.bestScore, score);
            progress.stars = Math.Max(progress.stars, stars);

            if (hasNextLevel)
            {
                saveData.highestUnlockedLevel = Math.Max(saveData.highestUnlockedLevel, levelNumber + 1);
            }
        }
    }
}

