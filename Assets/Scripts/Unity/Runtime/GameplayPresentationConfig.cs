namespace PotionPopQuest.Unity
{
    public static class GameplayPresentationConfig
    {
        // ── Core gameplay animation timing ──
        public const float SwapDuration = 0.18f;
        public const float InvalidShakeDuration = 0.22f;
        public const float ClearPopDuration = 0.18f;
        public const float DropDuration = 0.20f;
        public const float DropDurationPerRow = 0.025f;
        public const float DropMaxDuration = 0.30f;
        public const float SpawnDropDuration = 0.20f;
        public const float SpawnScaleDuration = 0.14f;
        public const float CascadeDelay = 0.12f;
        public const float PotionAnticipationDuration = 0.10f;
        public const float PotionBurstDuration = 0.24f;
        public const float BombPotionTotalDuration = 0.46f;
        public const float LightningPotionTotalDuration = 0.40f;
        public const float BeamDuration = 0.20f;
        public const float BoardPulseDuration = 0.24f;
        public const float ScoreCountDuration = 0.36f;
        public const float AutoHintDelay = 5f;

        // ── Enhanced animation parameters ──
        public const float TileBounceOvershoot = 0.15f;
        public const float ClearStaggerDelay = 0.03f;
        public const float SparkLifetime = 0.28f;
        public const int SparkCount = 6;
        public const int LargeCascadeSparkCount = 3;
        public const int MaxSparkBurstTiles = 12;
        public const float SparkSpeed = 200f;
        public const int MaxActiveVfxImages = 96;

        // ── Squash-and-stretch for tile drops ──
        public const float SquashStretchDuration = 0.10f;
        public const float SquashScaleX = 1.12f;
        public const float SquashScaleY = 0.88f;
        public const float StretchScaleX = 0.94f;
        public const float StretchScaleY = 1.06f;

        // ── Particle burst on clear ──
        public const float ParticleBurstDuration = 0.32f;
        public const int ParticleBurstCount = 8;
        public const float ParticleBurstSpeed = 240f;
        public const float ParticleBurstSize = 8f;

        // ── Shimmer sweep ──
        public const float ShimmerSweepDuration = 2.2f;
        public const float ShimmerSweepWidth = 0.15f;
        public const float ShimmerAlpha = 0.22f;

        // ── Screen flash & shockwave ──
        public const float ScreenFlashDuration = 0.08f;
        public const float ShockwaveExpandDuration = 0.28f;
        public const float ShockwaveMaxSize = 600f;

        // ── Ambient particles ──
        public const float AmbientParticleSpeed = 28f;
        public const float AmbientParticleMinSize = 3f;
        public const float AmbientParticleMaxSize = 8f;
        public const int AmbientParticleCount = 18;
        public const float AmbientParticleDrift = 12f;

        // ── Win/Lose celebration ──
        public const float WinCelebrationDuration = 0.9f;
        public const float StarRevealDelay = 0.32f;
        public const float StarRevealDuration = 0.44f;
        public const float ConfettiDuration = 2.6f;
        public const int ConfettiCount = 32;
        public const float WinScoreCountDuration = 1.1f;

        // ── Screen transitions ──
        public const float ScreenTransitionDuration = 0.32f;
        public const float ModalRevealDuration = 0.26f;

        // ── Micro-interactions ──
        public const float SelectionPulsePeriod = 0.6f;
        public const float HintPulsePeriod = 0.8f;
        public const float ButtonBounceBackDuration = 0.12f;
        public const float LowMovesPulseThreshold = 3;
        public const float LowMovesPulsePeriod = 0.8f;
        public const float TutorialBannerDuration = 5f;
        public const float TutorialSlideInDuration = 0.28f;
        public const float LevelIntroFadeDuration = 0.18f;
        public const float LevelIntroRevealDuration = 0.26f;
        public const float LevelIntroDismissDuration = 0.20f;

        // ── Title animation ──
        public const float TitlePulsePeriod = 2.4f;
        public const float TitlePulseScale = 1.03f;
        public const float TitleGlowPulsePeriod = 1.8f;

        // ── Floating feedback ──
        public const float FloatingScoreDuration = 0.78f;
        public const float FloatingScorePunchScale = 1.35f;
        public const float CascadeTextDuration = 0.55f;

        // ── Audio defaults ──
        public const float DefaultMusicVolume = 0.55f;
        public const float DefaultSfxVolume = 0.85f;
        public const float RepeatedCueCooldown = 0.06f;
    }
}
