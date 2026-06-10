using System;
using System.Collections.Generic;
using System.Linq;

namespace PotionPopQuest.Core
{
    public sealed class MatchGroup
    {
        public MatchGroup(
            IngredientType ingredient,
            IEnumerable<GridPosition> positions,
            MatchKind kind,
            GridPosition anchor,
            PotionType createdPotion)
        {
            Ingredient = ingredient;
            Positions = positions.Distinct().ToArray();
            Kind = kind;
            Anchor = anchor;
            CreatedPotion = createdPotion;
        }

        public IngredientType Ingredient { get; }
        public IReadOnlyList<GridPosition> Positions { get; }
        public MatchKind Kind { get; }
        public GridPosition Anchor { get; }
        public PotionType CreatedPotion { get; }
    }

    internal sealed class MatchRun
    {
        public MatchRun(IngredientType ingredient, bool horizontal, IEnumerable<GridPosition> positions)
        {
            Ingredient = ingredient;
            Horizontal = horizontal;
            Positions = positions.ToArray();
        }

        public IngredientType Ingredient { get; }
        public bool Horizontal { get; }
        public IReadOnlyList<GridPosition> Positions { get; }
    }

    public sealed class MatchFinder
    {
        public IReadOnlyList<MatchGroup> FindMatches(BoardState board, GridPosition? priorityAnchor = null)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var runs = FindRuns(board);
            if (runs.Count == 0)
            {
                return Array.Empty<MatchGroup>();
            }

            return MergeRunsIntoGroups(runs, priorityAnchor);
        }

        private static List<MatchRun> FindRuns(BoardState board)
        {
            var runs = new List<MatchRun>();

            for (var row = 0; row < board.Height; row++)
            {
                var column = 0;
                while (column < board.Width)
                {
                    var start = column;
                    var ingredient = MatchableIngredientAt(board, new GridPosition(row, column));
                    while (column < board.Width && MatchableIngredientAt(board, new GridPosition(row, column)) == ingredient)
                    {
                        column++;
                    }

                    if (ingredient != IngredientType.None && column - start >= 3)
                    {
                        var positions = Enumerable.Range(start, column - start).Select(c => new GridPosition(row, c));
                        runs.Add(new MatchRun(ingredient, true, positions));
                    }

                    if (column == start)
                    {
                        column++;
                    }
                }
            }

            for (var column = 0; column < board.Width; column++)
            {
                var row = 0;
                while (row < board.Height)
                {
                    var start = row;
                    var ingredient = MatchableIngredientAt(board, new GridPosition(row, column));
                    while (row < board.Height && MatchableIngredientAt(board, new GridPosition(row, column)) == ingredient)
                    {
                        row++;
                    }

                    if (ingredient != IngredientType.None && row - start >= 3)
                    {
                        var positions = Enumerable.Range(start, row - start).Select(r => new GridPosition(r, column));
                        runs.Add(new MatchRun(ingredient, false, positions));
                    }

                    if (row == start)
                    {
                        row++;
                    }
                }
            }

            return runs;
        }

        private static IngredientType MatchableIngredientAt(BoardState board, GridPosition position)
        {
            var cell = board.GetCell(position);
            return cell.AcceptsIngredient ? cell.Ingredient : IngredientType.None;
        }

        private static IReadOnlyList<MatchGroup> MergeRunsIntoGroups(IReadOnlyList<MatchRun> runs, GridPosition? priorityAnchor)
        {
            var groups = new List<MatchGroup>();
            var visited = new bool[runs.Count];

            for (var i = 0; i < runs.Count; i++)
            {
                if (visited[i])
                {
                    continue;
                }

                var component = new List<MatchRun>();
                var queue = new Queue<int>();
                queue.Enqueue(i);
                visited[i] = true;

                while (queue.Count > 0)
                {
                    var currentIndex = queue.Dequeue();
                    var current = runs[currentIndex];
                    component.Add(current);

                    for (var j = 0; j < runs.Count; j++)
                    {
                        if (visited[j] || runs[j].Ingredient != current.Ingredient)
                        {
                            continue;
                        }

                        if (current.Positions.Any(runs[j].Positions.Contains))
                        {
                            visited[j] = true;
                            queue.Enqueue(j);
                        }
                    }
                }

                var positions = component.SelectMany(run => run.Positions).Distinct().ToArray();
                var kind = Classify(component, positions);
                var anchor = ChooseAnchor(positions, priorityAnchor);
                var potion = PotionFor(kind, component);
                groups.Add(new MatchGroup(component[0].Ingredient, positions, kind, anchor, potion));
            }

            return groups;
        }

        private static MatchKind Classify(IReadOnlyList<MatchRun> component, IReadOnlyList<GridPosition> positions)
        {
            var hasHorizontal = component.Any(run => run.Horizontal);
            var hasVertical = component.Any(run => !run.Horizontal);
            if (hasHorizontal && hasVertical)
            {
                return MatchKind.Lightning;
            }

            var longestRun = component.Max(run => run.Positions.Count);
            if (longestRun >= 5 || positions.Count >= 5)
            {
                return MatchKind.Bomb;
            }

            return longestRun >= 4 ? MatchKind.Line : MatchKind.Basic;
        }

        private static GridPosition ChooseAnchor(IReadOnlyList<GridPosition> positions, GridPosition? priorityAnchor)
        {
            if (priorityAnchor.HasValue && positions.Contains(priorityAnchor.Value))
            {
                return priorityAnchor.Value;
            }

            return positions.OrderBy(position => position.Row).ThenBy(position => position.Column).First();
        }

        private static PotionType PotionFor(MatchKind kind, IReadOnlyList<MatchRun> component)
        {
            switch (kind)
            {
                case MatchKind.Line:
                    var horizontal = component.OrderByDescending(run => run.Positions.Count).First().Horizontal;
                    return horizontal ? PotionType.LineHorizontal : PotionType.LineVertical;
                case MatchKind.Bomb:
                    return PotionType.Bomb;
                case MatchKind.Lightning:
                    return PotionType.Lightning;
                default:
                    return PotionType.None;
            }
        }
    }
}

