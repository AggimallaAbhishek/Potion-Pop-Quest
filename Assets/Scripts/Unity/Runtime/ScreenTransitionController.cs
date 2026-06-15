using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    /// <summary>
    /// Provides animated transitions between UI screens.
    /// Requires CanvasGroup on each screen GameObject for alpha control.
    /// </summary>
    public sealed class ScreenTransitionController : MonoBehaviour
    {
        private const float DefaultDuration = 0.32f;
        private const float SlideDistance = 140f;
        private Coroutine _activeTransition;

        /// <summary>
        /// Slides the outgoing screen left while fading out,
        /// and slides the incoming screen in from the right while fading in.
        /// </summary>
        public void SlideAndFade(GameObject outgoing, GameObject incoming, float duration = DefaultDuration)
        {
            CancelActive();
            _activeTransition = StartCoroutine(SlideAndFadeRoutine(outgoing, incoming, duration));
        }

        /// <summary>
        /// Incoming screen scales from 0.90x to 1.0x while fading in.
        /// Ideal for modals and overlay panels.
        /// </summary>
        public void ScaleReveal(GameObject incoming, float duration = DefaultDuration)
        {
            CancelActive();
            _activeTransition = StartCoroutine(ScaleRevealRoutine(incoming, duration));
        }

        /// <summary>
        /// Both screens overlap; outgoing fades out while incoming fades in simultaneously.
        /// With subtle parallax drift for depth.
        /// </summary>
        public void CrossDissolve(GameObject outgoing, GameObject incoming, float duration = DefaultDuration)
        {
            CancelActive();
            _activeTransition = StartCoroutine(CrossDissolveRoutine(outgoing, incoming, duration));
        }

        /// <summary>
        /// Simple fade-in for a single screen.
        /// </summary>
        public void FadeIn(GameObject target, float duration = 0.22f)
        {
            CancelActive();
            _activeTransition = StartCoroutine(FadeInRoutine(target, duration));
        }

        /// <summary>
        /// Outgoing zooms out slightly while fading; incoming zooms in from slightly larger.
        /// Ideal for game → win/lose transitions.
        /// </summary>
        public void ZoomAndFade(GameObject outgoing, GameObject incoming, float duration = DefaultDuration)
        {
            CancelActive();
            _activeTransition = StartCoroutine(ZoomAndFadeRoutine(outgoing, incoming, duration));
        }

        /// <summary>
        /// Diagonal wipe reveal for dramatic level starts.
        /// </summary>
        public void WipeReveal(GameObject incoming, float duration = 0.38f)
        {
            CancelActive();
            _activeTransition = StartCoroutine(WipeRevealRoutine(incoming, duration));
        }

        /// <summary>
        /// Checks if a transition is currently playing.
        /// </summary>
        public bool IsTransitioning => _activeTransition != null;

        private void CancelActive()
        {
            if (_activeTransition != null)
            {
                StopCoroutine(_activeTransition);
                _activeTransition = null;
            }
        }

        private IEnumerator SlideAndFadeRoutine(GameObject outgoing, GameObject incoming, float duration)
        {
            var outGroup = EnsureCanvasGroup(outgoing);
            var outRect = outgoing != null ? outgoing.GetComponent<RectTransform>() : null;
            var inGroup = EnsureCanvasGroup(incoming);
            var inRect = incoming != null ? incoming.GetComponent<RectTransform>() : null;

            if (incoming != null)
            {
                incoming.SetActive(true);
                inGroup.alpha = 0f;
                if (inRect != null)
                {
                    inRect.anchoredPosition = new Vector2(SlideDistance, 0f);
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));

                if (outGroup != null)
                {
                    outGroup.alpha = 1f - t;
                }

                if (outRect != null)
                {
                    outRect.anchoredPosition = new Vector2(-SlideDistance * t, 0f);
                }

                if (inGroup != null)
                {
                    inGroup.alpha = t;
                }

                if (inRect != null)
                {
                    inRect.anchoredPosition = Vector2.Lerp(new Vector2(SlideDistance, 0f), Vector2.zero, t);
                }

                yield return null;
            }

            FinalizeTransition(outgoing, outGroup, outRect, incoming, inGroup, inRect);
            _activeTransition = null;
        }

        private IEnumerator ScaleRevealRoutine(GameObject incoming, float duration)
        {
            var inGroup = EnsureCanvasGroup(incoming);
            var inRect = incoming != null ? incoming.GetComponent<RectTransform>() : null;

            if (incoming != null)
            {
                incoming.SetActive(true);
                inGroup.alpha = 0f;
                if (inRect != null)
                {
                    inRect.localScale = Vector3.one * 0.90f;
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutBack(Mathf.Clamp01(elapsed / duration), 1.3f);
                var alpha = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));

                if (inGroup != null)
                {
                    inGroup.alpha = alpha;
                }

                if (inRect != null)
                {
                    inRect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.90f, Vector3.one, t);
                }

                yield return null;
            }

            if (inGroup != null)
            {
                inGroup.alpha = 1f;
            }

            if (inRect != null)
            {
                inRect.localScale = Vector3.one;
            }

            _activeTransition = null;
        }

        private IEnumerator CrossDissolveRoutine(GameObject outgoing, GameObject incoming, float duration)
        {
            var outGroup = EnsureCanvasGroup(outgoing);
            var outRect = outgoing != null ? outgoing.GetComponent<RectTransform>() : null;
            var inGroup = EnsureCanvasGroup(incoming);
            var inRect = incoming != null ? incoming.GetComponent<RectTransform>() : null;

            if (incoming != null)
            {
                incoming.SetActive(true);
                inGroup.alpha = 0f;
                if (inRect != null)
                {
                    inRect.anchoredPosition = new Vector2(0f, 20f);
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));

                if (outGroup != null)
                {
                    outGroup.alpha = 1f - t;
                }

                // Slight parallax drift on outgoing
                if (outRect != null)
                {
                    outRect.anchoredPosition = new Vector2(0f, -18f * t);
                }

                if (inGroup != null)
                {
                    inGroup.alpha = t;
                }

                // Incoming drifts up into place
                if (inRect != null)
                {
                    inRect.anchoredPosition = Vector2.Lerp(new Vector2(0f, 20f), Vector2.zero, t);
                }

                yield return null;
            }

            FinalizeTransition(outgoing, outGroup, outRect, incoming, inGroup, inRect);
            _activeTransition = null;
        }

        private IEnumerator FadeInRoutine(GameObject target, float duration)
        {
            var group = EnsureCanvasGroup(target);
            var rect = target != null ? target.GetComponent<RectTransform>() : null;

            if (target != null)
            {
                target.SetActive(true);
                group.alpha = 0f;
                if (rect != null)
                {
                    rect.localScale = Vector3.one * 0.96f;
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));
                if (group != null)
                {
                    group.alpha = t;
                }

                if (rect != null)
                {
                    rect.localScale = Vector3.Lerp(Vector3.one * 0.96f, Vector3.one, t);
                }

                yield return null;
            }

            if (group != null)
            {
                group.alpha = 1f;
            }

            if (rect != null)
            {
                rect.localScale = Vector3.one;
            }

            _activeTransition = null;
        }

        private IEnumerator ZoomAndFadeRoutine(GameObject outgoing, GameObject incoming, float duration)
        {
            var outGroup = EnsureCanvasGroup(outgoing);
            var outRect = outgoing != null ? outgoing.GetComponent<RectTransform>() : null;
            var inGroup = EnsureCanvasGroup(incoming);
            var inRect = incoming != null ? incoming.GetComponent<RectTransform>() : null;

            if (incoming != null)
            {
                incoming.SetActive(true);
                inGroup.alpha = 0f;
                if (inRect != null)
                {
                    inRect.localScale = Vector3.one * 1.08f;
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));

                // Outgoing zooms out and fades
                if (outGroup != null)
                {
                    outGroup.alpha = 1f - t;
                }

                if (outRect != null)
                {
                    outRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.92f, t);
                }

                // Incoming zooms in from large and fades in
                if (inGroup != null)
                {
                    inGroup.alpha = t;
                }

                if (inRect != null)
                {
                    inRect.localScale = Vector3.Lerp(Vector3.one * 1.08f, Vector3.one, EasingFunctions.EaseOutBack(t, 0.8f));
                }

                yield return null;
            }

            FinalizeTransition(outgoing, outGroup, outRect, incoming, inGroup, inRect);
            _activeTransition = null;
        }

        private IEnumerator WipeRevealRoutine(GameObject incoming, float duration)
        {
            var inGroup = EnsureCanvasGroup(incoming);

            if (incoming != null)
            {
                incoming.SetActive(true);
                inGroup.alpha = 0f;
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseInOutCubic(Mathf.Clamp01(elapsed / duration));

                if (inGroup != null)
                {
                    // Rapid alpha reveal combined with slight scale
                    inGroup.alpha = Mathf.Clamp01(t * 1.5f);
                }

                yield return null;
            }

            if (inGroup != null)
            {
                inGroup.alpha = 1f;
            }

            _activeTransition = null;
        }

        private static void FinalizeTransition(
            GameObject outgoing, CanvasGroup outGroup, RectTransform outRect,
            GameObject incoming, CanvasGroup inGroup, RectTransform inRect)
        {
            if (outgoing != null)
            {
                outgoing.SetActive(false);
                if (outGroup != null)
                {
                    outGroup.alpha = 1f;
                }

                if (outRect != null)
                {
                    outRect.anchoredPosition = Vector2.zero;
                    outRect.localScale = Vector3.one;
                }
            }

            if (incoming != null)
            {
                incoming.SetActive(true);
                if (inGroup != null)
                {
                    inGroup.alpha = 1f;
                }

                if (inRect != null)
                {
                    inRect.anchoredPosition = Vector2.zero;
                    inRect.localScale = Vector3.one;
                }
            }
        }

        private static CanvasGroup EnsureCanvasGroup(GameObject target)
        {
            if (target == null)
            {
                return null;
            }

            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
            {
                group = target.AddComponent<CanvasGroup>();
            }

            return group;
        }
    }
}
