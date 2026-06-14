using System;
using System.Collections;
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
        private BoardAnimationController _boardAnimationController;
        private ScreenTransitionController _screenTransition;
        private BoardVisualPresenter _boardPresenter;
        private GameObject _mainMenu;
        private GameObject _levelSelect;
        private GameObject _game;
        private GameObject _settings;
        private GameObject _modal;
        private GameObject _levelIntroOverlay;
        private GameObject _currentScreen;
        private RectTransform _boardRoot;
        private Text _movesText;
        private Text _goalText;
        private Text _scoreText;
        private Text _messageText;
        private Image _starProgressFill;
        private Text _starProgressText;
        private GameObject _tutorialPanel;
        private Text _tutorialText;
        private RectTransform _floatingLayer;
        private readonly List<Outline> _hintOutlines = new List<Outline>();

        private Action _play;
        private Action _showLevels;
        private Action _showSettings;
        private Action _quit;
        private Action<int> _startLevel;
        private Action<GridPosition> _tilePressed;
        private Action _hintRequested;
        private Action _restart;
        private Action _nextLevel;
        private Action _mainMenuAction;
        private Action _resetProgress;
        private Action<bool> _toggleMusic;
        private Action<bool> _toggleSfx;
        private Action<float> _setMusicVolume;
        private Action<float> _setSfxVolume;
        private Action<bool> _toggleVibration;
        private Action _levelIntroDismissed;
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
            Action hintRequested,
            Action restart,
            Action nextLevel,
            Action mainMenu,
            Action resetProgress,
            Action<bool> toggleMusic,
            Action<bool> toggleSfx,
            Action<float> setMusicVolume,
            Action<float> setSfxVolume,
            Action<bool> toggleVibration,
            Action levelIntroDismissed,
            Action<GameSfxCue> playSfx)
        {
            _play = play;
            _showLevels = showLevels;
            _showSettings = showSettings;
            _quit = quit;
            _startLevel = startLevel;
            _tilePressed = tilePressed;
            _hintRequested = hintRequested;
            _restart = restart;
            _nextLevel = nextLevel;
            _mainMenuAction = mainMenu;
            _resetProgress = resetProgress;
            _toggleMusic = toggleMusic;
            _toggleSfx = toggleSfx;
            _setMusicVolume = setMusicVolume;
            _setSfxVolume = setSfxVolume;
            _toggleVibration = toggleVibration;
            _levelIntroDismissed = levelIntroDismissed;
            _playSfx = playSfx;

            EnsureEventSystem();
            var canvasObject = CreateCanvas(parent);
            var safeAreaRoot = CreateSafeAreaRoot(canvasObject.transform);
            _feedbackAnimator = canvasObject.AddComponent<UiFeedbackAnimator>();
            _boardAnimationController = canvasObject.AddComponent<BoardAnimationController>();
            _screenTransition = canvasObject.AddComponent<ScreenTransitionController>();
            _boardPresenter = new BoardVisualPresenter(_logger, _iconFactory, () => Font);
            _root = safeAreaRoot.transform;
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
            ClearHint();
            HideLevelIntro();
            TransitionTo(_mainMenu);
        }

        public void ShowLevelSelect(IReadOnlyList<LevelData> levels, int highestUnlocked, Func<int, int> starsForLevel)
        {
            ClearHint();
            HideLevelIntro();
            ClearChildren(_levelSelect.transform);
            CreatePotionLabBackdrop(_levelSelect.transform);

            CreateTitle(_levelSelect.transform, "Level Select", 50);
            var grid = CreatePanel(_levelSelect.transform, "Levels Grid", UiColorPalette.LevelGridBackground);
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
                CreateLevelCard(grid.transform, level.LevelNumber, stars, locked);
            }

            CreateButton(_levelSelect.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(260, 72));
            TransitionTo(_levelSelect);
        }

        /// <summary>Creates a styled level card with number and star display.</summary>
        private void CreateLevelCard(Transform parent, int levelNumber, int stars, bool locked)
        {
            var cardColor = locked ? UiColorPalette.LevelCardLocked : UiColorPalette.LevelCardUnlocked;
            var button = CreateButton(parent, "", () => _startLevel(levelNumber), cardColor);
            button.interactable = !locked;
            var cardRect = button.GetComponent<RectTransform>();

            // Level number (large)
            var numberLabel = CreateLabel(button.transform, locked ? "\U0001F512" : levelNumber.ToString(), locked ? 28 : 36, TextAnchor.MiddleCenter);
            numberLabel.rectTransform.anchorMin = new Vector2(0, 0.35f);
            numberLabel.rectTransform.anchorMax = new Vector2(1, 1);
            numberLabel.rectTransform.offsetMin = Vector2.zero;
            numberLabel.rectTransform.offsetMax = Vector2.zero;
            numberLabel.color = locked ? UiColorPalette.TextMuted : UiColorPalette.TextPrimary;

            // Star row
            if (!locked)
            {
                var starText = "";
                for (var i = 1; i <= 3; i++)
                {
                    starText += i <= stars ? "\u2605" : "\u2606";
                }

                var starLabel = CreateLabel(button.transform, starText, 22, TextAnchor.MiddleCenter);
                starLabel.rectTransform.anchorMin = new Vector2(0, 0);
                starLabel.rectTransform.anchorMax = new Vector2(1, 0.35f);
                starLabel.rectTransform.offsetMin = Vector2.zero;
                starLabel.rectTransform.offsetMax = Vector2.zero;
                starLabel.color = stars > 0 ? UiColorPalette.Gold : UiColorPalette.TextMuted;
            }

            // Subtle top border for depth
            if (!locked)
            {
                var border = CreatePanel(button.transform, "CardBorder", UiColorPalette.LevelCardBorder);
                var borderRect = border.GetComponent<RectTransform>();
                borderRect.anchorMin = new Vector2(0, 0.96f);
                borderRect.anchorMax = new Vector2(1, 1);
                borderRect.offsetMin = Vector2.zero;
                borderRect.offsetMax = Vector2.zero;
                border.GetComponent<Image>().raycastTarget = false;
                border.AddComponent<LayoutElement>().ignoreLayout = true;
            }
        }

        public void ShowSettings(bool musicEnabled, bool sfxEnabled, float musicVolume, float sfxVolume, bool vibrationEnabled)
        {
            ClearHint();
            HideLevelIntro();
            ClearChildren(_settings.transform);
            CreatePotionLabBackdrop(_settings.transform);
            CreateTitle(_settings.transform, "Settings", 50);
            CreateToggle(_settings.transform, "Music", musicEnabled, _toggleMusic);
            CreateSlider(_settings.transform, "Music Volume", musicVolume, _setMusicVolume);
            CreateToggle(_settings.transform, "SFX", sfxEnabled, _toggleSfx);
            CreateSlider(_settings.transform, "SFX Volume", sfxVolume, _setSfxVolume);
            CreateToggle(_settings.transform, "Vibration", vibrationEnabled, _toggleVibration);
            CreateButton(_settings.transform, "Reset Progress", _resetProgress, UiColorPalette.Ruby, new Vector2(360, 76));
            CreateButton(_settings.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(260, 72));
            TransitionTo(_settings);
        }

        public void ShowGame(
            GameSession session,
            GridPosition? selectedTile,
            string message = null,
            UiFeedbackCue feedbackCue = UiFeedbackCue.None)
        {
            TransitionTo(_game);
            UpdateHud(session, message);
            _boardPresenter.Render(BoardSnapshot.From(session.Board), selectedTile, feedbackCue);
            _feedbackAnimator.PlayBoardFeedback(feedbackCue, _boardRoot);
        }

        public IEnumerator PlayMoveResult(
            GameSession session,
            GridPosition? selectedTile,
            MoveResult result,
            UiFeedbackCue feedbackCue)
        {
            HideAll();
            _game.SetActive(true);
            _boardPresenter.Render(result.BoardBeforeMove ?? BoardSnapshot.From(session.Board), selectedTile, UiFeedbackCue.None);
            var finalScore = session.Score;
            var startingScore = Math.Max(0, finalScore - result.ScoreGained);
            _movesText.text = $"Moves\n{session.MovesRemaining}";
            _goalText.text = GoalLabel(session.GoalTracker.Goals);
            _messageText.text = result.Message ?? string.Empty;
            _scoreText.text = $"Score\n{startingScore}";

            _feedbackAnimator.PlayBoardFeedback(feedbackCue, _boardRoot);
            yield return _boardPresenter.Play(result.AnimationEvents, BoardSnapshot.From(session.Board));
            ShowFloatingScore(result.ScoreGained, result.Cascades);
            yield return AnimateScore(startingScore, finalScore);
            UpdateHud(session, result.Message);
        }

        public void ShowLevelIntro(GameSession session)
        {
            HideLevelIntro();
            _messageText.text = string.Empty;

            _levelIntroOverlay = CreatePanel(_game.transform, "Level Intro Overlay", UiColorPalette.ModalBackdrop);
            var overlayRect = _levelIntroOverlay.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            _levelIntroOverlay.AddComponent<LayoutElement>().ignoreLayout = true;

            var blocker = _levelIntroOverlay.AddComponent<Button>();
            blocker.transition = Selectable.Transition.None;

            var panel = CreatePanel(_levelIntroOverlay.transform, "Level Intro Panel", new Color(0.08f, 0.10f, 0.16f, 0.98f));
            panel.GetComponent<Image>().raycastTarget = false;
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(760, 740);
            panel.AddComponent<CanvasGroup>();
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 16;
            layout.padding = new RectOffset(34, 34, 36, 30);

            var title = CreateTitle(panel.transform, $"Level {session.Level.LevelNumber}", 52);
            title.color = UiColorPalette.Gold;
            var subtitle = CreateLabel(panel.transform, session.Level.DisplayName, 30, TextAnchor.MiddleCenter);
            subtitle.color = UiColorPalette.TextSecondary;

            var goalsPanel = CreatePanel(panel.transform, "Intro Goals", new Color(0.12f, 0.14f, 0.20f, 0.85f));
            goalsPanel.GetComponent<Image>().raycastTarget = false;
            AddLayoutElement(goalsPanel, 660, Mathf.Max(110, session.Level.Goals.Count * 82));
            var goalsLayout = goalsPanel.AddComponent<VerticalLayoutGroup>();
            goalsLayout.childAlignment = TextAnchor.MiddleCenter;
            goalsLayout.spacing = 8;
            goalsLayout.padding = new RectOffset(14, 14, 14, 14);
            foreach (var goal in session.Level.Goals)
            {
                CreateIntroGoalRow(goalsPanel.transform, goal);
            }

            var obstacleText = session.Level.Obstacles.Count > 0
                ? $"Obstacles: {string.Join(", ", session.Level.Obstacles.Select(item => ObstacleName(item.ObstacleType)).Distinct())}"
                : "Obstacles: None";
            var obstacleLabel = CreateLabel(panel.transform, obstacleText, 24, TextAnchor.MiddleCenter);
            obstacleLabel.color = UiColorPalette.TextSecondary;

            var movesLabel = CreateLabel(panel.transform, $"{session.MovesRemaining} Moves", 34, TextAnchor.MiddleCenter);
            movesLabel.color = UiColorPalette.TextSuccess;
            var tapLabel = CreateLabel(panel.transform, "Tap to Start", 28, TextAnchor.MiddleCenter);
            tapLabel.color = UiColorPalette.GoldLight;

            blocker.onClick.AddListener(() =>
            {
                _playSfx?.Invoke(GameSfxCue.Tap);
                HideLevelIntro();
                _levelIntroDismissed?.Invoke();
            });

            _boardAnimationController.StartCoroutine(LevelIntroReveal(panelRect));
            _feedbackAnimator.PlayBoardFeedback(UiFeedbackCue.Cascade, _boardRoot);
        }

        public void ShowHint(CandidateMove move)
        {
            ClearHint();
            AddHintOutline(move.First);
            AddHintOutline(move.Second);
            _messageText.text = "Try swapping the highlighted ingredients.";
        }

        public void ClearHint()
        {
            foreach (var outline in _hintOutlines)
            {
                if (outline != null)
                {
                    UnityEngine.Object.Destroy(outline);
                }
            }

            _hintOutlines.Clear();
        }

        public void ShowTutorial(LevelData level)
        {
            if (_tutorialPanel == null || _tutorialText == null)
            {
                return;
            }

            var text = TutorialText(level);
            _tutorialPanel.SetActive(!string.IsNullOrEmpty(text));
            _tutorialText.text = text;
        }

        public void UpdateStarProgress(GameSession session)
        {
            if (_starProgressFill == null || _starProgressText == null || session == null)
            {
                return;
            }

            var thresholds = session.Level.StarThresholds;
            var maxScore = Mathf.Max(1, thresholds.ThreeStars);
            var progress = Mathf.Clamp01((float)session.Score / maxScore);
            var fillRect = _starProgressFill.rectTransform;
            fillRect.anchorMax = new Vector2(progress, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            _starProgressText.text = $"Stars {StarLabel(session.Stars)}  {session.Score}/{thresholds.ThreeStars}";
        }

        public void ShowWin(GameSession session, bool hasNextLevel)
        {
            ShowModal(
                "Level Complete",
                $"Score {session.Score}\nStars {StarLabel(session.Stars)}",
                hasNextLevel ? "Next" : "Levels",
                hasNextLevel ? _nextLevel : _showLevels,
                session.Stars);
        }

        public void ShowLose(GameSession session)
        {
            ShowModal(
                "Out of Moves",
                $"Score {session.Score}\nAlmost there! Try again?",
                "Retry",
                _restart,
                0);
        }

        private void AddHintOutline(GridPosition position)
        {
            if (_boardPresenter == null || !_boardPresenter.TryGetTile(position, out var rect) || rect == null)
            {
                return;
            }

            var outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0.58f, 1f, 0.72f, 1f);
            outline.effectDistance = new Vector2(5, -5);
            _hintOutlines.Add(outline);
            _boardAnimationController.StartCoroutine(PulseHint(rect));
        }

        private void ShowFloatingScore(int scoreGained, int cascades)
        {
            if (_floatingLayer == null || scoreGained <= 0)
            {
                return;
            }

            _floatingLayer.SetAsLastSibling();
            var cascadeText = cascades > 2 ? "INCREDIBLE!" : cascades > 1 ? "AMAZING!" : cascades > 0 ? $"Combo x{cascades + 1}" : "";
            var displayText = string.IsNullOrEmpty(cascadeText) ? $"+{scoreGained}" : $"+{scoreGained}\n{cascadeText}";
            var label = CreateLabel(_floatingLayer, displayText, cascades > 1 ? 38 : 32, TextAnchor.MiddleCenter);
            label.color = cascades > 1 ? UiColorPalette.Gold : cascades > 0 ? UiColorPalette.GoldLight : UiColorPalette.TextSuccess;
            label.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            label.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            label.rectTransform.anchoredPosition = Vector2.zero;
            label.rectTransform.sizeDelta = new Vector2(360, 130);
            label.raycastTarget = false;
            _boardAnimationController.StartCoroutine(FloatingScoreRoutine(label));
        }

        private static IEnumerator PulseHint(RectTransform target)
        {
            if (target == null)
            {
                yield break;
            }

            var elapsed = 0f;
            const float duration = 0.42f;
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var pulse = Mathf.Sin(Mathf.Clamp01(elapsed / duration) * Mathf.PI);
                target.localScale = Vector3.one * Mathf.Lerp(1f, 1.08f, pulse);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = Vector3.one;
            }
        }

        private static IEnumerator FloatingScoreRoutine(Text label)
        {
            var rect = label.rectTransform;
            var group = label.gameObject.AddComponent<CanvasGroup>();
            var start = rect.anchoredPosition;
            var duration = GameplayPresentationConfig.FloatingScoreDuration;
            var elapsed = 0f;

            // Punch scale in
            rect.localScale = Vector3.one * 0.6f;

            while (elapsed < duration && rect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);

                // Position: drift upward
                rect.anchoredPosition = Vector2.Lerp(start, start + new Vector2(0, 100), EasingFunctions.EaseOutQuart(t));

                // Scale: elastic punch-in then settle
                var scaleT = Mathf.Clamp01(t * 3f); // First third does the punch
                var scale = scaleT < 1f
                    ? Mathf.Lerp(0.6f, GameplayPresentationConfig.FloatingScorePunchScale, EasingFunctions.EaseOutElasticGentle(scaleT))
                    : Mathf.Lerp(GameplayPresentationConfig.FloatingScorePunchScale, 1f, (t - 0.33f) / 0.67f);
                rect.localScale = Vector3.one * scale;

                // Alpha: hold then fade
                group.alpha = t < 0.5f ? 1f : 1f - (t - 0.5f) * 2f;

                yield return null;
            }

            if (label != null)
            {
                UnityEngine.Object.Destroy(label.gameObject);
            }
        }

        private static string TutorialText(LevelData level)
        {
            if (level == null || !level.TutorialLevel)
            {
                return string.Empty;
            }

            switch (level.LevelNumber)
            {
                case 1:
                    return "Match 3 Red Herbs to collect them before moves run out.";
                case 2:
                    return "Blue Crystals are collected the same way. Watch the goal counter.";
                case 3:
                    return "Plan around the board: longer matches create useful potions.";
                case 4:
                    return "Match 4 ingredients to create a Line Potion.";
                case 5:
                    return "Match next to Wooden Boxes to crack and break them.";
                default:
                    return string.Empty;
            }
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

        private GameObject CreateSafeAreaRoot(Transform parent)
        {
            var safeAreaObject = new GameObject("Safe Area", typeof(RectTransform), typeof(SafeAreaView));
            safeAreaObject.transform.SetParent(parent, false);
            var rect = safeAreaObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return safeAreaObject;
        }

        private void BuildMainMenu()
        {
            CreateTitle(_mainMenu.transform, "Potion Pop Quest", 60);
            var subtitleLabel = CreateLabel(_mainMenu.transform, "2D Match-3 Potion Puzzle", 28, TextAnchor.MiddleCenter);
            subtitleLabel.color = UiColorPalette.TextSecondary;
            CreateButton(_mainMenu.transform, "Play", _play, UiColorPalette.Emerald, new Vector2(320, 82));
            CreateButton(_mainMenu.transform, "Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(320, 82));
            CreateButton(_mainMenu.transform, "Settings", _showSettings, UiColorPalette.Amethyst, new Vector2(320, 82));
            CreateButton(_mainMenu.transform, "Exit", _quit, UiColorPalette.Ruby, new Vector2(320, 82));
        }

        private void BuildGameScreen()
        {
            ClearChildren(_game.transform);
            CreatePotionLabBackdrop(_game.transform);
            var hud = CreatePanel(_game.transform, "HUD", UiColorPalette.HudBackground);
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

            var starProgress = CreatePanel(_game.transform, "Star Progress", new Color(0.11f, 0.14f, 0.18f, 0.86f));
            AddLayoutElement(starProgress, 860, 54);
            var starBarBackground = CreatePanel(starProgress.transform, "Star Bar Background", new Color(0.07f, 0.08f, 0.10f, 0.92f));
            var starBarRect = starBarBackground.GetComponent<RectTransform>();
            starBarRect.anchorMin = new Vector2(0.04f, 0.22f);
            starBarRect.anchorMax = new Vector2(0.96f, 0.78f);
            starBarRect.offsetMin = Vector2.zero;
            starBarRect.offsetMax = Vector2.zero;
            var starFillObject = CreatePanel(starBarBackground.transform, "Star Bar Fill", new Color(1f, 0.76f, 0.25f, 0.95f));
            _starProgressFill = starFillObject.GetComponent<Image>();
            var starFillRect = starFillObject.GetComponent<RectTransform>();
            starFillRect.anchorMin = new Vector2(0f, 0f);
            starFillRect.anchorMax = new Vector2(0f, 1f);
            starFillRect.offsetMin = Vector2.zero;
            starFillRect.offsetMax = Vector2.zero;
            _starProgressText = CreateLabel(starProgress.transform, "Stars 0/3", 20, TextAnchor.MiddleCenter);
            _starProgressText.rectTransform.anchorMin = Vector2.zero;
            _starProgressText.rectTransform.anchorMax = Vector2.one;
            _starProgressText.rectTransform.offsetMin = Vector2.zero;
            _starProgressText.rectTransform.offsetMax = Vector2.zero;
            _starProgressText.raycastTarget = false;

            var boardPanel = CreatePanel(_game.transform, "Board Panel", new Color(0.16f, 0.18f, 0.21f, 0.94f));
            _boardRoot = boardPanel.GetComponent<RectTransform>();
            _boardRoot.sizeDelta = new Vector2(720, 720);
            var boardElement = boardPanel.AddComponent<LayoutElement>();
            boardElement.preferredWidth = 720;
            boardElement.preferredHeight = 720;
            boardElement.flexibleWidth = 0;
            boardElement.flexibleHeight = 0;
            var floatingLayerObject = new GameObject("Floating Feedback Layer", typeof(RectTransform), typeof(LayoutElement));
            floatingLayerObject.transform.SetParent(boardPanel.transform, false);
            _floatingLayer = floatingLayerObject.GetComponent<RectTransform>();
            _floatingLayer.anchorMin = Vector2.zero;
            _floatingLayer.anchorMax = Vector2.one;
            _floatingLayer.offsetMin = Vector2.zero;
            _floatingLayer.offsetMax = Vector2.zero;
            floatingLayerObject.GetComponent<LayoutElement>().ignoreLayout = true;
            _boardPresenter.Configure(_boardRoot, _floatingLayer, _tilePressed, _playSfx);

            _messageText = CreateLabel(_game.transform, "", 24, TextAnchor.MiddleCenter);
            _messageText.rectTransform.sizeDelta = new Vector2(840, 64);

            _tutorialPanel = CreatePanel(_game.transform, "Tutorial Banner", new Color(0.20f, 0.15f, 0.28f, 0.92f));
            AddLayoutElement(_tutorialPanel, 860, 86);
            _tutorialText = CreateLabel(_tutorialPanel.transform, "", 22, TextAnchor.MiddleCenter);
            _tutorialText.rectTransform.anchorMin = Vector2.zero;
            _tutorialText.rectTransform.anchorMax = Vector2.one;
            _tutorialText.rectTransform.offsetMin = new Vector2(18, 10);
            _tutorialText.rectTransform.offsetMax = new Vector2(-18, -10);
            _tutorialPanel.SetActive(false);

            var actions = CreatePanel(_game.transform, "Game Actions", new Color(0, 0, 0, 0));
            AddLayoutElement(actions, 900, 76);
            var actionsLayout = actions.AddComponent<HorizontalLayoutGroup>();
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.spacing = 16;
            CreateButton(actions.transform, "Hint", _hintRequested, UiColorPalette.Emerald, new Vector2(200, 68));
            CreateButton(actions.transform, "Restart", _restart, UiColorPalette.Ruby, new Vector2(200, 68));
            CreateButton(actions.transform, "Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(200, 68));
            CreateButton(actions.transform, "Menu", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(200, 68));
        }

        private void UpdateHud(GameSession session, string message)
        {
            _movesText.text = $"Moves\n{session.MovesRemaining}";
            _goalText.text = GoalLabel(session.GoalTracker.Goals);
            _scoreText.text = $"Score\n{session.Score}";
            _messageText.text = message ?? string.Empty;
            UpdateStarProgress(session);
        }

        private void ShowModal(string title, string body, string primaryLabel, Action primaryAction, int starCount = 0)
        {
            _modal.SetActive(true);
            ClearChildren(_modal.transform);

            var panel = CreatePanel(_modal.transform, "Modal Panel", new Color(0.06f, 0.08f, 0.12f, 0.97f));
            var rect = panel.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(700, starCount > 0 ? 720 : 620);
            panel.AddComponent<CanvasGroup>();
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 18;
            layout.padding = new RectOffset(28, 28, 38, 38);

            // Title with golden color for win
            var titleText = CreateTitle(panel.transform, title, 48);
            if (starCount > 0)
            {
                titleText.color = UiColorPalette.Gold;
            }

            // Star display for win screen
            if (starCount > 0)
            {
                var starRow = CreatePanel(panel.transform, "Stars", new Color(0, 0, 0, 0));
                AddLayoutElement(starRow, 360, 70);
                var starLayout = starRow.AddComponent<HorizontalLayoutGroup>();
                starLayout.childAlignment = TextAnchor.MiddleCenter;
                starLayout.spacing = 16;

                for (var i = 1; i <= 3; i++)
                {
                    var starLabel = CreateLabel(starRow.transform, i <= starCount ? "\u2605" : "\u2606", 48, TextAnchor.MiddleCenter);
                    starLabel.color = i <= starCount ? UiColorPalette.StarEarned : UiColorPalette.StarEmpty;
                    AddLayoutElement(starLabel.gameObject, 60, 60);
                }
            }

            CreateLabel(panel.transform, body, 30, TextAnchor.MiddleCenter);
            CreateButton(panel.transform, primaryLabel, primaryAction, UiColorPalette.Emerald, new Vector2(300, 78));
            CreateButton(panel.transform, "Replay", _restart, UiColorPalette.Sapphire, new Vector2(300, 72));
            CreateButton(panel.transform, "Menu", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(300, 72));
            _feedbackAnimator.PlayModalIntro(rect);

            // Spawn confetti on win
            if (starCount > 0)
            {
                _boardAnimationController.StartCoroutine(SpawnConfetti(_modal.transform));
            }
        }

        private static IEnumerator SpawnConfetti(Transform parent)
        {
            var count = GameplayPresentationConfig.ConfettiCount;
            var duration = GameplayPresentationConfig.ConfettiDuration;
            var confettiPieces = new List<RectTransform>();
            var confettiImages = new List<Image>();
            var velocities = new List<Vector2>();
            var rotations = new List<float>();

            for (var i = 0; i < count; i++)
            {
                var piece = new GameObject("Confetti", typeof(RectTransform), typeof(Image));
                piece.transform.SetParent(parent, false);
                var pieceRect = piece.GetComponent<RectTransform>();
                pieceRect.anchorMin = new Vector2(0.5f, 0.5f);
                pieceRect.anchorMax = new Vector2(0.5f, 0.5f);
                pieceRect.sizeDelta = new Vector2(UnityEngine.Random.Range(6f, 14f), UnityEngine.Random.Range(6f, 14f));
                pieceRect.anchoredPosition = new Vector2(UnityEngine.Random.Range(-320f, 320f), UnityEngine.Random.Range(200f, 400f));
                var pieceImage = piece.GetComponent<Image>();
                pieceImage.color = UiColorPalette.Confetti[i % UiColorPalette.Confetti.Length];
                pieceImage.raycastTarget = false;
                confettiPieces.Add(pieceRect);
                confettiImages.Add(pieceImage);
                velocities.Add(new Vector2(UnityEngine.Random.Range(-40f, 40f), UnityEngine.Random.Range(-120f, -60f)));
                rotations.Add(UnityEngine.Random.Range(-180f, 180f));
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);

                for (var i = 0; i < confettiPieces.Count; i++)
                {
                    if (confettiPieces[i] == null) continue;
                    confettiPieces[i].anchoredPosition += velocities[i] * Time.unscaledDeltaTime;
                    confettiPieces[i].localRotation = Quaternion.Euler(0, 0, rotations[i] * elapsed);
                    if (confettiImages[i] != null)
                    {
                        var alpha = t < 0.7f ? 1f : 1f - (t - 0.7f) / 0.3f;
                        confettiImages[i].color = UiColorPalette.WithAlpha(confettiImages[i].color, alpha);
                    }
                }

                yield return null;
            }

            foreach (var piece in confettiPieces)
            {
                if (piece != null)
                {
                    UnityEngine.Object.Destroy(piece.gameObject);
                }
            }
        }

        private void HideLevelIntro()
        {
            if (_levelIntroOverlay == null)
            {
                return;
            }

            _levelIntroOverlay.SetActive(false);
            UnityEngine.Object.Destroy(_levelIntroOverlay);
            _levelIntroOverlay = null;
        }

        private IEnumerator LevelIntroReveal(RectTransform panel)
        {
            if (panel == null)
            {
                yield break;
            }

            var group = panel.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = panel.gameObject.AddComponent<CanvasGroup>();
            }

            group.alpha = 0f;
            panel.localScale = Vector3.one * 0.88f;
            var elapsed = 0f;
            while (elapsed < GameplayPresentationConfig.LevelIntroRevealDuration && panel != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / GameplayPresentationConfig.LevelIntroRevealDuration);
                var eased = EasingFunctions.EaseOutBack(t, 1.05f);
                group.alpha = Mathf.Clamp01(t / 0.65f);
                panel.localScale = Vector3.one * Mathf.LerpUnclamped(0.88f, 1f, eased);
                yield return null;
            }

            if (panel != null)
            {
                group.alpha = 1f;
                panel.localScale = Vector3.one;
            }
        }

        private void CreateIntroGoalRow(Transform parent, GoalData goal)
        {
            var row = CreatePanel(parent, "Intro Goal Row", new Color(0, 0, 0, 0));
            AddLayoutElement(row, 620, 66);
            row.GetComponent<Image>().raycastTarget = false;
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 14;
            layout.padding = new RectOffset(10, 10, 6, 6);

            var iconObject = new GameObject("Goal Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(row.transform, false);
            AddLayoutElement(iconObject, 54, 54);
            var icon = iconObject.GetComponent<Image>();
            icon.sprite = GoalSprite(goal);
            icon.color = Color.white;
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var label = CreateLabel(row.transform, $"{GoalName(goal)}  0/{goal.Amount}", 24, TextAnchor.MiddleLeft);
            AddLayoutElement(label.gameObject, 500, 58);
            label.color = UiColorPalette.TextPrimary;
        }

        private Sprite GoalSprite(GoalData goal)
        {
            switch (goal.GoalType)
            {
                case GoalType.CollectIngredient:
                    return _iconFactory.GetIngredientSprite(goal.Ingredient);
                case GoalType.BreakObstacle:
                case GoalType.ClearTile:
                    return _iconFactory.GetObstacleSprite(goal.Obstacle);
                case GoalType.CreatePotion:
                    return _iconFactory.GetPotionSprite(goal.Potion);
                default:
                    return _iconFactory.GetPotionSprite(PotionType.Mega);
            }
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
            image.color = name == "Modal" ? UiColorPalette.ModalBackdrop : UiColorPalette.BackgroundSolid;
            if (name != "Modal")
            {
                CreatePotionLabBackdrop(screen.transform);
            }

            // Add CanvasGroup for transitions
            screen.AddComponent<CanvasGroup>();

            var layout = screen.GetComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 24;
            layout.padding = new RectOffset(40, 40, 80, 80);

            screen.SetActive(false);
            return screen;
        }

        private void CreatePotionLabBackdrop(Transform parent)
        {
            var backWall = CreatePanel(parent, "Potion Lab Back Wall", UiColorPalette.LabBackWall);
            var backRect = backWall.GetComponent<RectTransform>();
            backRect.anchorMin = new Vector2(0, 0.66f);
            backRect.anchorMax = new Vector2(1, 1);
            backRect.offsetMin = Vector2.zero;
            backRect.offsetMax = Vector2.zero;
            backWall.GetComponent<Image>().raycastTarget = false;
            backWall.AddComponent<LayoutElement>().ignoreLayout = true;

            for (var index = 0; index < 3; index++)
            {
                var shelf = CreatePanel(parent, $"Potion Shelf {index + 1}", UiColorPalette.LabShelf);
                var shelfRect = shelf.GetComponent<RectTransform>();
                shelfRect.anchorMin = new Vector2(0.08f, 0.80f - index * 0.075f);
                shelfRect.anchorMax = new Vector2(0.92f, 0.815f - index * 0.075f);
                shelfRect.offsetMin = Vector2.zero;
                shelfRect.offsetMax = Vector2.zero;
                shelf.GetComponent<Image>().raycastTarget = false;
                shelf.AddComponent<LayoutElement>().ignoreLayout = true;
            }

            var table = CreatePanel(parent, "Potion Lab Table", UiColorPalette.LabTable);
            var tableRect = table.GetComponent<RectTransform>();
            tableRect.anchorMin = new Vector2(0, 0);
            tableRect.anchorMax = new Vector2(1, 0.13f);
            tableRect.offsetMin = Vector2.zero;
            tableRect.offsetMax = Vector2.zero;
            table.GetComponent<Image>().raycastTarget = false;
            table.AddComponent<LayoutElement>().ignoreLayout = true;

            CreateBokehDust(parent);
        }

        private void CreateBokehDust(Transform parent)
        {
            var container = new GameObject("Bokeh Dust", typeof(RectTransform));
            container.transform.SetParent(parent, false);
            var rect = container.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            container.AddComponent<LayoutElement>().ignoreLayout = true;

            _boardAnimationController.StartCoroutine(SpawnBokeh(container.transform, _iconFactory.GetPillSprite()));
        }

        private static IEnumerator SpawnBokeh(Transform parent, Sprite sprite)
        {
            var particles = new List<RectTransform>();
            var speeds = new List<float>();
            for (var i = 0; i < 15; i++)
            {
                var p = new GameObject("Bokeh", typeof(RectTransform), typeof(Image));
                p.transform.SetParent(parent, false);
                var r = p.GetComponent<RectTransform>();
                var size = UnityEngine.Random.Range(20f, 80f);
                r.sizeDelta = new Vector2(size, size);
                r.anchoredPosition = new Vector2(
                    UnityEngine.Random.Range(-540f, 540f),
                    UnityEngine.Random.Range(-960f, 960f));
                
                var img = p.GetComponent<Image>();
                img.sprite = sprite;
                img.color = new Color(1f, 0.9f, 0.6f, UnityEngine.Random.Range(0.02f, 0.08f));
                img.raycastTarget = false;
                
                particles.Add(r);
                speeds.Add(UnityEngine.Random.Range(10f, 30f));
            }

            while (parent != null)
            {
                for (var i = 0; i < particles.Count; i++)
                {
                    if (particles[i] == null) continue;
                    var pos = particles[i].anchoredPosition;
                    pos.y += speeds[i] * Time.unscaledDeltaTime;
                    if (pos.y > 1000f)
                    {
                        pos.y = -1000f;
                        pos.x = UnityEngine.Random.Range(-540f, 540f);
                    }
                    particles[i].anchoredPosition = pos;
                }
                yield return null;
            }
        }

        /// <summary>Smoothly transitions between screens using ScreenTransitionController.</summary>
        private void TransitionTo(GameObject target)
        {
            if (_currentScreen == target)
            {
                return;
            }

            var previous = _currentScreen;
            _currentScreen = target;

            if (previous == null)
            {
                HideAll();
                target.SetActive(true);
                _screenTransition.FadeIn(target);
                return;
            }

            HideAll();
            target.SetActive(true);
            if (previous != null)
            {
                previous.SetActive(true);
            }

            _screenTransition.CrossDissolve(previous, target, GameplayPresentationConfig.ScreenTransitionDuration);
        }

        private GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.sprite = _iconFactory.GetRoundedRectSprite(32);
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 3f;
            image.color = color;
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
            label.raycastTarget = false;
            return label;
        }

        private Button CreateButton(Transform parent, string text, Action action, Color color, Vector2? size = null)
        {
            var buttonObject = new GameObject($"Button - {text}", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            buttonObject.AddComponent<ButtonPressFeedback>();
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = size ?? new Vector2(180, 80);

            var image = buttonObject.GetComponent<Image>();
            image.sprite = _iconFactory.GetPillSprite();
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = 2f;
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

        private IEnumerator AnimateScore(int from, int to)
        {
            if (from == to)
            {
                _scoreText.text = $"Score\n{to}";
                yield break;
            }

            var duration = GameplayPresentationConfig.ScoreCountDuration;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var value = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                _scoreText.text = $"Score\n{value}";
                yield return null;
            }

            _scoreText.text = $"Score\n{to}";
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

        private void CreateSlider(Transform parent, string label, float value, Action<float> changed)
        {
            var sliderObject = new GameObject($"Slider - {label}", typeof(RectTransform));
            sliderObject.transform.SetParent(parent, false);
            AddLayoutElement(sliderObject, 520, 88);
            var layout = sliderObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 6;

            var caption = CreateLabel(sliderObject.transform, $"{label} {Mathf.RoundToInt(Mathf.Clamp01(value) * 100f)}%", 22, TextAnchor.MiddleCenter);
            AddLayoutElement(caption.gameObject, 500, 32);
            caption.color = UiColorPalette.TextSecondary;

            var trackObject = new GameObject("Track", typeof(RectTransform), typeof(Image), typeof(Slider));
            trackObject.transform.SetParent(sliderObject.transform, false);
            AddLayoutElement(trackObject, 500, 38);
            var trackImage = trackObject.GetComponent<Image>();
            trackImage.color = new Color(0.12f, 0.14f, 0.20f, 0.95f);

            var fillObject = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            fillObject.transform.SetParent(trackObject.transform, false);
            var fillRect = fillObject.GetComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0f, 0.2f);
            fillRect.anchorMax = new Vector2(1f, 0.8f);
            fillRect.offsetMin = new Vector2(8, 0);
            fillRect.offsetMax = new Vector2(-8, 0);
            fillObject.GetComponent<Image>().color = UiColorPalette.EmeraldLight;

            var handleObject = new GameObject("Handle", typeof(RectTransform), typeof(Image));
            handleObject.transform.SetParent(trackObject.transform, false);
            var handleRect = handleObject.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(34, 34);
            handleObject.GetComponent<Image>().color = UiColorPalette.GoldLight;

            var slider = trackObject.GetComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = Mathf.Clamp01(value);
            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleObject.GetComponent<Image>();
            slider.onValueChanged.AddListener(nextValue =>
            {
                caption.text = $"{label} {Mathf.RoundToInt(nextValue * 100f)}%";
                changed?.Invoke(nextValue);
            });
        }

        private static LayoutElement AddLayoutElement(GameObject target, float preferredWidth, float preferredHeight)
        {
            var element = target.GetComponent<LayoutElement>();
            if (element == null)
            {
                element = target.AddComponent<LayoutElement>();
            }

            element.preferredWidth = preferredWidth;
            element.preferredHeight = preferredHeight;
            element.flexibleWidth = 0;
            element.flexibleHeight = 0;
            return element;
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
                var child = parent.GetChild(i).gameObject;
                child.SetActive(false);
                UnityEngine.Object.Destroy(child);
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

        private static string IngredientName(IngredientType ingredient)
        {
            switch (ingredient)
            {
                case IngredientType.RedHerb:
                    return "Red Herb";
                case IngredientType.BlueCrystal:
                    return "Blue Crystal";
                case IngredientType.GreenLeaf:
                    return "Green Leaf";
                case IngredientType.YellowStarDust:
                    return "Yellow Star Dust";
                case IngredientType.PurpleMushroom:
                    return "Purple Mushroom";
                case IngredientType.OrangeFireDrop:
                    return "Orange Fire Drop";
                default:
                    return "Ingredient";
            }
        }

        private static string ObstacleName(ObstacleType obstacle)
        {
            switch (obstacle)
            {
                case ObstacleType.WoodenBox:
                    return "Wooden Box";
                case ObstacleType.StoneBlock:
                    return "Stone Block";
                case ObstacleType.DarkTile:
                    return "Dark Tile";
                case ObstacleType.FrozenIngredient:
                    return "Frozen Ingredient";
                case ObstacleType.MagicChain:
                    return "Magic Chain";
                default:
                    return "Obstacle";
            }
        }

        private static string PotionName(PotionType potion)
        {
            switch (potion)
            {
                case PotionType.LineHorizontal:
                case PotionType.LineVertical:
                    return "Line Potion";
                case PotionType.Bomb:
                    return "Bomb Potion";
                case PotionType.Lightning:
                    return "Lightning Potion";
                case PotionType.Mega:
                    return "Mega Potion";
                default:
                    return "Potion";
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
