using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed partial class TileIconFactory
    {
        // ── Ingredient Drawing (Glossy Candy Style) ─────────────────

        private static void DrawIngredient(Texture2D texture, IngredientType ingredient)
        {
            var primary = UiColorPalette.IngredientColor(ingredient);
            var light = UiColorPalette.IngredientColorLight(ingredient);
            var dark = UiColorPalette.IngredientColorDark(ingredient);
            var shadow = new Color(0f, 0f, 0f, 0.28f);
            const int cx = 128;
            const int cy = 128;
            const int shadowOffset = 5;

            // All ingredients share the same glossy candy base with unique shapes on top
            switch (ingredient)
            {
                case IngredientType.RedHerb:
                    // Drop shadow
                    DrawEllipseAA(texture, cx + shadowOffset, cy - 10 + shadowOffset, 50, 58, shadow);
                    // Radial body — main candy sphere
                    DrawRadialGradientCircle(texture, cx, cy - 6, 52, primary, dark);
                    // Inner leaf shapes
                    DrawEllipseAA(texture, cx - 18, cy + 8, 30, 18, new Color(primary.r + 0.10f, primary.g, primary.b, 1f));
                    DrawEllipseAA(texture, cx + 18, cy + 8, 30, 18, primary);
                    DrawEllipseAA(texture, cx, cy - 16, 24, 36, light);
                    // Stem
                    DrawLine(texture, cx, cy + 50, cx, cy + 74, new Color(0.30f, 0.14f, 0.10f, 1f), 8);
                    // Accessibility Shape: Triangle
                    DrawTriangle(texture, cx, cy - 6, 14, false, new Color(1f, 1f, 1f, 0.40f));
                    // Specular highlight
                    DrawSpecularHighlight(texture, cx - 14, cy - 28, 16, 10);
                    break;

                case IngredientType.BlueCrystal:
                    DrawDiamond(texture, cx + shadowOffset, cy + shadowOffset, 48, 68, shadow);
                    // Multi-facet crystal
                    DrawDiamond(texture, cx, cy, 48, 68, new Color(0.18f, 0.52f, 0.96f, 1f));
                    DrawDiamond(texture, cx, cy - 8, 28, 44, new Color(0.46f, 0.76f, 1f, 0.80f));
                    DrawDiamond(texture, cx - 6, cy - 14, 12, 22, new Color(0.78f, 0.94f, 1f, 0.70f));
                    // Facet line
                    DrawLine(texture, cx, cy - 68, cx, cy + 68, new Color(0.08f, 0.34f, 0.78f, 0.50f), 4);
                    DrawLine(texture, cx - 48, cy, cx + 48, cy, new Color(0.08f, 0.34f, 0.78f, 0.30f), 3);
                    // Accessibility Shape: Square
                    DrawRect(texture, cx - 10, cy - 10, 20, 20, new Color(1f, 1f, 1f, 0.30f));
                    DrawSpecularHighlight(texture, cx - 16, cy - 34, 14, 8);
                    break;

                case IngredientType.GreenLeaf:
                    DrawEllipseAA(texture, cx + shadowOffset, cy + 4 + shadowOffset, 48, 32, shadow);
                    // Main leaf body with radial gradient
                    DrawRadialGradientEllipse(texture, cx, cy + 4, 48, 32, primary, dark);
                    // Leaf veins
                    DrawLine(texture, cx - 36, cy + 34, cx + 36, cy - 22, new Color(0.08f, 0.42f, 0.16f, 0.80f), 6);
                    DrawLine(texture, cx - 14, cy + 14, cx - 26, cy - 2, new Color(0.12f, 0.50f, 0.20f, 0.60f), 4);
                    DrawLine(texture, cx + 10, cy + 2, cx + 24, cy - 10, new Color(0.12f, 0.50f, 0.20f, 0.60f), 4);
                    // Stem
                    DrawLine(texture, cx + 32, cy - 18, cx + 46, cy - 34, new Color(0.20f, 0.36f, 0.12f, 1f), 7);
                    // Dewdrop
                    DrawCircleAA(texture, cx - 12, cy + 14, 8, new Color(0.80f, 1f, 0.90f, 0.60f));
                    DrawCircleAA(texture, cx - 14, cy + 12, 4, new Color(1f, 1f, 1f, 0.50f));
                    // Accessibility Shape: Circle
                    DrawCircleAA(texture, cx, cy, 14, new Color(1f, 1f, 1f, 0.40f));
                    DrawCircleAA(texture, cx, cy, 10, primary); // hollow center
                    DrawSpecularHighlight(texture, cx - 16, cy - 10, 18, 10);
                    break;

                case IngredientType.YellowStarDust:
                    DrawStar(texture, cx + shadowOffset, cy + shadowOffset, 58, 26, shadow);
                    // Star body with gradient
                    DrawStar(texture, cx, cy, 58, 26, new Color(1f, 0.84f, 0.20f, 1f));
                    DrawStar(texture, cx, cy + 4, 36, 16, new Color(1f, 0.94f, 0.52f, 0.80f));
                    // Sparkle dots
                    DrawSparkle(texture, cx - 42, cy + 46, 10, new Color(1f, 0.96f, 0.60f, 0.70f));
                    DrawSparkle(texture, cx + 44, cy + 42, 8, new Color(1f, 0.96f, 0.60f, 0.65f));
                    DrawSparkle(texture, cx + 38, cy - 44, 9, new Color(1f, 0.96f, 0.60f, 0.60f));
                    DrawSparkle(texture, cx - 36, cy - 38, 7, new Color(1f, 0.96f, 0.60f, 0.55f));
                    // Accessibility Shape: Inner Star
                    DrawStar(texture, cx, cy, 18, 8, new Color(1f, 1f, 1f, 0.40f));
                    DrawSpecularHighlight(texture, cx - 12, cy - 24, 14, 8);
                    break;

                case IngredientType.PurpleMushroom:
                    DrawEllipseAA(texture, cx + shadowOffset, cy - 12 + shadowOffset, 48, 28, shadow);
                    // Mushroom cap
                    DrawRadialGradientEllipse(texture, cx, cy - 12, 48, 28, primary, dark);
                    // Cap spots
                    DrawCircleAA(texture, cx - 16, cy - 18, 10, new Color(0.92f, 0.82f, 1f, 0.75f));
                    DrawCircleAA(texture, cx + 18, cy - 10, 7, new Color(0.92f, 0.82f, 1f, 0.65f));
                    DrawCircleAA(texture, cx - 4, cy - 28, 6, new Color(0.92f, 0.82f, 1f, 0.55f));
                    // Stem
                    DrawRectRounded(texture, cx - 16, cy + 12, 32, 48, new Color(0.92f, 0.82f, 0.70f, 1f), 8);
                    DrawRectRounded(texture, cx - 10, cy + 16, 14, 38, new Color(0.96f, 0.90f, 0.82f, 0.50f), 6);
                    // Accessibility Shape: Cross
                    DrawLine(texture, cx - 12, cy - 12, cx + 12, cy - 12, new Color(1f, 1f, 1f, 0.40f), 4);
                    DrawLine(texture, cx, cy - 24, cx, cy, new Color(1f, 1f, 1f, 0.40f), 4);
                    DrawSpecularHighlight(texture, cx - 18, cy - 26, 16, 10);
                    break;

                case IngredientType.OrangeFireDrop:
                    DrawFlame(texture, primary, light);
                    // Accessibility Shape: Inner Diamond
                    DrawDiamond(texture, cx, 150, 14, 22, new Color(1f, 1f, 1f, 0.35f));
                    DrawSpecularHighlight(texture, cx - 10, cy - 24, 12, 8);
                    break;
            }
        }
    }
}
