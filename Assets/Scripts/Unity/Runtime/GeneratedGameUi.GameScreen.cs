using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PotionPopQuest.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
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
            SetGameplayChromeVisible(true);
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
            panelRect.sizeDelta = new Vector2(700, 560);
            panel.AddComponent<CanvasGroup>();
            var layout = panel.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 12;
            layout.padding = new RectOffset(30, 30, 30, 26);

            var title = CreateTitle(panel.transform, $"Level {session.Level.LevelNumber}", 44);
            title.color = UiColorPalette.Gold;
            var subtitle = CreateLabel(panel.transform, session.Level.DisplayName, 24, TextAnchor.MiddleCenter);
            subtitle.color = UiColorPalette.TextSecondary;
            AddLayoutElement(subtitle.gameObject, 600, 36);

            var goalsPanel = CreatePanel(panel.transform, "Intro Goals", new Color(0.12f, 0.14f, 0.20f, 0.85f));
            goalsPanel.GetComponent<Image>().raycastTarget = false;
            AddLayoutElement(goalsPanel, 620, Mathf.Max(86, session.Level.Goals.Count * 66));
            var goalsLayout = goalsPanel.AddComponent<VerticalLayoutGroup>();
            goalsLayout.childAlignment = TextAnchor.MiddleCenter;
            goalsLayout.spacing = 8;
            goalsLayout.padding = new RectOffset(14, 14, 14, 14);
            foreach (var goal in session.Level.Goals)
            {
                CreateIntroGoalRow(goalsPanel.transform, goal);
            }

            CreateIntroObstaclePreview(panel.transform, session.Level);

            var movesLabel = CreateLabel(panel.transform, $"{session.MovesRemaining} Moves", 30, TextAnchor.MiddleCenter);
            movesLabel.color = UiColorPalette.TextSuccess;
            AddLayoutElement(movesLabel.gameObject, 600, 42);
            var tapLabel = CreateLabel(panel.transform, "Tap to Start", 24, TextAnchor.MiddleCenter);
            tapLabel.color = UiColorPalette.GoldLight;
            AddLayoutElement(tapLabel.gameObject, 600, 34);

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

        private static IEnumerator FloatingScoreRoutine(TextMeshProUGUI label)
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


        private void BuildGameScreen()
        {
            ClearChildren(_game.transform);
            var screenLayout = _game.GetComponent<VerticalLayoutGroup>();
            screenLayout.spacing = UiLayoutMetrics.GameScreenSpacing();
            screenLayout.padding = UiLayoutMetrics.GameScreenPadding();

            ClearChildren(_game.transform);

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
            hudLayout.spacing = 14;
            hudLayout.padding = new RectOffset(16, 16, 10, 10);

            var movesBadge = _uiFactory.CreateGlowingBadge(hud.transform, "HUD Moves Badge", 138, UiColorPalette.WithAlpha(UiColorPalette.Sapphire, 0.58f));
            AddLayoutElement(movesBadge, 138, Mathf.Max(76f, hudHeight - 20f));
            _movesBadgeImage = movesBadge.GetComponent<Image>();
            _movesText = CreateLabel(movesBadge.transform, "Moves\n0", 23, TextAnchor.MiddleCenter);
            StretchInside(_movesText.rectTransform, 8, 6);
            _themeAssets.AddHighValueTextShadow(_movesText);

            var goalPanel = CreateHudBadge(hud.transform, "HUD Goal Panel", 540, UiColorPalette.WithAlpha(UiColorPalette.Amethyst, 0.42f));
            var goalLayout = goalPanel.AddComponent<VerticalLayoutGroup>();
            goalLayout.childAlignment = TextAnchor.MiddleCenter;
            goalLayout.spacing = 4;
            goalLayout.padding = new RectOffset(12, 12, 8, 8);
            _goalText = CreateLabel(goalPanel.transform, "Goal", 18, TextAnchor.MiddleCenter);
            _goalText.color = UiColorPalette.GoldLight;
            AddLayoutElement(_goalText.gameObject, 508, 22);
            var goalStripObject = new GameObject("HUD Goal Strip", typeof(RectTransform));
            goalStripObject.transform.SetParent(goalPanel.transform, false);
            _goalStrip = goalStripObject.GetComponent<RectTransform>();
            AddLayoutElement(goalStripObject, 508, Mathf.Max(44f, hudHeight - 54f));
            var goalStripLayout = goalStripObject.AddComponent<VerticalLayoutGroup>();
            goalStripLayout.childAlignment = TextAnchor.MiddleCenter;
            goalStripLayout.spacing = 6;
            goalStripLayout.padding = new RectOffset(0, 0, 0, 0);

            var scoreBadge = _uiFactory.CreateGlowingBadge(hud.transform, "HUD Score Badge", 138, UiColorPalette.WithAlpha(UiColorPalette.EmeraldDark, 0.52f));
            AddLayoutElement(scoreBadge, 138, Mathf.Max(76f, hudHeight - 20f));
            _scoreText = CreateLabel(scoreBadge.transform, "Score\n0", 23, TextAnchor.MiddleCenter);
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

            _tutorialPanel = CreatePanel(boardPanel.transform, "Tutorial Banner", UiColorPalette.WithAlpha(UiColorPalette.TutorialBackground, 0.9f));
            var tutRect = _tutorialPanel.GetComponent<RectTransform>();
            tutRect.anchorMin = new Vector2(0.05f, 0.35f);
            tutRect.anchorMax = new Vector2(0.95f, 0.65f);
            tutRect.offsetMin = Vector2.zero;
            tutRect.offsetMax = Vector2.zero;
            _tutorialText = CreateLabel(_tutorialPanel.transform, "", 26, TextAnchor.MiddleCenter);
            _themeAssets.AddHighValueTextShadow(_tutorialText);
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
            boostersLayout.spacing = 14;
            
            CreateButton(boosters.transform, "Hint", _hintRequested, UiColorPalette.Sapphire, new Vector2(150, touchHeight));

            var hammerBtn = CreateButton(boosters.transform, "Smash 0", _hammerBoosterPressed, UiColorPalette.Gold, new Vector2(150, touchHeight));
            _hammerText = hammerBtn.GetComponentInChildren<TextMeshProUGUI>();
            
            var shuffleBtn = CreateButton(boosters.transform, "Shuffle 0", _shuffleBoosterPressed, UiColorPalette.Gold, new Vector2(150, touchHeight));
            _shuffleText = shuffleBtn.GetComponentInChildren<TextMeshProUGUI>();
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



        private GameObject CreateHudBadge(Transform parent, string name, float width, Color color)
        {
            var badge = CreatePanel(parent, name, color);
            AddLayoutElement(badge, width, Mathf.Max(68f, UiLayoutMetrics.GameHudHeight() - 20f));
            return badge;
        }

        private void CreateHudGoalRow(Transform parent, GoalProgress progress)
        {
            var row = CreatePanel(parent, "HUD Goal Row", progress.IsComplete
                ? UiColorPalette.WithAlpha(UiColorPalette.EmeraldDark, 0.35f)
                : UiColorPalette.WithAlpha(UiColorPalette.BackgroundSolid, 0.20f));
            AddLayoutElement(row, 504, 36);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 8;
            layout.padding = new RectOffset(8, 8, 3, 3);

            var iconObject = new GameObject("HUD Goal Icon", typeof(RectTransform), typeof(Image));
            iconObject.transform.SetParent(row.transform, false);
            AddLayoutElement(iconObject, 30, 30);
            var icon = iconObject.GetComponent<Image>();
            icon.sprite = GoalSprite(progress.Goal);
            icon.preserveAspect = true;
            icon.raycastTarget = false;

            var label = CreateLabel(row.transform, $"{GoalName(progress.Goal)}  {progress.CurrentAmount}/{progress.Goal.Amount}", 18, TextAnchor.MiddleLeft);
            label.color = progress.IsComplete ? UiColorPalette.TextSuccess : UiColorPalette.TextPrimary;
            AddLayoutElement(label.gameObject, 452, 32);
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
    }
}
