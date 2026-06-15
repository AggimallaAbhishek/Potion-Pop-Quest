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
        private readonly MatchFinder _matchFinder;
        private readonly BoardMoveFinder _moveFinder;
        private readonly IGameLogger _logger;

        public LevelQaSimulator(MatchFinder matchFinder = null, BoardMoveFinder moveFinder = null, IGameLogger logger = null)
        {
            _matchFinder = matchFinder ?? new MatchFinder();
            _moveFinder = moveFinder ?? new BoardMoveFinder(_matchFinder);
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

                    var candidate = ChooseMove(session, validMoves, random);
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

        private CandidateMove ChooseMove(GameSession session, IReadOnlyList<CandidateMove> validMoves, IRandomSource random)
        {
            var bestScore = int.MinValue;
            var bestMoves = new List<CandidateMove>();
            foreach (var move in validMoves)
            {
                var score = ScoreMove(session, move) + random.Range(0, 4);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMoves.Clear();
                    bestMoves.Add(move);
                }
                else if (score == bestScore)
                {
                    bestMoves.Add(move);
                }
            }

            return bestMoves[random.Range(0, bestMoves.Count)];
        }

        private int ScoreMove(GameSession session, CandidateMove move)
        {
            var clone = session.Board.Clone();
            clone.SwapIngredients(move.First, move.Second);
            var matches = _matchFinder.FindMatches(clone, move.Second);
            var score = matches.Sum(match => match.Positions.Count);
            foreach (var match in matches)
            {
                score += MatchKindScore(match.Kind);
                foreach (var goal in session.GoalTracker.Goals.Where(item => !item.IsComplete))
                {
                    score += GoalScore(session.Board, goal.Goal, match);
                }
            }

            return score;
        }

        private static int MatchKindScore(MatchKind kind)
        {
            switch (kind)
            {
                case MatchKind.Lightning:
                    return 50;
                case MatchKind.Bomb:
                    return 40;
                case MatchKind.Line:
                    return 28;
                default:
                    return 10;
            }
        }

        private static int GoalScore(BoardState board, GoalData goal, MatchGroup match)
        {
            switch (goal.GoalType)
            {
                case GoalType.CollectIngredient:
                    return match.Ingredient == goal.Ingredient ? match.Positions.Count * 12 : 0;
                case GoalType.CreatePotion:
                    return PotionMatches(goal.Potion, match.CreatedPotion) ? 120 : 0;
                case GoalType.BreakObstacle:
                    return match.Positions.Sum(position => AdjacentObstacleScore(board, position, goal.Obstacle, clearTile: false));
                case GoalType.ClearTile:
                    return match.Positions.Sum(position => AdjacentObstacleScore(board, position, goal.Obstacle, clearTile: true));
                case GoalType.RestorePotionLab:
                    return match.Positions.Count * 8 + (match.CreatedPotion == PotionType.None ? 0 : 30);
                default:
                    return 0;
            }
        }

        private static bool PotionMatches(PotionType goalPotion, PotionType createdPotion)
        {
            if (goalPotion == PotionType.LineHorizontal || goalPotion == PotionType.LineVertical)
            {
                return createdPotion == PotionType.LineHorizontal || createdPotion == PotionType.LineVertical;
            }

            return goalPotion == createdPotion;
        }

        private static int AdjacentObstacleScore(BoardState board, GridPosition position, ObstacleType obstacle, bool clearTile)
        {
            var score = 0;
            foreach (var candidate in Adjacent(position).Where(board.InBounds))
            {
                var cell = board.GetCell(candidate);
                if (cell.Obstacle == obstacle)
                {
                    score += clearTile && candidate.Equals(position) ? 30 : 20;
                }
            }

            var currentCell = board.GetCell(position);
            if (clearTile && currentCell.Obstacle == obstacle)
            {
                score += 40;
            }

            return score;
        }

        private static IEnumerable<GridPosition> Adjacent(GridPosition position)
        {
            yield return new GridPosition(position.Row - 1, position.Column);
            yield return new GridPosition(position.Row + 1, position.Column);
            yield return new GridPosition(position.Row, position.Column - 1);
            yield return new GridPosition(position.Row, position.Column + 1);
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
