using System;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class GeneratedGameUi
    {
        private readonly IGameLogger _logger;
        private readonly TileIconFactory _iconFactory;
        private Font _font;
        private Transform _root;
        private UiFeedbackAnimator _feedbackAnimator;
        private GameObject _mainMenu;
        private GameObject _levelSelect;
        private GameObject _game;
        private GameObject _settings;
        private GameObject _modal;
        private RectTransform _boardRoot;
        private Text _movesText;
        private Text _goalText;
        private Text _scoreText;
        private Text _messageText;

        private Action _play;
        private Action _showLevels;
        private Action _showSettings;
        private Action _quit;
        private Action<int> _startLevel;
        private Action<GridPosition> _tilePressed;
        private Action _restart;
        private Action _nextLevel;
        private Action _mainMenuAction;
        private Action _resetProgress;
        private Action<bool> _toggleMusic;
        private Action<bool> _toggleSfx;
        private Action<GameSfxCue> _playSfx;

        public GeneratedGameUi(IGameLogger logger)
        {
            _logger = logger;
            _iconFactory = new TileIconFactory();
        }

        private Font Font
        {
            get
            {
                if (_font != null)
                {
                    return _font;
                }

                _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                return _font;
            }
        }

        public void Build(
            Transform parent,
            Action play,
            Action showLevels,
            Action showSettings,
            Action quit,
            Action<int> startLevel,
            Action<GridPosition> tilePressed,
            Action restart,
            Action nextLevel,
            Action mainMenu,
            Action resetProgress,
            Action<bool> toggleMusic,
            Action<bool> toggleSfx,
            Action<GameSfxCue> playSfx)
        {
            _play = play;
            _showLevels = showLevels;
            _showSettings = showSettings;
            _quit = quit;
            _startLevel = startLevel;
            _tilePressed = tilePressed;
            _restart = restart;
            _nextLevel = nextLevel;
            _mainMenuAction = mainMenu;
            _resetProgress = resetProgress;
            _toggleMusic = toggleMusic;
            _toggleSfx = toggleSfx;
            _playSfx = playSfx;

            EnsureEventSystem();
            var canvasObject = CreateCanvas(parent);
            _feedbackAnimator = canvasObject.AddComponent<UiFeedbackAnimator>();
            _root = canvasObject.transform;
            _mainMenu = CreateScreen("Main Menu");
            _levelSelect = CreateScreen("Level Select");
            _game = CreateScreen("Game");
            _settings = CreateScreen("Settings");
            _modal = CreateScreen("Modal");
            BuildMainMenu();
            BuildGameScreen();
        }

        public void ShowMainMenu()
        {
            HideAll();
            _mainMenu.SetActive(true);
        }

        public void ShowLevelSelect(IReadOnlyList<LevelData> levels, int highestUnlocked, Func<int, int> starsForLevel)
        {
            HideAll();
            ClearChildren(_levelSelect.transform);
            _levelSelect.SetActive(true);

            CreateTitle(_levelSelect.transform, "Level Select", 50);
            var grid = CreatePanel(_levelSelect.transform, "Levels Grid", new Color(0.12f, 0.16f, 0.20f, 0.85f));
            var gridRect = grid.GetComponent<RectTransform>();
            gridRect.sizeDelta = new Vector2(760, 760);
            var layout = grid.AddComponent<GridLayoutGroup>();
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 5;
            layout.cellSize = new Vector2(130, 130);
            layout.spacing = new Vector2(12, 12);
            layout.padding = new RectOffset(20, 20, 20, 20);

            foreach (var level in levels)
            {
                var locked = level.LevelNumber > highestUnlocked;
                var stars = starsForLevel(level.LevelNumber);
                var label = locked ? $"L{level.LevelNumber}\nLOCK" : $"L{level.LevelNumber}\n{StarLabel(stars)}";
                var button = CreateButton(grid.transform, label, () => _startLevel(level.LevelNumber), locked ? new Color(0.28f, 0.30f, 0.34f) : new Color(0.22f, 0.46f, 0.56f));
                button.interactable = !locked;
            }

            CreateButton(_levelSelect.transform, "Back", _mainMenuAction, new Color(0.28f, 0.22f, 0.32f), new Vector2(260, 72));
        }

        public void ShowSettings(bool musicEnabled, bool sfxEnabled)
        {
            HideAll();
            ClearChildren(_settings.transform);
            _settings.SetActive(true);
            CreateTitle(_settings.transform, "Settings", 50);
            CreateToggle(_settings.transform, "Music", musicEnabled, _toggleMusic);
            CreateToggle(_settings.transform, "SFX", sfxEnabled, _toggleSfx);
            CreateButton(_settings.transform, "Reset Progress", _resetProgress, new Color(0.55f, 0.22f, 0.22f), new Vector2(360, 76));
            CreateButton(_settings.transform, "Back", _mainMenuAction, new Color(0.28f, 0.22f, 0.32f), new Vector2(260, 72));
        }

        public void ShowGame(
            GameSession session,
            GridPosition? selectedTile,
            string message = null,
            UiFeedbackCue feedbackCue = UiFeedbackCue.None)
        {
            HideAll();
            _game.SetActive(true);
            UpdateHud(session, message);
            RenderBoard(session.Board, selectedTile);
            _feedbackAnimator.PlayBoardFeedback(feedbackCue, _boardRoot);
        }

        public void ShowWin(GameSession session, bool hasNextLevel)
        {
            ShowModal(
                "Level Complete",
                $"Score {session.Score}\nStars {StarLabel(session.Stars)}",
                hasNextLevel ? "Next" : "Levels",
                hasNextLevel ? _nextLevel : _showLevels);
        }

        public void ShowLose(GameSession session)
        {
            ShowModal(
                "Out of Moves",
                $"Score {session.Score}\nGoal still incomplete",
                "Retry",
                _restart);
        }

        private GameObject CreateCanvas(Transform parent)
        {
            var canvasObject = new GameObject("Potion Pop Quest Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(parent, false);

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;

            return canvasObject;
        }

        private void BuildMainMenu()
        {
            CreateTitle(_mainMenu.transform, "Potion Pop Quest", 60);
            CreateLabel(_mainMenu.transform, "2D Match-3 Potion Puzzle", 28, TextAnchor.MiddleCenter);
            CreateButton(_mainMenu.transform, "Play", _play, new Color(0.24f, 0.52f, 0.46f), new Vector2(320, 82));
            CreateButton(_mainMenu.transform, "Levels", _showLevels, new Color(0.24f, 0.42f, 0.62f), new Vector2(320, 82));
            CreateButton(_mainMenu.transform, "Settings", _showSettings, new Color(0.40f, 0.32f, 0.58f), new Vector2(320, 82));
            CreateButton(_mainMenu.transform, "Exit", _quit, new Color(0.42f, 0.30f, 0.30f), new Vector2(320, 82));
        }

        private void BuildGameScreen()
        {
            ClearChildren(_game.transform);
            var hud = CreatePanel(_game.transform, "HUD", new Color(0.10f, 0.13f, 0.17f, 0.92f));
            var hudRect = hud.GetComponent<RectTransform>();
            hudRect.sizeDelta = new Vector2(920, 150);
            var hudElement = hud.AddComponent<LayoutElement>();
            hudElement.preferredWidth = 920;
            hudElement.preferredHeight = 150;
            hudElement.flexibleWidth = 0;
            var hudLayout = hud.AddComponent<HorizontalLayoutGroup>();
            hudLayout.childAlignment = TextAnchor.MiddleCenter;
            hudLayout.spacing = 20;
            hudLayout.padding = new RectOffset(20, 20, 15, 15);
            _movesText = CreateLabel(hud.transform, "Moves 0", 26, TextAnchor.MiddleCenter);
            _goalText = CreateLabel(hud.transform, "Goal", 24, TextAnchor.MiddleCenter);
            _scoreText = CreateLabel(hud.transform, "Score 0", 26, TextAnchor.MiddleCenter);
            AddLayoutElement(_movesText.gameObject, 190, 110);
            AddLayoutElement(_goalText.gameObject, 480, 110);
            AddLayoutElement(_scoreText.gameObject, 190, 110);

            var boardPanel = CreatePanel(_game.transform, "Board Panel", new Color(0.16f, 0.18f, 0.21f, 0.94f));
            _boardRoot = boardPanel.GetComponent<RectTransform>();
            _boardRoot.sizeDelta = new Vector2(720, 720);
            var boardElement = boardPanel.AddComponent<LayoutElement>();
            boardElement.preferredWidth = 720;
            boardElement.preferredHeight = 720;
            boardElement.flexibleWidth = 0;
            boardElement.flexibleHeight = 0;
            var boardLayout = boardPanel.AddComponent<GridLayoutGroup>();
            boardLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            boardLayout.constraintCount = 8;
            boardLayout.cellSize = new Vector2(78, 78);
            boardLayout.spacing = new Vector2(8, 8);
            boardLayout.padding = new RectOffset(24, 24, 24, 24);
            boardLayout.childAlignment = TextAnchor.MiddleCenter;

            _messageText = CreateLabel(_game.transform, "", 24, TextAnchor.MiddleCenter);
            _messageText.rectTransform.sizeDelta = new Vector2(840, 64);

            var actions = CreatePanel(_game.transform, "Game Actions", new Color(0, 0, 0, 0));
            AddLayoutElement(actions, 720, 76);
            var actionsLayout = actions.AddComponent<HorizontalLayoutGroup>();
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.spacing = 18;
            CreateButton(actions.transform, "Restart", _restart, new Color(0.42f, 0.30f, 0.30f), new Vector2(220, 68));
            CreateButton(actions.transform, "Levels", _showLevels, new Color(0.24f, 0.42f, 0.62f), new Vector2(220, 68));
            CreateButton(actions.transform, "Menu", _mainMenuAction, new Color(0.28f, 0.22f, 0.32f), new Vector2(220, 68));
        }

        private void UpdateHud(GameSession session, string message)
        {
            _movesText.text = $"Moves\n{session.MovesRemaining}";
            _goalText.text = GoalLabel(session.GoalTracker.Goals);
            _scoreText.text = $"Score\n{session.Score}";
            _messageText.text = message ?? string.Empty;
        }

        private void RenderBoard(BoardState board, GridPosition? selectedTile)
        {
            ClearChildren(_boardRoot);
            var layout = _boardRoot.GetComponent<GridLayoutGroup>();
            ConfigureBoardLayout(board, layout);

            for (var row = 0; row < board.Height; row++)
            {
                for (var column = 0; column < board.Width; column++)
                {
                    var position = new GridPosition(row, column);
                    var cell = board.GetCell(position);
                    var button = CreateTileButton(_boardRoot, cell, () => _tilePressed(position));
                    button.interactable = cell.CanMoveIngredient;

                    if (selectedTile.HasValue && selectedTile.Value == position)
                    {
                        var outline = button.gameObject.AddComponent<Outline>();
                        outline.effectColor = new Color(1f, 0.95f, 0.45f);
                        outline.effectDistance = new Vector2(4, -4);
                    }
                }
            }
        }

        private void ShowModal(string title, string body, string primaryLabel, Action primaryAction)
        {
            _modal.SetActive(true);
            ClearChildren(_modal.transform);

            var panel = CreatePanel(_modal.transform, "Modal Panel", new Color(0.08f, 0.10f, 0.13f, 0.96f));
            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(680, 620);
            panel.AddComponent<CanvasGroup>();
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 18;
            layout.padding = new RectOffset(28, 28, 38, 38);
            CreateTitle(panel.transform, title, 48);
            CreateLabel(panel.transform, body, 30, TextAnchor.MiddleCenter);
            CreateButton(panel.transform, primaryLabel, primaryAction, new Color(0.24f, 0.52f, 0.46f), new Vector2(300, 78));
            CreateButton(panel.transform, "Replay", _restart, new Color(0.24f, 0.42f, 0.62f), new Vector2(300, 72));
            CreateButton(panel.transform, "Menu", _mainMenuAction, new Color(0.28f, 0.22f, 0.32f), new Vector2(300, 72));
            _feedbackAnimator.PlayModalIntro(rect);
        }

        private GameObject CreateScreen(string name)
        {
            var screen = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(VerticalLayoutGroup));
            screen.transform.SetParent(_root, false);
            var rect = screen.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = screen.GetComponent<Image>();
            image.color = name == "Modal" ? new Color(0, 0, 0, 0.50f) : new Color(0.07f, 0.09f, 0.12f);

            var layout = screen.GetComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 24;
            layout.padding = new RectOffset(40, 40, 80, 80);

            screen.SetActive(false);
            return screen;
        }

        private GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            panel.GetComponent<Image>().color = color;
            return panel;
        }

        private Text CreateTitle(Transform parent, string text, int size)
        {
            return CreateLabel(parent, text, size, TextAnchor.MiddleCenter);
        }

        private Text CreateLabel(Transform parent, string text, int size, TextAnchor alignment)
        {
            var labelObject = new GameObject("Text", typeof(RectTransform), typeof(Text));
            labelObject.transform.SetParent(parent, false);
            var label = labelObject.GetComponent<Text>();
            label.text = text;
            label.font = Font;
            label.color = Color.white;
            label.fontSize = size;
            label.alignment = alignment;
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 16;
            label.resizeTextMaxSize = size;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.rectTransform.sizeDelta = new Vector2(840, Mathf.Max(64, size * 2));
            return label;
        }

        private Button CreateButton(Transform parent, string text, Action action, Color color, Vector2? size = null)
        {
            var buttonObject = new GameObject($"Button - {text}", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = size ?? new Vector2(180, 80);

            var image = buttonObject.GetComponent<Image>();
            image.color = color;

            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() =>
            {
                _playSfx?.Invoke(GameSfxCue.Tap);
                action?.Invoke();
            });

            var label = CreateLabel(buttonObject.transform, text, 28, TextAnchor.MiddleCenter);
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = new Vector2(8, 6);
            label.rectTransform.offsetMax = new Vector2(-8, -6);
            label.raycastTarget = false;
            return button;
        }

        private void CreateToggle(Transform parent, string label, bool value, Action<bool> changed)
        {
            var toggleObject = new GameObject($"Toggle - {label}", typeof(RectTransform), typeof(Toggle));
            toggleObject.transform.SetParent(parent, false);
            toggleObject.GetComponent<RectTransform>().sizeDelta = new Vector2(420, 78);

            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(toggleObject.transform, false);
            var backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.2f);
            backgroundRect.anchorMax = new Vector2(0, 0.8f);
            backgroundRect.sizeDelta = new Vector2(56, 56);
            backgroundRect.anchoredPosition = new Vector2(32, 0);
            background.GetComponent<Image>().color = new Color(0.18f, 0.22f, 0.28f);

            var check = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            check.transform.SetParent(background.transform, false);
            var checkRect = check.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.18f, 0.18f);
            checkRect.anchorMax = new Vector2(0.82f, 0.82f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;
            check.GetComponent<Image>().color = new Color(0.32f, 0.78f, 0.56f);

            var text = CreateLabel(toggleObject.transform, label, 28, TextAnchor.MiddleLeft);
            text.rectTransform.anchorMin = new Vector2(0, 0);
            text.rectTransform.anchorMax = new Vector2(1, 1);
            text.rectTransform.offsetMin = new Vector2(90, 0);
            text.rectTransform.offsetMax = Vector2.zero;

            var toggle = toggleObject.GetComponent<Toggle>();
            toggle.targetGraphic = background.GetComponent<Image>();
            toggle.graphic = check.GetComponent<Image>();
            toggle.isOn = value;
            toggle.onValueChanged.AddListener(enabled =>
            {
                _playSfx?.Invoke(GameSfxCue.Tap);
                changed?.Invoke(enabled);
            });
        }

        private void HideAll()
        {
            _mainMenu.SetActive(false);
            _levelSelect.SetActive(false);
            _game.SetActive(false);
            _settings.SetActive(false);
            _modal.SetActive(false);
        }

        private static void ClearChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.Destroy(parent.GetChild(i).gameObject);
            }
        }

        private static string GoalLabel(IReadOnlyList<GoalProgress> goals)
        {
            return string.Join("\n", goals.Select(goal => $"{GoalName(goal.Goal)}  {goal.CurrentAmount}/{goal.Goal.Amount}"));
        }

        private static string GoalName(GoalData goal)
        {
            switch (goal.GoalType)
            {
                case GoalType.CollectIngredient:
                    return $"Collect {IngredientName(goal.Ingredient)}";
                case GoalType.BreakObstacle:
                    return $"Break {ObstacleName(goal.Obstacle)}";
                case GoalType.ClearTile:
                    return $"Clear {ObstacleName(goal.Obstacle)}";
                case GoalType.CreatePotion:
                    return goal.Potion == PotionType.None ? "Create Potion" : $"Create {PotionName(goal.Potion)}";
                case GoalType.RestorePotionLab:
                    return "Restore Potion Lab";
                default:
                    return "Goal";
            }
        }

        private static string ShortPotion(PotionType potion)
        {
            switch (potion)
            {
                case PotionType.LineHorizontal:
                    return "H";
                case PotionType.LineVertical:
                    return "V";
                case PotionType.Bomb:
                    return "X";
                case PotionType.Lightning:
                    return "L";
                case PotionType.Mega:
                    return "M";
                default:
                    return string.Empty;
            }
        }

        private static Color CellColor(BoardCell cell)
        {
            if (cell.Obstacle == ObstacleType.WoodenBox)
            {
                return new Color(0.50f, 0.32f, 0.18f);
            }

            if (cell.Obstacle == ObstacleType.StoneBlock)
            {
                return new Color(0.36f, 0.38f, 0.42f);
            }

            if (cell.Obstacle == ObstacleType.DarkTile)
            {
                return new Color(0.20f, 0.12f, 0.30f);
            }

            switch (cell.Ingredient)
            {
                case IngredientType.RedHerb:
                    return new Color(0.75f, 0.22f, 0.24f);
                case IngredientType.BlueCrystal:
                    return new Color(0.18f, 0.42f, 0.76f);
                case IngredientType.GreenLeaf:
                    return new Color(0.22f, 0.58f, 0.34f);
                case IngredientType.YellowStarDust:
                    return new Color(0.86f, 0.68f, 0.22f);
                case IngredientType.PurpleMushroom:
                    return new Color(0.48f, 0.28f, 0.68f);
                case IngredientType.OrangeFireDrop:
                    return new Color(0.85f, 0.42f, 0.18f);
                default:
                    return new Color(0.18f, 0.20f, 0.23f);
            }
        }

        private static string StarLabel(int stars)
        {
            if (stars <= 0)
            {
                return "---";
            }

            return new string('*', Mathf.Clamp(stars, 0, 3));
        }

        private static void EnsureEventSystem()
        {
            if (UnityEngine.Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            UnityEngine.Object.DontDestroyOnLoad(eventSystem);
        }
    }
}
