using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class UiFeedbackAnimator : MonoBehaviour
    {
        public void PlayBoardFeedback(UiFeedbackCue cue, RectTransform target)
        {
            if (target == null || cue == UiFeedbackCue.None)
            {
                return;
            }

            StopAllCoroutines();
            switch (cue)
            {
                case UiFeedbackCue.InvalidSwap:
                    StartCoroutine(Shake(target));
                    break;
                case UiFeedbackCue.Potion:
                    StartCoroutine(PotionPulse(target));
                    StartCoroutine(ScreenFlash(target));
                    break;
                case UiFeedbackCue.Cascade:
                    StartCoroutine(CascadeShake(target));
                    break;
                default:
                    StartCoroutine(Pulse(target, 1.04f, GameplayPresentationConfig.BoardPulseDuration));
                    break;
            }
        }

        public void PlayModalIntro(RectTransform target)
        {
            if (target == null)
            {
                return;
            }

            StartCoroutine(ModalIntro(target));
        }

        /// <summary>
        /// Plays a brief color tint overlay on the board to match a potion activation.
        /// </summary>
        public void PlayPotionTint(RectTransform target, Color potionColor)
        {
            if (target == null)
            {
                return;
            }

            StartCoroutine(PotionTintRoutine(target, potionColor));
        }

        /// <summary>
        /// Plays a screen-wide flash effect for large matches.
        /// </summary>
        public void PlayScreenFlash(RectTransform target)
        {
            if (target == null)
            {
                return;
            }

            StartCoroutine(ScreenFlash(target));
        }

        /// <summary>
        /// Plays a radial shockwave ring expanding from center (for bomb/lightning potions).
        /// </summary>
        public void PlayShockwave(RectTransform target, Color color)
        {
            if (target == null)
            {
                return;
            }

            StartCoroutine(ShockwaveRoutine(target, color));
        }

        /// <summary>
        /// Plays a board rumble with X+Y oscillation (for bomb potions).
        /// </summary>
        public void PlayBoardRumble(RectTransform target)
        {
            if (target == null)
            {
                return;
            }

            StartCoroutine(BoardRumble(target));
        }

        private static IEnumerator Shake(RectTransform target)
        {
            var start = target.anchoredPosition;
            var duration = GameplayPresentationConfig.InvalidShakeDuration;
            var elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var decay = 1f - elapsed / duration;
                var offset = Mathf.Sin(elapsed * 95f) * Mathf.Lerp(18f, 0f, 1f - decay * decay);
                target.anchoredPosition = start + new Vector2(offset, 0f);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = start;
            }
        }

        /// <summary>
        /// Cascade feedback: escalating vertical shake then settle — builds excitement.
        /// </summary>
        private static IEnumerator CascadeShake(RectTransform target)
        {
            var start = target.anchoredPosition;
            const float duration = 0.14f;
            var elapsed = 0f;

            // Brief vertical + horizontal jiggle
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var decay = 1f - elapsed / duration;
                var offsetY = Mathf.Sin(elapsed * 85f) * 4f * decay;
                var offsetX = Mathf.Sin(elapsed * 65f) * 2f * decay;
                target.anchoredPosition = start + new Vector2(offsetX, offsetY);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = start;
            }

            // Gentle scale pulse after the shake
            yield return Pulse(target, 1.03f, 0.12f);
        }

        /// <summary>
        /// Potion feedback: larger pulse with brightness flash and shockwave.
        /// </summary>
        private static IEnumerator PotionPulse(RectTransform target)
        {
            // Two-phase: quick scale up, then overshoot settle
            var startScale = target.localScale;
            var peak = startScale * 1.08f;
            const float phase1 = 0.08f;
            const float phase2 = 0.14f;

            var elapsed = 0f;
            while (elapsed < phase1 && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / phase1));
                target.localScale = Vector3.Lerp(startScale, peak, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < phase2 && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutElasticGentle(Mathf.Clamp01(elapsed / phase2));
                target.localScale = Vector3.Lerp(peak, startScale, t);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = startScale;
            }
        }

        private static IEnumerator Pulse(RectTransform target, float peakScale, float duration)
        {
            if (target == null)
            {
                yield break;
            }

            var startScale = target.localScale;
            var peak = startScale * peakScale;
            var elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = EasingFunctions.PingPong(t);
                target.localScale = Vector3.Lerp(startScale, peak, eased);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = startScale;
            }
        }

        /// <summary>
        /// Modal intro: scale from 0.65x with EaseOutBack overshoot + alpha fade-in.
        /// </summary>
        private static IEnumerator ModalIntro(RectTransform target)
        {
            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = target.gameObject.AddComponent<CanvasGroup>();
            }

            var duration = GameplayPresentationConfig.ModalRevealDuration;
            var elapsed = 0f;
            var startScale = Vector3.one * 0.65f;
            target.localScale = startScale;
            group.alpha = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var scaleT = EasingFunctions.EaseOutBack(t, 1.6f);
                var alphaT = EasingFunctions.EaseOutQuart(t);
                group.alpha = alphaT;
                target.localScale = Vector3.LerpUnclamped(startScale, Vector3.one, scaleT);
                yield return null;
            }

            if (target != null)
            {
                group.alpha = 1f;
                target.localScale = Vector3.one;
            }
        }

        /// <summary>
        /// Brief screen flash — white overlay that quickly fades out.
        /// </summary>
        private static IEnumerator ScreenFlash(RectTransform target)
        {
            var flashObject = new GameObject("Screen Flash", typeof(RectTransform), typeof(Image));
            flashObject.transform.SetParent(target, false);
            var rect = flashObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = flashObject.GetComponent<Image>();
            image.color = UiColorPalette.ScreenFlash;
            image.raycastTarget = false;

            var duration = GameplayPresentationConfig.ScreenFlashDuration;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                image.color = new Color(1f, 1f, 1f, UiColorPalette.ScreenFlash.a * (1f - t));
                yield return null;
            }

            Object.Destroy(flashObject);
        }

        /// <summary>
        /// Radial shockwave ring that expands and fades.
        /// </summary>
        private static IEnumerator ShockwaveRoutine(RectTransform target, Color color)
        {
            var ring = new GameObject("Shockwave Ring", typeof(RectTransform), typeof(Image));
            ring.transform.SetParent(target, false);
            var rect = ring.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.one * 40f;
            var image = ring.GetComponent<Image>();
            image.color = UiColorPalette.WithAlpha(color, 0.50f);
            image.raycastTarget = false;

            var duration = GameplayPresentationConfig.ShockwaveExpandDuration;
            var maxSize = GameplayPresentationConfig.ShockwaveMaxSize;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = EasingFunctions.EaseOutQuart(t);
                rect.sizeDelta = Vector2.one * Mathf.Lerp(40f, maxSize, eased);
                image.color = UiColorPalette.WithAlpha(color, 0.50f * (1f - t));
                yield return null;
            }

            Object.Destroy(ring);
        }

        /// <summary>
        /// Board rumble: sinusoidal shake in both X and Y axes.
        /// </summary>
        private static IEnumerator BoardRumble(RectTransform target)
        {
            var start = target.anchoredPosition;
            const float duration = 0.18f;
            var elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var decay = 1f - elapsed / duration;
                var intensity = 6f * decay * decay;
                var offsetX = Mathf.Sin(elapsed * 90f) * intensity;
                var offsetY = Mathf.Cos(elapsed * 75f) * intensity * 0.6f;
                target.anchoredPosition = start + new Vector2(offsetX, offsetY);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = start;
            }
        }

        /// <summary>
        /// Brief color overlay on the board that fades in and out.
        /// </summary>
        private static IEnumerator PotionTintRoutine(RectTransform target, Color color)
        {
            var tintObject = new GameObject("Potion Tint", typeof(RectTransform), typeof(Image));
            tintObject.transform.SetParent(target, false);
            var rect = tintObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = tintObject.GetComponent<Image>();
            image.color = new Color(color.r, color.g, color.b, 0f);
            image.raycastTarget = false;

            const float fadeIn = 0.06f;
            const float fadeOut = 0.12f;
            const float peakAlpha = 0.10f;

            var elapsed = 0f;
            while (elapsed < fadeIn)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / fadeIn);
                image.color = new Color(color.r, color.g, color.b, t * peakAlpha);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < fadeOut)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / fadeOut);
                image.color = new Color(color.r, color.g, color.b, (1f - t) * peakAlpha);
                yield return null;
            }

            Object.Destroy(tintObject);
        }
    }
}
