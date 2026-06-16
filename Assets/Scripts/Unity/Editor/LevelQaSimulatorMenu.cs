using System.Linq;
using System.Text;
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

            ExportReport(results, AttemptsPerLevel);
        }

        private static void ExportReport(System.Collections.Generic.IReadOnlyList<LevelQaResult> results, int attemptsPerLevel)
        {
            var projectRoot = System.IO.Directory.GetParent(Application.dataPath)?.FullName;
            if (string.IsNullOrEmpty(projectRoot))
            {
                Debug.LogWarning("[PotionPopQuest][QA] Could not resolve project root; QA report was not exported.");
                return;
            }

            var outputDirectory = System.IO.Path.Combine(projectRoot, "Library", "PotionPopQuestQa");
            System.IO.Directory.CreateDirectory(outputDirectory);

            var timestamp = System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var report = BuildReport(results, attemptsPerLevel);
            var jsonPath = System.IO.Path.Combine(outputDirectory, $"level_qa_{timestamp}.json");
            var csvPath = System.IO.Path.Combine(outputDirectory, $"level_qa_{timestamp}.csv");
            System.IO.File.WriteAllText(jsonPath, JsonUtility.ToJson(report, prettyPrint: true));
            System.IO.File.WriteAllText(csvPath, BuildCsv(report));

            Debug.Log($"[PotionPopQuest][QA] Exported QA reports: {jsonPath} and {csvPath}");
        }

        private static LevelQaReport BuildReport(System.Collections.Generic.IReadOnlyList<LevelQaResult> results, int attemptsPerLevel)
        {
            var report = new LevelQaReport
            {
                generatedAtUtc = System.DateTime.UtcNow.ToString("o"),
                attemptsPerLevel = attemptsPerLevel,
                levelCount = results.Count,
                totalStuckBoards = results.Sum(result => result.StuckBoards),
                lowestWinRate = results.Count == 0 ? 0f : (float)results.Min(result => result.WinRate)
            };

            foreach (var result in results)
            {
                report.levels.Add(new LevelQaReportRow
                {
                    levelNumber = result.LevelNumber,
                    attempts = result.Attempts,
                    wins = result.Wins,
                    losses = result.Losses,
                    stuckBoards = result.StuckBoards,
                    winRate = (float)result.WinRate,
                    averageMovesRemaining = result.AverageMovesRemaining,
                    averageScore = result.AverageScore,
                    failureReasons = FormatReasons(result)
                });
            }

            return report;
        }

        private static string BuildCsv(LevelQaReport report)
        {
            var builder = new StringBuilder();
            builder.AppendLine("level,attempts,wins,losses,winRate,stuckBoards,averageMovesRemaining,averageScore,failureReasons");
            foreach (var row in report.levels)
            {
                builder
                    .Append(row.levelNumber).Append(',')
                    .Append(row.attempts).Append(',')
                    .Append(row.wins).Append(',')
                    .Append(row.losses).Append(',')
                    .Append(row.winRate.ToString("0.000")).Append(',')
                    .Append(row.stuckBoards).Append(',')
                    .Append(row.averageMovesRemaining.ToString("0.00")).Append(',')
                    .Append(row.averageScore.ToString("0")).Append(',')
                    .Append(EscapeCsv(row.failureReasons))
                    .AppendLine();
            }

            return builder.ToString();
        }

        private static string FormatReasons(LevelQaResult result)
        {
            return result.FailureReasons.Count == 0
                ? "none"
                : string.Join("; ", result.FailureReasons.OrderByDescending(item => item.Value).Select(item => $"{item.Key}: {item.Value}"));
        }

        private static string EscapeCsv(string value)
        {
            value = value ?? string.Empty;
            return value.Contains(",") || value.Contains("\"") || value.Contains("\n")
                ? $"\"{value.Replace("\"", "\"\"")}\""
                : value;
        }
    }
}
