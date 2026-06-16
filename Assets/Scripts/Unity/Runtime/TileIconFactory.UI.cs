using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed partial class TileIconFactory
    {
        // ── Potion Drawing ──────────────────────────────────────────

        private static void DrawPotion(Texture2D texture, PotionType potion)
        {
            const int cx = 128;
            const int cy = 128;
            var shadow = new Color(0f, 0f, 0f, 0.25f);

            var color = potion == PotionType.Bomb
                ? new Color(1f, 0.50f, 0.18f, 1f)
                : potion == PotionType.Mega
                    ? new Color(1f, 0.86f, 0.28f, 1f)
                    : new Color(0.52f, 0.88f, 1f, 1f);

            // Flask body shadow
            DrawEllipseAA(texture, cx + 4, cy + 12 + 4, 38, 42, shadow);
            // Flask body
            DrawRadialGradientCircle(texture, cx, cy + 12, 38, color, UiColorPalette.Darken(color, 0.20f));
            // Flask neck
            DrawRectRounded(texture, cx - 12, cy - 42, 24, 34, new Color(0.78f, 0.88f, 0.96f, 0.90f), 6);
            // Cork/cap
            DrawRectRounded(texture, cx - 16, cy - 48, 32, 12, new Color(0.62f, 0.44f, 0.26f, 1f), 5);
            // Liquid highlight
            DrawEllipseAA(texture, cx - 8, cy + 4, 14, 18, UiColorPalette.WithAlpha(UiColorPalette.Brighten(color, 0.20f), 0.50f));

            // Potion-specific markings
            if (potion == PotionType.LineHorizontal)
            {
                DrawLine(texture, cx - 40, cy + 12, cx + 40, cy + 12, Color.white, 7);
                DrawTriangle(texture, cx - 48, cy + 12, 10, true, Color.white);
                DrawTriangle(texture, cx + 48, cy + 12, 10, false, Color.white);
            }
            else if (potion == PotionType.LineVertical)
            {
                DrawLine(texture, cx, cy - 30, cx, cy + 54, Color.white, 7);
            }
            else if (potion == PotionType.Lightning)
            {
                DrawLine(texture, cx + 6, cy - 22, cx - 14, cy + 10, Color.white, 7);
                DrawLine(texture, cx - 14, cy + 10, cx + 10, cy + 6, Color.white, 7);
                DrawLine(texture, cx + 10, cy + 6, cx - 8, cy + 42, Color.white, 7);
            }
            else if (potion == PotionType.Mega)
            {
                DrawStar(texture, cx, cy + 12, 28, 12, Color.white);
            }

            // Glass specular
            DrawSpecularHighlight(texture, cx - 14, cy - 4, 10, 6);
        }

        // ── Star Icon ───────────────────────────────────────────────

        private static void DrawStarIcon(Texture2D texture, bool earned)
        {
            const int cx = 128;
            const int cy = 120;
            var shadow = new Color(0f, 0f, 0f, earned ? 0.32f : 0.18f);

            // Shadow
            DrawStar(texture, cx + 5, cy - 5, 56, 24, shadow);

            if (earned)
            {
                // Main star body
                DrawStar(texture, cx, cy, 56, 24, UiColorPalette.StarEarned);
                // Inner bright star
                DrawStar(texture, cx, cy + 6, 34, 14, UiColorPalette.GoldLight);
                // Sparkle accents
                DrawSparkle(texture, cx - 48, cy + 36, 8, new Color(1f, 0.96f, 0.60f, 0.60f));
                DrawSparkle(texture, cx + 50, cy + 30, 7, new Color(1f, 0.96f, 0.60f, 0.50f));
                DrawSparkle(texture, cx, cy - 52, 6, new Color(1f, 0.96f, 0.60f, 0.55f));
                // Specular
                DrawSpecularHighlight(texture, cx - 10, cy + 14, 12, 6);
            }
            else
            {
                DrawStar(texture, cx, cy, 56, 24, UiColorPalette.StarEmpty);
                // Hollow center
                DrawStar(texture, cx, cy, 38, 16, new Color(0.14f, 0.16f, 0.22f, 0.70f));
            }
        }
    }
}
