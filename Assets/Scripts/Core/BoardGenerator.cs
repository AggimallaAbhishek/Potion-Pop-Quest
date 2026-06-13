using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class BoardGenerator
    {
        private const int MaxGenerationAttempts = 100;
        private readonly MatchFinder _matchFinder;
        private readonly BoardMoveFinder _moveFinder;

        public BoardGenerator(MatchFinder matchFinder = null)
        {
            _matchFinder = matchFinder ?? new MatchFinder();
            _moveFinder = new BoardMoveFinder(_matchFinder);
        }

        public BoardState Generate(LevelData level, IRandomSource random)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            for (var attempt = 0; attempt < MaxGenerationAttempts; attempt++)
            {
                var board = CreateCandidate(level, random);
                if (_matchFinder.FindMatches(board).Count == 0 && HasAnyValidMove(board))
                {
                    return board;
                }
            }

            var fallback = CreateCandidate(level, random);
            return TrySeedGuaranteedMove(fallback, level.ActiveIngredients, out var seeded)
                ? seeded
                : fallback;
        }

        public bool HasAnyValidMove(BoardState board)
        {
            return _moveFinder.TryFindValidMove(board, out _);
        }

        private BoardState CreateCandidate(LevelData level, IRandomSource random)
        {
            var board = new BoardState(level.GridWidth, level.GridHeight);
            foreach (var obstacle in level.Obstacles)
            {
                if (!board.InBounds(obstacle.Position))
                {
                    continue;
                }

                var health = obstacle.HealthOverride > 0
                    ? obstacle.HealthOverride
                    : BoardRules.DefaultObstacleHealth(obstacle.ObstacleType);
                board.SetObstacle(obstacle.Position, obstacle.ObstacleType, health);
            }

            foreach (var position in board.AllPositions())
            {
                var cell = board.GetCell(position);
                if (!cell.AcceptsIngredient || cell.HasIngredient)
                {
                    continue;
                }

                board.SetIngredient(position, PickIngredientAvoidingImmediateMatch(board, position, level.ActiveIngredients, random));
            }

            return board;
        }

        private IngredientType PickIngredientAvoidingImmediateMatch(
            BoardState board,
            GridPosition position,
            IReadOnlyList<IngredientType> ingredients,
            IRandomSource random)
        {
            var candidates = new List<IngredientType>(ingredients);
            while (candidates.Count > 0)
            {
                var index = random.Range(0, candidates.Count);
                var ingredient = candidates[index];
                candidates.RemoveAt(index);

                if (!WouldCreateImmediateMatch(board, position, ingredient))
                {
                    return ingredient;
                }
            }

            return ingredients[random.Range(0, ingredients.Count)];
        }

        private static bool WouldCreateImmediateMatch(BoardState board, GridPosition position, IngredientType ingredient)
        {
            return CountSame(board, position, ingredient, 0, -1) >= 2
                   || CountSame(board, position, ingredient, -1, 0) >= 2;
        }

        private static int CountSame(BoardState board, GridPosition start, IngredientType ingredient, int rowDelta, int columnDelta)
        {
            var count = 0;
            var position = new GridPosition(start.Row + rowDelta, start.Column + columnDelta);
            while (board.InBounds(position) && board.GetCell(position).Ingredient == ingredient)
            {
                count++;
                position = new GridPosition(position.Row + rowDelta, position.Column + columnDelta);
            }

            return count;
        }

        private bool TrySeedGuaranteedMove(
            BoardState board,
            IReadOnlyList<IngredientType> activeIngredients,
            out BoardState seeded)
        {
            seeded = null;
            if (activeIngredients.Count < 2)
            {
                return false;
            }

            for (var row = 0; row < board.Height; row++)
            {
                for (var column = 0; column < board.Width; column++)
                {
                    if (TrySeedHorizontalMove(board, activeIngredients, row, column, out seeded)
                        || TrySeedVerticalMove(board, activeIngredients, row, column, out seeded))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TrySeedHorizontalMove(
            BoardState board,
            IReadOnlyList<IngredientType> activeIngredients,
            int row,
            int column,
            out BoardState seeded)
        {
            seeded = null;
            var first = new GridPosition(row, column);
            var second = new GridPosition(row, column + 1);
            var swapSource = new GridPosition(row, column + 2);
            var swapTarget = new GridPosition(row + 1, column + 2);
            return TrySeedPattern(board, activeIngredients, first, second, swapSource, swapTarget, out seeded);
        }

        private bool TrySeedVerticalMove(
            BoardState board,
            IReadOnlyList<IngredientType> activeIngredients,
            int row,
            int column,
            out BoardState seeded)
        {
            seeded = null;
            var first = new GridPosition(row, column);
            var second = new GridPosition(row + 1, column);
            var swapSource = new GridPosition(row + 2, column);
            var swapTarget = new GridPosition(row + 2, column + 1);
            return TrySeedPattern(board, activeIngredients, first, second, swapSource, swapTarget, out seeded);
        }

        private bool TrySeedPattern(
            BoardState board,
            IReadOnlyList<IngredientType> activeIngredients,
            GridPosition first,
            GridPosition second,
            GridPosition swapSource,
            GridPosition swapTarget,
            out BoardState seeded)
        {
            seeded = null;
            if (!CanSeed(board, first)
                || !CanSeed(board, second)
                || !CanSeed(board, swapSource)
                || !CanSeed(board, swapTarget))
            {
                return false;
            }

            foreach (var matchIngredient in activeIngredients)
            {
                foreach (var fillerIngredient in activeIngredients.Where(item => item != matchIngredient))
                {
                    var candidate = board.Clone();
                    candidate.SetIngredient(first, matchIngredient);
                    candidate.SetIngredient(second, matchIngredient);
                    candidate.SetIngredient(swapSource, fillerIngredient);
                    candidate.SetIngredient(swapTarget, matchIngredient);

                    if (_matchFinder.FindMatches(candidate).Count == 0 && HasAnyValidMove(candidate))
                    {
                        seeded = candidate;
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CanSeed(BoardState board, GridPosition position)
        {
            return board.InBounds(position)
                   && board.GetCell(position).AcceptsIngredient
                   && !board.GetCell(position).LocksIngredient;
        }
    }
}
