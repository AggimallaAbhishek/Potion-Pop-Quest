using System;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed partial class TileIconFactory
    {
        private const int Size = 256;
        private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public Sprite GetIngredientSprite(IngredientType ingredient)
        {
            return GetImportedOrCreate(
                $"ingredient-{ingredient}",
                IngredientResourcePath(ingredient),
                texture => DrawIngredient(texture, ingredient));
        }

        public Sprite GetObstacleSprite(ObstacleType obstacle)
        {
            return GetImportedOrCreate(
                $"obstacle-{obstacle}",
                ObstacleResourcePath(obstacle),
                texture => DrawObstacle(texture, obstacle));
        }

        public Sprite GetPotionSprite(PotionType potion)
        {
            return GetImportedOrCreate(
                $"potion-{potion}",
                PotionResourcePath(potion),
                texture => DrawPotion(texture, potion));
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
            return GetImportedOrCreate(
                earned ? "star-earned" : "star-empty",
                earned ? "Sprites/UI/SPR_UI_Star_Earned" : "Sprites/UI/SPR_UI_Star_Empty",
                texture => DrawStarIcon(texture, earned));
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

        private Sprite GetImportedOrCreate(string key, string resourcePath, Action<Texture2D> draw, Vector4 border = default)
        {
            if (_cache.TryGetValue(key, out var sprite))
            {
                return sprite;
            }

            if (!string.IsNullOrEmpty(resourcePath))
            {
                var texture = Resources.Load<Texture2D>(resourcePath);
                if (texture != null)
                {
                    sprite = CreateSpriteFromTexture(key, texture, border);
                    _cache[key] = sprite;
                    return sprite;
                }
            }

            return GetOrCreate(key, draw, border);
        }

        private static Sprite CreateSpriteFromTexture(string key, Texture2D texture, Vector4 border = default)
        {
            var pivot = new Vector2(0.5f, 0.5f);
            var rect = new Rect(0, 0, texture.width, texture.height);
            var pixelsPerUnit = Mathf.Max(texture.width, texture.height);
            var sprite = border == default
                ? Sprite.Create(texture, rect, pivot, pixelsPerUnit)
                : Sprite.Create(texture, rect, pivot, pixelsPerUnit, 0, SpriteMeshType.FullRect, border);
            sprite.name = key;
            return sprite;
        }

        private static string IngredientResourcePath(IngredientType ingredient)
        {
            switch (ingredient)
            {
                case IngredientType.RedHerb:
                    return "Sprites/Ingredients/SPR_Ingredient_RedHerb_01";
                case IngredientType.BlueCrystal:
                    return "Sprites/Ingredients/SPR_Ingredient_BlueCrystal_01";
                case IngredientType.GreenLeaf:
                    return "Sprites/Ingredients/SPR_Ingredient_GreenLeaf_01";
                case IngredientType.YellowStarDust:
                    return "Sprites/Ingredients/SPR_Ingredient_YellowStarDust_01";
                case IngredientType.PurpleMushroom:
                    return "Sprites/Ingredients/SPR_Ingredient_PurpleMushroom_01";
                case IngredientType.OrangeFireDrop:
                    return "Sprites/Ingredients/SPR_Ingredient_OrangeFireDrop_01";
                default:
                    return null;
            }
        }

        private static string PotionResourcePath(PotionType potion)
        {
            switch (potion)
            {
                case PotionType.LineHorizontal:
                    return "Sprites/Potions/SPR_Potion_LineHorizontal_01";
                case PotionType.LineVertical:
                    return "Sprites/Potions/SPR_Potion_LineVertical_01";
                case PotionType.Bomb:
                    return "Sprites/Potions/SPR_Potion_Bomb_01";
                case PotionType.Lightning:
                    return "Sprites/Potions/SPR_Potion_Lightning_01";
                case PotionType.Mega:
                    return "Sprites/Potions/SPR_Potion_Mega_01";
                default:
                    return null;
            }
        }

        private static string ObstacleResourcePath(ObstacleType obstacle)
        {
            switch (obstacle)
            {
                case ObstacleType.WoodenBox:
                    return "Sprites/Obstacles/SPR_Obstacle_WoodenBox_01";
                case ObstacleType.StoneBlock:
                    return "Sprites/Obstacles/SPR_Obstacle_StoneBlock_01";
                case ObstacleType.DarkTile:
                    return "Sprites/Obstacles/SPR_Obstacle_DarkTile_01";
                case ObstacleType.FrozenIngredient:
                    return "Sprites/Obstacles/SPR_Obstacle_FrozenIngredient_01";
                case ObstacleType.MagicChain:
                    return "Sprites/Obstacles/SPR_Obstacle_MagicChain_01";
                default:
                    return null;
            }
        }

        // ── Ingredient Drawing (Glossy Candy Style) ─────────────────



        // ── Obstacle Drawing ────────────────────────────────────────





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
