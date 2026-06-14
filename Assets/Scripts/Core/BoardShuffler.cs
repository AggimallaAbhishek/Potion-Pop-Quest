using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class BoardShuffler
    {
        private const int RandomAttempts = 80;
        private readonly MatchFinder _matchFinder;
        private readonly BoardMoveFinder _moveFinder;

        public BoardShuffler(MatchFinder matchFinder = null, BoardMoveFinder moveFinder = null)
        {
            _matchFinder = matchFinder ?? new MatchFinder();
            _moveFinder = moveFinder ?? new BoardMoveFinder(_matchFinder);
        }

        public bool TryShuffle(BoardState board, IRandomSource random, out IReadOnlyList<GridPosition> shuffledPositions)
        {
            return TryShuffle(board, random, out shuffledPositions, out _);
        }

        public bool TryShuffle(
            BoardState board,
            IRandomSource random,
            out IReadOnlyList<GridPosition> shuffledPositions,
            out IReadOnlyList<TileMovementEvent> movements)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (random == null)
            {
                throw new ArgumentNullException(nameof(random));
            }

            var positions = board.AllPositions()
                .Where(position => board.GetCell(position).CanMoveIngredient)
                .ToArray();
            var originalPayloads = positions
                .Select(position => TilePayload.From(board.GetCell(position), position))
                .ToArray();

            shuffledPositions = Array.Empty<GridPosition>();
            movements = Array.Empty<TileMovementEvent>();
            if (positions.Length < 3)
            {
                return false;
            }

            for (var offset = 1; offset < originalPayloads.Length; offset++)
            {
                var rotated = Rotate(originalPayloads, offset);
                if (TryApplyCandidate(board, positions, rotated))
                {
                    shuffledPositions = positions;
                    movements = BuildMovements(positions, rotated);
                    return true;
                }
            }

            for (var attempt = 0; attempt < RandomAttempts; attempt++)
            {
                var candidate = originalPayloads.ToArray();
                Shuffle(candidate, random);
                if (SameOrder(originalPayloads, candidate))
                {
                    continue;
                }

                if (TryApplyCandidate(board, positions, candidate))
                {
                    shuffledPositions = positions;
                    movements = BuildMovements(positions, candidate);
                    return true;
                }
            }

            ApplyPayloads(board, positions, originalPayloads);
            return false;
        }

        private bool TryApplyCandidate(BoardState board, IReadOnlyList<GridPosition> positions, IReadOnlyList<TilePayload> payloads)
        {
            ApplyPayloads(board, positions, payloads);
            return _matchFinder.FindMatches(board).Count == 0
                   && _moveFinder.TryFindValidMove(board, out _);
        }

        private static TilePayload[] Rotate(IReadOnlyList<TilePayload> payloads, int offset)
        {
            var rotated = new TilePayload[payloads.Count];
            for (var index = 0; index < payloads.Count; index++)
            {
                rotated[index] = payloads[(index + offset) % payloads.Count];
            }

            return rotated;
        }

        private static void Shuffle(IList<TilePayload> payloads, IRandomSource random)
        {
            for (var index = payloads.Count - 1; index > 0; index--)
            {
                var swapIndex = random.Range(0, index + 1);
                var current = payloads[index];
                payloads[index] = payloads[swapIndex];
                payloads[swapIndex] = current;
            }
        }

        private static bool SameOrder(IReadOnlyList<TilePayload> first, IReadOnlyList<TilePayload> second)
        {
            if (first.Count != second.Count)
            {
                return false;
            }

            for (var index = 0; index < first.Count; index++)
            {
                if (!first[index].Equals(second[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private static void ApplyPayloads(BoardState board, IReadOnlyList<GridPosition> positions, IReadOnlyList<TilePayload> payloads)
        {
            for (var index = 0; index < positions.Count; index++)
            {
                var cell = board.GetCell(positions[index]);
                cell.Ingredient = payloads[index].Ingredient;
                cell.Potion = payloads[index].Potion;
            }
        }

        private static IReadOnlyList<TileMovementEvent> BuildMovements(IReadOnlyList<GridPosition> positions, IReadOnlyList<TilePayload> payloads)
        {
            var movements = new List<TileMovementEvent>();
            for (var index = 0; index < positions.Count; index++)
            {
                var target = positions[index];
                var payload = payloads[index];
                if (payload.SourcePosition == target)
                {
                    continue;
                }

                movements.Add(new TileMovementEvent(payload.SourcePosition, target, payload.Ingredient, payload.Potion));
            }

            return movements;
        }

        private readonly struct TilePayload : IEquatable<TilePayload>
        {
            private TilePayload(IngredientType ingredient, PotionType potion, GridPosition sourcePosition)
            {
                Ingredient = ingredient;
                Potion = potion;
                SourcePosition = sourcePosition;
            }

            public IngredientType Ingredient { get; }
            public PotionType Potion { get; }
            public GridPosition SourcePosition { get; }

            public static TilePayload From(BoardCell cell, GridPosition sourcePosition)
            {
                return new TilePayload(cell.Ingredient, cell.Potion, sourcePosition);
            }

            public bool Equals(TilePayload other)
            {
                return Ingredient == other.Ingredient && Potion == other.Potion;
            }
        }
    }
}
