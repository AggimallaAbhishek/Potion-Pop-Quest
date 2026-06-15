using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class TileIconFactory
    {
        private const int Size = 256;
        private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public Sprite GetIngredientSprite(IngredientType ingredient)
        {
            return GetOrCreate($"ingredient-{ingredient}", texture => DrawIngredient(texture, ingredient));
        }

        public Sprite GetObstacleSprite(ObstacleType obstacle)
        {
            return GetOrCreate($"obstacle-{obstacle}", texture => DrawObstacle(texture, obstacle));
        }

        public Sprite GetPotionSprite(PotionType potion)
        {
            return GetOrCreate($"potion-{potion}", texture => DrawPotion(texture, potion));
        }

        public Sprite GetRoundedRectSprite(int radius)
        {
            var scaledRadius = radius * 2;
            return GetOrCreate($"roundedrect-{radius}", texture => DrawRoundedRect(texture, scaledRadius), new Vector4(scaledRadius, scaledRadius, scaledRadius, scaledRadius));
        }

        public Sprite GetPillSprite()
        {
            return GetOrCreate("pill", texture => DrawRoundedRect(texture, Size / 2), new Vector4(Size / 2, Size / 2, Size / 2, Size / 2));
        }

        public Sprite GetStarSprite(bool earned)
        {
            return GetOrCreate(earned ? "star-earned" : "star-empty", texture => DrawStarIcon(texture, earned));
        }

        public Sprite GetBackgroundGradientSprite(Color top, Color bottom)
        {
            var key = $"gradient-{ColorUtility.ToHtmlStringRGBA(top)}-{ColorUtility.ToHtmlStringRGBA(bottom)}";
            return GetOrCreate(key, texture => DrawVerticalGradient(texture, top, bottom));
        }

        private Sprite GetOrCreate(string key, Action<Texture2D> draw, Vector4 border = default)
        {
            if (_cache.TryGetValue(key, out var sprite))
            {
                return sprite;
            }

            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp
            };
            Clear(texture);
            draw(texture);
            texture.Apply();
            var pivot = new Vector2(0.5f, 0.5f);
            sprite = border == default
                ? Sprite.Create(texture, new Rect(0, 0, Size, Size), pivot, Size)
                : Sprite.Create(texture, new Rect(0, 0, Size, Size), pivot, Size, 0, SpriteMeshType.FullRect, border);
            sprite.name = key;
            _cache[key] = sprite;
            return sprite;
        }

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
                    DrawSpecularHighlight(texture, cx - 18, cy - 26, 16, 10);
                    break;

                case IngredientType.OrangeFireDrop:
                    DrawFlame(texture, primary, light);
                    DrawSpecularHighlight(texture, cx - 10, cy - 24, 12, 8);
                    break;
            }
        }

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

        // ── Core Drawing Primitives ─────────────────────────────────

        private static void DrawVerticalGradient(Texture2D texture, Color top, Color bottom)
        {
            for (var y = 0; y < Size; y++)
            {
                var t = y / (float)(Size - 1);
                var color = Color.Lerp(bottom, top, t);
                for (var x = 0; x < Size; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        private static void Clear(Texture2D texture)
        {
            var pixels = new Color[Size * Size];
            texture.SetPixels(pixels);
        }

        private static void SetPixel(Texture2D texture, int x, int y, Color color)
        {
            if (x >= 0 && x < Size && y >= 0 && y < Size)
            {
                if (color.a < 1f)
                {
                    var existing = texture.GetPixel(x, y);
                    texture.SetPixel(x, y, Color.Lerp(existing, color, color.a));
                }
                else
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        private static void DrawCircle(Texture2D texture, int cx, int cy, int radius, Color color)
        {
            DrawEllipse(texture, cx, cy, radius, radius, color);
        }

        private static void DrawCircleAA(Texture2D texture, int cx, int cy, int radius, Color color)
        {
            DrawEllipseAA(texture, cx, cy, radius, radius, color);
        }

        private static void DrawEllipse(Texture2D texture, int cx, int cy, int rx, int ry, Color color)
        {
            for (var y = cy - ry; y <= cy + ry; y++)
            {
                for (var x = cx - rx; x <= cx + rx; x++)
                {
                    var dx = (x - cx) / (float)rx;
                    var dy = (y - cy) / (float)ry;
                    if (dx * dx + dy * dy <= 1f)
                    {
                        SetPixel(texture, x, y, color);
                    }
                }
            }
        }

        private static void DrawEllipseAA(Texture2D texture, int cx, int cy, int rx, int ry, Color color)
        {
            var margin = 2;
            for (var y = cy - ry - margin; y <= cy + ry + margin; y++)
            {
                for (var x = cx - rx - margin; x <= cx + rx + margin; x++)
                {
                    var dx = (x - cx) / (float)rx;
                    var dy = (y - cy) / (float)ry;
                    var dist = dx * dx + dy * dy;
                    if (dist <= 1f)
                    {
                        SetPixel(texture, x, y, color);
                    }
                    else if (dist < 1.10f)
                    {
                        var edgeAlpha = 1f - (dist - 1f) / 0.10f;
                        var edgeColor = new Color(color.r, color.g, color.b, color.a * Mathf.Clamp01(edgeAlpha));
                        SetPixel(texture, x, y, edgeColor);
                    }
                }
            }
        }

        /// <summary>Draws a circle with a radial gradient from center color to edge color.</summary>
        private static void DrawRadialGradientCircle(Texture2D texture, int cx, int cy, int radius, Color center, Color edge)
        {
            var margin = 2;
            for (var y = cy - radius - margin; y <= cy + radius + margin; y++)
            {
                for (var x = cx - radius - margin; x <= cx + radius + margin; x++)
                {
                    var dx = (x - cx) / (float)radius;
                    var dy = (y - cy) / (float)radius;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist <= 1f)
                    {
                        var color = Color.Lerp(center, edge, dist * dist);
                        SetPixel(texture, x, y, color);
                    }
                    else if (dist < 1.06f)
                    {
                        var edgeAlpha = 1f - (dist - 1f) / 0.06f;
                        var color = new Color(edge.r, edge.g, edge.b, edge.a * Mathf.Clamp01(edgeAlpha));
                        SetPixel(texture, x, y, color);
                    }
                }
            }
        }

        /// <summary>Draws an ellipse with a radial gradient from center color to edge color.</summary>
        private static void DrawRadialGradientEllipse(Texture2D texture, int cx, int cy, int rx, int ry, Color center, Color edge)
        {
            var margin = 2;
            for (var y = cy - ry - margin; y <= cy + ry + margin; y++)
            {
                for (var x = cx - rx - margin; x <= cx + rx + margin; x++)
                {
                    var dx = (x - cx) / (float)rx;
                    var dy = (y - cy) / (float)ry;
                    var dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist <= 1f)
                    {
                        var color = Color.Lerp(center, edge, dist * dist);
                        SetPixel(texture, x, y, color);
                    }
                    else if (dist < 1.08f)
                    {
                        var edgeAlpha = 1f - (dist - 1f) / 0.08f;
                        var color = new Color(edge.r, edge.g, edge.b, edge.a * Mathf.Clamp01(edgeAlpha));
                        SetPixel(texture, x, y, color);
                    }
                }
            }
        }

        /// <summary>Draws a candy-like specular highlight (white ellipse).</summary>
        private static void DrawSpecularHighlight(Texture2D texture, int cx, int cy, int rx, int ry)
        {
            // Outer soft glow
            DrawEllipseAA(texture, cx, cy, rx + 2, ry + 2, new Color(1f, 1f, 1f, 0.12f));
            // Inner bright highlight
            DrawEllipseAA(texture, cx, cy, rx, ry, new Color(1f, 1f, 1f, 0.45f));
            // Core bright dot
            DrawEllipseAA(texture, cx, cy - 1, rx / 2, ry / 2, new Color(1f, 1f, 1f, 0.60f));
        }

        /// <summary>Draws a small 4-point sparkle burst.</summary>
        private static void DrawSparkle(Texture2D texture, int cx, int cy, int size, Color color)
        {
            // Glow circle
            DrawCircleAA(texture, cx, cy, size, UiColorPalette.WithAlpha(color, color.a * 0.40f));
            // Cross sparkle
            DrawLine(texture, cx - size, cy, cx + size, cy, color, 2);
            DrawLine(texture, cx, cy - size, cx, cy + size, color, 2);
            // Core bright dot
            DrawCircleAA(texture, cx, cy, size / 3 + 1, UiColorPalette.WithAlpha(color, Mathf.Min(1f, color.a + 0.30f)));
        }

        private static void DrawDiamond(Texture2D texture, int cx, int cy, int rx, int ry, Color color)
        {
            for (var y = cy - ry; y <= cy + ry; y++)
            {
                for (var x = cx - rx; x <= cx + rx; x++)
                {
                    if (Mathf.Abs(x - cx) / (float)rx + Mathf.Abs(y - cy) / (float)ry <= 1f)
                    {
                        SetPixel(texture, x, y, color);
                    }
                }
            }
        }

        private static void DrawRect(Texture2D texture, int x, int y, int width, int height, Color color)
        {
            for (var py = y; py < y + height; py++)
            {
                for (var px = x; px < x + width; px++)
                {
                    SetPixel(texture, px, py, color);
                }
            }
        }

        /// <summary>Draws a rectangle with rounded corners.</summary>
        private static void DrawRectRounded(Texture2D texture, int x, int y, int width, int height, Color color, int radius)
        {
            for (var py = y; py < y + height; py++)
            {
                for (var px = x; px < x + width; px++)
                {
                    var dx = 0;
                    var dy = 0;
                    if (px < x + radius) dx = x + radius - px;
                    else if (px >= x + width - radius) dx = px - (x + width - radius - 1);
                    if (py < y + radius) dy = y + radius - py;
                    else if (py >= y + height - radius) dy = py - (y + height - radius - 1);

                    if (dx > 0 && dy > 0)
                    {
                        if (Mathf.Sqrt(dx * dx + dy * dy) <= radius)
                        {
                            SetPixel(texture, px, py, color);
                        }
                    }
                    else
                    {
                        SetPixel(texture, px, py, color);
                    }
                }
            }
        }

        private static void DrawRoundedRect(Texture2D texture, int radius)
        {
            for (var y = 0; y < Size; y++)
            {
                var pixelColor = Color.white;
                // Top highlight for 3D button depth
                if (y > Size - 10)
                {
                    pixelColor = new Color(1.15f, 1.15f, 1.15f, 1f);
                }
                else if (y > Size - 20)
                {
                    var gradT = (y - (Size - 20)) / 10f;
                    pixelColor = Color.Lerp(Color.white, new Color(1.15f, 1.15f, 1.15f, 1f), gradT);
                }
                // Bottom shadow
                else if (y < 10)
                {
                    pixelColor = new Color(0.72f, 0.72f, 0.72f, 1f);
                }
                else if (y < 20)
                {
                    var gradT = (y - 10) / 10f;
                    pixelColor = Color.Lerp(new Color(0.72f, 0.72f, 0.72f, 1f), Color.white, gradT);
                }

                for (var x = 0; x < Size; x++)
                {
                    var dx = 0;
                    var dy = 0;
                    if (x < radius) dx = radius - x;
                    else if (x >= Size - radius) dx = x - (Size - radius - 1);

                    if (y < radius) dy = radius - y;
                    else if (y >= Size - radius) dy = y - (Size - radius - 1);

                    if (dx > 0 && dy > 0)
                    {
                        var dist = Mathf.Sqrt(dx * dx + dy * dy);
                        if (dist <= radius)
                        {
                            SetPixel(texture, x, y, pixelColor);
                        }
                        else if (dist < radius + 1.5f)
                        {
                            var alpha = 1f - (dist - radius) / 1.5f;
                            SetPixel(texture, x, y, new Color(pixelColor.r, pixelColor.g, pixelColor.b, Mathf.Clamp01(alpha)));
                        }
                    }
                    else
                    {
                        SetPixel(texture, x, y, pixelColor);
                    }
                }
            }
        }

        private static void DrawLine(Texture2D texture, int x0, int y0, int x1, int y1, Color color, int thickness)
        {
            var dx = Mathf.Abs(x1 - x0);
            var dy = -Mathf.Abs(y1 - y0);
            var sx = x0 < x1 ? 1 : -1;
            var sy = y0 < y1 ? 1 : -1;
            var error = dx + dy;

            while (true)
            {
                DrawCircle(texture, x0, y0, Mathf.Max(1, thickness / 2), color);
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                var e2 = 2 * error;
                if (e2 >= dy)
                {
                    error += dy;
                    x0 += sx;
                }

                if (e2 <= dx)
                {
                    error += dx;
                    y0 += sy;
                }
            }
        }

        private static void DrawStar(Texture2D texture, int cx, int cy, int outerRadius, int innerRadius, Color color)
        {
            for (var y = cy - outerRadius; y <= cy + outerRadius; y++)
            {
                for (var x = cx - outerRadius; x <= cx + outerRadius; x++)
                {
                    var dx = x - cx;
                    var dy = y - cy;
                    var distance = Mathf.Sqrt(dx * dx + dy * dy);
                    var angle = Mathf.Atan2(dy, dx);
                    var spike = Mathf.Abs(Mathf.Cos(angle * 5f));
                    var limit = Mathf.Lerp(innerRadius, outerRadius, spike);
                    if (distance <= limit)
                    {
                        SetPixel(texture, x, y, color);
                    }
                }
            }
        }

        /// <summary>Draws a simple directional triangle (arrow tip).</summary>
        private static void DrawTriangle(Texture2D texture, int cx, int cy, int size, bool pointLeft, Color color)
        {
            var dir = pointLeft ? -1 : 1;
            for (var dy = -size; dy <= size; dy++)
            {
                var width = size - Mathf.Abs(dy);
                for (var dx = 0; dx < width; dx++)
                {
                    SetPixel(texture, cx + dx * dir, cy + dy, color);
                }
            }
        }

        private static void DrawFlame(Texture2D texture, Color outer, Color inner)
        {
            const int cx = 128;
            // Shadow pass
            for (var y = 42; y <= 224; y++)
            {
                for (var x = 52; x <= 212; x++)
                {
                    var nx = (x - 4 - cx) / 54f;
                    var ny = (y - 4 - 96) / 100f;
                    var width = Mathf.Lerp(0.22f, 0.92f, Mathf.Clamp01((y - 4 - 38) / 182f));
                    if (Mathf.Abs(nx) < width * (1f - Mathf.Clamp01(ny) * 0.32f) && ny > -0.12f)
                    {
                        SetPixel(texture, x, y, new Color(0f, 0f, 0f, 0.22f));
                    }
                }
            }

            // Main flame
            for (var y = 38; y <= 220; y++)
            {
                for (var x = 48; x <= 208; x++)
                {
                    var nx = (x - cx) / 54f;
                    var ny = (y - 96) / 100f;
                    var width = Mathf.Lerp(0.22f, 0.92f, Mathf.Clamp01((y - 38) / 182f));
                    if (Mathf.Abs(nx) < width * (1f - Mathf.Clamp01(ny) * 0.32f) && ny > -0.12f)
                    {
                        SetPixel(texture, x, y, outer);
                    }
                }
            }

            // Inner core
            DrawEllipseAA(texture, cx, 146, 22, 42, inner);
            // Hot center
            DrawEllipseAA(texture, cx, 150, 10, 22, new Color(1f, 0.96f, 0.80f, 0.50f));
        }
    }
}
