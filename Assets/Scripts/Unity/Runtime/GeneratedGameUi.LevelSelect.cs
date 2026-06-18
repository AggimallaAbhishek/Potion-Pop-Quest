using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
        public void ShowLevelSelect(IReadOnlyList<LevelData> levels, int highestUnlocked, Func<int, int> starsForLevel)
        {
            ClearHint();
            HideLevelIntro();
            ClearChildren(_levelSelect.transform);

            var title = CreateTitle(_levelSelect.transform, "Level Select", 42);
            title.color = UiColorPalette.TextPrimary;
            AddLayoutElement(title.gameObject, UiLayoutMetrics.MenuContentWidth(), 54);
            var subtitle = CreateLabel(_levelSelect.transform, "Candy Potion Map", 20, TextAnchor.MiddleCenter);
            subtitle.color = UiColorPalette.TextSecondary;
            AddLayoutElement(subtitle.gameObject, UiLayoutMetrics.MenuContentWidth(), 30);

            var scrollFrame = CreatePanel(_levelSelect.transform, "Levels Scroll View", UiColorPalette.LevelGridBackground);
            
            // For the winding path, we just need a single column basically, handled by LevelScrollPool
            var gridWidth = 400f; // Width of the winding path area
            var frameWidth = Mathf.Min(UiLayoutMetrics.ScreenMaxWidth, gridWidth);
            var frameHeight = Screen.width > Screen.height ? 420f : 710f;
            AddLayoutElement(scrollFrame, frameWidth, frameHeight);
            
            var scrollRect = scrollFrame.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 35f;
            scrollFrame.AddComponent<RectMask2D>();

            var grid = new GameObject("Levels Map", typeof(RectTransform));
            grid.transform.SetParent(scrollFrame.transform, false);
            var gridRect = grid.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0.5f, 0f); // Anchor to bottom to scroll up like Candy Crush
            gridRect.anchorMax = new Vector2(0.5f, 0f);
            gridRect.pivot = new Vector2(0.5f, 0f);
            gridRect.anchoredPosition = Vector2.zero;
            gridRect.sizeDelta = new Vector2(gridWidth, frameHeight); // Height is managed by pool
            scrollRect.content = gridRect;
            scrollRect.viewport = scrollFrame.GetComponent<RectTransform>();

            var pool = grid.AddComponent<LevelScrollPool>();
            pool.scrollRect = scrollRect;
            pool.content = gridRect;
            pool.cellSize = 110f;
            pool.spacing = 30f;
            
            pool.onBindCell = (index, cell) => {
                var levelData = levels[index];
                var locked = levelData.LevelNumber > highestUnlocked;
                var isNext = levelData.LevelNumber == highestUnlocked;
                var stars = starsForLevel(levelData.LevelNumber);
                var view = cell.GetComponent<LevelCardView>();
                view.Bind(levelData.LevelNumber, stars, locked, isNext, _startLevel);
            };

            pool.Initialize(levels.Count, () => CreateLevelCardPrefab(grid.transform).transform);

            CreateButton(_levelSelect.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(240, 58));
            TransitionTo(_levelSelect);
        }

        private GameObject CreateLevelCardPrefab(Transform parent)
        {
            var button = CreateButton(parent, "", null, UiColorPalette.LevelCardUnlocked);
            
            var buttonImage = button.transform.Find("ButtonGraphic")?.GetComponent<Image>();
            if (buttonImage == null) buttonImage = button.GetComponent<Image>();

            if (buttonImage != null)
            {
                buttonImage.sprite = _iconFactory.GetCircleSprite();
                buttonImage.type = Image.Type.Simple;
            }

            var shadowImage = button.transform.Find("ButtonShadow")?.GetComponent<Image>();
            if (shadowImage != null)
            {
                shadowImage.sprite = _iconFactory.GetCircleSprite();
                shadowImage.type = Image.Type.Simple;
            }

            var cardView = button.gameObject.AddComponent<LevelCardView>();
            cardView.button = button;
            cardView.backgroundImage = buttonImage;

            // Gradient overlay for unlocked cards (Candy Crush glossy feel)
            var gradient = new GameObject("CardGradient", typeof(RectTransform), typeof(Image));
            gradient.transform.SetParent(button.transform, false);
            gradient.transform.SetAsFirstSibling();
            var gradRect = gradient.GetComponent<RectTransform>();
            gradRect.anchorMin = Vector2.zero;
            gradRect.anchorMax = Vector2.one;
            gradRect.offsetMin = Vector2.zero;
            gradRect.offsetMax = Vector2.zero;
            var gradImage = gradient.GetComponent<Image>();
            gradImage.sprite = _iconFactory.GetCircleSprite();
            gradImage.color = UiColorPalette.WithAlpha(UiColorPalette.LevelCardUnlockedGradient, 0.40f);
            gradImage.raycastTarget = false;
            cardView.gradientOverlay = gradient;

            // Current level outline (pulsing ring around the circle)
            var outline = new GameObject("CurrentOutline", typeof(RectTransform), typeof(Image));
            outline.transform.SetParent(button.transform, false);
            var outRect = outline.GetComponent<RectTransform>();
            outRect.anchorMin = Vector2.zero;
            outRect.anchorMax = Vector2.one;
            outRect.offsetMin = new Vector2(-8, -8);
            outRect.offsetMax = new Vector2(8, 8);
            var outImg = outline.GetComponent<Image>();
            outImg.sprite = _iconFactory.GetCircleSprite();
            outImg.color = UiColorPalette.WithAlpha(UiColorPalette.Gold, 0.6f);
            outImg.raycastTarget = false;
            
            var innerMask = new GameObject("InnerMask", typeof(RectTransform), typeof(Image));
            innerMask.transform.SetParent(outline.transform, false);
            var innerRect = innerMask.GetComponent<RectTransform>();
            innerRect.anchorMin = Vector2.zero;
            innerRect.anchorMax = Vector2.one;
            innerRect.offsetMin = new Vector2(8, 8);
            innerRect.offsetMax = new Vector2(-8, -8);
            var innerImg = innerMask.GetComponent<Image>();
            innerImg.sprite = _iconFactory.GetCircleSprite();
            innerImg.color = UiColorPalette.LevelCardUnlocked;
            innerImg.raycastTarget = false;
            
            outline.transform.SetAsFirstSibling();
            cardView.currentOutline = outline;

            // Lock Icon / Label
            var lockLabel = CreateLabel(button.transform, "LOCKED", 15, TextAnchor.MiddleCenter);
            lockLabel.rectTransform.anchorMin = new Vector2(0.1f, 0.1f);
            lockLabel.rectTransform.anchorMax = new Vector2(0.9f, 0.9f);
            lockLabel.rectTransform.offsetMin = Vector2.zero;
            lockLabel.rectTransform.offsetMax = Vector2.zero;
            lockLabel.color = UiColorPalette.TextMuted;
            cardView.lockIcon = lockLabel.gameObject;

            // Level number (large, centered in the circle)
            var numberLabel = CreateLabel(button.transform, "1", 38, TextAnchor.MiddleCenter);
            numberLabel.rectTransform.anchorMin = Vector2.zero;
            numberLabel.rectTransform.anchorMax = Vector2.one;
            numberLabel.rectTransform.offsetMin = Vector2.zero;
            numberLabel.rectTransform.offsetMax = Vector2.zero;
            numberLabel.color = UiColorPalette.TextPrimary;
            _themeAssets.AddHighValueTextShadow(numberLabel);
            cardView.levelText = numberLabel;

            // Star row (floating below the circle)
            var starRow = new GameObject("Level Card Stars", typeof(RectTransform), typeof(LayoutElement));
            starRow.transform.SetParent(button.transform, false);
            var starRect = starRow.GetComponent<RectTransform>();
            starRect.anchorMin = new Vector2(0.5f, 0f);
            starRect.anchorMax = new Vector2(0.5f, 0f);
            starRect.pivot = new Vector2(0.5f, 0.5f);
            starRect.anchoredPosition = new Vector2(0, -15f); // Placed slightly below the circle
            starRect.sizeDelta = new Vector2(100, 30);
            starRow.GetComponent<LayoutElement>().ignoreLayout = true;
            var starLayout = starRow.AddComponent<HorizontalLayoutGroup>();
            starLayout.childAlignment = TextAnchor.MiddleCenter;
            starLayout.spacing = 2;
            cardView.stars = new Image[3];
            for (var i = 0; i < 3; i++)
            {
                var starObj = new GameObject("Star", typeof(RectTransform), typeof(Image));
                starObj.transform.SetParent(starRow.transform, false);
                AddLayoutElement(starObj, 28, 28);
                var img = starObj.GetComponent<Image>();
                img.sprite = _iconFactory.GetStarSprite(true);
                img.preserveAspect = true;
                img.raycastTarget = false;
                cardView.stars[i] = img;
            }

            return button.gameObject;
        }
    }
}
