using System;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed partial class GeneratedGameUi
    {
        public void ShowSettings(bool musicEnabled, bool sfxEnabled, float musicVolume, float sfxVolume, bool vibrationEnabled)
        {
            ClearHint();
            HideLevelIntro();
            ClearChildren(_settings.transform);
            
            var title = CreateTitle(_settings.transform, "Settings", 42);
            AddLayoutElement(title.gameObject, UiLayoutMetrics.MenuContentWidth(), 54);
            var audioSection = CreateSettingsSection(_settings.transform, "Audio", 286);
            CreateToggle(audioSection.transform, "Music", musicEnabled, _toggleMusic);
            CreateSlider(audioSection.transform, "Music Volume", musicVolume, _setMusicVolume);
            CreateToggle(audioSection.transform, "SFX", sfxEnabled, _toggleSfx);
            CreateSlider(audioSection.transform, "SFX Volume", sfxVolume, _setSfxVolume);

            var gameplaySection = CreateSettingsSection(_settings.transform, "Gameplay", 96);
            CreateToggle(gameplaySection.transform, "Vibration", vibrationEnabled, _toggleVibration);

            var resetSection = CreateSettingsSection(_settings.transform, "Progress", 116, UiColorPalette.WithAlpha(UiColorPalette.Ruby, 0.14f));
            CreateButton(resetSection.transform, "Reset Progress", _resetProgress, UiColorPalette.Ruby, new Vector2(280, 52));
            CreateButton(_settings.transform, "Back", _mainMenuAction, UiColorPalette.Amethyst, new Vector2(240, 58));
            TransitionTo(_settings);
        }
    }
}
