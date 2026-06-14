using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    /// <summary>
    /// Centralized color palette for the entire game UI.
    /// All colors are defined here to ensure visual consistency.
    /// </summary>
    public static class UiColorPalette
    {
        // ── Background & Atmosphere ──────────────────────────────────

        /// <summary>Top color of the screen background gradient (deep purple-midnight).</summary>
        public static readonly Color BackgroundTop = new Color(0.06f, 0.04f, 0.14f);

        /// <summary>Bottom color of the screen background gradient (dark teal-navy).</summary>
        public static readonly Color BackgroundBottom = new Color(0.04f, 0.10f, 0.16f);

        /// <summary>Solid fallback background.</summary>
        public static readonly Color BackgroundSolid = new Color(0.05f, 0.07f, 0.13f);

        /// <summary>Modal backdrop overlay.</summary>
        public static readonly Color ModalBackdrop = new Color(0f, 0f, 0f, 0.60f);

        // ── Board & Panels ──────────────────────────────────────────

        /// <summary>Board panel background.</summary>
        public static readonly Color BoardBackground = new Color(0.10f, 0.11f, 0.16f, 0.95f);

        /// <summary>HUD bar background.</summary>
        public static readonly Color HudBackground = new Color(0.08f, 0.09f, 0.14f, 0.94f);

        /// <summary>Subtle border/separator line.</summary>
        public static readonly Color SubtleBorder = new Color(0.25f, 0.28f, 0.38f, 0.50f);

        /// <summary>Panel inner glow.</summary>
        public static readonly Color PanelGlow = new Color(0.30f, 0.35f, 0.55f, 0.12f);

        // ── Ingredient Tile Colors (Primary / Light / Dark) ─────────

        public static readonly Color RedHerb = new Color(0.82f, 0.18f, 0.22f);
        public static readonly Color RedHerbLight = new Color(1f, 0.42f, 0.38f);
        public static readonly Color RedHerbDark = new Color(0.52f, 0.10f, 0.12f);

        public static readonly Color BlueCrystal = new Color(0.16f, 0.44f, 0.82f);
        public static readonly Color BlueCrystalLight = new Color(0.42f, 0.68f, 1f);
        public static readonly Color BlueCrystalDark = new Color(0.08f, 0.24f, 0.52f);

        public static readonly Color GreenLeaf = new Color(0.18f, 0.64f, 0.32f);
        public static readonly Color GreenLeafLight = new Color(0.40f, 0.86f, 0.50f);
        public static readonly Color GreenLeafDark = new Color(0.08f, 0.36f, 0.16f);

        public static readonly Color YellowStarDust = new Color(0.92f, 0.72f, 0.16f);
        public static readonly Color YellowStarDustLight = new Color(1f, 0.88f, 0.42f);
        public static readonly Color YellowStarDustDark = new Color(0.60f, 0.44f, 0.08f);

        public static readonly Color PurpleMushroom = new Color(0.54f, 0.26f, 0.76f);
        public static readonly Color PurpleMushroomLight = new Color(0.72f, 0.50f, 0.94f);
        public static readonly Color PurpleMushroomDark = new Color(0.30f, 0.14f, 0.46f);

        public static readonly Color OrangeFireDrop = new Color(0.92f, 0.44f, 0.14f);
        public static readonly Color OrangeFireDropLight = new Color(1f, 0.64f, 0.32f);
        public static readonly Color OrangeFireDropDark = new Color(0.58f, 0.26f, 0.06f);

        public static readonly Color EmptyTile = new Color(0.14f, 0.16f, 0.22f);

        // ── Obstacle Colors ─────────────────────────────────────────

        public static readonly Color WoodenBox = new Color(0.56f, 0.36f, 0.18f);
        public static readonly Color WoodenBoxLight = new Color(0.72f, 0.52f, 0.30f);
        public static readonly Color StoneBlock = new Color(0.40f, 0.42f, 0.48f);
        public static readonly Color StoneBlockLight = new Color(0.56f, 0.58f, 0.64f);
        public static readonly Color DarkTile = new Color(0.18f, 0.10f, 0.28f);
        public static readonly Color DarkTileLight = new Color(0.32f, 0.22f, 0.44f);

        // ── Potion Colors ───────────────────────────────────────────

        public static readonly Color PotionLine = new Color(0.52f, 0.86f, 1f);
        public static readonly Color PotionBomb = new Color(1f, 0.52f, 0.18f);
        public static readonly Color PotionLightning = new Color(0.96f, 0.98f, 1f);
        public static readonly Color PotionMega = new Color(1f, 0.84f, 0.30f);

        // ── UI Accent Colors ────────────────────────────────────────

        /// <summary>Gold — used for scores, stars, and highlights.</summary>
        public static readonly Color Gold = new Color(1f, 0.82f, 0.24f);
        public static readonly Color GoldLight = new Color(1f, 0.92f, 0.52f);
        public static readonly Color GoldDark = new Color(0.72f, 0.56f, 0.10f);

        /// <summary>Emerald — used for positive actions (Play, Hint, Next).</summary>
        public static readonly Color Emerald = new Color(0.20f, 0.62f, 0.52f);
        public static readonly Color EmeraldLight = new Color(0.30f, 0.78f, 0.64f);
        public static readonly Color EmeraldDark = new Color(0.12f, 0.42f, 0.34f);

        /// <summary>Ruby — used for danger states (low moves, restart, exit).</summary>
        public static readonly Color Ruby = new Color(0.72f, 0.22f, 0.22f);
        public static readonly Color RubyLight = new Color(0.88f, 0.36f, 0.34f);

        /// <summary>Sapphire — used for info actions (Levels, Settings).</summary>
        public static readonly Color Sapphire = new Color(0.22f, 0.44f, 0.68f);
        public static readonly Color SapphireLight = new Color(0.34f, 0.58f, 0.84f);

        /// <summary>Amethyst — used for secondary actions (Menu, Back).</summary>
        public static readonly Color Amethyst = new Color(0.34f, 0.24f, 0.44f);
        public static readonly Color AmethystLight = new Color(0.48f, 0.36f, 0.58f);

        // ── Text Colors ─────────────────────────────────────────────

        public static readonly Color TextPrimary = new Color(0.96f, 0.97f, 1f);
        public static readonly Color TextSecondary = new Color(0.72f, 0.77f, 0.82f);
        public static readonly Color TextMuted = new Color(0.42f, 0.48f, 0.54f);
        public static readonly Color TextDanger = new Color(1f, 0.36f, 0.30f);
        public static readonly Color TextSuccess = new Color(0.40f, 0.90f, 0.56f);

        // ── Selection & Hint ────────────────────────────────────────

        public static readonly Color SelectionGlow = new Color(1f, 0.88f, 0.30f, 0.85f);
        public static readonly Color SelectionGlowDim = new Color(1f, 0.88f, 0.30f, 0.40f);
        public static readonly Color HintGlow = new Color(0.40f, 1f, 0.62f, 0.85f);
        public static readonly Color HintGlowDim = new Color(0.40f, 1f, 0.62f, 0.35f);

        // ── Clear & Match Feedback ──────────────────────────────────

        public static readonly Color ClearFlash = new Color(1f, 1f, 1f, 0.92f);
        public static readonly Color ClearGlow = new Color(1f, 0.95f, 0.55f);
        public static readonly Color ObstacleDamageFlash = new Color(1f, 0.58f, 0.28f);
        public static readonly Color ObstacleDestroyFlash = new Color(0.78f, 0.54f, 1f);

        // ── Star Progress ───────────────────────────────────────────

        public static readonly Color StarBarFill = new Color(1f, 0.76f, 0.22f, 0.95f);
        public static readonly Color StarBarBackground = new Color(0.06f, 0.07f, 0.10f, 0.92f);
        public static readonly Color StarEarned = new Color(1f, 0.82f, 0.24f);
        public static readonly Color StarEmpty = new Color(0.28f, 0.30f, 0.36f);

        // ── Backdrop Elements ───────────────────────────────────────

        public static readonly Color LabBackWall = new Color(0.06f, 0.08f, 0.12f, 0.72f);
        public static readonly Color LabShelf = new Color(0.22f, 0.16f, 0.20f, 0.46f);
        public static readonly Color LabTable = new Color(0.16f, 0.10f, 0.14f, 0.64f);

        // ── Tutorial ────────────────────────────────────────────────

        public static readonly Color TutorialBackground = new Color(0.18f, 0.12f, 0.28f, 0.94f);
        public static readonly Color TutorialBorder = new Color(0.48f, 0.36f, 0.72f, 0.60f);

        // ── Level Select ────────────────────────────────────────────

        public static readonly Color LevelCardUnlocked = new Color(0.18f, 0.42f, 0.54f);
        public static readonly Color LevelCardLocked = new Color(0.22f, 0.24f, 0.28f);
        public static readonly Color LevelCardBorder = new Color(0.32f, 0.56f, 0.68f, 0.45f);
        public static readonly Color LevelGridBackground = new Color(0.08f, 0.10f, 0.16f, 0.88f);

        // ── Confetti Colors ─────────────────────────────────────────

        public static readonly Color[] Confetti = new[]
        {
            new Color(1f, 0.36f, 0.42f),
            new Color(0.36f, 0.82f, 1f),
            new Color(1f, 0.82f, 0.24f),
            new Color(0.52f, 1f, 0.62f),
            new Color(0.82f, 0.52f, 1f),
            new Color(1f, 0.62f, 0.28f)
        };

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
    }
}
