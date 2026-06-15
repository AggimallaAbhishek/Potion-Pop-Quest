using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    /// <summary>
    /// Centralized color palette for the entire game UI.
    /// All colors are defined here to ensure visual consistency.
    /// Candy-crush inspired vibrant, glossy palette.
    /// </summary>
    public static class UiColorPalette
    {
        // ── Background & Atmosphere ──────────────────────────────────

        /// <summary>Top color of the screen background gradient (deep cosmic purple).</summary>
        public static readonly Color BackgroundTop = new Color(0.10f, 0.06f, 0.20f);

        /// <summary>Bottom color of the screen background gradient (rich midnight amethyst).</summary>
        public static readonly Color BackgroundBottom = new Color(0.28f, 0.12f, 0.48f);

        /// <summary>Solid fallback background.</summary>
        public static readonly Color BackgroundSolid = new Color(0.18f, 0.09f, 0.30f);

        /// <summary>Modal backdrop overlay.</summary>
        public static readonly Color ModalBackdrop = new Color(0.04f, 0.02f, 0.08f, 0.88f);

        /// <summary>Vignette edge color for atmospheric depth.</summary>
        public static readonly Color Vignette = new Color(0.02f, 0.01f, 0.06f, 0.72f);

        // ── Board & Panels ──────────────────────────────────────────

        /// <summary>Board panel background.</summary>
        public static readonly Color BoardBackground = new Color(0.07f, 0.08f, 0.14f, 0.96f);

        /// <summary>HUD bar background.</summary>
        public static readonly Color HudBackground = new Color(0.06f, 0.07f, 0.12f, 0.95f);

        /// <summary>Subtle border/separator line.</summary>
        public static readonly Color SubtleBorder = new Color(0.30f, 0.34f, 0.50f, 0.40f);

        /// <summary>Panel inner glow.</summary>
        public static readonly Color PanelGlow = new Color(0.35f, 0.40f, 0.65f, 0.10f);

        /// <summary>Board grid line between tiles.</summary>
        public static readonly Color BoardGridLine = new Color(0.12f, 0.10f, 0.20f, 0.60f);

        // ── Tile 3D Depth Layers ─────────────────────────────────────

        /// <summary>Top edge of a tile gradient (lighter).</summary>
        public static readonly Color TileBaseGradientTop = new Color(1f, 1f, 1f, 0.14f);

        /// <summary>Bottom edge of a tile gradient (darker).</summary>
        public static readonly Color TileBaseGradientBottom = new Color(0f, 0f, 0f, 0.18f);

        /// <summary>Specular highlight on tiles (glossy candy shine).</summary>
        public static readonly Color TileSpecularHighlight = new Color(1f, 1f, 1f, 0.36f);

        /// <summary>Inner shadow at bottom-right of tiles.</summary>
        public static readonly Color TileInnerShadow = new Color(0f, 0f, 0f, 0.26f);

        /// <summary>Top highlight band on tiles.</summary>
        public static readonly Color TileTopHighlight = new Color(1f, 1f, 1f, 0.20f);

        // ── Ingredient Tile Colors (Primary / Light / Dark) ─────────
        // Shifted toward more vibrant, candy-like hues

        public static readonly Color RedHerb = new Color(0.92f, 0.14f, 0.26f);
        public static readonly Color RedHerbLight = new Color(1f, 0.48f, 0.52f);
        public static readonly Color RedHerbDark = new Color(0.58f, 0.08f, 0.14f);

        public static readonly Color BlueCrystal = new Color(0.12f, 0.42f, 0.92f);
        public static readonly Color BlueCrystalLight = new Color(0.46f, 0.72f, 1f);
        public static readonly Color BlueCrystalDark = new Color(0.06f, 0.22f, 0.58f);

        public static readonly Color GreenLeaf = new Color(0.14f, 0.72f, 0.34f);
        public static readonly Color GreenLeafLight = new Color(0.42f, 0.92f, 0.54f);
        public static readonly Color GreenLeafDark = new Color(0.06f, 0.40f, 0.16f);

        public static readonly Color YellowStarDust = new Color(1f, 0.78f, 0.12f);
        public static readonly Color YellowStarDustLight = new Color(1f, 0.92f, 0.48f);
        public static readonly Color YellowStarDustDark = new Color(0.68f, 0.48f, 0.06f);

        public static readonly Color PurpleMushroom = new Color(0.62f, 0.22f, 0.86f);
        public static readonly Color PurpleMushroomLight = new Color(0.78f, 0.50f, 1f);
        public static readonly Color PurpleMushroomDark = new Color(0.34f, 0.12f, 0.52f);

        public static readonly Color OrangeFireDrop = new Color(1f, 0.48f, 0.08f);
        public static readonly Color OrangeFireDropLight = new Color(1f, 0.68f, 0.30f);
        public static readonly Color OrangeFireDropDark = new Color(0.66f, 0.28f, 0.04f);

        public static readonly Color EmptyTile = new Color(0.12f, 0.14f, 0.20f);

        // ── Obstacle Colors ─────────────────────────────────────────

        public static readonly Color WoodenBox = new Color(0.58f, 0.36f, 0.16f);
        public static readonly Color WoodenBoxLight = new Color(0.76f, 0.54f, 0.30f);
        public static readonly Color StoneBlock = new Color(0.42f, 0.44f, 0.50f);
        public static readonly Color StoneBlockLight = new Color(0.58f, 0.60f, 0.66f);
        public static readonly Color DarkTile = new Color(0.16f, 0.08f, 0.26f);
        public static readonly Color DarkTileLight = new Color(0.34f, 0.22f, 0.48f);

        // ── Potion Colors ───────────────────────────────────────────

        public static readonly Color PotionLine = new Color(0.48f, 0.88f, 1f);
        public static readonly Color PotionBomb = new Color(1f, 0.52f, 0.14f);
        public static readonly Color PotionLightning = new Color(0.94f, 0.96f, 1f);
        public static readonly Color PotionMega = new Color(1f, 0.86f, 0.26f);

        // ── UI Accent Colors ────────────────────────────────────────

        /// <summary>Gold — used for scores, stars, and highlights.</summary>
        public static readonly Color Gold = new Color(1f, 0.82f, 0.36f);
        public static readonly Color GoldLight = new Color(1f, 0.95f, 0.65f);
        public static readonly Color GoldDark = new Color(0.82f, 0.56f, 0.08f);

        /// <summary>Emerald — used for positive actions (Play, Hint, Next).</summary>
        public static readonly Color Emerald = new Color(0.02f, 0.84f, 0.58f);
        public static readonly Color EmeraldLight = new Color(0.46f, 1f, 0.82f);
        public static readonly Color EmeraldDark = new Color(0.01f, 0.48f, 0.36f);

        /// <summary>Ruby — used for danger states (low moves, restart, exit).</summary>
        public static readonly Color Ruby = new Color(0.96f, 0.24f, 0.40f);
        public static readonly Color RubyLight = new Color(1f, 0.66f, 0.74f);

        /// <summary>Sapphire — used for info actions (Levels, Settings).</summary>
        public static readonly Color Sapphire = new Color(0.06f, 0.52f, 0.72f);
        public static readonly Color SapphireLight = new Color(0.26f, 0.78f, 0.92f);

        /// <summary>Amethyst — used for secondary actions (Menu, Back).</summary>
        public static readonly Color Amethyst = new Color(0.38f, 0.16f, 0.54f);
        public static readonly Color AmethystLight = new Color(0.58f, 0.36f, 0.76f);

        // ── Text Colors ─────────────────────────────────────────────

        public static readonly Color TextPrimary = new Color(0.97f, 0.98f, 1f);
        public static readonly Color TextSecondary = new Color(0.74f, 0.78f, 0.84f);
        public static readonly Color TextMuted = new Color(0.44f, 0.50f, 0.56f);
        public static readonly Color TextDanger = new Color(1f, 0.36f, 0.30f);
        public static readonly Color TextSuccess = new Color(0.38f, 0.92f, 0.54f);

        // ── Selection & Hint ────────────────────────────────────────

        public static readonly Color SelectionGlow = new Color(1f, 0.90f, 0.28f, 0.90f);
        public static readonly Color SelectionGlowDim = new Color(1f, 0.90f, 0.28f, 0.45f);
        public static readonly Color HintGlow = new Color(0.38f, 1f, 0.60f, 0.88f);
        public static readonly Color HintGlowDim = new Color(0.38f, 1f, 0.60f, 0.38f);

        // ── Clear & Match Feedback ──────────────────────────────────

        public static readonly Color ClearFlash = new Color(1f, 1f, 1f, 0.94f);
        public static readonly Color ClearGlow = new Color(1f, 0.96f, 0.52f);
        public static readonly Color ObstacleDamageFlash = new Color(1f, 0.56f, 0.26f);
        public static readonly Color ObstacleDestroyFlash = new Color(0.80f, 0.52f, 1f);
        public static readonly Color ScreenFlash = new Color(1f, 1f, 1f, 0.18f);

        // ── Star Progress ───────────────────────────────────────────

        public static readonly Color StarBarFill = new Color(1f, 0.78f, 0.20f, 0.96f);
        public static readonly Color StarBarBackground = new Color(0.05f, 0.06f, 0.09f, 0.94f);
        public static readonly Color StarEarned = new Color(1f, 0.84f, 0.22f);
        public static readonly Color StarEmpty = new Color(0.26f, 0.28f, 0.34f);

        // ── Backdrop Elements ───────────────────────────────────────

        public static readonly Color LabBackWall = new Color(0.04f, 0.06f, 0.10f, 0.76f);
        public static readonly Color LabShelf = new Color(0.20f, 0.14f, 0.18f, 0.50f);
        public static readonly Color LabTable = new Color(0.14f, 0.08f, 0.12f, 0.68f);

        // ── Ambient Light Rays ──────────────────────────────────────

        public static readonly Color LightRay = new Color(0.80f, 0.70f, 1f, 0.06f);
        public static readonly Color LightRayWarm = new Color(1f, 0.85f, 0.55f, 0.05f);

        // ── Particle Glow Colors ────────────────────────────────────

        public static readonly Color[] ParticleGlow = new[]
        {
            new Color(0.70f, 0.50f, 1f, 0.55f),    // violet
            new Color(0.40f, 0.80f, 1f, 0.50f),     // cyan
            new Color(1f, 0.80f, 0.30f, 0.50f),     // gold
            new Color(0.50f, 1f, 0.70f, 0.45f),     // mint
            new Color(1f, 0.50f, 0.70f, 0.48f),     // pink
        };

        // ── Tutorial ────────────────────────────────────────────────

        public static readonly Color TutorialBackground = new Color(0.16f, 0.10f, 0.26f, 0.95f);
        public static readonly Color TutorialBorder = new Color(0.50f, 0.38f, 0.74f, 0.55f);

        // ── Level Select ────────────────────────────────────────────

        public static readonly Color LevelCardUnlocked = new Color(0.14f, 0.38f, 0.56f);
        public static readonly Color LevelCardUnlockedGradient = new Color(0.10f, 0.30f, 0.50f);
        public static readonly Color LevelCardLocked = new Color(0.20f, 0.22f, 0.26f);
        public static readonly Color LevelCardBorder = new Color(0.36f, 0.60f, 0.74f, 0.50f);
        public static readonly Color LevelCardCurrentBorder = new Color(1f, 0.82f, 0.30f, 0.80f);
        public static readonly Color LevelGridBackground = new Color(0.06f, 0.08f, 0.14f, 0.90f);

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

        public static readonly Color GlassBackground = new Color(0.08f, 0.06f, 0.16f, 0.92f);
        public static readonly Color GlassBorder = new Color(0.50f, 0.45f, 0.70f, 0.30f);
        public static readonly Color GlassInnerGlow = new Color(0.40f, 0.35f, 0.65f, 0.08f);

        // ── Helper Methods ──────────────────────────────────────────

        /// <summary>Returns the primary tile color for an ingredient.</summary>
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

        /// <summary>Returns the light variant of an ingredient color (for glow/highlight).</summary>
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

        /// <summary>Returns the dark variant of an ingredient color (for shadows).</summary>
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

        /// <summary>Returns the color for a potion type.</summary>
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

        /// <summary>Returns the cell background color for a board cell.</summary>
        public static Color CellColor(BoardCellSnapshot cell)
        {
            if (cell.Obstacle == ObstacleType.WoodenBox) return WoodenBox;
            if (cell.Obstacle == ObstacleType.StoneBlock) return StoneBlock;
            if (cell.Obstacle == ObstacleType.DarkTile) return DarkTile;
            return IngredientColor(cell.Ingredient);
        }

        /// <summary>Lerps between two colors with alpha blending.</summary>
        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>Brightens a color by a factor.</summary>
        public static Color Brighten(Color color, float factor)
        {
            return new Color(
                Mathf.Clamp01(color.r + factor),
                Mathf.Clamp01(color.g + factor),
                Mathf.Clamp01(color.b + factor),
                color.a);
        }

        /// <summary>Darkens a color by a factor.</summary>
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
