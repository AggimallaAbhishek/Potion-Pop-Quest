namespace PotionPopQuest.Unity
{
    public static class GameplayPresentationConfig
    {
        // ── Core gameplay animation timing ──
        public const float SwapDuration = 0.18f;
        public const float InvalidShakeDuration = 0.20f;
        public const float ClearPopDuration = 0.16f;
        public const float DropDuration = 0.18f;
        public const float SpawnDropDuration = 0.16f;
        public const float SpawnScaleDuration = 0.12f;
        public const float CascadeDelay = 0.10f;
        public const float PotionBurstDuration = 0.28f;
        public const float BeamDuration = 0.18f;
        public const float BoardPulseDuration = 0.22f;
        public const float ScoreCountDuration = 0.32f;
        public const float AutoHintDelay = 5f;

        // ── Enhanced animation parameters ──
        public const float TileBounceOvershoot = 0.15f;
        public const float ClearStaggerDelay = 0.03f;
        public const float SparkLifetime = 0.28f;
        public const int SparkCount = 4;
        public const float SparkSpeed = 180f;

        // ── Win/Lose celebration ──
        public const float WinCelebrationDuration = 0.8f;
        public const float StarRevealDelay = 0.30f;
        public const float StarRevealDuration = 0.40f;
        public const float ConfettiDuration = 2.2f;
        public const int ConfettiCount = 24;
        public const float WinScoreCountDuration = 1.0f;

        // ── Screen transitions ──
        public const float ScreenTransitionDuration = 0.25f;
        public const float ModalRevealDuration = 0.22f;

        // ── Micro-interactions ──
        public const float SelectionPulsePeriod = 0.6f;
        public const float HintPulsePeriod = 0.8f;
        public const float ButtonBounceBackDuration = 0.12f;
        public const float LowMovesPulseThreshold = 3;
        public const float LowMovesPulsePeriod = 0.8f;
        public const float TutorialBannerDuration = 5f;
        public const float TutorialSlideInDuration = 0.28f;

        // ── Floating feedback ──
        public const float FloatingScoreDuration = 0.72f;
        public const float FloatingScorePunchScale = 1.3f;
        public const float CascadeTextDuration = 0.50f;
    }
}
