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
            GameSessionState state,
            IReadOnlyList<BoardAnimationEvent> animationEvents = null)
        {
            ValidMove = validMove;
            Message = message;
            ScoreGained = scoreGained;
            Cascades = cascades;
            ClearedPositions = clearedPositions;
            CreatedPotions = createdPotions;
            State = state;
            AnimationEvents = animationEvents ?? Array.Empty<BoardAnimationEvent>();
        }

        public bool ValidMove { get; }
        public string Message { get; }
        public int ScoreGained { get; }
        public int Cascades { get; }
        public IReadOnlyList<GridPosition> ClearedPositions { get; }
        public IReadOnlyList<PotionType> CreatedPotions { get; }
        public GameSessionState State { get; }
        public IReadOnlyList<BoardAnimationEvent> AnimationEvents { get; }
    }

    public sealed class GameSession
    {
        private const int MaxCascadePasses = 20;
        private readonly MatchFinder _matchFinder;
        private readonly DropResolver _dropResolver;
        private readonly PotionResolver _potionResolver;
        private readonly ScoreManager _scoreManager;
        private readonly BoardMoveFinder _moveFinder;
        private readonly BoardShuffler _boardShuffler;
        private readonly IRandomSource _random;
        private readonly IGameLogger _logger;

        public GameSession(
            LevelData level,
            BoardGenerator boardGenerator = null,
            MatchFinder matchFinder = null,
            DropResolver dropResolver = null,
            PotionResolver potionResolver = null,
            ScoreManager scoreManager = null,
            BoardShuffler boardShuffler = null,
            IRandomSource random = null,
            IGameLogger logger = null)
        {
            Level = level ?? throw new ArgumentNullException(nameof(level));
            _random = random ?? new SystemRandomSource();
            _matchFinder = matchFinder ?? new MatchFinder();
            _dropResolver = dropResolver ?? new DropResolver();
            _potionResolver = potionResolver ?? new PotionResolver();
            _scoreManager = scoreManager ?? new ScoreManager();
            _moveFinder = new BoardMoveFinder(_matchFinder);
            _boardShuffler = boardShuffler ?? new BoardShuffler(_matchFinder, _moveFinder);
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
                return Invalid("Tiles are not swappable.", first, second);
            }

            var firstPotionBefore = Board.GetCell(first).Potion;
            var secondPotionBefore = Board.GetCell(second).Potion;
            Board.SwapIngredients(first, second);
            _logger.Log(LogCategory.Swap, $"Swapped {first} and {second}.");
            var animationEvents = new List<BoardAnimationEvent>
            {
                new BoardAnimationEvent(BoardAnimationEventKind.Swap, new[] { first, second }, first, second)
            };

            if (firstPotionBefore != PotionType.None || secondPotionBefore != PotionType.None)
            {
                return ResolvePotionSwap(first, second, firstPotionBefore, secondPotionBefore, animationEvents);
            }

            var matches = _matchFinder.FindMatches(Board, second);
            if (matches.Count == 0)
            {
                Board.SwapIngredients(first, second);
                _logger.Log(LogCategory.Swap, $"Invalid swap {first} -> {second}; swap reversed.");
                animationEvents.Add(new BoardAnimationEvent(BoardAnimationEventKind.InvalidSwap, new[] { first, second }, first, second));
                return Invalid("Swap did not create a match.", animationEvents);
            }

            MovesRemaining--;
            return ResolveMatches(matches, second, animationEvents);
        }

        public MoveResult TryShuffleIfNeeded()
        {
            if (State != GameSessionState.Playing)
            {
                return Invalid($"Level is already {State}.");
            }

            var animationEvents = new List<BoardAnimationEvent>();
            if (!TryShuffleBoardIfNeeded(animationEvents))
            {
                return Invalid("Board already has a valid move.", animationEvents);
            }

            return new MoveResult(
                true,
                "No moves available. Shuffled the board.",
                0,
                0,
                Array.Empty<GridPosition>(),
                Array.Empty<PotionType>(),
                State,
                animationEvents);
        }

        private MoveResult ResolvePotionSwap(
            GridPosition first,
            GridPosition second,
            PotionType firstPotionBefore,
            PotionType secondPotionBefore,
            List<BoardAnimationEvent> animationEvents)
        {
            MovesRemaining--;
            var activationPosition = firstPotionBefore != PotionType.None ? second : first;
            var activatedPotion = firstPotionBefore != PotionType.None ? firstPotionBefore : secondPotionBefore;
            if (firstPotionBefore != PotionType.None && secondPotionBefore != PotionType.None)
            {
                activatedPotion = PotionType.Mega;
            }

            var activation = _potionResolver.Resolve(Board, activationPosition, activatedPotion);
            animationEvents.Add(new BoardAnimationEvent(
                BoardAnimationEventKind.PotionActivated,
                activation.AffectedPositions,
                activationPosition,
                activationPosition,
                potion: activatedPotion));

            Board.GetCell(activationPosition).Potion = PotionType.None;
            var drop = _dropResolver.ClearDropAndSpawn(Board, activation.AffectedPositions, Level.ActiveIngredients, _random);
            AppendDropAnimationEvents(drop, animationEvents, 0);
            GoalTracker.ApplyMatchEvents(
                drop.ClearedIngredients,
                drop.DestroyedObstacles,
                drop.ClearedTiles,
                Array.Empty<PotionType>());

            var scoreGained = _scoreManager.CalculatePotionScore(activation.AffectedPositions.Count, activatedPotion);
            Score += scoreGained;
            _logger.Log(LogCategory.Potion, $"Activated {activatedPotion} at {activationPosition}, cleared {activation.AffectedPositions.Count} cells.");

            var cascadeScore = ResolveCascades(new List<GridPosition>(activation.AffectedPositions), new List<PotionType>(), animationEvents, out var cascades);
            Score += cascadeScore;
            scoreGained += cascadeScore;
            UpdateState(animationEvents);
            TryShuffleBoardIfNeeded(animationEvents);
            return new MoveResult(true, "Potion activated.", scoreGained, cascades, activation.AffectedPositions, Array.Empty<PotionType>(), State, animationEvents);
        }

        private MoveResult ResolveMatches(IReadOnlyList<MatchGroup> initialMatches, GridPosition priorityAnchor, List<BoardAnimationEvent> animationEvents)
        {
            var allCleared = new List<GridPosition>();
            var allCreatedPotions = new List<PotionType>();
            var scoreGained = ApplyMatchPass(initialMatches, 0, allCleared, allCreatedPotions, animationEvents);
            scoreGained += ResolveCascades(allCleared, allCreatedPotions, animationEvents, out var cascades);
            Score += scoreGained;
            UpdateState(animationEvents);
            TryShuffleBoardIfNeeded(animationEvents);

            _logger.Log(LogCategory.Match, $"Move resolved with {allCleared.Count} cleared positions, {cascades} cascades, +{scoreGained} score.");
            return new MoveResult(true, "Match resolved.", scoreGained, cascades, allCleared, allCreatedPotions, State, animationEvents);
        }

        private bool TryShuffleBoardIfNeeded(ICollection<BoardAnimationEvent> animationEvents)
        {
            if (State != GameSessionState.Playing || _moveFinder.TryFindValidMove(Board, out _))
            {
                return false;
            }

            if (_boardShuffler.TryShuffle(Board, _random, out var shuffledPositions))
            {
                animationEvents.Add(new BoardAnimationEvent(BoardAnimationEventKind.BoardShuffled, shuffledPositions));
                _logger.Log(LogCategory.Board, $"No valid moves remained; shuffled {shuffledPositions.Count} movable tiles.");
                return true;
            }

            _logger.Warn(LogCategory.Board, "No valid moves remained, but board shuffle could not find a playable layout.");
            return false;
        }

        private int ResolveCascades(
            List<GridPosition> allCleared,
            List<PotionType> allCreatedPotions,
            List<BoardAnimationEvent> animationEvents,
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
                scoreGained += ApplyMatchPass(matches, cascade, allCleared, allCreatedPotions, animationEvents);
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
            ICollection<PotionType> allCreatedPotions,
            List<BoardAnimationEvent> animationEvents)
        {
            if (cascadeIndex > 0)
            {
                animationEvents.Add(new BoardAnimationEvent(
                    BoardAnimationEventKind.CascadeStarted,
                    matches.SelectMany(match => match.Positions),
                    cascadeIndex: cascadeIndex));
            }

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

            animationEvents.Add(new BoardAnimationEvent(
                BoardAnimationEventKind.Clear,
                impactPositions,
                cascadeIndex: cascadeIndex));

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
                    animationEvents.Add(new BoardAnimationEvent(
                        BoardAnimationEventKind.PotionCreated,
                        new[] { anchor.Key },
                        anchor.Key,
                        anchor.Key,
                        match.Ingredient,
                        match.CreatedPotion,
                        cascadeIndex: cascadeIndex));
                }
            }

            AppendDropAnimationEvents(drop, animationEvents, cascadeIndex);

            GoalTracker.ApplyMatchEvents(
                matches.SelectMany(match => match.Positions).Select(position => new ClearedIngredient(position, matches.First(m => m.Positions.Contains(position)).Ingredient)),
                drop.DestroyedObstacles,
                drop.ClearedTiles,
                createdPotions);

            _logger.Log(LogCategory.Goals, $"Updated goals after {matches.Count} match groups.");
            return _scoreManager.CalculateMatchScore(matches, cascadeIndex);
        }

        private static void AppendDropAnimationEvents(
            DropResult drop,
            ICollection<BoardAnimationEvent> animationEvents,
            int cascadeIndex)
        {
            foreach (var damaged in drop.DamagedObstacles)
            {
                animationEvents.Add(new BoardAnimationEvent(
                    BoardAnimationEventKind.ObstacleDamaged,
                    new[] { damaged.Position },
                    obstacle: damaged.ObstacleType,
                    cascadeIndex: cascadeIndex));
            }

            foreach (var destroyed in drop.DestroyedObstacles)
            {
                animationEvents.Add(new BoardAnimationEvent(
                    BoardAnimationEventKind.ObstacleDestroyed,
                    new[] { destroyed.Position },
                    obstacle: destroyed.ObstacleType,
                    cascadeIndex: cascadeIndex));
            }

            foreach (var tile in drop.ClearedTiles)
            {
                animationEvents.Add(new BoardAnimationEvent(
                    BoardAnimationEventKind.ObstacleDestroyed,
                    new[] { tile.Position },
                    obstacle: tile.ObstacleType,
                    cascadeIndex: cascadeIndex));
            }

            foreach (var movement in drop.DroppedTiles)
            {
                animationEvents.Add(new BoardAnimationEvent(
                    BoardAnimationEventKind.TileDropped,
                    new[] { movement.To },
                    movement.From,
                    movement.To,
                    movement.Ingredient,
                    movement.Potion,
                    cascadeIndex: cascadeIndex));
            }

            foreach (var spawn in drop.SpawnedTiles)
            {
                animationEvents.Add(new BoardAnimationEvent(
                    BoardAnimationEventKind.TileSpawned,
                    new[] { spawn.Position },
                    new GridPosition(-1, spawn.Position.Column),
                    spawn.Position,
                    spawn.Ingredient,
                    cascadeIndex: cascadeIndex));
            }
        }

        private void UpdateState(ICollection<BoardAnimationEvent> animationEvents)
        {
            if (GoalTracker.IsComplete)
            {
                State = GameSessionState.Won;
                animationEvents.Add(new BoardAnimationEvent(BoardAnimationEventKind.Win));
                _logger.Log(LogCategory.Goals, $"Level {Level.LevelNumber} complete with {Score} score and {Stars} stars.");
                return;
            }

            if (MovesRemaining <= 0)
            {
                State = GameSessionState.Lost;
                animationEvents.Add(new BoardAnimationEvent(BoardAnimationEventKind.Lose));
                _logger.Log(LogCategory.Goals, $"Level {Level.LevelNumber} failed at {Score} score.");
            }
        }

        private MoveResult Invalid(string message, GridPosition first, GridPosition second)
        {
            return Invalid(message, new[]
            {
                new BoardAnimationEvent(BoardAnimationEventKind.InvalidSwap, new[] { first, second }, first, second)
            });
        }

        private MoveResult Invalid(string message, IReadOnlyList<BoardAnimationEvent> animationEvents = null)
        {
            return new MoveResult(false, message, 0, 0, Array.Empty<GridPosition>(), Array.Empty<PotionType>(), State, animationEvents);
        }
    }
}
