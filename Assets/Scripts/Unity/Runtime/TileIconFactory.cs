using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class TileIconFactory
    {
        private const int Size = 128;
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
            return GetOrCreate($"roundedrect-{radius}", texture => DrawRoundedRect(texture, radius), new Vector4(radius, radius, radius, radius));
        }

        public Sprite GetPillSprite()
        {
            return GetOrCreate("pill", texture => DrawRoundedRect(texture, Size / 2), new Vector4(Size / 2, Size / 2, Size / 2, Size / 2));
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

        private static void DrawIngredient(Texture2D texture, IngredientType ingredient)
        {
            var shadow = new Color(0f, 0f, 0f, 0.30f);
            const int shadowOffset = 3;
            switch (ingredient)
            {
                case IngredientType.RedHerb:
                    // Shadow pass
                    DrawEllipse(texture, 64 + shadowOffset, 37 + shadowOffset, 20, 29, shadow);
                    // Main shapes (scaled from 96 to 128)
                    DrawLine(texture, 64, 96, 64, 37, new Color(0.35f, 0.12f, 0.10f, 1f), 7);
                    DrawEllipseAA(texture, 48, 60, 27, 16, new Color(0.96f, 0.28f, 0.30f, 1f));
                    DrawEllipseAA(texture, 80, 60, 27, 16, new Color(0.86f, 0.16f, 0.18f, 1f));
                    DrawEllipseAA(texture, 64, 37, 20, 29, new Color(1f, 0.36f, 0.32f, 1f));
                    break;
                case IngredientType.BlueCrystal:
                    DrawDiamond(texture, 64 + shadowOffset, 64 + shadowOffset, 37, 53, shadow);
                    DrawDiamond(texture, 64, 64, 37, 53, new Color(0.28f, 0.70f, 1f, 1f));
                    DrawDiamond(texture, 64, 57, 17, 32, new Color(0.74f, 0.93f, 1f, 0.85f));
                    DrawLine(texture, 64, 12, 64, 116, new Color(0.10f, 0.36f, 0.72f, 1f), 3);
                    break;
                case IngredientType.GreenLeaf:
                    DrawEllipseAA(texture, 64 + shadowOffset, 60 + shadowOffset, 40, 27, shadow);
                    DrawEllipseAA(texture, 64, 60, 40, 27, new Color(0.26f, 0.78f, 0.36f, 1f));
                    DrawLine(texture, 32, 87, 96, 37, new Color(0.11f, 0.36f, 0.16f, 1f), 5);
                    DrawLine(texture, 56, 67, 45, 52, new Color(0.16f, 0.48f, 0.20f, 1f), 3);
                    DrawLine(texture, 69, 57, 84, 48, new Color(0.16f, 0.48f, 0.20f, 1f), 3);
                    break;
                case IngredientType.YellowStarDust:
                    DrawStar(texture, 64 + shadowOffset, 56 + shadowOffset, 45, 20, shadow);
                    DrawStar(texture, 64, 56, 45, 20, new Color(1f, 0.82f, 0.24f, 1f));
                    // Sparkle dots with glow
                    DrawCircleAA(texture, 32, 96, 8, new Color(1f, 0.95f, 0.50f, 0.5f));
                    DrawCircleAA(texture, 32, 96, 5, new Color(1f, 0.95f, 0.50f, 0.9f));
                    DrawCircleAA(texture, 93, 93, 7, new Color(1f, 0.95f, 0.50f, 0.5f));
                    DrawCircleAA(texture, 93, 93, 4, new Color(1f, 0.95f, 0.50f, 0.9f));
                    DrawCircleAA(texture, 100, 27, 7, new Color(1f, 0.95f, 0.50f, 0.5f));
                    DrawCircleAA(texture, 100, 27, 4, new Color(1f, 0.95f, 0.50f, 0.9f));
                    break;
                case IngredientType.PurpleMushroom:
                    DrawEllipseAA(texture, 64 + shadowOffset, 48 + shadowOffset, 40, 24, shadow);
                    DrawEllipseAA(texture, 64, 48, 40, 24, new Color(0.68f, 0.36f, 0.92f, 1f));
                    DrawRect(texture, 49, 53, 30, 45, new Color(0.94f, 0.80f, 0.66f, 1f));
                    DrawCircleAA(texture, 48, 40, 7, new Color(0.96f, 0.86f, 1f, 1f));
                    DrawCircleAA(texture, 73, 36, 5, new Color(0.96f, 0.86f, 1f, 1f));
                    break;
                case IngredientType.OrangeFireDrop:
                    DrawFlame(texture, new Color(1f, 0.42f, 0.14f, 1f), new Color(1f, 0.86f, 0.26f, 1f));
                    break;
            }
        }

        private static void DrawObstacle(Texture2D texture, ObstacleType obstacle)
        {
            switch (obstacle)
            {
                case ObstacleType.WoodenBox:
                    DrawRect(texture, 14, 16, 68, 64, new Color(0.58f, 0.34f, 0.16f, 1f));
                    DrawLine(texture, 16, 28, 80, 28, new Color(0.30f, 0.16f, 0.08f, 1f), 4);
                    DrawLine(texture, 16, 50, 80, 50, new Color(0.30f, 0.16f, 0.08f, 1f), 4);
                    DrawLine(texture, 30, 18, 30, 78, new Color(0.30f, 0.16f, 0.08f, 1f), 4);
                    DrawLine(texture, 66, 18, 66, 78, new Color(0.30f, 0.16f, 0.08f, 1f), 4);
                    break;
                case ObstacleType.StoneBlock:
                    DrawRect(texture, 16, 18, 64, 60, new Color(0.52f, 0.55f, 0.58f, 1f));
                    DrawLine(texture, 16, 39, 80, 39, new Color(0.28f, 0.30f, 0.34f, 1f), 3);
                    DrawLine(texture, 44, 18, 44, 39, new Color(0.28f, 0.30f, 0.34f, 1f), 3);
                    DrawLine(texture, 58, 39, 58, 78, new Color(0.28f, 0.30f, 0.34f, 1f), 3);
                    break;
                case ObstacleType.DarkTile:
                    DrawCircle(texture, 48, 48, 34, new Color(0.18f, 0.08f, 0.28f, 0.88f));
                    DrawStar(texture, 48, 48, 24, 9, new Color(0.56f, 0.36f, 0.82f, 0.75f));
                    break;
            }
        }

        private static void DrawPotion(Texture2D texture, PotionType potion)
        {
            var color = potion == PotionType.Bomb
                ? new Color(0.98f, 0.46f, 0.22f, 1f)
                : new Color(0.68f, 0.90f, 1f, 1f);
            DrawCircle(texture, 48, 54, 24, color);
            DrawRect(texture, 38, 18, 20, 18, new Color(0.82f, 0.92f, 1f, 1f));

            if (potion == PotionType.LineHorizontal)
            {
                DrawLine(texture, 22, 54, 74, 54, Color.white, 6);
            }
            else if (potion == PotionType.LineVertical)
            {
                DrawLine(texture, 48, 27, 48, 80, Color.white, 6);
            }
            else if (potion == PotionType.Lightning)
            {
                DrawLine(texture, 53, 24, 36, 54, Color.white, 5);
                DrawLine(texture, 36, 54, 57, 50, Color.white, 5);
                DrawLine(texture, 57, 50, 42, 78, Color.white, 5);
            }
            else if (potion == PotionType.Mega)
            {
                DrawStar(texture, 48, 54, 25, 10, Color.white);
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

        /// <summary>Anti-aliased circle with 1px edge blend.</summary>
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

        /// <summary>Anti-aliased ellipse with smooth edge blending.</summary>
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
                    else if (dist < 1.12f)
                    {
                        var edgeAlpha = 1f - (dist - 1f) / 0.12f;
                        var edgeColor = new Color(color.r, color.g, color.b, color.a * Mathf.Clamp01(edgeAlpha));
                        SetPixel(texture, x, y, edgeColor);
                    }
                }
            }
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

        private static void DrawRoundedRect(Texture2D texture, int radius)
        {
            for (var y = 0; y < Size; y++)
            {
                var pixelColor = Color.white;
                if (y > Size - 8) pixelColor = new Color(1.2f, 1.2f, 1.2f, 1f); // Top highlight
                else if (y < 12) pixelColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Bottom shadow
                
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

        private static void DrawFlame(Texture2D texture, Color outer, Color inner)
        {
            // Shadow pass
            for (var y = 21; y <= 112; y++)
            {
                for (var x = 26; x <= 106; x++)
                {
                    var nx = (x - 2 - 64) / 40f;
                    var ny = (y - 2 - 48) / 64f;
                    var width = Mathf.Lerp(0.24f, 0.95f, Mathf.Clamp01((y - 2 - 19) / 91f));
                    if (Mathf.Abs(nx) < width * (1f - Mathf.Clamp01(ny) * 0.32f) && ny > -0.12f)
                    {
                        SetPixel(texture, x, y, new Color(0f, 0f, 0f, 0.25f));
                    }
                }
            }

            // Main flame (scaled to 128)
            for (var y = 19; y <= 110; y++)
            {
                for (var x = 24; x <= 104; x++)
                {
                    var nx = (x - 64) / 40f;
                    var ny = (y - 48) / 64f;
                    var width = Mathf.Lerp(0.24f, 0.95f, Mathf.Clamp01((y - 19) / 91f));
                    if (Mathf.Abs(nx) < width * (1f - Mathf.Clamp01(ny) * 0.32f) && ny > -0.12f)
                    {
                        SetPixel(texture, x, y, outer);
                    }
                }
            }

            DrawEllipseAA(texture, 64, 73, 17, 29, inner);
        }
    }
}

