using System.Collections;
using UnityEngine;

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
            if (cue == UiFeedbackCue.InvalidSwap)
            {
                StartCoroutine(Shake(target));
                return;
            }

            var pulseScale = cue == UiFeedbackCue.Potion ? 1.06f : 1.035f;
            StartCoroutine(Pulse(target, pulseScale, 0.16f));
        }

        public void PlayModalIntro(RectTransform target)
        {
            if (target == null)
            {
                return;
            }

            StartCoroutine(ModalIntro(target));
        }

        private static IEnumerator Shake(RectTransform target)
        {
            var start = target.anchoredPosition;
            const float duration = 0.18f;
            var elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var offset = Mathf.Sin(elapsed * 95f) * Mathf.Lerp(16f, 0f, elapsed / duration);
                target.anchoredPosition = start + new Vector2(offset, 0f);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = start;
            }
        }

        private static IEnumerator Pulse(RectTransform target, float peakScale, float duration)
        {
            var startScale = target.localScale;
            var peak = startScale * peakScale;
            var elapsed = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = t < 0.5f ? t * 2f : (1f - t) * 2f;
                target.localScale = Vector3.Lerp(startScale, peak, eased);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = startScale;
            }
        }

        private static IEnumerator ModalIntro(RectTransform target)
        {
            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = target.gameObject.AddComponent<CanvasGroup>();
            }

            const float duration = 0.18f;
            var elapsed = 0f;
            var startScale = Vector3.one * 0.94f;
            target.localScale = startScale;
            group.alpha = 0f;

            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                group.alpha = t;
                target.localScale = Vector3.Lerp(startScale, Vector3.one, t);
                yield return null;
            }

            if (target != null)
            {
                group.alpha = 1f;
                target.localScale = Vector3.one;
            }
        }
    }
}

