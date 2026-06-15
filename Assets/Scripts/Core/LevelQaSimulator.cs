using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class LevelQaResult
    {
        public LevelQaResult(
            int levelNumber,
            int attempts,
            int wins,
            int losses,
            int stuckBoards,
            double averageMovesRemaining,
            double averageScore,
            IReadOnlyDictionary<string, int> failureReasons)
        {
            LevelNumber = levelNumber;
            Attempts = attempts;
            Wins = wins;
            Losses = losses;
            StuckBoards = stuckBoards;
            AverageMovesRemaining = averageMovesRemaining;
            AverageScore = averageScore;
            FailureReasons = failureReasons ?? new Dictionary<string, int>();
        }

        public int LevelNumber { get; }
        public int Attempts { get; }
        public int Wins { get; }
        public int Losses { get; }
        public int StuckBoards { get; }
        public double AverageMovesRemaining { get; }
        public double AverageScore { get; }
        public IReadOnlyDictionary<string, int> FailureReasons { get; }
        public double WinRate => Attempts <= 0 ? 0d : (double)Wins / Attempts;
    }

    public sealed class LevelQaSimulator
    {
        private const int DefaultMaxMovesPerAttempt = 400;
        private readonly BoardMoveFinder _moveFinder;
        private readonly IGameLogger _logger;

        public LevelQaSimulator(BoardMoveFinder moveFinder = null, IGameLogger logger = null)
        {
            _moveFinder = moveFinder ?? new BoardMoveFinder();
            _logger = logger ?? new NullGameLogger();
        }

        public IReadOnlyList<LevelQaResult> Run(
            IReadOnlyList<LevelData> levels,
            int attemptsPerLevel = 300,
            int seed = 1907)
        {
            if (levels == null)
            {
                throw new ArgumentNullException(nameof(levels));
            }

            return levels
                .OrderBy(level => level.LevelNumber)
                .Select(level => RunLevel(level, attemptsPerLevel, seed + level.LevelNumber * 997))
                .ToArray();
        }

        public LevelQaResult RunLevel(LevelData level, int attempts, int seed)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            attempts = Math.Max(1, attempts);
            var wins = 0;
            var losses = 0;
            var stuckBoards = 0;
            var totalMovesRemaining = 0;
            var totalScore = 0;
            var failureReasons = new Dictionary<string, int>();

            for (var attempt = 0; attempt < attempts; attempt++)
            {
                var random = new SystemRandomSource(seed + attempt * 7919);
                var session = new GameSession(level, random: random, logger: _logger);
                var safety = 0;

                while (session.State == GameSessionState.Playing && safety++ < DefaultMaxMovesPerAttempt)
                {
                    var validMoves = _moveFinder.FindValidMoves(session.Board);
                    if (validMoves.Count == 0)
                    {
                        var shuffle = session.TryShuffleIfNeeded();
                        if (!shuffle.ValidMove)
                        {
                            stuckBoards++;
                            AddReason(failureReasons, "No valid move and shuffle failed");
                            break;
                        }

                        continue;
                    }

                    var candidate = ChooseMove(validMoves, random);
                    var result = session.TrySwap(candidate.First, candidate.Second);
                    if (!result.ValidMove)
                    {
                        AddReason(failureReasons, result.Message ?? "Rejected candidate move");
                        break;
                    }
                }

                if (safety >= DefaultMaxMovesPerAttempt && session.State == GameSessionState.Playing)
                {
                    stuckBoards++;
                    AddReason(failureReasons, "Move safety limit reached");
                }

                if (session.State == GameSessionState.Won)
                {
                    wins++;
                }
                else
                {
                    losses++;
                    if (session.State == GameSessionState.Lost)
                    {
                        AddReason(failureReasons, "Out of moves");
                    }
                }

                totalMovesRemaining += Math.Max(0, session.MovesRemaining);
                totalScore += session.Score;
            }

            return new LevelQaResult(
                level.LevelNumber,
                attempts,
                wins,
                losses,
                stuckBoards,
                (double)totalMovesRemaining / attempts,
                (double)totalScore / attempts,
                failureReasons);
        }

        private static CandidateMove ChooseMove(IReadOnlyList<CandidateMove> validMoves, IRandomSource random)
        {
            return validMoves[random.Range(0, validMoves.Count)];
        }

        private static void AddReason(IDictionary<string, int> reasons, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                reason = "Unknown";
            }

            reasons.TryGetValue(reason, out var count);
            reasons[reason] = count + 1;
        }
    }
}
