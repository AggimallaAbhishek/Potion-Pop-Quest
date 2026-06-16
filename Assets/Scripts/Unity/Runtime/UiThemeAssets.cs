using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class UiThemeAssets
    {
        private const string DisplayFontPath = "Fonts/PPQ_Display_SDF";
        private TMP_FontAsset _font;

        public TMP_FontAsset Font
        {
            get
            {
                if (_font != null)
                {
                    return _font;
                }

                _font = Resources.Load<TMP_FontAsset>(DisplayFontPath);
                if (_font == null)
                {
                    _font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                }

                return _font;
            }
        }

        public void AddHighValueTextShadow(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return;
            }

            // In TextMeshPro, shadow is often handled by the material underlay or <mark> tags.
            // For programmatic quick wins, we can enable font material underlay or use the outline property.
            // A simple approach is adding a slight outline and underlay via the material or font settings.
            text.fontStyle |= FontStyles.Bold;
        }

        /// <summary>
        /// Adds a bold outline + shadow combo for title text.
        /// </summary>
        public void AddTitleTextEffects(TextMeshProUGUI text)
        {
            if (text == null)
            {
                return;
            }

            // Using TextMeshPro's outline properties
            text.outlineWidth = 0.2f;
            text.outlineColor = new Color32(0, 0, 0, 180);
            text.fontStyle |= FontStyles.Bold;
        }

        /// <summary>
        /// Creates a glow Image behind a target UI element.
        /// </summary>
        public static GameObject AddOuterGlow(RectTransform target, Color color, float expand = 0.10f)
        {
            if (target == null)
            {
                return null;
            }

            var glow = new GameObject("OuterGlow", typeof(RectTransform), typeof(Image));
            glow.transform.SetParent(target, false);
            glow.transform.SetAsFirstSibling();
            var rect = glow.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(-expand, -expand);
            rect.anchorMax = new Vector2(1f + expand, 1f + expand);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = glow.GetComponent<Image>();
            img.color = UiColorPalette.WithAlpha(color, 0.22f);
            img.raycastTarget = false;
            return glow;
        }

        /// <summary>
        /// Creates a vertical gradient overlay on a target element.
        /// </summary>
        public static GameObject AddGradientOverlay(RectTransform target, Color top, Color bottom)
        {
            if (target == null)
            {
                return null;
            }

            // Top highlight
            var topOverlay = new GameObject("GradientTop", typeof(RectTransform), typeof(Image));
            topOverlay.transform.SetParent(target, false);
            var topRect = topOverlay.GetComponent<RectTransform>();
            topRect.anchorMin = new Vector2(0f, 0.82f);
            topRect.anchorMax = new Vector2(1f, 1f);
            topRect.offsetMin = Vector2.zero;
            topRect.offsetMax = Vector2.zero;
            topOverlay.GetComponent<Image>().color = top;
            topOverlay.GetComponent<Image>().raycastTarget = false;

            // Bottom shadow
            var bottomOverlay = new GameObject("GradientBottom", typeof(RectTransform), typeof(Image));
            bottomOverlay.transform.SetParent(target, false);
            var bottomRect = bottomOverlay.GetComponent<RectTransform>();
            bottomRect.anchorMin = new Vector2(0f, 0f);
            bottomRect.anchorMax = new Vector2(1f, 0.20f);
            bottomRect.offsetMin = Vector2.zero;
            bottomRect.offsetMax = Vector2.zero;
            bottomOverlay.GetComponent<Image>().color = bottom;
            bottomOverlay.GetComponent<Image>().raycastTarget = false;

            return topOverlay;
        }

        /// <summary>
        /// Creates an animated diagonal shimmer sweep on a target element.
        /// Returns a MonoBehaviour that can be used to stop the effect.
        /// </summary>
        public static void AddShimmerEffect(RectTransform target, MonoBehaviour host)
        {
            if (target == null || host == null)
            {
                return;
            }

            var shimmer = new GameObject("Shimmer", typeof(RectTransform), typeof(Image));
            shimmer.transform.SetParent(target, false);
            var rect = shimmer.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(-0.3f, 0f);
            rect.anchorMax = new Vector2(-0.1f, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = shimmer.GetComponent<Image>();
            img.color = new Color(1f, 1f, 1f, GameplayPresentationConfig.ShimmerAlpha);
            img.raycastTarget = false;

            host.StartCoroutine(ShimmerLoop(rect));
        }

        private static IEnumerator ShimmerLoop(RectTransform shimmerRect)
        {
            var period = GameplayPresentationConfig.ShimmerSweepDuration;
            var width = GameplayPresentationConfig.ShimmerSweepWidth;
            var waitBetween = 3.5f;

            while (shimmerRect != null)
            {
                // Sweep across
                var elapsed = 0f;
                while (elapsed < period && shimmerRect != null)
                {
                    elapsed += Time.unscaledDeltaTime;
                    var t = Mathf.Clamp01(elapsed / period);
                    var pos = Mathf.Lerp(-0.3f, 1.1f, EasingFunctions.EaseInOutCubic(t));
                    shimmerRect.anchorMin = new Vector2(pos, 0f);
                    shimmerRect.anchorMax = new Vector2(pos + width, 1f);
                    shimmerRect.offsetMin = Vector2.zero;
                    shimmerRect.offsetMax = Vector2.zero;
                    yield return null;
                }

                // Reset and wait
                if (shimmerRect != null)
                {
                    shimmerRect.anchorMin = new Vector2(-0.3f, 0f);
                    shimmerRect.anchorMax = new Vector2(-0.1f, 1f);
                    shimmerRect.offsetMin = Vector2.zero;
                    shimmerRect.offsetMax = Vector2.zero;
                }

                yield return new WaitForSecondsRealtime(waitBetween);
            }
        }
    }
}
