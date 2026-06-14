using System.Collections;
using System.Linq;
using NUnit.Framework;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

namespace PotionPopQuest.PlayMode.Tests
{
    public sealed class GameControllerPlayModeTests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("PotionPopQuest.SaveData.v1");
            DestroyExistingRuntime();
        }

        [TearDown]
        public void TearDown()
        {
            DestroyExistingRuntime();
            PlayerPrefs.DeleteKey("PotionPopQuest.SaveData.v1");
        }

        [UnityTest]
        public IEnumerator Start_BuildsMainMenuWithPrimaryActions()
        {
            CreateRuntime();
            yield return null;

            Assert.That(GameObject.Find("Potion Pop Quest Canvas"), Is.Not.Null);
            Assert.That(FindButton("Play"), Is.Not.Null);
            Assert.That(FindButton("Levels"), Is.Not.Null);
            Assert.That(FindButton("Settings"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator PlayButton_StartsLevelOneWithPolishedHud()
        {
            CreateRuntime();
            yield return null;

            FindButton("Play").onClick.Invoke();
            yield return null;

            Assert.That(GameObject.Find("Board Panel"), Is.Not.Null);
            Assert.That(GameObject.Find("Star Progress"), Is.Not.Null);
            Assert.That(GameObject.Find("Tutorial Banner"), Is.Not.Null);
            Assert.That(FindButton("Hint"), Is.Not.Null);
            Assert.That(GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None).Count(button => button.name == "Tile"), Is.GreaterThanOrEqualTo(64));
        }

        [UnityTest]
        public IEnumerator HintButton_HighlightsCandidateMove()
        {
            CreateRuntime();
            yield return null;

            FindButton("Play").onClick.Invoke();
            yield return null;

            FindButton("Hint").onClick.Invoke();
            yield return null;

            Assert.That(GameObject.FindObjectsByType<Outline>(FindObjectsSortMode.None).Length, Is.GreaterThanOrEqualTo(2));
        }

        [UnityTest]
        public IEnumerator BoardVisualPresenter_SwapReplayPreservesTileViewIdentity()
        {
            var canvas = new GameObject("Presenter Test Canvas", typeof(Canvas));
            var boardObject = new GameObject("Board Panel", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            boardObject.transform.SetParent(canvas.transform, false);
            var boardRoot = boardObject.GetComponent<RectTransform>();
            var floatingObject = new GameObject("Floating Feedback Layer", typeof(RectTransform));
            floatingObject.transform.SetParent(boardObject.transform, false);

            var presenter = new BoardVisualPresenter(
                new NullGameLogger(),
                new TileIconFactory(),
                () => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
            presenter.Configure(boardRoot, floatingObject.GetComponent<RectTransform>(), _ => { }, _ => { });

            var board = CreatePresenterBoard();
            var first = new GridPosition(0, 0);
            var second = new GridPosition(0, 1);
            presenter.Render(BoardSnapshot.From(board), null, UiFeedbackCue.None);
            Assert.That(presenter.TryGetTile(first, out var firstRect), Is.True);

            board.SwapIngredients(first, second);
            yield return presenter.Play(
                new[] { new BoardAnimationEvent(BoardAnimationEventKind.Swap, new[] { first, second }, first, second) },
                BoardSnapshot.From(board));

            Assert.That(presenter.TryGetTile(second, out var destinationRect), Is.True);
            Assert.That(destinationRect, Is.SameAs(firstRect));
            Object.DestroyImmediate(canvas);
        }

        private static void CreateRuntime()
        {
            var root = new GameObject("Potion Pop Quest PlayMode Test Runtime");
            root.AddComponent<GameController>();
        }

        private static Button FindButton(string label)
        {
            return GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None)
                .FirstOrDefault(button => button.name == $"Button - {label}");
        }

        private static void DestroyExistingRuntime()
        {
            foreach (var controller in GameObject.FindObjectsByType<GameController>(FindObjectsSortMode.None))
            {
                Object.DestroyImmediate(controller.gameObject);
            }

            foreach (var canvas in GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            {
                if (canvas.name == "Potion Pop Quest Canvas" || canvas.name == "Presenter Test Canvas")
                {
                    Object.DestroyImmediate(canvas.gameObject);
                }
            }
        }

        private static BoardState CreatePresenterBoard()
        {
            var board = new BoardState(3, 3);
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
                board.SetIngredient(position, ingredients[(position.Row + position.Column) % ingredients.Length]);
            }

            return board;
        }
    }
}
