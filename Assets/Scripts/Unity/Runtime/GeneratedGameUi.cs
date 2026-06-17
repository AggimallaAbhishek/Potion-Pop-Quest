using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
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
        private TextMeshProUGUI _movesText;
        private TextMeshProUGUI _goalText;
        private TextMeshProUGUI _scoreText;
        private TextMeshProUGUI _messageText;
        private Image _movesBadgeImage;
        private RectTransform _goalStrip;
        private Image _starProgressFill;
        private TextMeshProUGUI _starProgressText;
        private readonly List<Image> _starProgressIcons = new List<Image>();
        private GameObject _tutorialPanel;
        private TextMeshProUGUI _tutorialText;
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
        private GameObject _pauseButtonObject;
        private TextMeshProUGUI _livesText;
        private TextMeshProUGUI _coinsText;
        private TextMeshProUGUI _hammerText;
        private TextMeshProUGUI _shuffleText;

        public GeneratedGameUi(IGameLogger logger)
        {
            _logger = logger;
            _iconFactory = new TileIconFactory();
            _themeAssets = new UiThemeAssets();
            _uiFactory = new UiElementFactory(_iconFactory, _themeAssets, () => Font);
            _backdropView = new PotionLabBackdropView(_uiFactory);
        }

        private TMP_FontAsset Font
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

            // Add the static background directly to the canvas so it's shared and behind safe area
            var bgImage = canvasObject.AddComponent<Image>();
            bgImage.sprite = _iconFactory.GetSplashBackgroundSprite();
            bgImage.type = Image.Type.Simple;
            bgImage.preserveAspect = false;

            _backdropView.Build(canvasObject.transform);

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
            rect.offsetMin = new Vector2(24, -70);
            rect.offsetMax = new Vector2(-24, -14);

            _pauseButtonObject = CreateButton(_economyPanel.transform, "II", _pauseRequested, UiColorPalette.Amethyst, new Vector2(54, 44)).gameObject;
            var pauseRect = _pauseButtonObject.GetComponent<RectTransform>();
            pauseRect.anchorMin = new Vector2(0, 0.5f);
            pauseRect.anchorMax = new Vector2(0, 0.5f);
            pauseRect.pivot = new Vector2(0, 0.5f);
            pauseRect.anchoredPosition = Vector2.zero;
            _pauseButtonObject.SetActive(false);

            var rightCluster = new GameObject("Economy Right Cluster", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            rightCluster.transform.SetParent(_economyPanel.transform, false);
            var rightRect = rightCluster.GetComponent<RectTransform>();
            rightRect.anchorMin = new Vector2(1, 0.5f);
            rightRect.anchorMax = new Vector2(1, 0.5f);
            rightRect.pivot = new Vector2(1, 0.5f);
            rightRect.anchoredPosition = Vector2.zero;
            rightRect.sizeDelta = new Vector2(330, 48);
            var rightLayout = rightCluster.GetComponent<HorizontalLayoutGroup>();
            rightLayout.childAlignment = TextAnchor.MiddleRight;
            rightLayout.spacing = 8;
            rightLayout.childControlWidth = false;
            rightLayout.childControlHeight = false;
            rightLayout.childForceExpandWidth = false;
            rightLayout.childForceExpandHeight = false;

            var livesBtn = CreateButton(rightCluster.transform, "Lives 5", _buyLivesPressed, UiColorPalette.Ruby, new Vector2(112, 44));
            _livesText = livesBtn.GetComponentInChildren<TextMeshProUGUI>();

            var coinsBtn = CreateButton(rightCluster.transform, "Coins 100", _showShop, UiColorPalette.Gold, new Vector2(126, 44));
            _coinsText = coinsBtn.GetComponentInChildren<TextMeshProUGUI>();

            CreateButton(rightCluster.transform, "+", _showShop, UiColorPalette.Emerald, new Vector2(44, 44));
            
            _economyPanel.SetActive(true);
        }

        public void UpdateEconomy(int lives, long regenTimeSeconds, int coins, int hammers, int shuffles)
        {
            if (_livesText != null)
            {
                var regenText = lives < 5 && regenTimeSeconds > 0 ? $" ({regenTimeSeconds / 60}:{(regenTimeSeconds % 60):D2})" : "";
                _livesText.text = $"Lives {lives}" + regenText;
            }
            if (_coinsText != null)
            {
                _coinsText.text = $"Coins {coins}";
            }
            if (_hammerText != null)
            {
                _hammerText.text = $"Smash {hammers}";
            }
            if (_shuffleText != null)
            {
                _shuffleText.text = $"Shuffle {shuffles}";
            }
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
            AddLayoutElement(row, 580, 54);
            row.GetComponent<Image>().raycastTarget = false;
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 14;
            layout.padding = new RectOffset(10, 10, 6, 6);

            var iconObject = new GameObject("Goal Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(row.transform, false);
            AddLayoutElement(iconObject, 44, 44);
            var icon = iconObject.GetComponent<Image>();
            icon.sprite = GoalSprite(goal);
            icon.color = Color.white;
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var label = CreateLabel(row.transform, $"{GoalName(goal)}  0/{goal.Amount}", 22, TextAnchor.MiddleLeft);
            AddLayoutElement(label.gameObject, 488, 48);
            label.color = UiColorPalette.TextPrimary;
        }

        private void CreateIntroObstaclePreview(Transform parent, LevelData level)
        {
            var preview = CreatePanel(parent, "Intro Obstacle Preview", UiColorPalette.WithAlpha(UiColorPalette.Sapphire, 0.18f));
            AddLayoutElement(preview, 620, 58);
            var layout = preview.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 12;
            layout.padding = new RectOffset(16, 16, 8, 8);

            var label = CreateLabel(preview.transform, "Obstacles", 20, TextAnchor.MiddleRight);
            label.color = UiColorPalette.TextSecondary;
            AddLayoutElement(label.gameObject, 126, 42);

            var obstacleTypes = level.Obstacles
                .Select(item => item.ObstacleType)
                .Where(item => item != ObstacleType.None)
                .Distinct()
                .ToArray();

            if (obstacleTypes.Length == 0)
            {
                var none = CreateLabel(preview.transform, "None", 22, TextAnchor.MiddleLeft);
                none.color = UiColorPalette.TextSuccess;
                AddLayoutElement(none.gameObject, 420, 42);
                return;
            }

            foreach (var obstacle in obstacleTypes)
            {
                var item = CreatePanel(preview.transform, $"Obstacle Preview - {ObstacleName(obstacle)}", UiColorPalette.WithAlpha(UiColorPalette.BackgroundSolid, 0.22f));
                AddLayoutElement(item, 136, 42);
                var itemLayout = item.AddComponent<HorizontalLayoutGroup>();
                itemLayout.childAlignment = TextAnchor.MiddleCenter;
                itemLayout.spacing = 8;
                itemLayout.padding = new RectOffset(8, 8, 6, 6);

                var iconObject = new GameObject("Obstacle Icon", typeof(RectTransform), typeof(Image));
                iconObject.transform.SetParent(item.transform, false);
                AddLayoutElement(iconObject, 30, 30);
                var icon = iconObject.GetComponent<Image>();
                icon.sprite = _iconFactory.GetObstacleSprite(obstacle);
                icon.preserveAspect = true;
                icon.raycastTarget = false;

                var text = CreateLabel(item.transform, ObstacleName(obstacle), 16, TextAnchor.MiddleLeft);
                text.color = UiColorPalette.TextPrimary;
                AddLayoutElement(text.gameObject, 82, 34);
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
                image.color = new Color(0, 0, 0, 0); // Transparent for screens since canvas has background
            }

            // Add CanvasGroup for transitions
            screen.AddComponent<CanvasGroup>();

            var layout = screen.GetComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 24;
            layout.padding = UiLayoutMetrics.ScreenPadding();
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            screen.SetActive(false);
            return screen;
        }





        /// <summary>Smoothly transitions between screens using ScreenTransitionController.</summary>
        private void TransitionTo(GameObject target)
        {
            if (_currentScreen == target)
            {
                SetGameplayChromeVisible(target == _game);
                return;
            }

            var previous = _currentScreen;
            _currentScreen = target;
            SetGameplayChromeVisible(target == _game);

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

        private void SetGameplayChromeVisible(bool visible)
        {
            if (_pauseButtonObject != null)
            {
                _pauseButtonObject.SetActive(visible);
            }
        }

        private GameObject CreatePanel(Transform parent, string name, Color color)
        {
            return _uiFactory.CreatePanel(parent, name, color);
        }

        private GameObject CreateSettingsSection(Transform parent, string title, float height, Color? color = null)
        {
            var section = CreatePanel(parent, $"Settings {title} Section", color ?? UiColorPalette.WithAlpha(UiColorPalette.HudBackground, 0.86f));
            AddLayoutElement(section, Mathf.Min(720f, UiLayoutMetrics.MenuContentWidth()), height);
            var layout = section.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 5;
            layout.padding = new RectOffset(24, 24, 14, 14);

            var heading = CreateLabel(section.transform, title, 22, TextAnchor.MiddleCenter);
            heading.color = UiColorPalette.GoldLight;
            AddLayoutElement(heading.gameObject, 600, 28);
            return section;
        }

        private static void StretchInside(RectTransform rect, float horizontalPadding, float verticalPadding)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(horizontalPadding, verticalPadding);
            rect.offsetMax = new Vector2(-horizontalPadding, -verticalPadding);
        }

        private TextMeshProUGUI CreateTitle(Transform parent, string text, int size)
        {
            var title = CreateLabel(parent, text, size, TextAnchor.MiddleCenter);
            _themeAssets.AddHighValueTextShadow(title);
            return title;
        }

        private TextMeshProUGUI CreateLabel(Transform parent, string text, int size, TextAnchor alignment)
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
            AddLayoutElement(toggleObject, 560, 48);

            var layout = toggleObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleRight;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var text = CreateLabel(toggleObject.transform, label, 24, TextAnchor.MiddleLeft);
            var textLayout = text.gameObject.AddComponent<LayoutElement>();
            textLayout.flexibleWidth = 1;

            var background = new GameObject("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(toggleObject.transform, false);
            var backgroundLayout = background.AddComponent<LayoutElement>();
            backgroundLayout.preferredWidth = 42;
            backgroundLayout.preferredHeight = 42;
            background.GetComponent<Image>().color = new Color(0.18f, 0.22f, 0.28f);

            var check = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            check.transform.SetParent(background.transform, false);
            var checkRect = check.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.18f, 0.18f);
            checkRect.anchorMax = new Vector2(0.82f, 0.82f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;
            check.GetComponent<Image>().color = new Color(0.32f, 0.78f, 0.56f);

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
            AddLayoutElement(sliderObject, 560, 58);
            var layout = sliderObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 16;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var caption = CreateLabel(sliderObject.transform, $"{label} {Mathf.RoundToInt(Mathf.Clamp01(value) * 100f)}%", 20, TextAnchor.MiddleLeft);
            var captionLayout = caption.gameObject.AddComponent<LayoutElement>();
            captionLayout.preferredWidth = 200;

            var trackObject = new GameObject("Track", typeof(RectTransform), typeof(Image), typeof(Slider));
            trackObject.transform.SetParent(sliderObject.transform, false);
            var trackLayout = trackObject.AddComponent<LayoutElement>();
            trackLayout.flexibleWidth = 1;
            trackLayout.preferredHeight = 28;
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
            handleRect.sizeDelta = new Vector2(28, 28);
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

            var rect = target.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(preferredWidth, preferredHeight);
            }

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
            _playSfx?.Invoke(GameSfxCue.Tap);
            
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
