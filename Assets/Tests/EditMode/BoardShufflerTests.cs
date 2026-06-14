using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class BoardShufflerTests
    {
        [Test]
        public void TryShuffle_CreatesBoardWithAtLeastOneValidMove()
        {
            var board = CreateDeadBoard();
            var moveFinder = new BoardMoveFinder();
            var shuffler = new BoardShuffler();

            Assert.That(moveFinder.TryFindValidMove(board, out _), Is.False);

            var shuffled = shuffler.TryShuffle(board, new SystemRandomSource(7), out var shuffledPositions, out var movements);

            Assert.That(shuffled, Is.True);
            Assert.That(shuffledPositions, Is.Not.Empty);
            Assert.That(movements, Is.Not.Empty);
            Assert.That(movements.All(movement => shuffledPositions.Contains(movement.From) && shuffledPositions.Contains(movement.To)), Is.True);
            Assert.That(new MatchFinder().FindMatches(board), Is.Empty);
            Assert.That(moveFinder.TryFindValidMove(board, out _), Is.True);
        }

        [Test]
        public void TryShuffle_PreservesObstacleLayoutAndHealth()
        {
            var board = CreateDeadBoard();
            board.SetObstacle(new GridPosition(1, 1), ObstacleType.DarkTile, 1);
            board.SetObstacle(new GridPosition(0, 2), ObstacleType.WoodenBox, 1);
            var before = board.AllPositions()
                .ToDictionary(position => position, position => (board.GetCell(position).Obstacle, board.GetCell(position).ObstacleHealth));

            var shuffled = new BoardShuffler().TryShuffle(board, new SystemRandomSource(11), out _);

            Assert.That(shuffled, Is.True);
            foreach (var position in board.AllPositions())
            {
                var cell = board.GetCell(position);
                Assert.That(cell.Obstacle, Is.EqualTo(before[position].Obstacle));
                Assert.That(cell.ObstacleHealth, Is.EqualTo(before[position].ObstacleHealth));
            }
        }

        [Test]
        public void TryShuffleIfNeeded_DoesNotConsumeMoveAndEmitsBoardShuffled()
        {
            var level = new LevelData(
                100,
                3,
                3,
                12,
                new[] { IngredientType.RedHerb, IngredientType.BlueCrystal, IngredientType.GreenLeaf },
                new[] { new GoalData(GoalType.CollectIngredient, 99, IngredientType.RedHerb) },
                new StarThresholds(100, 200, 300));
            var session = new GameSession(level, random: new SystemRandomSource(5), logger: new NullGameLogger());
            FillDeadBoard(session.Board);
            var beforeMoves = session.MovesRemaining;

            var result = session.TryShuffleIfNeeded();

            Assert.That(result.ValidMove, Is.True);
            Assert.That(result.BoardBeforeMove, Is.Not.Null);
            Assert.That(session.MovesRemaining, Is.EqualTo(beforeMoves));
            var shuffleEvent = result.AnimationEvents.FirstOrDefault(item => item.Kind == BoardAnimationEventKind.BoardShuffled);
            Assert.That(shuffleEvent, Is.Not.Null);
            Assert.That(shuffleEvent.Movements, Is.Not.Empty);
            Assert.That(new BoardMoveFinder().TryFindValidMove(session.Board, out _), Is.True);
        }

        private static BoardState CreateDeadBoard()
        {
            var board = new BoardState(3, 3);
            FillDeadBoard(board);
            return board;
        }

        private static void FillDeadBoard(BoardState board)
        {
            var pattern = new[]
            {
                IngredientType.GreenLeaf,
                IngredientType.GreenLeaf,
                IngredientType.BlueCrystal,
                IngredientType.BlueCrystal,
                IngredientType.RedHerb,
                IngredientType.RedHerb,
                IngredientType.BlueCrystal,
                IngredientType.RedHerb,
                IngredientType.RedHerb
            };

            foreach (var position in board.AllPositions())
            {
                board.SetObstacle(position, ObstacleType.None, 0);
                board.SetIngredient(position, pattern[position.Row * board.Width + position.Column]);
            }
        }
    }
}
