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
            Assert.That(GameObject.Find("HUD Moves Badge"), Is.Not.Null);
            Assert.That(GameObject.Find("HUD Goal Strip"), Is.Not.Null);
            Assert.That(GameObject.Find("HUD Score Badge"), Is.Not.Null);
            Assert.That(GameObject.Find("Level Intro Overlay"), Is.Not.Null);
            Assert.That(GameObject.Find("Intro Obstacle Preview"), Is.Not.Null);
            Assert.That(GameObject.Find("Tutorial Banner"), Is.Not.Null);
            Assert.That(FindButton("Hint"), Is.Not.Null);
            Assert.That(GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None).Count(button => button.name == "Tile"), Is.GreaterThanOrEqualTo(64));
        }

        [UnityTest]
        public IEnumerator SettingsScreen_ShowsAudioSlidersAndVibrationToggle()
        {
            CreateRuntime();
            yield return null;

            FindButton("Settings").onClick.Invoke();
            yield return null;

            Assert.That(GameObject.Find("Slider - Music Volume"), Is.Not.Null);
            Assert.That(GameObject.Find("Slider - SFX Volume"), Is.Not.Null);
            Assert.That(GameObject.Find("Settings Audio Section"), Is.Not.Null);
            Assert.That(GameObject.Find("Settings Gameplay Section"), Is.Not.Null);
            Assert.That(GameObject.Find("Settings Progress Section"), Is.Not.Null);
            Assert.That(GameObject.Find("Toggle - Vibration"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator HintButton_HighlightsCandidateMove()
        {
            CreateRuntime();
            yield return null;

            FindButton("Play").onClick.Invoke();
            yield return null;
            DismissLevelIntro();
            yield return null;

            FindButton("Hint").onClick.Invoke();
            yield return null;

            Assert.That(GameObject.FindObjectsByType<Outline>(FindObjectsSortMode.None).Length, Is.GreaterThanOrEqualTo(2));
        }

        [UnityTest]
        public IEnumerator BoardVisualPresenter_SwapReplayPreservesTileViewIdentity()
        {
            var presenter = CreatePresenter(out var canvas);

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
            AssertPresenterMatchesSnapshot(presenter, BoardSnapshot.From(board));
            Object.DestroyImmediate(canvas);
        }

        [UnityTest]
        public IEnumerator BoardVisualPresenter_MixedDropAndSpawnDoesNotLoseSpawnTargetCell()
        {
            var presenter = CreatePresenter(out var canvas);
            var board = CreatePresenterBoard();
            var source = new GridPosition(0, 2);
            var destination = new GridPosition(1, 2);
            var droppedIngredient = board.GetCell(source).Ingredient;
            var spawnedIngredient = IngredientType.OrangeFireDrop;

            presenter.Render(BoardSnapshot.From(board), null, UiFeedbackCue.None);
            Assert.That(presenter.TryGetTile(source, out var sourceRect), Is.True);

            board.SetIngredient(source, spawnedIngredient);
            board.SetIngredient(destination, droppedIngredient);
            var finalSnapshot = BoardSnapshot.From(board);

            yield return presenter.Play(
                new[]
                {
                    new BoardAnimationEvent(BoardAnimationEventKind.TileDropped, new[] { destination }, source, destination, droppedIngredient),
                    new BoardAnimationEvent(BoardAnimationEventKind.TileSpawned, new[] { source }, new GridPosition(-1, source.Column), source, spawnedIngredient)
                },
                finalSnapshot);

            Assert.That(presenter.TryGetTile(destination, out var destinationRect), Is.True);
            Assert.That(destinationRect, Is.SameAs(sourceRect));
            Assert.That(presenter.TryGetTile(source, out var spawnedRect), Is.True);
            Assert.That(spawnedRect, Is.Not.SameAs(sourceRect));
            AssertPresenterMatchesSnapshot(presenter, finalSnapshot);
            Object.DestroyImmediate(canvas);
        }

        [UnityTest]
        public IEnumerator BoardVisualPresenter_DropOnlyPreservesMovedTileViewIdentity()
        {
            var presenter = CreatePresenter(out var canvas);
            var board = CreatePresenterBoard();
            var source = new GridPosition(0, 0);
            var destination = new GridPosition(2, 0);
            var droppedIngredient = board.GetCell(source).Ingredient;

            presenter.Render(BoardSnapshot.From(board), null, UiFeedbackCue.None);
            Assert.That(presenter.TryGetTile(source, out var sourceRect), Is.True);

            board.SetIngredient(source, IngredientType.None);
            board.SetIngredient(destination, droppedIngredient);
            var finalSnapshot = BoardSnapshot.From(board);

            yield return presenter.Play(
                new[]
                {
                    new BoardAnimationEvent(BoardAnimationEventKind.Clear, new[] { destination }),
                    new BoardAnimationEvent(BoardAnimationEventKind.TileDropped, new[] { destination }, source, destination, droppedIngredient)
                },
                finalSnapshot);

            Assert.That(presenter.TryGetTile(destination, out var destinationRect), Is.True);
            Assert.That(destinationRect, Is.SameAs(sourceRect));
            AssertPresenterMatchesSnapshot(presenter, finalSnapshot);
            Object.DestroyImmediate(canvas);
        }

        [UnityTest]
        public IEnumerator BoardVisualPresenter_SpawnOnlyRestoresSpawnPosition()
        {
            var presenter = CreatePresenter(out var canvas);
            var board = CreatePresenterBoard();
            var spawnPosition = new GridPosition(0, 1);
            var spawnedIngredient = IngredientType.OrangeFireDrop;

            presenter.Render(BoardSnapshot.From(board), null, UiFeedbackCue.None);
            Assert.That(presenter.TryGetTile(spawnPosition, out _), Is.True);

            board.SetIngredient(spawnPosition, spawnedIngredient);
            var finalSnapshot = BoardSnapshot.From(board);

            yield return presenter.Play(
                new[]
                {
                    new BoardAnimationEvent(BoardAnimationEventKind.Clear, new[] { spawnPosition }),
                    new BoardAnimationEvent(BoardAnimationEventKind.TileSpawned, new[] { spawnPosition }, new GridPosition(-1, spawnPosition.Column), spawnPosition, spawnedIngredient)
                },
                finalSnapshot);

            Assert.That(presenter.TryGetTile(spawnPosition, out var spawnedRect), Is.True);
            Assert.That(spawnedRect.gameObject.activeSelf, Is.True);
            AssertPresenterMatchesSnapshot(presenter, finalSnapshot);
            Object.DestroyImmediate(canvas);
        }

        [UnityTest]
        public IEnumerator BoardVisualPresenter_ClearDropAndSpawnEndsWithFullEightByEightMap()
        {
            var presenter = CreatePresenter(out var canvas);
            var board = CreatePresenterBoard(8, 8);
            var source = new GridPosition(0, 2);
            var destination = new GridPosition(3, 2);
            var spawnPosition = new GridPosition(0, 2);
            var droppedIngredient = board.GetCell(source).Ingredient;
            var spawnedIngredient = IngredientType.OrangeFireDrop;

            presenter.Render(BoardSnapshot.From(board), null, UiFeedbackCue.None);
            board.SetIngredient(spawnPosition, spawnedIngredient);
            board.SetIngredient(destination, droppedIngredient);
            var finalSnapshot = BoardSnapshot.From(board);

            yield return presenter.Play(
                new[]
                {
                    new BoardAnimationEvent(BoardAnimationEventKind.Clear, new[] { destination }),
                    new BoardAnimationEvent(BoardAnimationEventKind.TileDropped, new[] { destination }, source, destination, droppedIngredient),
                    new BoardAnimationEvent(BoardAnimationEventKind.TileSpawned, new[] { spawnPosition }, new GridPosition(-1, spawnPosition.Column), spawnPosition, spawnedIngredient)
                },
                finalSnapshot);

            Assert.That(presenter.TileViews.Count, Is.EqualTo(64));
            AssertPresenterMatchesSnapshot(presenter, finalSnapshot);
            Object.DestroyImmediate(canvas);
        }

        private static void CreateRuntime()
        {
            var root = new GameObject("Potion Pop Quest PlayMode Test Runtime");
            if (GameObject.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length == 0)
            {
                root.AddComponent<AudioListener>();
            }

            root.AddComponent<GameController>();
        }

        private static Button FindButton(string label)
        {
            return GameObject.FindObjectsByType<Button>(FindObjectsSortMode.None)
                .FirstOrDefault(button => button.name == $"Button - {label}");
        }

        private static void DismissLevelIntro()
        {
            var overlay = GameObject.Find("Level Intro Overlay");
            Assert.That(overlay, Is.Not.Null);
            var button = overlay.GetComponent<Button>();
            Assert.That(button, Is.Not.Null);
            button.onClick.Invoke();
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

        private static BoardVisualPresenter CreatePresenter(out GameObject canvas)
        {
            canvas = new GameObject("Presenter Test Canvas", typeof(Canvas));
            var boardObject = new GameObject("Board Panel", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            boardObject.transform.SetParent(canvas.transform, false);
            var floatingObject = new GameObject("Floating Feedback Layer", typeof(RectTransform));
            floatingObject.transform.SetParent(boardObject.transform, false);

            var presenter = new BoardVisualPresenter(
                new NullGameLogger(),
                new TileIconFactory(),
                () => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
            presenter.Configure(boardObject.GetComponent<RectTransform>(), floatingObject.GetComponent<RectTransform>(), _ => { }, _ => { });
            return presenter;
        }

        private static BoardState CreatePresenterBoard(int width = 3, int height = 3)
        {
            var board = new BoardState(width, height);
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

        private static void AssertPresenterMatchesSnapshot(BoardVisualPresenter presenter, BoardSnapshot snapshot)
        {
            Assert.That(presenter.TileViews.Count, Is.EqualTo(snapshot.Width * snapshot.Height));
            foreach (var position in snapshot.AllPositions())
            {
                Assert.That(presenter.TryGetTile(position, out _), Is.True, $"Missing tile view at {position}.");
            }
        }
    }
}
