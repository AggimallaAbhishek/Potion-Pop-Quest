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
            var isFirstTimeClear = progress.stars == 0; // If previously had 0 stars, it's a first time clear
            
            progress.bestScore = Math.Max(progress.bestScore, score);
            progress.stars = Math.Max(progress.stars, stars);

            if (hasNextLevel)
            {
                saveData.highestUnlockedLevel = Math.Max(saveData.highestUnlockedLevel, levelNumber + 1);
            }
            
            // Reward coins for completing the level
            EconomyManager.RewardLevelCompletion(saveData, score, stars, isFirstTimeClear);
        }
    }
}

