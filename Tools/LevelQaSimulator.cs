using System;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;

public static class LevelQaSimulator
{
    private const int DefaultAttempts = 100;
    private const float MinimumTargetWinRate = 0.60f;
    private const float MaximumTargetWinRate = 0.95f;

    public static int Main(string[] args)
    {
        var attempts = args.Length > 0 && int.TryParse(args[0], out var parsedAttempts)
            ? Math.Max(1, parsedAttempts)
            : DefaultAttempts;
        var levels = MvpLevelCatalog.CreateLevels();
        var reports = new List<LevelReport>();

        Console.WriteLine("Potion Pop Quest level QA simulation");
        Console.WriteLine("Attempts per level: " + attempts);
        Console.WriteLine("Policy: greedy goal-progress move picker");
        Console.WriteLine();

        foreach (var level in levels)
        {
            var report = RunLevel(level, attempts);
            reports.Add(report);
            PrintReport(report);
        }

        Console.WriteLine("Summary");
        foreach (var report in reports)
        {
            Console.WriteLine(
                $"L{report.LevelNumber}: win {report.WinRate:P0}, avg left {report.AverageMovesLeft:0.0}, " +
                $"stuck {report.StuckCount}, {report.Difficulty}");
        }

        return reports.Any(report => report.StuckCount > 0 || report.WinRate < 0.35f) ? 1 : 0;
    }

    private static LevelReport RunLevel(LevelData level, int attempts)
    {
        var outcomes = new List<AttemptOutcome>();
        for (var seed = 0; seed < attempts; seed++)
        {
            outcomes.Add(RunAttempt(level, seed));
        }

        return new LevelReport(level.LevelNumber, level.DisplayName, level.Moves, outcomes);
    }

    private static AttemptOutcome RunAttempt(LevelData level, int seed)
    {
        var session = new GameSession(level, random: new SystemRandomSource(seed), logger: new NullGameLogger());
        var moveFinder = new BoardMoveFinder();
        var movesMade = 0;
        var stuck = false;

        while (session.State == GameSessionState.Playing && session.MovesRemaining > 0)
        {
            var moves = FindCandidateMoves(session.Board, moveFinder).ToArray();
            if (moves.Length == 0)
            {
                stuck = true;
                break;
            }

            var move = moves
                .Select(candidate => new ScoredMove(candidate, ScoreMove(session, candidate)))
                .OrderByDescending(item => item.Score)
                .ThenBy(item => item.Move.First.Row)
                .ThenBy(item => item.Move.First.Column)
                .ThenBy(item => item.Move.Second.Row)
                .ThenBy(item => item.Move.Second.Column)
                .First()
                .Move;

            var result = session.TrySwap(move.First, move.Second);
            if (!result.ValidMove)
            {
                stuck = true;
                break;
            }

            movesMade++;
        }

        return new AttemptOutcome(
            session.State == GameSessionState.Won,
            stuck,
            movesMade,
            session.MovesRemaining,
            session.Score,
            session.Stars);
    }

    private static IEnumerable<CandidateMove> FindCandidateMoves(BoardState board, BoardMoveFinder moveFinder)
    {
        var yielded = new HashSet<string>();
        foreach (var move in moveFinder.FindValidMoves(board))
        {
            yielded.Add(Key(move));
            yield return move;
        }

        foreach (var position in board.AllPositions())
        {
            var right = new GridPosition(position.Row, position.Column + 1);
            var down = new GridPosition(position.Row + 1, position.Column);
            if (IsPotionMove(board, position, right))
            {
                var move = new CandidateMove(position, right);
                if (yielded.Add(Key(move)))
                {
                    yield return move;
                }
            }

            if (IsPotionMove(board, position, down))
            {
                var move = new CandidateMove(position, down);
                if (yielded.Add(Key(move)))
                {
                    yield return move;
                }
            }
        }
    }

    private static bool IsPotionMove(BoardState board, GridPosition first, GridPosition second)
    {
        return BoardRules.CanSwap(board, first, second)
               && (board.GetCell(first).Potion != PotionType.None || board.GetCell(second).Potion != PotionType.None);
    }

    private static int ScoreMove(GameSession session, CandidateMove move)
    {
        var board = session.Board.Clone();
        var firstPotion = board.GetCell(move.First).Potion;
        var secondPotion = board.GetCell(move.Second).Potion;
        board.SwapIngredients(move.First, move.Second);

        if (firstPotion != PotionType.None || secondPotion != PotionType.None)
        {
            var activationPosition = firstPotion != PotionType.None ? move.Second : move.First;
            var potion = firstPotion != PotionType.None ? firstPotion : secondPotion;
            if (firstPotion != PotionType.None && secondPotion != PotionType.None)
            {
                potion = PotionType.Mega;
            }

            var affected = new PotionResolver().Resolve(board, activationPosition, potion).AffectedPositions;
            return 10000 + ScoreAffectedPositions(session, board, affected) + PotionValue(potion);
        }

        var matches = new MatchFinder().FindMatches(board, move.Second);
        var affectedPositions = matches.SelectMany(match => match.Positions).Distinct().ToArray();
        var score = ScoreAffectedPositions(session, board, affectedPositions);
        score += matches.Sum(match => match.Positions.Count * 100);
        score += matches.Sum(match => PotionValue(match.CreatedPotion));

        return score;
    }

