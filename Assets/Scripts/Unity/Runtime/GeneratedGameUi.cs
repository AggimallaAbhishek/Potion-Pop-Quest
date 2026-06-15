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
        private readonly UiThemeAssets _themeAssets;
        private readonly UiElementFactory _uiFactory;
        private readonly PotionLabBackdropView _backdropView;
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
        private Image _movesBadgeImage;
        private RectTransform _goalStrip;
        private Image _starProgressFill;
        private Text _starProgressText;
        private readonly List<Image> _starProgressIcons = new List<Image>();
        private GameObject _tutorialPanel;
        private Text _tutorialText;
        private RectTransform _floatingLayer;
        private string _lastGoalSummary;
        private Coroutine _goalPulseRoutine;
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

        // Economy & Pause
        private Action _buyLivesPressed;
        private Action _hammerBoosterPressed;
        private Action _shuffleBoosterPressed;
        private Action _pauseRequested;
        private Action _showShop;
        private Action _closeShop;
        private Action<int> _buyCoinPackage;
        private Action _claimDailyReward;

        private GameObject _economyPanel;
        private GameObject _shopModal;
        private GameObject _dailyRewardModal;
        private Text _livesText;
        private Text _coinsText;
        private Text _hammerText;
        private Text _shuffleText;

        public GeneratedGameUi(IGameLogger logger)
        {
            _logger = logger;
            _iconFactory = new TileIconFactory();
            _themeAssets = new UiThemeAssets();
            _uiFactory = new UiElementFactory(_iconFactory, _themeAssets, () => Font);
            _backdropView = new PotionLabBackdropView(_uiFactory);
        }

        private Font Font
        {
            get
            {
                return _themeAssets.Font;
            }
        }

        public void Build(Transform parent, GeneratedGameUiActions actions)
        {
            actions = actions ?? new GeneratedGameUiActions();
            _play = actions.Play;
            _showLevels = actions.ShowLevels;
            _showSettings = actions.ShowSettings;
            _quit = actions.Quit;
            _startLevel = actions.StartLevel;
            _tilePressed = actions.TilePressed;
            _hintRequested = actions.HintRequested;
            _restart = actions.Restart;
            _nextLevel = actions.NextLevel;
            _mainMenuAction = actions.MainMenu;
            _resetProgress = actions.ResetProgress;
            _toggleMusic = actions.ToggleMusic;
            _toggleSfx = actions.ToggleSfx;
            _setMusicVolume = actions.SetMusicVolume;
            _setSfxVolume = actions.SetSfxVolume;
            _toggleVibration = actions.ToggleVibration;
            _levelIntroDismissed = actions.LevelIntroDismissed;
            _playSfx = actions.PlaySfx;
            _buyLivesPressed = actions.BuyLivesPressed;
            _hammerBoosterPressed = actions.HammerBoosterPressed;
            _shuffleBoosterPressed = actions.ShuffleBoosterPressed;
            _pauseRequested = () => ShowPauseMenu();
            _showShop = actions.ShowShop;
            _closeShop = actions.CloseShop;
            _buyCoinPackage = actions.BuyCoinPackage;
            _claimDailyReward = actions.ClaimDailyReward;

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
            _shopModal = CreateScreen("Shop Modal");
            _dailyRewardModal = CreateScreen("Daily Reward Modal");

            BuildMainMenu();
            BuildSettingsScreen();
            BuildGameScreen();
            BuildEconomyHud();
            BuildShopModal();
            BuildDailyRewardModal();
        }

        private void BuildEconomyHud()
        {
            _economyPanel = CreatePanel(_root, "Economy HUD", new Color(0, 0, 0, 0));
            var rect = _economyPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 1);
            rect.offsetMin = new Vector2(20, -80);
            rect.offsetMax = new Vector2(-20, -20);

            var layout = _economyPanel.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.spacing = 15;
            layout.childControlWidth = false;
            layout.childControlHeight = false;

            CreateButton(_economyPanel.transform, "⏸", _pauseRequested, UiColorPalette.Amethyst, new Vector2(60, 50));

            var livesBtn = CreateButton(_economyPanel.transform, "Lives: 5", _buyLivesPressed, UiColorPalette.Ruby, new Vector2(160, 50));
            _livesText = livesBtn.GetComponentInChildren<Text>();

            var coinContainer = new GameObject("CoinContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            coinContainer.transform.SetParent(_economyPanel.transform, false);
            var coinLayout = coinContainer.GetComponent<HorizontalLayoutGroup>();
            coinLayout.spacing = 5;
            coinLayout.childAlignment = TextAnchor.MiddleRight;
            coinLayout.childForceExpandHeight = false;
            coinLayout.childForceExpandWidth = false;

            var coinsBtn = CreateButton(coinContainer.transform, "Coins: 100", _showShop, UiColorPalette.Gold, new Vector2(160, 50));
            _coinsText = coinsBtn.GetComponentInChildren<Text>();

            CreateButton(coinContainer.transform, "+", _showShop, UiColorPalette.Emerald, new Vector2(50, 50));
            
            _economyPanel.SetActive(true);
        }

        public void UpdateEconomy(int lives, long regenTimeSeconds, int coins, int hammers, int shuffles)
        {
            if (_livesText != null)
            {
                var regenText = lives < 5 && regenTimeSeconds > 0 ? $" ({regenTimeSeconds / 60}:{(regenTimeSeconds % 60):D2})" : "";
                _livesText.text = $"Lives: {lives}" + regenText;
            }
            if (_coinsText != null)
            {
                _coinsText.text = $"Coins: {coins}";
            }
            if (_hammerText != null)
            {
                _hammerText.text = $"[Smash]: {hammers}";
            }
            if (_shuffleText != null)
            {
                _shuffleText.text = $"[Shuffle]: {shuffles}";
            }
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
            _backdropView.Build(_levelSelect.transform);

            CreateTitle(_levelSelect.transform, "Level Select", 50);
            var grid = CreatePanel(_levelSelect.transform, "Levels Grid", UiColorPalette.LevelGridBackground);
            var gridRect = grid.GetComponent<RectTransform>();
            var columns = UiLayoutMetrics.LevelSelectColumnCount();
            var rows = Mathf.CeilToInt(levels.Count / (float)columns);
            var cellSize = columns <= 3 ? 156f : columns == 4 ? 142f : 130f;
            var gridWidth = columns * cellSize + (columns - 1) * 14f + 48f;
            var gridHeight = rows * cellSize + Mathf.Max(0, rows - 1) * 14f + 48f;
            gridRect.sizeDelta = new Vector2(Mathf.Min(UiLayoutMetrics.ScreenMaxWidth, gridWidth), Mathf.Min(760f, gridHeight));
            AddLayoutElement(grid, Mathf.Min(UiLayoutMetrics.ScreenMaxWidth, gridWidth), Mathf.Min(760f, gridHeight));
            var layout = grid.AddComponent<GridLayoutGroup>();
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = columns;
            layout.cellSize = new Vector2(cellSize, cellSize);
            layout.spacing = new Vector2(14, 14);
            layout.padding = new RectOffset(24, 24, 24, 24);

            foreach (var level in levels)
            {
                var locked = level.LevelNumber > highestUnlocked;
                var stars = starsForLevel(level.LevelNumber);
                CreateLevelCard(grid.transform, level.LevelNumber, stars, locked);
            }

            CreateButton(_levelSelect.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(260, 72));
            TransitionTo(_levelSelect);
        }

        /// <summary>Creates a styled level card with number, star display, and premium visual effects.</summary>
        private void CreateLevelCard(Transform parent, int levelNumber, int stars, bool locked)
        {
            var cardColor = locked ? UiColorPalette.LevelCardLocked : UiColorPalette.LevelCardUnlocked;
            var button = CreateButton(parent, "", () => _startLevel(levelNumber), cardColor);
            button.interactable = !locked;
            var cardRect = button.GetComponent<RectTransform>();

            // Gradient overlay for unlocked cards
            if (!locked)
            {
                var gradient = new GameObject("CardGradient", typeof(RectTransform), typeof(Image));
                gradient.transform.SetParent(button.transform, false);
                gradient.transform.SetAsFirstSibling();
                var gradRect = gradient.GetComponent<RectTransform>();
                gradRect.anchorMin = new Vector2(0f, 0f);
                gradRect.anchorMax = new Vector2(1f, 0.50f);
                gradRect.offsetMin = Vector2.zero;
                gradRect.offsetMax = Vector2.zero;
                gradient.GetComponent<Image>().color = UiColorPalette.WithAlpha(UiColorPalette.LevelCardUnlockedGradient, 0.40f);
                gradient.GetComponent<Image>().raycastTarget = false;
            }

            // Level number (large, with shadow)
            var numberLabel = CreateLabel(button.transform, locked ? "\U0001F512" : levelNumber.ToString(), locked ? 28 : 40, TextAnchor.MiddleCenter);
            numberLabel.rectTransform.anchorMin = new Vector2(0, 0.32f);
            numberLabel.rectTransform.anchorMax = new Vector2(1, 1);
            numberLabel.rectTransform.offsetMin = Vector2.zero;
            numberLabel.rectTransform.offsetMax = Vector2.zero;
            numberLabel.color = locked ? UiColorPalette.TextMuted : UiColorPalette.TextPrimary;
            if (!locked)
            {
                _themeAssets.AddHighValueTextShadow(numberLabel);
            }

            // Star row with glow effect
            if (!locked)
            {
                var starRow = new GameObject("Level Card Stars", typeof(RectTransform), typeof(LayoutElement));
                starRow.transform.SetParent(button.transform, false);
                var starRect = starRow.GetComponent<RectTransform>();
                starRect.anchorMin = new Vector2(0, 0);
                starRect.anchorMax = new Vector2(1, 0.34f);
                starRect.offsetMin = Vector2.zero;
                starRect.offsetMax = Vector2.zero;
                starRow.GetComponent<LayoutElement>().ignoreLayout = true;
                var starLayout = starRow.AddComponent<HorizontalLayoutGroup>();
                starLayout.childAlignment = TextAnchor.MiddleCenter;
                starLayout.spacing = 4;
                for (var i = 1; i <= 3; i++)
                {
                    CreateStarImage(starRow.transform, i <= stars, 30);
                }
            }

            // Golden top border for unlocked cards
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
            _backdropView.Build(_settings.transform);
            CreateTitle(_settings.transform, "Settings", 50);
            var audioSection = CreateSettingsSection(_settings.transform, "Audio", 332);
            CreateToggle(audioSection.transform, "Music", musicEnabled, _toggleMusic);
            CreateSlider(audioSection.transform, "Music Volume", musicVolume, _setMusicVolume);
            CreateToggle(audioSection.transform, "SFX", sfxEnabled, _toggleSfx);
            CreateSlider(audioSection.transform, "SFX Volume", sfxVolume, _setSfxVolume);

            var gameplaySection = CreateSettingsSection(_settings.transform, "Gameplay", 126);
            CreateToggle(gameplaySection.transform, "Vibration", vibrationEnabled, _toggleVibration);

            var resetSection = CreateSettingsSection(_settings.transform, "Progress", 144, UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.16f));
            CreateButton(resetSection.transform, "Reset Progress", _resetProgress, UiColorPalette.Ruby, new Vector2(360, 70));
            CreateButton(_settings.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(260, 70));
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
            UpdateMovesBadge(session.MovesRemaining);
            RenderGoalProgress(session.GoalTracker.Goals, false);
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
            panelRect.sizeDelta = new Vector2(760, 700);
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

            CreateIntroObstaclePreview(panel.transform, session.Level);

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
            _starProgressText.text = $"Score {session.Score}/{thresholds.ThreeStars}";
            for (var i = 0; i < _starProgressIcons.Count; i++)
            {
                _starProgressIcons[i].sprite = _iconFactory.GetStarSprite(i < session.Stars);
            }
        }

        public void ShowWin(GameSession session, bool hasNextLevel)
        {
            ShowWinModal(session, hasNextLevel);
        }

        public void ShowLose(GameSession session)
        {
            ShowLoseModal(session);
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
            var cascadeText = cascades > 2 ? "\u2728 INCREDIBLE! \u2728" : cascades > 1 ? "\u2B50 AMAZING! \u2B50" : cascades > 0 ? $"Combo x{cascades + 1}" : "";
            var displayText = string.IsNullOrEmpty(cascadeText) ? $"+{scoreGained}" : $"+{scoreGained}\n{cascadeText}";
            var fontSize = cascades > 2 ? 44 : cascades > 1 ? 40 : cascades > 0 ? 36 : 32;
            var label = CreateLabel(_floatingLayer, displayText, fontSize, TextAnchor.MiddleCenter);
            label.color = cascades > 2 ? UiColorPalette.Gold : cascades > 1 ? UiColorPalette.GoldLight : cascades > 0 ? UiColorPalette.EmeraldLight : UiColorPalette.TextSuccess;
            label.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            label.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            label.rectTransform.anchoredPosition = Vector2.zero;
            label.rectTransform.sizeDelta = new Vector2(400, 150);
            label.raycastTarget = false;
            _themeAssets.AddHighValueTextShadow(label);
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

        private void BuildMainMenu()
        {
            // Animated title with glow effects
            var titleText = CreateTitle(_mainMenu.transform, "Potion Pop Quest", 64);
            titleText.color = UiColorPalette.Gold;
            _themeAssets.AddTitleTextEffects(titleText);
            AddLayoutElement(titleText.gameObject, 840, 100);

            var subtitleLabel = CreateLabel(_mainMenu.transform, "\u2728 2D Match-3 Potion Puzzle \u2728", 28, TextAnchor.MiddleCenter);
            subtitleLabel.color = UiColorPalette.TextSecondary;
            AddLayoutElement(subtitleLabel.gameObject, 600, 48);

            // Decorative potion icon between title and buttons
            var potionIcon = new GameObject("Menu Potion Icon", typeof(RectTransform), typeof(Image));
            potionIcon.transform.SetParent(_mainMenu.transform, false);
            AddLayoutElement(potionIcon, 96, 96);
            var potionImage = potionIcon.GetComponent<Image>();
            potionImage.sprite = _iconFactory.GetPotionSprite(PotionType.Mega);
            potionImage.preserveAspect = true;
            potionImage.raycastTarget = false;

            // Buttons with generous sizing
            CreateButton(_mainMenu.transform, "\u25B6  Play", _play, UiColorPalette.Emerald, new Vector2(340, 86));
            CreateButton(_mainMenu.transform, "\u2606  Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(340, 82));
            CreateButton(_mainMenu.transform, "\u2699  Settings", _showSettings, UiColorPalette.Amethyst, new Vector2(340, 82));
            CreateButton(_mainMenu.transform, "Exit", _quit, UiColorPalette.Ruby, new Vector2(340, 74));
        }

        private void BuildGameScreen()
        {
            ClearChildren(_game.transform);
            var screenLayout = _game.GetComponent<VerticalLayoutGroup>();
            screenLayout.spacing = UiLayoutMetrics.GameScreenSpacing();
            screenLayout.padding = UiLayoutMetrics.GameScreenPadding();

            _backdropView.Build(_game.transform);

            // Ambient floating particles behind the board
            var particleHost = new GameObject("AmbientParticles", typeof(RectTransform), typeof(AmbientParticleView), typeof(LayoutElement));
            particleHost.transform.SetParent(_game.transform, false);
            var particleRect = particleHost.GetComponent<RectTransform>();
            particleRect.anchorMin = Vector2.zero;
            particleRect.anchorMax = Vector2.one;
            particleRect.offsetMin = Vector2.zero;
            particleRect.offsetMax = Vector2.zero;
            particleHost.GetComponent<LayoutElement>().ignoreLayout = true;
            particleHost.GetComponent<AmbientParticleView>().Initialize(particleRect);

            var hud = CreatePanel(_game.transform, "HUD", UiColorPalette.HudBackground);
            var hudRect = hud.GetComponent<RectTransform>();
            var hudHeight = UiLayoutMetrics.GameHudHeight();
            hudRect.sizeDelta = new Vector2(UiLayoutMetrics.HudWidth, hudHeight);
            AddLayoutElement(hud, UiLayoutMetrics.HudWidth, hudHeight);
            var hudLayout = hud.AddComponent<HorizontalLayoutGroup>();
            hudLayout.childAlignment = TextAnchor.MiddleCenter;
            hudLayout.spacing = 16;
            hudLayout.padding = new RectOffset(18, 18, 14, 14);

            // Moves badge with enhanced styling
            var movesBadge = _uiFactory.CreateGlowingBadge(hud.transform, "HUD Moves Badge", 166, UiColorPalette.WithAlpha(UiColorPalette.Sapphire, 0.58f));
            AddLayoutElement(movesBadge, 166, Mathf.Max(96f, hudHeight - 30f));
            _movesBadgeImage = movesBadge.GetComponent<Image>();
            _movesText = CreateLabel(movesBadge.transform, "Moves\n0", 28, TextAnchor.MiddleCenter);
            StretchInside(_movesText.rectTransform, 8, 6);
            _themeAssets.AddHighValueTextShadow(_movesText);

            var goalPanel = CreateHudBadge(hud.transform, "HUD Goal Panel", 520, UiColorPalette.WithAlpha(UiColorPalette.Amethyst, 0.48f));
            var goalLayout = goalPanel.AddComponent<VerticalLayoutGroup>();
            goalLayout.childAlignment = TextAnchor.MiddleCenter;
            goalLayout.spacing = 8;
            goalLayout.padding = new RectOffset(12, 12, 10, 10);
            _goalText = CreateLabel(goalPanel.transform, "Goals", 20, TextAnchor.MiddleCenter);
            _goalText.color = UiColorPalette.GoldLight;
            AddLayoutElement(_goalText.gameObject, 490, 26);
            var goalStripObject = new GameObject("HUD Goal Strip", typeof(RectTransform));
            goalStripObject.transform.SetParent(goalPanel.transform, false);
            _goalStrip = goalStripObject.GetComponent<RectTransform>();
            AddLayoutElement(goalStripObject, 490, Mathf.Max(52f, hudHeight - 76f));
            var goalStripLayout = goalStripObject.AddComponent<VerticalLayoutGroup>();
            goalStripLayout.childAlignment = TextAnchor.MiddleCenter;
            goalStripLayout.spacing = 6;
            goalStripLayout.padding = new RectOffset(0, 0, 0, 0);

            // Score badge with glow
            var scoreBadge = _uiFactory.CreateGlowingBadge(hud.transform, "HUD Score Badge", 166, UiColorPalette.WithAlpha(UiColorPalette.EmeraldDark, 0.52f));
            AddLayoutElement(scoreBadge, 166, Mathf.Max(96f, hudHeight - 30f));
            _scoreText = CreateLabel(scoreBadge.transform, "Score\n0", 28, TextAnchor.MiddleCenter);
            StretchInside(_scoreText.rectTransform, 8, 6);
            _themeAssets.AddHighValueTextShadow(_scoreText);

            // Star progress bar with shimmer
            var starProgress = CreatePanel(_game.transform, "Star Progress", new Color(0.09f, 0.11f, 0.16f, 0.90f));
            starProgress.SetActive(false); // Hide to save space
            AddLayoutElement(starProgress, UiLayoutMetrics.StarProgressWidth, UiLayoutMetrics.GameStarProgressHeight());
            var starBarBackground = CreatePanel(starProgress.transform, "Star Bar Background", UiColorPalette.StarBarBackground);
            var starBarRect = starBarBackground.GetComponent<RectTransform>();
            starBarRect.anchorMin = new Vector2(0.04f, 0.22f);
            starBarRect.anchorMax = new Vector2(0.96f, 0.78f);
            starBarRect.offsetMin = Vector2.zero;
            starBarRect.offsetMax = Vector2.zero;
            var starFillObject = CreatePanel(starBarBackground.transform, "Star Bar Fill", UiColorPalette.StarBarFill);
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
            _themeAssets.AddHighValueTextShadow(_starProgressText);
            _starProgressIcons.Clear();
            var starIconRow = new GameObject("Star Progress Stars", typeof(RectTransform), typeof(LayoutElement));
            starIconRow.transform.SetParent(starProgress.transform, false);
            var starIconRect = starIconRow.GetComponent<RectTransform>();
            starIconRect.anchorMin = new Vector2(0.04f, 0.14f);
            starIconRect.anchorMax = new Vector2(0.24f, 0.86f);
            starIconRect.offsetMin = Vector2.zero;
            starIconRect.offsetMax = Vector2.zero;
            starIconRow.GetComponent<LayoutElement>().ignoreLayout = true;
            var starIconLayout = starIconRow.AddComponent<HorizontalLayoutGroup>();
            starIconLayout.childAlignment = TextAnchor.MiddleLeft;
            starIconLayout.spacing = 3;
            for (var i = 0; i < 3; i++)
            {
                _starProgressIcons.Add(CreateStarImage(starIconRow.transform, false, 26));
            }

            // Board panel
            var boardPanel = CreatePanel(_game.transform, "Board Panel", new Color(0.14f, 0.16f, 0.20f, 0.95f));
            _boardRoot = boardPanel.GetComponent<RectTransform>();
            var boardSize = UiLayoutMetrics.GameBoardSize();
            _boardRoot.sizeDelta = new Vector2(boardSize, boardSize);
            AddLayoutElement(boardPanel, boardSize, boardSize);
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
            _messageText.rectTransform.sizeDelta = new Vector2(UiLayoutMetrics.MessageWidth, UiLayoutMetrics.GameMessageHeight());

            _tutorialPanel = CreatePanel(_game.transform, "Tutorial Banner", UiColorPalette.TutorialBackground);
            AddLayoutElement(_tutorialPanel, UiLayoutMetrics.TutorialWidth, UiLayoutMetrics.GameTutorialHeight());
            _tutorialText = CreateLabel(_tutorialPanel.transform, "", 22, TextAnchor.MiddleCenter);
            _tutorialText.rectTransform.anchorMin = Vector2.zero;
            _tutorialText.rectTransform.anchorMax = Vector2.one;
            _tutorialText.rectTransform.offsetMin = new Vector2(18, 10);
            _tutorialText.rectTransform.offsetMax = new Vector2(-18, -10);
            _tutorialPanel.SetActive(false);

            var touchHeight = UiLayoutMetrics.GameTouchHeight();
            var boosters = CreatePanel(_game.transform, "Game Boosters", new Color(0, 0, 0, 0));
            AddLayoutElement(boosters, UiLayoutMetrics.ActionsWidth, UiLayoutMetrics.GameActionsHeight());
            var boostersLayout = boosters.AddComponent<HorizontalLayoutGroup>();
            boostersLayout.childAlignment = TextAnchor.MiddleCenter;
            boostersLayout.spacing = 20;
            
            var hammerBtn = CreateButton(boosters.transform, "[Smash] 0", _hammerBoosterPressed, UiColorPalette.Gold, new Vector2(160, touchHeight));
            _hammerText = hammerBtn.GetComponentInChildren<Text>();
            
            var shuffleBtn = CreateButton(boosters.transform, "[Shuffle] 0", _shuffleBoosterPressed, UiColorPalette.Gold, new Vector2(160, touchHeight));
            _shuffleText = shuffleBtn.GetComponentInChildren<Text>();
        }

        private void UpdateHud(GameSession session, string message)
        {
            UpdateMovesBadge(session.MovesRemaining);
            RenderGoalProgress(session.GoalTracker.Goals, true);
            _scoreText.text = $"Score\n{session.Score}";
            _messageText.text = message ?? string.Empty;
            UpdateStarProgress(session);
        }

        private void UpdateMovesBadge(int movesRemaining)
        {
            _movesText.text = $"Moves\n{movesRemaining}";
            if (_movesBadgeImage == null)
            {
                return;
            }

            if (movesRemaining <= 1)
            {
                _movesBadgeImage.color = UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.72f);
                _movesText.color = UiColorPalette.RubyLight;
            }
            else if (movesRemaining <= 3)
            {
                _movesBadgeImage.color = UiColorPalette.WithAlpha(UiColorPalette.GoldDark, 0.70f);
                _movesText.color = UiColorPalette.GoldLight;
            }
            else
            {
                _movesBadgeImage.color = UiColorPalette.WithAlpha(UiColorPalette.Sapphire, 0.55f);
                _movesText.color = UiColorPalette.TextPrimary;
            }
        }

        private void RenderGoalProgress(IReadOnlyList<GoalProgress> goals, bool animateChanged)
        {
            if (_goalStrip == null)
            {
                return;
            }

            var summary = GoalLabel(goals);
            _goalText.text = goals.Count == 1 ? "Goal" : "Goals";
            ClearChildren(_goalStrip);

            foreach (var goal in goals)
            {
                CreateHudGoalRow(_goalStrip, goal);
            }

            if (animateChanged && _lastGoalSummary != null && _lastGoalSummary != summary)
            {
                PulseGoalStrip();
            }

            _lastGoalSummary = summary;
        }

        private void PulseGoalStrip()
        {
            if (_goalStrip == null || _boardAnimationController == null)
            {
                return;
            }

            if (_goalPulseRoutine != null)
            {
                _boardAnimationController.StopCoroutine(_goalPulseRoutine);
            }

            _goalPulseRoutine = _boardAnimationController.StartCoroutine(PulseRect(_goalStrip, 1.035f, 0.18f));
        }

        private static IEnumerator PulseRect(RectTransform rect, float scale, float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration && rect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var pulse = Mathf.Sin(t * Mathf.PI);
                rect.localScale = Vector3.one * Mathf.Lerp(1f, scale, pulse);
                yield return null;
            }

            if (rect != null)
            {
                rect.localScale = Vector3.one;
            }
        }

        private void ShowWinModal(GameSession session, bool hasNextLevel)
        {
            var panel = CreateModalPanel(800, 18);

            // Golden gradient header
            var headerGlow = CreatePanel(panel.transform, "WinHeaderGlow", UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.12f));
            AddLayoutElement(headerGlow, 680, 8);
            headerGlow.GetComponent<Image>().raycastTarget = false;

            var titleText = CreateTitle(panel.transform, "\u2728 Level Complete \u2728", 50);
            titleText.color = UiColorPalette.Gold;
            _themeAssets.AddTitleTextEffects(titleText);

            var scoreLabel = CreateLabel(panel.transform, "Score 0", 36, TextAnchor.MiddleCenter);
            scoreLabel.color = UiColorPalette.TextPrimary;
            AddLayoutElement(scoreLabel.gameObject, 560, 60);

            var starLabels = CreateStarRow(panel.transform, session.Stars);
            CreateGoalSummary(panel.transform, session.GoalTracker.Goals, false);
            CreateModalActions(panel.transform, hasNextLevel ? "\u25B6  Next" : "\u2606  Levels", hasNextLevel ? _nextLevel : _showLevels, showLevelsButton: hasNextLevel);

            var rect = panel.GetComponent<RectTransform>();
            _feedbackAnimator.PlayModalIntro(rect);
            _boardAnimationController.StartCoroutine(AnimateModalScore(scoreLabel, session.Score));
            _boardAnimationController.StartCoroutine(RevealStars(starLabels, session.Stars));
            _boardAnimationController.StartCoroutine(SpawnConfetti(_modal.transform));
        }

        private void ShowLoseModal(GameSession session)
        {
            var panel = CreateModalPanel(740, 16);

            // Dramatic red header
            var headerGlow = CreatePanel(panel.transform, "LoseHeaderGlow", UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.15f));
            AddLayoutElement(headerGlow, 680, 8);
            headerGlow.GetComponent<Image>().raycastTarget = false;

            var titleText = CreateTitle(panel.transform, "Out of Moves", 48);
            titleText.color = UiColorPalette.RubyLight;
            _themeAssets.AddTitleTextEffects(titleText);
            var body = CreateLabel(panel.transform, "Try again to finish the remaining goals.", 26, TextAnchor.MiddleCenter);
            body.color = UiColorPalette.TextSecondary;
            AddLayoutElement(body.gameObject, 590, 60);
            CreateGoalSummary(panel.transform, session.GoalTracker.Goals, true);
            CreateModalActions(panel.transform, "\u21BB  Retry", _restart, showLevelsButton: true);
            _feedbackAnimator.PlayModalIntro(panel.GetComponent<RectTransform>());
        }

        private GameObject CreateHudBadge(Transform parent, string name, float width, Color color)
        {
            var badge = CreatePanel(parent, name, color);
            AddLayoutElement(badge, width, Mathf.Max(96f, UiLayoutMetrics.GameHudHeight() - 30f));
            return badge;
        }

        private void CreateHudGoalRow(Transform parent, GoalProgress progress)
        {
            var row = CreatePanel(parent, "HUD Goal Row", progress.IsComplete
                ? UiColorPalette.WithAlpha(UiColorPalette.EmeraldDark, 0.35f)
                : UiColorPalette.WithAlpha(UiColorPalette.BackgroundSolid, 0.20f));
            AddLayoutElement(row, 490, 42);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10;
            layout.padding = new RectOffset(8, 10, 4, 4);

            var iconObject = new GameObject("HUD Goal Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(row.transform, false);
            AddLayoutElement(iconObject, 34, 34);
            var icon = iconObject.GetComponent<Image>();
            icon.sprite = GoalSprite(progress.Goal);
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var label = CreateLabel(row.transform, $"{GoalName(progress.Goal)}  {progress.CurrentAmount}/{progress.Goal.Amount}", 19, TextAnchor.MiddleLeft);
            label.color = progress.IsComplete ? UiColorPalette.TextSuccess : UiColorPalette.TextPrimary;
            AddLayoutElement(label.gameObject, 420, 36);
        }

        private GameObject CreateModalPanel(float height, int spacing)
        {
            _modal.SetActive(true);
            ClearChildren(_modal.transform);

            // Glassmorphism modal panel
            var panel = _uiFactory.CreateGlassPanel(_modal.transform, "Modal Panel", UiLayoutMetrics.ModalWidth, height);
            panel.AddComponent<CanvasGroup>();
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = spacing;
            layout.padding = new RectOffset(32, 32, 36, 36);
            return panel;
        }

        private List<Image> CreateStarRow(Transform parent, int starCount)
        {
            var labels = new List<Image>();
            var starRow = CreatePanel(parent, "Stars", new Color(0, 0, 0, 0));
            AddLayoutElement(starRow, 380, 72);
            var starLayout = starRow.AddComponent<HorizontalLayoutGroup>();
            starLayout.childAlignment = TextAnchor.MiddleCenter;
            starLayout.spacing = 18;

            for (var i = 1; i <= 3; i++)
            {
                labels.Add(CreateStarImage(starRow.transform, false, 66));
            }

            return labels;
        }

        private void CreateGoalSummary(Transform parent, IReadOnlyList<GoalProgress> goals, bool remainingOnly)
        {
            var summary = CreatePanel(parent, remainingOnly ? "Remaining Goals" : "Completed Goals", UiColorPalette.WithAlpha(UiColorPalette.Amethyst, 0.32f));
            AddLayoutElement(summary, 620, Mathf.Clamp(goals.Count * 58 + 28, 112, 232));
            var layout = summary.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 8;
            layout.padding = new RectOffset(14, 14, 14, 14);

            foreach (var progress in goals)
            {
                var amount = remainingOnly
                    ? $"{progress.RemainingAmount} left"
                    : $"{progress.CurrentAmount}/{progress.Goal.Amount}";
                var row = CreatePanel(summary.transform, "Modal Goal Row", new Color(0, 0, 0, 0));
                AddLayoutElement(row, 560, 48);
                row.GetComponent<Image>().raycastTarget = false;
                var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
                rowLayout.childAlignment = TextAnchor.MiddleCenter;
                rowLayout.spacing = 12;

                var iconObject = new GameObject("Goal Icon", typeof(RectTransform), typeof(Image));
                iconObject.transform.SetParent(row.transform, false);
                AddLayoutElement(iconObject, 42, 42);
                var icon = iconObject.GetComponent<Image>();
                icon.sprite = GoalSprite(progress.Goal);
                icon.preserveAspect = true;
                icon.raycastTarget = false;

                var label = CreateLabel(row.transform, $"{GoalName(progress.Goal)}  {amount}", 22, TextAnchor.MiddleLeft);
                label.color = remainingOnly && progress.RemainingAmount > 0 ? UiColorPalette.GoldLight : UiColorPalette.TextSuccess;
                AddLayoutElement(label.gameObject, 490, 44);
            }
        }

        private void CreateModalActions(Transform parent, string primaryLabel, Action primaryAction, bool showLevelsButton)
        {
            CreateButton(parent, primaryLabel, primaryAction, UiColorPalette.Emerald, new Vector2(320, 74));
            CreateButton(parent, "Replay", _restart, UiColorPalette.Sapphire, new Vector2(320, 68));
            if (showLevelsButton)
            {
                CreateButton(parent, "Levels", _showLevels, UiColorPalette.Amethyst, new Vector2(320, 68));
            }
            else
            {
                CreateButton(parent, "Menu", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(320, 68));
            }
        }

        private Image CreateStarImage(Transform parent, bool earned, float size)
        {
            var starObject = new GameObject(earned ? "Star Earned" : "Star Empty", typeof(RectTransform), typeof(Image));
            starObject.transform.SetParent(parent, false);
            AddLayoutElement(starObject, size, size);
            var image = starObject.GetComponent<Image>();
            image.sprite = _iconFactory.GetStarSprite(earned);
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        private static IEnumerator AnimateModalScore(Text label, int finalScore)
        {
            if (label == null)
            {
                yield break;
            }

            var elapsed = 0f;
            while (elapsed < GameplayPresentationConfig.ScoreCountDuration && label != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / GameplayPresentationConfig.ScoreCountDuration);
                label.text = $"Score {Mathf.RoundToInt(Mathf.Lerp(0, finalScore, EasingFunctions.EaseOutQuart(t)))}";
                yield return null;
            }

            if (label != null)
            {
                label.text = $"Score {finalScore}";
            }
        }

        private IEnumerator RevealStars(IReadOnlyList<Image> starLabels, int starCount)
        {
            for (var i = 0; i < starLabels.Count; i++)
            {
                var image = starLabels[i];
                if (image == null)
                {
                    continue;
                }

                yield return new WaitForSecondsRealtime(0.18f);
                image.sprite = _iconFactory.GetStarSprite(i < starCount);
                yield return PulseRect(image.rectTransform, i < starCount ? 1.28f : 1.08f, 0.20f);
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
            var spiralSpeeds = new List<float>();

            for (var i = 0; i < count; i++)
            {
                var piece = new GameObject("Confetti", typeof(RectTransform), typeof(Image));
                piece.transform.SetParent(parent, false);
                var pieceRect = piece.GetComponent<RectTransform>();
                pieceRect.anchorMin = new Vector2(0.5f, 0.5f);
                pieceRect.anchorMax = new Vector2(0.5f, 0.5f);
                // Varied shapes: some square, some rectangular
                var w = UnityEngine.Random.Range(5f, 16f);
                var h = UnityEngine.Random.Range(5f, 16f);
                pieceRect.sizeDelta = new Vector2(w, h);
                pieceRect.anchoredPosition = new Vector2(UnityEngine.Random.Range(-360f, 360f), UnityEngine.Random.Range(180f, 460f));
                var pieceImage = piece.GetComponent<Image>();
                pieceImage.color = UiColorPalette.Confetti[i % UiColorPalette.Confetti.Length];
                pieceImage.raycastTarget = false;
                confettiPieces.Add(pieceRect);
                confettiImages.Add(pieceImage);
                // Spiral trajectories: initial upward velocity + strong horizontal spread
                velocities.Add(new Vector2(
                    UnityEngine.Random.Range(-80f, 80f),
                    UnityEngine.Random.Range(-140f, -50f)));
                rotations.Add(UnityEngine.Random.Range(-280f, 280f));
                spiralSpeeds.Add(UnityEngine.Random.Range(40f, 100f));
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);

                for (var i = 0; i < confettiPieces.Count; i++)
                {
                    if (confettiPieces[i] == null) continue;

                    // Add spiral drift for more dynamic motion
                    var spiralOffset = new Vector2(
                        Mathf.Sin(elapsed * 3f + i * 1.3f) * spiralSpeeds[i] * Time.unscaledDeltaTime * 0.5f,
                        0f);
                    confettiPieces[i].anchoredPosition += velocities[i] * Time.unscaledDeltaTime + spiralOffset;
                    confettiPieces[i].localRotation = Quaternion.Euler(0, 0, rotations[i] * elapsed);

                    // Scale down slightly over time
                    var scaleDown = t < 0.6f ? 1f : 1f - (t - 0.6f) * 0.8f;
                    confettiPieces[i].localScale = Vector3.one * Mathf.Max(0.3f, scaleDown);

                    if (confettiImages[i] != null)
                    {
                        var alpha = t < 0.65f ? 1f : 1f - (t - 0.65f) / 0.35f;
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

        private void CreateIntroObstaclePreview(Transform parent, LevelData level)
        {
            var preview = CreatePanel(parent, "Intro Obstacle Preview", UiColorPalette.WithAlpha(UiColorPalette.Sapphire, 0.18f));
            AddLayoutElement(preview, 660, 72);
            var layout = preview.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 12;
            layout.padding = new RectOffset(16, 16, 8, 8);

            var label = CreateLabel(preview.transform, "Obstacles", 22, TextAnchor.MiddleRight);
            label.color = UiColorPalette.TextSecondary;
            AddLayoutElement(label.gameObject, 140, 54);

            var obstacleTypes = level.Obstacles
                .Select(item => item.ObstacleType)
                .Where(item => item != ObstacleType.None)
                .Distinct()
                .ToArray();

            if (obstacleTypes.Length == 0)
            {
                var none = CreateLabel(preview.transform, "None", 24, TextAnchor.MiddleLeft);
                none.color = UiColorPalette.TextSuccess;
                AddLayoutElement(none.gameObject, 450, 54);
                return;
            }

            foreach (var obstacle in obstacleTypes)
            {
                var item = CreatePanel(preview.transform, $"Obstacle Preview - {ObstacleName(obstacle)}", UiColorPalette.WithAlpha(UiColorPalette.BackgroundSolid, 0.22f));
                AddLayoutElement(item, 150, 54);
                var itemLayout = item.AddComponent<HorizontalLayoutGroup>();
                itemLayout.childAlignment = TextAnchor.MiddleCenter;
                itemLayout.spacing = 8;
                itemLayout.padding = new RectOffset(8, 8, 6, 6);

                var iconObject = new GameObject("Obstacle Icon", typeof(RectTransform), typeof(Image));
                iconObject.transform.SetParent(item.transform, false);
                AddLayoutElement(iconObject, 38, 38);
                var icon = iconObject.GetComponent<Image>();
                icon.sprite = _iconFactory.GetObstacleSprite(obstacle);
                icon.preserveAspect = true;
                icon.raycastTarget = false;

                var text = CreateLabel(item.transform, ObstacleName(obstacle), 18, TextAnchor.MiddleLeft);
                text.color = UiColorPalette.TextPrimary;
                AddLayoutElement(text.gameObject, 88, 40);
            }
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
            if (name == "Modal" || name == "Shop Modal" || name == "Daily Reward Modal")
            {
                image.color = UiColorPalette.ModalBackdrop;
            }
            else
            {
                image.sprite = _iconFactory.GetBackgroundGradientSprite(UiColorPalette.BackgroundTop, UiColorPalette.BackgroundBottom);
                image.type = Image.Type.Simple;
                image.color = Color.white;
            }
            if (name != "Modal" && name != "Shop Modal" && name != "Daily Reward Modal")
            {
                _backdropView.Build(screen.transform);
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

        private void BuildShopModal()
        {
            var content = CreatePanel(_shopModal.transform, "Content", UiColorPalette.HudBackground);
            var rect = content.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(600, 500);
            _themeAssets.AddRoundedCorners(content, 20);
            
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 40, 40);
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;

            CreateLabel(content.transform, "Coin Shop", 48, UiColorPalette.TextLight);

            CreateButton(content.transform, "Buy 100 Coins", () => { _buyCoinPackage?.Invoke(100); }, UiColorPalette.Gold, new Vector2(400, 80));
            CreateButton(content.transform, "Buy 500 Coins", () => { _buyCoinPackage?.Invoke(500); }, UiColorPalette.Gold, new Vector2(400, 80));
            CreateButton(content.transform, "Buy 1200 Coins", () => { _buyCoinPackage?.Invoke(1200); }, UiColorPalette.Gold, new Vector2(400, 80));

            CreateButton(content.transform, "Close", () => { _shopModal.SetActive(false); _closeShop?.Invoke(); }, UiColorPalette.ButtonDisabled, new Vector2(200, 60));
            _shopModal.SetActive(false);
        }

        private void BuildDailyRewardModal()
        {
            var content = CreatePanel(_dailyRewardModal.transform, "Content", UiColorPalette.HudBackground);
            var rect = content.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(500, 400);
            _themeAssets.AddRoundedCorners(content, 20);
            
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 40, 40);
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.MiddleCenter;

            CreateLabel(content.transform, "Daily Reward!", 48, UiColorPalette.TextLight);
            CreateLabel(content.transform, "Log in every day for bigger rewards!", 24, UiColorPalette.TextDim);

            CreateButton(content.transform, "Claim Coins", () => { _claimDailyReward?.Invoke(); _dailyRewardModal.SetActive(false); }, UiColorPalette.Emerald, new Vector2(300, 80));
            _dailyRewardModal.SetActive(false);
        }

        public void ShowShop()
        {
            _playSfx?.Invoke(GameSfxCue.Tap);
            _screenTransition.ScaleReveal(_shopModal);
        }

        public void ShowDailyReward()
        {
            _playSfx?.Invoke(GameSfxCue.Tap);
            _screenTransition.ScaleReveal(_dailyRewardModal);
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
            return _uiFactory.CreatePanel(parent, name, color);
        }

        private GameObject CreateSettingsSection(Transform parent, string title, float height, Color? color = null)
        {
            var section = CreatePanel(parent, $"Settings {title} Section", color ?? UiColorPalette.WithAlpha(UiColorPalette.HudBackground, 0.86f));
            AddLayoutElement(section, 680, height);
            var layout = section.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10;
            layout.padding = new RectOffset(24, 24, 18, 18);

            var heading = CreateLabel(section.transform, title, 24, TextAnchor.MiddleCenter);
            heading.color = UiColorPalette.GoldLight;
            AddLayoutElement(heading.gameObject, 600, 34);
            return section;
        }

        private static void StretchInside(RectTransform rect, float horizontalPadding, float verticalPadding)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontalPadding, verticalPadding);
            rect.offsetMax = new Vector2(-horizontalPadding, -verticalPadding);
        }

        private Text CreateTitle(Transform parent, string text, int size)
        {
            var title = CreateLabel(parent, text, size, TextAnchor.MiddleCenter);
            _themeAssets.AddHighValueTextShadow(title);
            return title;
        }

        private Text CreateLabel(Transform parent, string text, int size, TextAnchor alignment)
        {
            return _uiFactory.CreateLabel(parent, text, size, alignment);
        }

        private Button CreateButton(Transform parent, string text, Action action, Color color, Vector2? size = null)
        {
            return _uiFactory.CreateButton(parent, text, () =>
            {
                _playSfx?.Invoke(GameSfxCue.Tap);
                action?.Invoke();
            }, color, size);
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
            AddLayoutElement(toggleObject, 560, 62);

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
            AddLayoutElement(sliderObject, 560, 78);
            var layout = sliderObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 5;

            var caption = CreateLabel(sliderObject.transform, $"{label} {Mathf.RoundToInt(Mathf.Clamp01(value) * 100f)}%", 22, TextAnchor.MiddleCenter);
            AddLayoutElement(caption.gameObject, 540, 28);
            caption.color = UiColorPalette.TextSecondary;

            var trackObject = new GameObject("Track", typeof(RectTransform), typeof(Image), typeof(Slider));
            trackObject.transform.SetParent(sliderObject.transform, false);
            AddLayoutElement(trackObject, 540, 34);
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
            _shopModal.SetActive(false);
            _dailyRewardModal.SetActive(false);
        }

        private void ShowPauseMenu()
        {
            _playSfx(GameSfxCue.Tap);
            
            ClearChildren(_modal.transform);
            var modalPanel = CreatePanel(_modal.transform, "Pause Menu", UiColorPalette.HudBackground);
            var rect = modalPanel.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(800, 400);

            var layout = modalPanel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 30;

            var title = CreateTitle(modalPanel.transform, "Game Paused", 50);
            title.color = UiColorPalette.Gold;

            var buttonGroup = new GameObject("Buttons", typeof(RectTransform));
            buttonGroup.transform.SetParent(modalPanel.transform, false);
            var btnLayout = buttonGroup.AddComponent<HorizontalLayoutGroup>();
            btnLayout.childAlignment = TextAnchor.MiddleCenter;
            btnLayout.spacing = 20;

            CreateButton(buttonGroup.transform, "Resume", () => _modal.SetActive(false), UiColorPalette.Emerald, new Vector2(180, 60));
            CreateButton(buttonGroup.transform, "Restart", _restart, UiColorPalette.Ruby, new Vector2(180, 60));
            CreateButton(buttonGroup.transform, "Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(180, 60));
            CreateButton(buttonGroup.transform, "Menu", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(180, 60));

            _screenTransition.ScaleReveal(_modal);
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
