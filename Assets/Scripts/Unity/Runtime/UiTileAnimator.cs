using System.Collections;
using UnityEngine;

namespace PotionPopQuest.Unity
{
    public sealed class UiTileAnimator : MonoBehaviour
    {
        public void PlayIntro(float delay, UiFeedbackCue cue)
        {
            StartCoroutine(Intro(delay, cue));
        }

        private IEnumerator Intro(float delay, UiFeedbackCue cue)
        {
            var rect = transform as RectTransform;
            if (rect == null)
            {
                yield break;
            }

            if (delay > 0f)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            var peak = cue == UiFeedbackCue.Potion ? 1.12f : 1.06f;
            var startScale = Vector3.one * 0.82f;
            var peakScale = Vector3.one * peak;
            rect.localScale = startScale;

            const float duration = 0.16f;
            var elapsed = 0f;
            while (elapsed < duration && rect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                rect.localScale = t < 0.62f
                    ? Vector3.Lerp(startScale, peakScale, t / 0.62f)
                    : Vector3.Lerp(peakScale, Vector3.one, (t - 0.62f) / 0.38f);
                yield return null;
            }

            if (rect != null)
            {
                rect.localScale = Vector3.one;
            }
        }
    }
}