    private static int ScoreAffectedPositions(GameSession session, BoardState board, IEnumerable<GridPosition> positions)
    {
        var score = 0;
        var positionSet = positions.Distinct().ToArray();
        foreach (var goal in session.GoalTracker.Goals.Where(goal => !goal.IsComplete))
        {
            switch (goal.Goal.GoalType)
            {
                case GoalType.CollectIngredient:
                    score += positionSet.Count(position => board.GetCell(position).Ingredient == goal.Goal.Ingredient) * 1200;
                    break;
                case GoalType.BreakObstacle:
                    score += AdjacentToObstacle(board, positionSet, goal.Goal.Obstacle) * 1800;
                    break;
                case GoalType.ClearTile:
                    score += positionSet.Count(position => board.GetCell(position).Obstacle == goal.Goal.Obstacle) * 1800;
                    break;
                case GoalType.CreatePotion:
                    break;
                case GoalType.RestorePotionLab:
                    score += positionSet.Count(position => board.GetCell(position).Ingredient != IngredientType.None) * 250;
                    score += AdjacentToObstacle(board, positionSet, ObstacleType.WoodenBox) * 700;
                    score += positionSet.Count(position => board.GetCell(position).Obstacle == ObstacleType.DarkTile) * 700;
                    break;
            }
        }

        return score;
    }

    private static int AdjacentToObstacle(BoardState board, IEnumerable<GridPosition> positions, ObstacleType obstacle)
    {
        var damaged = new HashSet<GridPosition>();
        foreach (var position in positions)
        {
            foreach (var neighbor in Adjacent(position))
            {
                if (board.InBounds(neighbor) && board.GetCell(neighbor).Obstacle == obstacle)
                {
                    damaged.Add(neighbor);
                }
            }
        }

        return damaged.Count;
    }

    private static IEnumerable<GridPosition> Adjacent(GridPosition position)
    {
        yield return new GridPosition(position.Row - 1, position.Column);
        yield return new GridPosition(position.Row + 1, position.Column);
        yield return new GridPosition(position.Row, position.Column - 1);
        yield return new GridPosition(position.Row, position.Column + 1);
    }

    private static int PotionValue(PotionType potion)
    {
        switch (potion)
        {
            case PotionType.LineHorizontal:
            case PotionType.LineVertical:
                return 3500;
            case PotionType.Bomb:
                return 4500;
            case PotionType.Lightning:
                return 5500;
            case PotionType.Mega:
                return 7500;
            default:
                return 0;
        }
    }

    private static string Key(CandidateMove move)
    {
        return move.First + "->" + move.Second;
    }

    private static void PrintReport(LevelReport report)
    {
        Console.WriteLine(
            $"L{report.LevelNumber} {report.Name}: wins {report.Wins}/{report.Attempts} ({report.WinRate:P0}), " +
            $"avg moves left {report.AverageMovesLeft:0.0}, avg score {report.AverageScore:0}, stars {report.AverageStars:0.0}, " +
            $"score p50/p75/p90 {report.ScorePercentile(0.50f):0}/{report.ScorePercentile(0.75f):0}/{report.ScorePercentile(0.90f):0}, " +
            $"stuck {report.StuckCount}, {report.Difficulty}");
    }

    private sealed class ScoredMove
    {
        public ScoredMove(CandidateMove move, int score)
        {
            Move = move;
            Score = score;
        }

        public CandidateMove Move { get; }
        public int Score { get; }
    }

    private sealed class AttemptOutcome
    {
        public AttemptOutcome(bool won, bool stuck, int movesMade, int movesLeft, int score, int stars)
        {
            Won = won;
            Stuck = stuck;
            MovesMade = movesMade;
            MovesLeft = movesLeft;
            Score = score;
            Stars = stars;
        }

        public bool Won { get; }
        public bool Stuck { get; }
        public int MovesMade { get; }
        public int MovesLeft { get; }
        public int Score { get; }
        public int Stars { get; }
    }

    private sealed class LevelReport
    {
        private readonly IReadOnlyList<AttemptOutcome> _outcomes;

        public LevelReport(int levelNumber, string name, int availableMoves, IReadOnlyList<AttemptOutcome> outcomes)
        {
            LevelNumber = levelNumber;
            Name = name;
            AvailableMoves = availableMoves;
            _outcomes = outcomes;
        }

        public int LevelNumber { get; }
        public string Name { get; }
        public int AvailableMoves { get; }
        public int Attempts => _outcomes.Count;
        public int Wins => _outcomes.Count(outcome => outcome.Won);
        public int StuckCount => _outcomes.Count(outcome => outcome.Stuck);
        public float WinRate => Wins / (float)Attempts;
        public float AverageMovesLeft => _outcomes.Where(outcome => outcome.Won).DefaultIfEmpty().Average(outcome => outcome == null ? 0f : outcome.MovesLeft);
        public float AverageScore => (float)_outcomes.Average(outcome => outcome.Score);
        public float AverageStars => (float)_outcomes.Average(outcome => outcome.Stars);

        public float ScorePercentile(float percentile)
        {
            var scores = _outcomes.Select(outcome => outcome.Score).OrderBy(score => score).ToArray();
            if (scores.Length == 0)
            {
                return 0f;
            }

            var index = Math.Max(0, Math.Min(scores.Length - 1, (int)Math.Round((scores.Length - 1) * percentile)));
            return scores[index];
        }

        public string Difficulty
        {
            get
            {
                if (StuckCount > 0)
                {
                    return "BROKEN/STUCK";
                }

                if (WinRate < MinimumTargetWinRate)
                {
                    return "TOO HARD";
                }

                if (WinRate > MaximumTargetWinRate)
                {
                    return "TOO EASY";
                }

                return "GOOD";
            }
        }
    }
}
