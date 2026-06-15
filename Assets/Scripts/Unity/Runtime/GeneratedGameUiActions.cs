using System;
using PotionPopQuest.Core;

namespace PotionPopQuest.Unity
{
    public sealed class GeneratedGameUiActions
    {
        public Action Play { get; set; }
        public Action ShowLevels { get; set; }
        public Action ShowSettings { get; set; }
        public Action Quit { get; set; }
        public Action<int> StartLevel { get; set; }
        public Action<GridPosition> TilePressed { get; set; }
        public Action HintRequested { get; set; }
        public Action Restart { get; set; }
        public Action NextLevel { get; set; }
        public Action MainMenu { get; set; }
        public Action ResetProgress { get; set; }
        public Action<bool> ToggleMusic { get; set; }
        public Action<bool> ToggleSfx { get; set; }
        public Action<float> SetMusicVolume { get; set; }
        public Action<float> SetSfxVolume { get; set; }
        public Action<bool> ToggleVibration { get; set; }
        public Action LevelIntroDismissed { get; set; }
        public Action<GameSfxCue> PlaySfx { get; set; }
        
        // Economy & Boosters
        public Action BuyLivesPressed { get; set; }
        public Action HammerBoosterPressed { get; set; }
        public Action ShuffleBoosterPressed { get; set; }
    }
}
