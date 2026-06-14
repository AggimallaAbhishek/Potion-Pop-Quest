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
                    break;
                case UiFeedbackCue.Cascade:
                    StartCoroutine(CascadeShake(target));
                    break;
                default:
                    StartCoroutine(Pulse(target, 1.035f, GameplayPresentationConfig.BoardPulseDuration));
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

        private static IEnumerator Shake(RectTransform target)
        {
            var start = target.anchoredPosition;
            var duration = GameplayPresentationConfig.InvalidShakeDuration;
            var elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var decay = 1f - elapsed / duration;
                var offset = Mathf.Sin(elapsed * 95f) * Mathf.Lerp(16f, 0f, 1f - decay * decay);
                target.anchoredPosition = start + new Vector2(offset, 0f);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = start;
            }
        }

        /// <summary>
        /// Cascade feedback: vertical shake then settle — builds excitement.
        /// </summary>
        private static IEnumerator CascadeShake(RectTransform target)
        {
            var start = target.anchoredPosition;
            const float duration = 0.12f;
            var elapsed = 0f;

            // Brief vertical jiggle
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var decay = 1f - elapsed / duration;
                var offsetY = Mathf.Sin(elapsed * 80f) * 3f * decay;
                target.anchoredPosition = start + new Vector2(0f, offsetY);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = start;
            }

            // Gentle scale pulse after the shake
            yield return Pulse(target, 1.025f, 0.10f);
        }

        /// <summary>
        /// Potion feedback: slightly larger pulse with a brief brightness flash.
        /// </summary>
        private static IEnumerator PotionPulse(RectTransform target)
        {
            // Two-phase: quick scale up, then overshoot settle
            var startScale = target.localScale;
            var peak = startScale * 1.06f;
            var overshoot = startScale * 1.02f;
            const float phase1 = 0.08f;
            const float phase2 = 0.12f;

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
        /// Modal intro: scale from 0.7x with EaseOutBack overshoot + alpha fade-in.
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
            var startScale = Vector3.one * 0.70f;
            target.localScale = startScale;
            group.alpha = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var scaleT = EasingFunctions.EaseOutBack(t, 1.4f);
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
            const float fadeOut = 0.10f;
            const float peakAlpha = 0.08f;

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
