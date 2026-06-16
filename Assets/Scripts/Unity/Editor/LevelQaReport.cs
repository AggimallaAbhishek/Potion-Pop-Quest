using System;
using System.Collections.Generic;

namespace PotionPopQuest.Editor
{
    [Serializable]
    public sealed class LevelQaReport
    {
        public string generatedAtUtc;
        public int attemptsPerLevel;
        public int levelCount;
        public int totalStuckBoards;
        public float lowestWinRate;
        public List<LevelQaReportRow> levels = new List<LevelQaReportRow>();
    }

    [Serializable]
    public sealed class LevelQaReportRow
    {
        public int levelNumber;
        public int attempts;
        public int wins;
        public int losses;
        public int stuckBoards;
        public float winRate;
        public double averageMovesRemaining;
        public double averageScore;
        public string failureReasons;
    }
}
