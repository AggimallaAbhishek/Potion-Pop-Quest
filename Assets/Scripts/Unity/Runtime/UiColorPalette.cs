using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    /// <summary>
    /// Centralized color palette for the entire game UI.
    /// Adapted to match the HTML design template (pop.html).
    /// </summary>
    public static class UiColorPalette
    {
        // ── Background & Atmosphere ──────────────────────────────────

        public static readonly Color32 BackgroundTop = new Color32(46, 24, 117, 255); // #2E1875
        public static readonly Color32 BackgroundMid = new Color32(26, 16, 53, 255); // #1A1035
        public static readonly Color32 BackgroundBottom = new Color32(13, 8, 32, 255); // #0D0820
        public static readonly Color32 BackgroundSolid = new Color32(36, 21, 72, 255); // #241548
        
        public static readonly Color ModalBackdrop = new Color(0f, 0f, 0f, 0.65f); // rgba(0,0,0,0.65)
        public static readonly Color Vignette = new Color(0f, 0f, 0f, 0.50f);

        // ── Board & Panels ──────────────────────────────────────────

        public static readonly Color BoardBackground = new Color(0.141f, 0.082f, 0.282f, 0.92f); // --bg-panel
        public static readonly Color HudBackground = new Color(0.216f, 0.125f, 0.431f, 0.85f); // --bg-card
        public static readonly Color SubtleBorder = new Color(0.557f, 0.424f, 1.000f, 0.40f); // --border-glow
        public static readonly Color PanelGlow = new Color(1f, 0.80f, 0.95f, 0.30f);
        public static readonly Color BoardGridLine = new Color(1f, 1f, 1f, 0.06f);

        // ── Tile 3D Depth Layers ─────────────────────────────────────

        public static readonly Color TileBaseGradientTop = new Color(1f, 1f, 1f, 0.25f);
        public static readonly Color TileBaseGradientBottom = new Color(0f, 0f, 0f, 0.25f);
        public static readonly Color TileSpecularHighlight = new Color(1f, 1f, 1f, 0.45f);
        public static readonly Color TileInnerShadow = new Color(0f, 0f, 0f, 0.35f);
        public static readonly Color TileTopHighlight = new Color(1f, 1f, 1f, 0.35f); // rgba(255,255,255,0.35)

        // ── Ingredient Tile Colors (Primary / Light / Dark) ─────────

        public static readonly Color32 RedHerb = new Color32(255, 94, 122, 255); // --tile-red
        public static readonly Color32 RedHerbLight = new Color32(255, 122, 145, 255); // #FF7A91
        public static readonly Color32 RedHerbDark = new Color32(255, 23, 68, 255); // #FF1744

        public static readonly Color32 BlueCrystal = new Color32(77, 166, 255, 255); // --tile-blue
        public static readonly Color32 BlueCrystalLight = new Color32(126, 194, 255, 255); // #7EC2FF
        public static readonly Color32 BlueCrystalDark = new Color32(42, 102, 186, 255);

        public static readonly Color32 GreenLeaf = new Color32(74, 222, 128, 255); // --tile-green
        public static readonly Color32 GreenLeafLight = new Color32(122, 238, 156, 255); // #7AEE9C
        public static readonly Color32 GreenLeafDark = new Color32(34, 197, 94, 255); // --success-dark

        public static readonly Color32 YellowStarDust = new Color32(255, 203, 43, 255); // --tile-yellow
        public static readonly Color32 YellowStarDustLight = new Color32(255, 224, 102, 255); // #FFE066
        public static readonly Color32 YellowStarDustDark = new Color32(232, 168, 0, 255);

        public static readonly Color32 PurpleMushroom = new Color32(180, 73, 255, 255); // --tile-purple
        public static readonly Color32 PurpleMushroomLight = new Color32(208, 123, 255, 255); // #D07BFF
        public static readonly Color32 PurpleMushroomDark = new Color32(142, 108, 255, 255);

        public static readonly Color32 OrangeFireDrop = new Color32(255, 140, 66, 255); // --tile-orange
        public static readonly Color32 OrangeFireDropLight = new Color32(255, 180, 122, 255); // #FFB47A
        public static readonly Color32 OrangeFireDropDark = new Color32(251, 146, 60, 255); // --warning

        public static readonly Color32 EmptyTile = new Color32(42, 22, 96, 255); // #2A1660

        // ── Obstacle Colors ─────────────────────────────────────────

        public static readonly Color32 WoodenBox = new Color32(123, 92, 53, 255); // #7B5C35
        public static readonly Color32 WoodenBoxLight = new Color32(160, 132, 92, 255); // #A0845C
        public static readonly Color32 StoneBlock = new Color32(128, 140, 153, 255);
        public static readonly Color32 StoneBlockLight = new Color32(178, 191, 204, 255);
        public static readonly Color32 DarkTile = new Color32(37, 13, 74, 255); // #250D4A
        public static readonly Color32 DarkTileLight = new Color32(61, 31, 110, 255); // #3D1F6E

        // ── Potion Colors ───────────────────────────────────────────

        public static readonly Color32 PotionLine = new Color32(255, 23, 68, 255); // #FF1744
        public static readonly Color32 PotionBomb = new Color32(255, 140, 0, 255); // #FF8C00
        public static readonly Color32 PotionLightning = new Color32(155, 50, 255, 255); // #9B32FF
        public static readonly Color32 PotionMega = new Color32(255, 201, 74, 255); // --gold

        // ── UI Accent Colors ────────────────────────────────────────

        public static readonly Color32 Gold = new Color32(255, 201, 74, 255);
        public static readonly Color32 GoldLight = new Color32(255, 216, 107, 255);
        public static readonly Color32 GoldDark = new Color32(232, 168, 0, 255);

        public static readonly Color32 Emerald = new Color32(74, 222, 128, 255); // --success
        public static readonly Color32 EmeraldLight = new Color32(106, 238, 150, 255); // #6AEE96
        public static readonly Color32 EmeraldDark = new Color32(34, 197, 94, 255); // --success-dark

        public static readonly Color32 Ruby = new Color32(239, 68, 68, 255); // --danger
        public static readonly Color32 RubyLight = new Color32(248, 113, 113, 255); // #F87171

        public static readonly Color32 Sapphire = new Color32(77, 159, 255, 255); // --blue-1
        public static readonly Color32 SapphireLight = new Color32(108, 196, 255, 255); // --blue-2

        public static readonly Color32 Amethyst = new Color32(106, 76, 255, 255); // --purple-1
        public static readonly Color32 AmethystLight = new Color32(142, 108, 255, 255); // --purple-2

        // ── Text Colors ─────────────────────────────────────────────

        public static readonly Color32 TextPrimary = new Color32(255, 255, 255, 255);
        public static readonly Color32 TextSecondary = new Color32(201, 184, 255, 255); // --text-soft
        public static readonly Color32 TextMuted = new Color32(139, 120, 197, 255); // --text-dim
        public static readonly Color32 TextDanger = new Color32(239, 68, 68, 255);
        public static readonly Color32 TextSuccess = new Color32(74, 222, 128, 255);

        // ── Selection & Hint ────────────────────────────────────────

        public static readonly Color SelectionGlow = new Color(1f, 1f, 1f, 0.5f);
        public static readonly Color SelectionGlowDim = new Color(1f, 1f, 1f, 0.3f);
        public static readonly Color HintGlow = new Color(1f, 1f, 1f, 0.6f);
        public static readonly Color HintGlowDim = new Color(1f, 1f, 1f, 0.3f);

        // ── Clear & Match Feedback ──────────────────────────────────

        public static readonly Color ClearFlash = new Color(1f, 1f, 1f, 0.94f);
        public static readonly Color ClearGlow = new Color(1f, 0.96f, 0.52f);
        public static readonly Color ObstacleDamageFlash = new Color(1f, 0.56f, 0.26f);
        public static readonly Color ObstacleDestroyFlash = new Color(0.80f, 0.52f, 1f);
        public static readonly Color ScreenFlash = new Color(1f, 1f, 1f, 0.25f);

        // ── Star Progress ───────────────────────────────────────────

        public static readonly Color StarBarFill = new Color(1f, 0.85f, 0.15f, 1f);
        public static readonly Color StarBarBackground = new Color(0f, 0f, 0f, 0.40f); // rgba(0,0,0,0.4)
        public static readonly Color StarEarned = new Color(1f, 0.85f, 0.15f);
        public static readonly Color StarEmpty = new Color(1f, 1f, 1f, 0.3f);

        // ── Backdrop Elements ───────────────────────────────────────

        public static readonly Color LabBackWall = new Color(0.16f, 0.08f, 0.37f, 1f); // #2A1660
        public static readonly Color LabShelf = new Color(0.10f, 0.06f, 0.20f, 1f); // #1A1035
        public static readonly Color LabTable = new Color(0.05f, 0.03f, 0.12f, 1f); // #0D0820

        // ── Ambient Light Rays ──────────────────────────────────────

        public static readonly Color LightRay = new Color(1f, 0.85f, 0.95f, 0.05f);
        public static readonly Color LightRayWarm = new Color(1f, 0.95f, 0.65f, 0.05f);

        // ── Particle Glow Colors ────────────────────────────────────

        public static readonly Color[] ParticleGlow = new[]
        {
            new Color(1f, 0.50f, 0.90f, 0.65f),    // pink
            new Color(0.40f, 0.85f, 1f, 0.60f),     // cyan
            new Color(1f, 0.85f, 0.30f, 0.60f),     // gold
            new Color(0.50f, 1f, 0.70f, 0.55f),     // mint
            new Color(0.80f, 0.40f, 1f, 0.58f),     // violet
        };

        // ── Tutorial ────────────────────────────────────────────────

        public static readonly Color TutorialBackground = new Color(0.141f, 0.082f, 0.282f, 0.92f);
        public static readonly Color TutorialBorder = new Color(0.557f, 0.424f, 1.000f, 0.40f);

        // ── Level Select ────────────────────────────────────────────

        public static readonly Color LevelCardUnlocked = new Color(0.29f, 0.87f, 0.50f); // #4ADE80
        public static readonly Color LevelCardUnlockedGradient = new Color(0.13f, 0.77f, 0.36f); // #22C55E
        public static readonly Color LevelCardLocked = new Color(0.16f, 0.12f, 0.36f); // #2A1F5C
        public static readonly Color LevelCardBorder = new Color(1f, 1f, 1f, 0.1f);
        public static readonly Color LevelCardCurrentBorder = new Color(0.70f, 0.61f, 1f, 1f); // #B49EFF
        public static readonly Color LevelGridBackground = new Color(0f, 0f, 0f, 0f);

        // ── Confetti Colors ─────────────────────────────────────────

        public static readonly Color[] Confetti = new[]
        {
            new Color(1f, 0.32f, 0.40f),
            new Color(0.32f, 0.84f, 1f),
            new Color(1f, 0.84f, 0.22f),
            new Color(0.48f, 1f, 0.60f),
            new Color(0.84f, 0.48f, 1f),
            new Color(1f, 0.60f, 0.24f),
            new Color(0.40f, 0.72f, 1f),
            new Color(1f, 0.50f, 0.70f),
        };

        // ── Glassmorphism Modal ─────────────────────────────────────

        public static readonly Color GlassBackground = new Color(0.16f, 0.08f, 0.37f, 1f);
        public static readonly Color GlassBorder = new Color(0.55f, 0.42f, 1.00f, 0.40f);
        public static readonly Color GlassInnerGlow = new Color(1f, 1f, 1f, 0.05f);

        // ── Helper Methods ──────────────────────────────────────────

        public static Color IngredientColor(IngredientType ingredient)
        {
            switch (ingredient)
            {
                case IngredientType.RedHerb: return RedHerb;
                case IngredientType.BlueCrystal: return BlueCrystal;
                case IngredientType.GreenLeaf: return GreenLeaf;
                case IngredientType.YellowStarDust: return YellowStarDust;
                case IngredientType.PurpleMushroom: return PurpleMushroom;
                case IngredientType.OrangeFireDrop: return OrangeFireDrop;
                default: return EmptyTile;
            }
        }

        public static Color IngredientColorLight(IngredientType ingredient)
        {
            switch (ingredient)
            {
                case IngredientType.RedHerb: return RedHerbLight;
                case IngredientType.BlueCrystal: return BlueCrystalLight;
                case IngredientType.GreenLeaf: return GreenLeafLight;
                case IngredientType.YellowStarDust: return YellowStarDustLight;
                case IngredientType.PurpleMushroom: return PurpleMushroomLight;
                case IngredientType.OrangeFireDrop: return OrangeFireDropLight;
                default: return EmptyTile;
            }
        }

        public static Color IngredientColorDark(IngredientType ingredient)
        {
            switch (ingredient)
            {
                case IngredientType.RedHerb: return RedHerbDark;
                case IngredientType.BlueCrystal: return BlueCrystalDark;
                case IngredientType.GreenLeaf: return GreenLeafDark;
                case IngredientType.YellowStarDust: return YellowStarDustDark;
                case IngredientType.PurpleMushroom: return PurpleMushroomDark;
                case IngredientType.OrangeFireDrop: return OrangeFireDropDark;
                default: return EmptyTile;
            }
        }

        public static Color PotionColor(PotionType potion)
        {
            switch (potion)
            {
                case PotionType.Bomb: return PotionBomb;
                case PotionType.Lightning: return PotionLightning;
                case PotionType.Mega: return PotionMega;
                default: return PotionLine;
            }
        }

        public static Color CellColor(BoardCellSnapshot cell)
        {
            if (cell.Obstacle == ObstacleType.WoodenBox) return WoodenBox;
            if (cell.Obstacle == ObstacleType.StoneBlock) return StoneBlock;
            if (cell.Obstacle == ObstacleType.DarkTile) return DarkTile;
            return IngredientColor(cell.Ingredient);
        }

        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static Color Brighten(Color color, float factor)
        {
            return new Color(
                Mathf.Clamp01(color.r + factor),
                Mathf.Clamp01(color.g + factor),
                Mathf.Clamp01(color.b + factor),
                color.a);
        }

        public static Color Darken(Color color, float factor)
        {
            return new Color(
                Mathf.Clamp01(color.r - factor),
                Mathf.Clamp01(color.g - factor),
                Mathf.Clamp01(color.b - factor),
                color.a);
        }
    }
}
