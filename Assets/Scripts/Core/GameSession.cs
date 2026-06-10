using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public interface IGameLogger
    {
        void Log(LogCategory category, string message);
        void Warn(LogCategory category, string message);
    }

    public sealed class NullGameLogger : IGameLogger
    {
        public void Log(LogCategory category, string message)
        {
        }

        public void Warn(LogCategory category, string message)
        {
        }
    }

    public sealed class MoveResult
    {
        public MoveResult(
            bool validMove,
            string message,
            int scoreGained,
            int cascades,
            IReadOnlyList<GridPosition> clearedPositions,
            IReadOnlyList<PotionType> createdPotions,
            GameSessionState state)
        {
            ValidMove = validMove;
            Message = message;
            ScoreGained = scoreGained;
            Cascades = cascades;
            ClearedPositions = clearedPositions;
            CreatedPotions = createdPotions;
            State = state;
        }

        public bool ValidMove { get; }
        public string Message { get; }
        public int ScoreGained { get; }
        public int Cascades { get; }
        public IReadOnlyList<GridPosition> ClearedPositions { get; }
        public IReadOnlyList<PotionType> CreatedPotions { get; }
        public GameSessionState State { get; }
    }

    public sealed class GameSession
    {
        private const int MaxCascadePasses = 20;
        private readonly MatchFinder _matchFinder;
        private readonly DropResolver _dropResolver;
        private readonly PotionResolver _potionResolver;
        private readonly ScoreManager _scoreManager;
        private readonly IRandomSource _random;
        private readonly IGameLogger _logger;

        public GameSession(
            LevelData level,
            BoardGenerator boardGenerator = null,
            MatchFinder matchFinder = null,
            DropResolver dropResolver = null,
            PotionResolver potionResolver = null,
            ScoreManager scoreManager = null,
            IRandomSource random = null,
            IGameLogger logger = null)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            _random = random ?? new SystemRandomSource();
            _matchFinder = matchFinder ?? new MatchFinder();
            _dropResolver = dropResolver ?? new DropResolver();
            _potionResolver = potionResolver ?? new PotionResolver();
            _scoreManager = scoreManager ?? new ScoreManager();
            _logger = logger ?? new NullGameLogger();

            Board = (boardGenerator ?? new BoardGenerator(_matchFinder)).Generate(Level, _random);
            GoalTracker = new GoalTracker(Level.Goals);
            MovesRemaining = Level.Moves;
            State = GameSessionState.Playing;
            _logger.Log(LogCategory.Board, $"Started level {Level.LevelNumber} with {Board.Width}x{Board.Height} board.");
        }

        public LevelData Level { get; }
        public BoardState Board { get; }
        public GoalTracker GoalTracker { get; }
        public int MovesRemaining { get; private set; }
        public int Score { get; private set; }
        public int Stars => Level.StarThresholds.StarsForScore(Score);
        public GameSessionState State { get; private set; }

        public MoveResult TrySwap(GridPosition first, GridPosition second)
        {
            if (State != GameSessionState.Playing)
            {
                return Invalid($"Level is already {State}.");
            }

            if (!BoardRules.CanSwap(Board, first, second))
            {
                _logger.Warn(LogCategory.Swap, $"Rejected non-adjacent or blocked swap {first} -> {second}.");
                return Invalid("Tiles are not swappable.");
            }

            var firstPotionBefore = Board.GetCell(first).Potion;
            var secondPotionBefore = Board.GetCell(second).Potion;
            Board.SwapIngredients(first, second);
            _logger.Log(LogCategory.Swap, $"Swapped {first} and {second}.");

            if (firstPotionBefore != PotionType.None || secondPotionBefore != PotionType.None)
            {
                return ResolvePotionSwap(first, second, firstPotionBefore, secondPotionBefore);
            }

            var matches = _matchFinder.FindMatches(Board, second);
            if (matches.Count == 0)
            {
                Board.SwapIngredients(first, second);
                _logger.Log(LogCategory.Swap, $"Invalid swap {first} -> {second}; swap reversed.");
                return Invalid("Swap did not create a match.");
            }

            MovesRemaining--;
            return ResolveMatches(matches, second);
        }

        private MoveResult ResolvePotionSwap(
            GridPosition first,
            GridPosition second,
            PotionType firstPotionBefore,
            PotionType secondPotionBefore)
        {
            MovesRemaining--;
            var activationPosition = firstPotionBefore != PotionType.None ? second : first;
            var activatedPotion = firstPotionBefore != PotionType.None ? firstPotionBefore : secondPotionBefore;
            if (firstPotionBefore != PotionType.None && secondPotionBefore != PotionType.None)
            {
                activatedPotion = PotionType.Mega;
            }

            var activation = _potionResolver.Resolve(Board, activationPosition, activatedPotion);
            Board.GetCell(activationPosition).Potion = PotionType.None;
            var drop = _dropResolver.ClearDropAndSpawn(Board, activation.AffectedPositions, Level.ActiveIngredients, _random);
            GoalTracker.ApplyMatchEvents(
                drop.ClearedIngredients,
                drop.DestroyedObstacles,
                drop.ClearedTiles,
                Array.Empty<PotionType>());

            var scoreGained = _scoreManager.CalculatePotionScore(activation.AffectedPositions.Count, activatedPotion);
            Score += scoreGained;
            _logger.Log(LogCategory.Potion, $"Activated {activatedPotion} at {activationPosition}, cleared {activation.AffectedPositions.Count} cells.");

            var cascadeScore = ResolveCascades(new List<GridPosition>(activation.AffectedPositions), new List<PotionType>(), out var cascades);
            scoreGained += cascadeScore;
            UpdateState();
            return new MoveResult(true, "Potion activated.", scoreGained, cascades, activation.AffectedPositions, Array.Empty<PotionType>(), State);
        }

        private MoveResult ResolveMatches(IReadOnlyList<MatchGroup> initialMatches, GridPosition priorityAnchor)
        {
            var allCleared = new List<GridPosition>();
            var allCreatedPotions = new List<PotionType>();
            var scoreGained = ApplyMatchPass(initialMatches, 0, allCleared, allCreatedPotions);
            scoreGained += ResolveCascades(allCleared, allCreatedPotions, out var cascades);
            Score += scoreGained;
            UpdateState();

            _logger.Log(LogCategory.Match, $"Move resolved with {allCleared.Count} cleared positions, {cascades} cascades, +{scoreGained} score.");
            return new MoveResult(true, "Match resolved.", scoreGained, cascades, allCleared, allCreatedPotions, State);
        }

        private int ResolveCascades(
            List<GridPosition> allCleared,
            List<PotionType> allCreatedPotions,
            out int cascades)
        {
            var scoreGained = 0;
            cascades = 0;

            for (var cascade = 1; cascade <= MaxCascadePasses; cascade++)
            {
                var matches = _matchFinder.FindMatches(Board);
                if (matches.Count == 0)
                {
                    break;
                }

                cascades++;
                scoreGained += ApplyMatchPass(matches, cascade, allCleared, allCreatedPotions);
            }

            if (cascades >= MaxCascadePasses)
            {
                _logger.Warn(LogCategory.Drop, "Cascade safeguard reached. Check level generation and drop rules.");
            }

            return scoreGained;
        }

        private int ApplyMatchPass(
            IReadOnlyList<MatchGroup> matches,
            int cascadeIndex,
            ICollection<GridPosition> allCleared,
            ICollection<PotionType> allCreatedPotions)
        {
            var potionAnchors = matches
                .Where(match => match.CreatedPotion != PotionType.None)
                .ToDictionary(match => match.Anchor, match => match);
            var clearPositions = matches
                .SelectMany(match => match.Positions)
                .Where(position => !potionAnchors.ContainsKey(position))
                .Distinct()
                .ToArray();
            var impactPositions = matches
                .SelectMany(match => match.Positions)
                .Distinct()
                .ToArray();

            foreach (var position in impactPositions)
            {
                allCleared.Add(position);
            }

            var createdPotions = potionAnchors.Values.Select(match => match.CreatedPotion).ToArray();
            foreach (var potion in createdPotions)
            {
                allCreatedPotions.Add(potion);
            }

            var drop = _dropResolver.ClearDropAndSpawn(Board, clearPositions, Level.ActiveIngredients, _random, impactPositions);
            foreach (var anchor in potionAnchors)
            {
                var match = anchor.Value;
                var cell = Board.GetCell(anchor.Key);
                if (cell.AcceptsIngredient)
                {
                    cell.Ingredient = match.Ingredient;
                    cell.Potion = match.CreatedPotion;
                }
            }

            GoalTracker.ApplyMatchEvents(
                matches.SelectMany(match => match.Positions).Select(position => new ClearedIngredient(position, matches.First(m => m.Positions.Contains(position)).Ingredient)),
                drop.DestroyedObstacles,
                drop.ClearedTiles,
                createdPotions);

            _logger.Log(LogCategory.Goals, $"Updated goals after {matches.Count} match groups.");
            return _scoreManager.CalculateMatchScore(matches, cascadeIndex);
        }

        private void UpdateState()
        {
            if (GoalTracker.IsComplete)
            {
                State = GameSessionState.Won;
                _logger.Log(LogCategory.Goals, $"Level {Level.LevelNumber} complete with {Score} score and {Stars} stars.");
                return;
            }

            if (MovesRemaining <= 0)
            {
                State = GameSessionState.Lost;
                _logger.Log(LogCategory.Goals, $"Level {Level.LevelNumber} failed at {Score} score.");
            }
        }

        private MoveResult Invalid(string message)
        {
            return new MoveResult(false, message, 0, 0, Array.Empty<GridPosition>(), Array.Empty<PotionType>(), State);
        }
    }
}
