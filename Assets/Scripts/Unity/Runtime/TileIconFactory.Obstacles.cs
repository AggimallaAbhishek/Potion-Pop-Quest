using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed partial class TileIconFactory
    {
        // ── Obstacle Drawing ────────────────────────────────────────

        private static void DrawObstacle(Texture2D texture, ObstacleType obstacle)
        {
            const int cx = 128;
            const int cy = 128;

            switch (obstacle)
            {
                case ObstacleType.WoodenBox:
                    // Wooden plank with grain
                    DrawRectRounded(texture, 24, 28, 104, 96, new Color(0.62f, 0.38f, 0.18f, 1f), 12);
                    // Wood grain lines
                    for (var i = 0; i < 5; i++)
                    {
                        var y = 44 + i * 18;
                        DrawLine(texture, 32, y, 120, y, new Color(0.34f, 0.18f, 0.08f, 0.35f), 3);
                    }
                    // Plank dividers
                    DrawLine(texture, 76, 30, 76, 122, new Color(0.28f, 0.14f, 0.06f, 0.50f), 5);
                    // Nails
                    DrawCircleAA(texture, 42, 42, 5, new Color(0.46f, 0.44f, 0.40f, 0.80f));
                    DrawCircleAA(texture, 110, 42, 5, new Color(0.46f, 0.44f, 0.40f, 0.80f));
                    DrawCircleAA(texture, 42, 110, 5, new Color(0.46f, 0.44f, 0.40f, 0.80f));
                    DrawCircleAA(texture, 110, 110, 5, new Color(0.46f, 0.44f, 0.40f, 0.80f));
                    // Top highlight
                    DrawLine(texture, 30, 120, 122, 120, new Color(1f, 1f, 1f, 0.12f), 4);
                    break;

                case ObstacleType.StoneBlock:
                    DrawRectRounded(texture, 28, 32, 100, 92, new Color(0.54f, 0.56f, 0.60f, 1f), 10);
                    // Stone cracks
                    DrawLine(texture, 28, 76, 128, 76, new Color(0.26f, 0.28f, 0.32f, 0.70f), 4);
                    DrawLine(texture, 72, 34, 72, 76, new Color(0.26f, 0.28f, 0.32f, 0.55f), 4);
                    DrawLine(texture, 92, 76, 92, 122, new Color(0.26f, 0.28f, 0.32f, 0.55f), 4);
                    // Surface highlight
                    DrawLine(texture, 34, 118, 122, 118, new Color(1f, 1f, 1f, 0.14f), 4);
                    break;

                case ObstacleType.DarkTile:
                    // Swirling dark magic circle
                    DrawRadialGradientCircle(texture, cx, cy, 52, new Color(0.24f, 0.12f, 0.38f, 0.90f), new Color(0.10f, 0.04f, 0.18f, 0.90f));
                    DrawStar(texture, cx, cy, 32, 14, new Color(0.60f, 0.36f, 0.88f, 0.55f));
                    DrawCircleAA(texture, cx, cy, 20, new Color(0.44f, 0.22f, 0.66f, 0.35f));
                    // Magic sparkles
                    DrawSparkle(texture, cx - 28, cy - 24, 6, new Color(0.80f, 0.60f, 1f, 0.50f));
                    DrawSparkle(texture, cx + 30, cy + 20, 5, new Color(0.80f, 0.60f, 1f, 0.40f));
                    break;
            }
        }
    }
}
