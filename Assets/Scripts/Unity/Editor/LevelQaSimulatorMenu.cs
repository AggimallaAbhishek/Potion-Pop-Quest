using System.Linq;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;
using UnityEditor;
using UnityEngine;

namespace PotionPopQuest.Editor
{
    public static class LevelQaSimulatorMenu
    {
        private const int AttemptsPerLevel = 300;

        [MenuItem("Potion Pop Quest/QA/Run Level QA Simulator")]
        public static void RunLevelQaSimulator()
        {
            var logger = new NullGameLogger();
            var levels = new LevelCatalogLoader(logger)
                .LoadLevels(null)
                .OrderBy(level => level.LevelNumber)
                .ToArray();
            var results = new LevelQaSimulator(logger: logger).Run(levels, AttemptsPerLevel);

            foreach (var result in results)
            {
                var topReason = result.FailureReasons.Count == 0
                    ? "none"
                    : string.Join(", ", result.FailureReasons
                        .OrderByDescending(item => item.Value)
                        .Take(2)
                        .Select(item => $"{item.Key}: {item.Value}"));

                Debug.Log(
                    $"[PotionPopQuest][QA] Level {result.LevelNumber}: " +
                    $"{result.WinRate:P0} wins ({result.Wins}/{result.Attempts}), " +
                    $"{result.StuckBoards} stuck boards, " +
                    $"{result.AverageMovesRemaining:F1} avg moves left, " +
                    $"{result.AverageScore:F0} avg score, failures: {topReason}");
            }

            var stuckTotal = results.Sum(result => result.StuckBoards);
            var lowestWinRate = results.Min(result => result.WinRate);
            Debug.Log(
                $"[PotionPopQuest][QA] Completed {results.Count} levels x {AttemptsPerLevel} attempts. " +
                $"Lowest win rate {lowestWinRate:P0}; total stuck boards {stuckTotal}.");
        }
    }
}
