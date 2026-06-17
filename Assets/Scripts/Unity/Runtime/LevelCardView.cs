using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PotionPopQuest.Unity
{
    public class LevelCardView : MonoBehaviour
    {
        public Button button;
        public Image backgroundImage;
        public GameObject gradientOverlay;
        public GameObject currentOutline;
        public TextMeshProUGUI levelText;
        public Image[] stars;
        public GameObject lockIcon;
        
        public void Bind(int levelNumber, int starCount, bool isLocked, bool isNext, Action<int> onStartLevel)
        {
            levelText.text = levelNumber.ToString();
            levelText.color = isLocked ? UiColorPalette.TextMuted : UiColorPalette.TextPrimary;
            
            backgroundImage.color = isLocked ? UiColorPalette.LevelCardLocked : UiColorPalette.LevelCardUnlocked;
            button.interactable = !isLocked;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onStartLevel(levelNumber));

            if (gradientOverlay != null) gradientOverlay.SetActive(!isLocked);
            if (currentOutline != null) currentOutline.SetActive(isNext);
            if (lockIcon != null) lockIcon.SetActive(isLocked);

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].gameObject.SetActive(!isLocked);
                stars[i].color = i < starCount ? UiColorPalette.Gold : new Color(0, 0, 0, 0.4f);
            }
        }
    }
}
