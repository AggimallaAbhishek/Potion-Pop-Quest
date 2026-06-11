using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class TileIconFactory
    {
        private const int Size = 96;
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

        private Sprite GetOrCreate(string key, Action<Texture2D> draw)
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
            sprite = Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), Size);
            sprite.name = key;
            _cache[key] = sprite;
            return sprite;
        }

        private static void DrawIngredient(Texture2D texture, IngredientType ingredient)
        {
            switch (ingredient)
            {
                case IngredientType.RedHerb:
                    DrawLine(texture, 48, 72, 48, 28, new Color(0.35f, 0.12f, 0.10f, 1f), 5);
                    DrawEllipse(texture, 36, 45, 20, 12, new Color(0.96f, 0.28f, 0.30f, 1f));
                    DrawEllipse(texture, 60, 45, 20, 12, new Color(0.86f, 0.16f, 0.18f, 1f));
                    DrawEllipse(texture, 48, 28, 15, 22, new Color(1f, 0.36f, 0.32f, 1f));
                    break;
                case IngredientType.BlueCrystal:
                    DrawDiamond(texture, 48, 48, 28, 40, new Color(0.28f, 0.70f, 1f, 1f));
                    DrawDiamond(texture, 48, 43, 13, 24, new Color(0.74f, 0.93f, 1f, 0.85f));
                    DrawLine(texture, 48, 9, 48, 87, new Color(0.10f, 0.36f, 0.72f, 1f), 2);
                    break;
                case IngredientType.GreenLeaf:
                    DrawEllipse(texture, 48, 45, 30, 20, new Color(0.26f, 0.78f, 0.36f, 1f));
                    DrawLine(texture, 24, 65, 72, 28, new Color(0.11f, 0.36f, 0.16f, 1f), 4);
                    DrawLine(texture, 42, 50, 34, 39, new Color(0.16f, 0.48f, 0.20f, 1f), 2);
                    DrawLine(texture, 52, 43, 63, 36, new Color(0.16f, 0.48f, 0.20f, 1f), 2);
                    break;
                case IngredientType.YellowStarDust:
                    DrawStar(texture, 48, 42, 34, 15, new Color(1f, 0.82f, 0.24f, 1f));
                    DrawCircle(texture, 24, 72, 5, new Color(1f, 0.95f, 0.50f, 0.9f));
                    DrawCircle(texture, 70, 70, 4, new Color(1f, 0.95f, 0.50f, 0.9f));
                    DrawCircle(texture, 75, 20, 4, new Color(1f, 0.95f, 0.50f, 0.9f));
                    break;
                case IngredientType.PurpleMushroom:
                    DrawEllipse(texture, 48, 36, 30, 18, new Color(0.68f, 0.36f, 0.92f, 1f));
                    DrawRect(texture, 37, 40, 22, 34, new Color(0.94f, 0.80f, 0.66f, 1f));
                    DrawCircle(texture, 36, 30, 5, new Color(0.96f, 0.86f, 1f, 1f));
                    DrawCircle(texture, 55, 27, 4, new Color(0.96f, 0.86f, 1f, 1f));
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
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }

        private static void SetPixel(Texture2D texture, int x, int y, Color color)
        {
            if (x >= 0 && x < Size && y >= 0 && y < Size)
            {
                texture.SetPixel(x, y, color);
            }
        }

        private static void DrawCircle(Texture2D texture, int cx, int cy, int radius, Color color)
        {
            DrawEllipse(texture, cx, cy, radius, radius, color);
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
            for (var y = 14; y <= 82; y++)
            {
                for (var x = 18; x <= 78; x++)
                {
                    var nx = (x - 48) / 30f;
                    var ny = (y - 36) / 48f;
                    var width = Mathf.Lerp(0.24f, 0.95f, Mathf.Clamp01((y - 14) / 68f));
                    if (Mathf.Abs(nx) < width * (1f - Mathf.Clamp01(ny) * 0.32f) && ny > -0.12f)
                    {
                        SetPixel(texture, x, y, outer);
                    }
                }
            }

            DrawEllipse(texture, 48, 55, 13, 22, inner);
        }
    }
}

