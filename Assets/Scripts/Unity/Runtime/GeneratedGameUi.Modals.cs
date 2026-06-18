using System.Collections;
using System.Collections.Generic;
using PotionPopQuest.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
        public bool IsModalOpen()
        {
            return (_shopModal != null && _shopModal.activeInHierarchy) ||
                   (_dailyRewardModal != null && _dailyRewardModal.activeInHierarchy);
        }

        public void CloseTopModal()
        {
            if (_dailyRewardModal != null && _dailyRewardModal.activeInHierarchy)
            {
                _dailyRewardModal.SetActive(false);
                return;
            }
            if (_shopModal != null && _shopModal.activeInHierarchy)
            {
                _shopModal.SetActive(false);
                return;
            }
        }

        private void ShowWinModal(GameSession session, bool hasNextLevel)
        {
            var panel = CreateModalPanel(700, 14);

            // Golden gradient header
            var headerGlow = CreatePanel(panel.transform, "WinHeaderGlow", UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.12f));
            AddLayoutElement(headerGlow, 680, 8);
            headerGlow.GetComponent<Image>().raycastTarget = false;

            var titleText = CreateTitle(panel.transform, "Level Complete", 44);
            titleText.color = UiColorPalette.Gold;
            _themeAssets.AddTitleTextEffects(titleText);

            var scoreLabel = CreateLabel(panel.transform, "Score 0", 32, TextAnchor.MiddleCenter);
            scoreLabel.color = UiColorPalette.TextPrimary;
            AddLayoutElement(scoreLabel.gameObject, 560, 60);

            var starLabels = CreateStarRow(panel.transform, session.Stars);
            CreateGoalSummary(panel.transform, session.GoalTracker.Goals, false);
            
            var actionsPanel = new GameObject("Modal Actions", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            actionsPanel.transform.SetParent(panel.transform, false);
            AddLayoutElement(actionsPanel, 600, 80);
            var actionsLayout = actionsPanel.GetComponent<HorizontalLayoutGroup>();
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.spacing = 20;

            if (hasNextLevel)
            {
                CreateButton(actionsPanel.transform, "\u2606  Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(260, 64));
                CreateButton(actionsPanel.transform, "\u25B6  Next", _nextLevel, UiColorPalette.Emerald, new Vector2(260, 64));
            }
            else
            {
                CreateButton(actionsPanel.transform, "Replay", _restart, UiColorPalette.Sapphire, new Vector2(260, 64));
                CreateButton(actionsPanel.transform, "Menu", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(260, 64));
            }

            var rect = panel.GetComponent<RectTransform>();
            _feedbackAnimator.PlayModalIntro(rect);
            _boardAnimationController.StartCoroutine(AnimateModalScore(scoreLabel, session.Score));
            _boardAnimationController.StartCoroutine(RevealStars(starLabels, session.Stars));
            _boardAnimationController.StartCoroutine(SpawnConfetti(_modal.transform));
        }

        private void ShowLoseModal(GameSession session)
        {
            var panel = CreateModalPanel(640, 14);

            // Dramatic red header
            var headerGlow = CreatePanel(panel.transform, "LoseHeaderGlow", UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.15f));
            AddLayoutElement(headerGlow, 680, 8);
            headerGlow.GetComponent<Image>().raycastTarget = false;

            var titleText = CreateTitle(panel.transform, "Out of Moves", 42);
            titleText.color = UiColorPalette.RubyLight;
            _themeAssets.AddTitleTextEffects(titleText);
            var body = CreateLabel(panel.transform, "Try again to finish the remaining goals.", 23, TextAnchor.MiddleCenter);
            body.color = UiColorPalette.TextSecondary;
            AddLayoutElement(body.gameObject, 560, 48);
            CreateGoalSummary(panel.transform, session.GoalTracker.Goals, true);
            
            var actionsPanel = new GameObject("Modal Actions", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            actionsPanel.transform.SetParent(panel.transform, false);
            AddLayoutElement(actionsPanel, 600, 80);
            var actionsLayout = actionsPanel.GetComponent<HorizontalLayoutGroup>();
            actionsLayout.childAlignment = TextAnchor.MiddleCenter;
            actionsLayout.spacing = 20;

            CreateButton(actionsPanel.transform, "Levels", _showLevels, UiColorPalette.Sapphire, new Vector2(260, 64));
            CreateButton(actionsPanel.transform, "\u21BB  Retry", _restart, UiColorPalette.Emerald, new Vector2(260, 64));

            _feedbackAnimator.PlayModalIntro(panel.GetComponent<RectTransform>());
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
            AddLayoutElement(starRow, 340, 58);
            var starLayout = starRow.AddComponent<HorizontalLayoutGroup>();
            starLayout.childAlignment = TextAnchor.MiddleCenter;
            starLayout.spacing = 18;

            for (var i = 1; i <= 3; i++)
            {
                labels.Add(CreateStarImage(starRow.transform, false, 54));
            }

            return labels;
        }

        private void CreateGoalSummary(Transform parent, IReadOnlyList<GoalProgress> goals, bool remainingOnly)
        {
            var summary = CreatePanel(parent, remainingOnly ? "Remaining Goals" : "Completed Goals", UiColorPalette.WithAlpha(UiColorPalette.Amethyst, 0.32f));
            AddLayoutElement(summary, 600, Mathf.Clamp(goals.Count * 50 + 24, 96, 202));
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
                AddLayoutElement(row, 540, 42);
                row.GetComponent<Image>().raycastTarget = false;
                var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
                rowLayout.childAlignment = TextAnchor.MiddleCenter;
                rowLayout.spacing = 12;

                var iconObject = new GameObject("Goal Icon", typeof(RectTransform), typeof(Image));
                iconObject.transform.SetParent(row.transform, false);
                AddLayoutElement(iconObject, 34, 34);
                var icon = iconObject.GetComponent<Image>();
                icon.sprite = GoalSprite(progress.Goal);
                icon.preserveAspect = true;
                icon.raycastTarget = false;

                var label = CreateLabel(row.transform, $"{GoalName(progress.Goal)}  {amount}", 20, TextAnchor.MiddleLeft);
                label.color = remainingOnly && progress.RemainingAmount > 0 ? UiColorPalette.GoldLight : UiColorPalette.TextSuccess;
                AddLayoutElement(label.gameObject, 474, 38);
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

        private static IEnumerator AnimateModalScore(TextMeshProUGUI label, int finalScore)
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

        private void BuildShopModal()
        {
            var content = _uiFactory.CreateGlassPanel(_shopModal.transform, "Content", 600, 500);
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 40, 40);
            layout.spacing = 20;
            layout.childAlignment = TextAnchor.MiddleCenter;

            var title = CreateLabel(content.transform, "Test Coin Shop", 48, TextAnchor.MiddleCenter);
            title.color = UiColorPalette.TextPrimary;

            CreateButton(content.transform, "Test: Add 100 Coins", () => { _buyCoinPackage?.Invoke(100); }, UiColorPalette.Gold, new Vector2(400, 80));
            CreateButton(content.transform, "Test: Add 500 Coins", () => { _buyCoinPackage?.Invoke(500); }, UiColorPalette.Gold, new Vector2(400, 80));
            CreateButton(content.transform, "Test: Add 1200 Coins", () => { _buyCoinPackage?.Invoke(1200); }, UiColorPalette.Gold, new Vector2(400, 80));

            CreateButton(content.transform, "Close", () => { _shopModal.SetActive(false); _closeShop?.Invoke(); }, UiColorPalette.StoneBlock, new Vector2(200, 60));
            _shopModal.SetActive(false);
        }

        private void BuildDailyRewardModal()
        {
            var content = _uiFactory.CreateGlassPanel(_dailyRewardModal.transform, "Content", 500, 400);
            var layout = content.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(40, 40, 40, 40);
            layout.spacing = 30;
            layout.childAlignment = TextAnchor.MiddleCenter;

            var title = CreateLabel(content.transform, "Daily Reward!", 48, TextAnchor.MiddleCenter);
            title.color = UiColorPalette.Gold;
            var subtitle = CreateLabel(content.transform, "Log in every day for bigger rewards!", 24, TextAnchor.MiddleCenter);
            subtitle.color = UiColorPalette.TextSecondary;

            CreateButton(content.transform, "Claim Test Coins", () => { _claimDailyReward?.Invoke(); _dailyRewardModal.SetActive(false); }, UiColorPalette.Emerald, new Vector2(300, 80));
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
    }
}
