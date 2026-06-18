using System;
using PotionPopQuest.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
        private TextMeshProUGUI _menuLevelBadgeText;
        private TextMeshProUGUI _menuCurrencyText;

        public void ShowMainMenu(int highestUnlocked = 1, int coins = 0)
        {
            ClearHint();
            HideLevelIntro();
            if (_menuLevelBadgeText != null) _menuLevelBadgeText.text = $"Level {highestUnlocked}";
            if (_menuCurrencyText != null) _menuCurrencyText.text = $"Coins {coins}";
            TransitionTo(_mainMenu);
        }

        private void BuildMainMenu()
        {
            // Top Bar
            var topBar = new GameObject("Top Bar", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            topBar.transform.SetParent(_mainMenu.transform, false);
            AddLayoutElement(topBar, UiLayoutMetrics.MenuContentWidth(), 64);
            var topLayout = topBar.GetComponent<HorizontalLayoutGroup>();
            topLayout.childAlignment = TextAnchor.MiddleCenter;
            topLayout.spacing = 16;
            topLayout.childControlWidth = false;
            topLayout.childControlHeight = false;

            var currencyPill = CreatePanel(topBar.transform, "Currency Pill", new Color(0, 0, 0, 0.35f));
            AddLayoutElement(currencyPill, 140, 44);
            _menuCurrencyText = CreateLabel(currencyPill.transform, "Coins 0", 18, TextAnchor.MiddleCenter);
            StretchInside(_menuCurrencyText.rectTransform, 0, 0);

            var spacer = new GameObject("Spacer", typeof(RectTransform));
            spacer.transform.SetParent(topBar.transform, false);
            var spacerLayout = spacer.AddComponent<LayoutElement>();
            spacerLayout.flexibleWidth = 1;

            CreateButton(topBar.transform, "Map", _showLevels, UiColorPalette.WithAlpha(Color.white, 0.15f), new Vector2(80, 52));
            CreateButton(topBar.transform, "Settings", _showSettings, UiColorPalette.WithAlpha(Color.white, 0.15f), new Vector2(100, 52));

            // Hero Section
            var heroSection = new GameObject("Hero Section", typeof(RectTransform), typeof(VerticalLayoutGroup));
            heroSection.transform.SetParent(_mainMenu.transform, false);
            AddLayoutElement(heroSection, UiLayoutMetrics.MenuContentWidth(), 340);
            var heroLayout = heroSection.GetComponent<VerticalLayoutGroup>();
            heroLayout.childAlignment = TextAnchor.MiddleCenter;
            heroLayout.spacing = 8;
            heroLayout.childControlWidth = false;
            heroLayout.childControlHeight = false;

            var potionIcon = new GameObject("Menu Potion Icon", typeof(RectTransform), typeof(Image));
            potionIcon.transform.SetParent(heroSection.transform, false);
            AddLayoutElement(potionIcon, 180, 180);
            var potionImage = potionIcon.GetComponent<Image>();
            potionImage.sprite = _iconFactory.GetPotionSprite(PotionType.Mega);
            potionImage.preserveAspect = true;

            var titleText = CreateTitle(heroSection.transform, "Potion Pop Quest", 36);
            titleText.color = UiColorPalette.TextPrimary;
            _themeAssets.AddTitleTextEffects(titleText);
            AddLayoutElement(titleText.gameObject, UiLayoutMetrics.MenuContentWidth(), 46);

            var badgePanel = CreatePanel(heroSection.transform, "Level Badge", UiColorPalette.Amethyst);
            AddLayoutElement(badgePanel, 120, 32);
            _menuLevelBadgeText = CreateLabel(badgePanel.transform, "Level 1", 16, TextAnchor.MiddleCenter);
            StretchInside(_menuLevelBadgeText.rectTransform, 0, 0);

            // Cards Section
            var dailyCard = _uiFactory.CreateGlassPanel(_mainMenu.transform, "Daily Reward", Mathf.Min(400, UiLayoutMetrics.MenuContentWidth()), 80);
            var dailyText = CreateLabel(dailyCard.transform, "Daily Reward Ready!", 20, TextAnchor.MiddleCenter);
            dailyText.color = UiColorPalette.Gold;
            StretchInside(dailyText.rectTransform, 0, 0);

            // Bottom Spacer
            var bottomSpacer = new GameObject("Spacer", typeof(RectTransform));
            bottomSpacer.transform.SetParent(_mainMenu.transform, false);
            var bottomSpacerLayout = bottomSpacer.AddComponent<LayoutElement>();
            bottomSpacerLayout.flexibleHeight = 1;

            // Bottom Actions
            var bottomActions = new GameObject("Bottom Actions", typeof(RectTransform), typeof(VerticalLayoutGroup));
            bottomActions.transform.SetParent(_mainMenu.transform, false);
            AddLayoutElement(bottomActions, UiLayoutMetrics.MenuContentWidth(), 100);
            var bottomLayout = bottomActions.GetComponent<VerticalLayoutGroup>();
            bottomLayout.childAlignment = TextAnchor.MiddleCenter;

            CreateButton(bottomActions.transform, "Play Journey", _play, UiColorPalette.Emerald, new Vector2(320, 72));
        }
    }
}
