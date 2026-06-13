using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;

namespace PotionPopQuest.Tests
{
    public sealed class AnimationEventTests
    {
        [Test]
        public void TrySwap_ValidMoveEmitsSwapAndClearEvents()
        {
            var session = CreateSessionWithPatternBoard();
            var move = new CandidateMove(new GridPosition(2, 2), new GridPosition(3, 2));

            var result = session.TrySwap(move.First, move.Second);

            Assert.That(result.ValidMove, Is.True);
            Assert.That(result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.Swap), Is.True);
            Assert.That(result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.Clear), Is.True);
            Assert.That(result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.TileDropped || item.Kind == BoardAnimationEventKind.TileSpawned), Is.True);
        }

        [Test]
        public void TrySwap_InvalidMoveEmitsInvalidSwapEventAndKeepsMoves()
        {
            var session = CreateSessionWithPatternBoard();
            var before = session.MovesRemaining;

            var result = session.TrySwap(new GridPosition(0, 0), new GridPosition(0, 1));

            Assert.That(result.ValidMove, Is.False);
            Assert.That(session.MovesRemaining, Is.EqualTo(before));
            Assert.That(result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.InvalidSwap), Is.True);
        }

        [Test]
        public void TrySwap_MatchFourEmitsPotionCreatedEvent()
        {
            var session = CreateSessionWithPatternBoard();
            session.Board.SetIngredient(new GridPosition(2, 3), IngredientType.RedHerb);

            var result = session.TrySwap(new GridPosition(2, 2), new GridPosition(3, 2));

            Assert.That(result.ValidMove, Is.True);
            Assert.That(result.AnimationEvents.Any(item =>
                item.Kind == BoardAnimationEventKind.PotionCreated
                && (item.Potion == PotionType.LineHorizontal || item.Potion == PotionType.LineVertical)), Is.True);
        }

        [Test]
        public void TrySwap_ObstacleBreakEmitsObstacleEvents()
        {
            var session = CreateSessionWithPatternBoard();
            session.Board.SetObstacle(new GridPosition(2, 3), ObstacleType.WoodenBox, 1);

            var result = session.TrySwap(new GridPosition(2, 2), new GridPosition(3, 2));

            Assert.That(result.ValidMove, Is.True);
            Assert.That(result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.ObstacleDamaged), Is.True);
            Assert.That(result.AnimationEvents.Any(item => item.Kind == BoardAnimationEventKind.ObstacleDestroyed), Is.True);
        }

        [Test]
        public void TrySwap_CascadeEmitsOrderedCascadeEvents()
        {
            var session = CreateSessionWithPatternBoard(new ZeroRandomSource());
            session.Board.SetIngredient(new GridPosition(0, 0), IngredientType.RedHerb);
            session.Board.SetIngredient(new GridPosition(0, 1), IngredientType.RedHerb);
            session.Board.SetIngredient(new GridPosition(0, 2), IngredientType.BlueCrystal);
            session.Board.SetIngredient(new GridPosition(1, 2), IngredientType.RedHerb);

            var result = session.TrySwap(new GridPosition(0, 2), new GridPosition(1, 2));
            var cascadeStartIndex = result.AnimationEvents
                .Select((item, index) => new { item, index })
                .FirstOrDefault(pair => pair.item.Kind == BoardAnimationEventKind.CascadeStarted)
                ?.index ?? -1;
            var cascadeClearIndex = result.AnimationEvents
                .Select((item, index) => new { item, index })
                .FirstOrDefault(pair => pair.item.Kind == BoardAnimationEventKind.Clear && pair.item.CascadeIndex == 1)
                ?.index ?? -1;

            Assert.That(result.ValidMove, Is.True);
            Assert.That(result.Cascades, Is.GreaterThanOrEqualTo(1));
            Assert.That(cascadeStartIndex, Is.GreaterThanOrEqualTo(0));
            Assert.That(cascadeClearIndex, Is.GreaterThan(cascadeStartIndex));
        }

        private static GameSession CreateSessionWithPatternBoard(IRandomSource random = null)
        {
            var level = new LevelData(
                99,
                5,
                5,
                20,
                new[]
                {
                    IngredientType.RedHerb,
                    IngredientType.BlueCrystal,
                    IngredientType.GreenLeaf,
                    IngredientType.YellowStarDust,
                    IngredientType.PurpleMushroom
                },
                new[] { new GoalData(GoalType.CollectIngredient, 99, IngredientType.RedHerb) },
                new StarThresholds(100, 200, 300));
            var session = new GameSession(level, random: random ?? new DeterministicRandomSource(), logger: new NullGameLogger());
            FillNoMatchPattern(session.Board);
            session.Board.SetIngredient(new GridPosition(2, 0), IngredientType.RedHerb);
            session.Board.SetIngredient(new GridPosition(2, 1), IngredientType.RedHerb);
            session.Board.SetIngredient(new GridPosition(2, 2), IngredientType.BlueCrystal);
            session.Board.SetIngredient(new GridPosition(3, 2), IngredientType.RedHerb);
            return session;
        }

        private static void FillNoMatchPattern(BoardState board)
        {
            var ingredients = new[]
            {
                IngredientType.RedHerb,
                IngredientType.BlueCrystal,
                IngredientType.GreenLeaf,
                IngredientType.YellowStarDust,
                IngredientType.PurpleMushroom
            };

            foreach (var position in board.AllPositions())
            {
                board.SetObstacle(position, ObstacleType.None, 0);
                board.SetIngredient(position, ingredients[(position.Row * 2 + position.Column) % ingredients.Length]);
            }
        }

        private sealed class ZeroRandomSource : IRandomSource
        {
            public int Range(int minInclusive, int maxExclusive)
            {
                return minInclusive;
            }
        }
    }
}
