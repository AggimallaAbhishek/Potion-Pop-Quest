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
        private const float DefaultDuration = 0.25f;
        private const float SlideDistance = 120f;
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
        /// Incoming screen scales from 0.92x to 1.0x while fading in.
        /// Ideal for modals and overlay panels.
        /// </summary>
        public void ScaleReveal(GameObject incoming, float duration = DefaultDuration)
        {
            CancelActive();
            _activeTransition = StartCoroutine(ScaleRevealRoutine(incoming, duration));
        }

        /// <summary>
        /// Both screens overlap; outgoing fades out while incoming fades in simultaneously.
        /// </summary>
        public void CrossDissolve(GameObject outgoing, GameObject incoming, float duration = DefaultDuration)
        {
            CancelActive();
            _activeTransition = StartCoroutine(CrossDissolveRoutine(outgoing, incoming, duration));
        }

        /// <summary>
        /// Simple fade-in for a single screen.
        /// </summary>
        public void FadeIn(GameObject target, float duration = 0.18f)
        {
            CancelActive();
            _activeTransition = StartCoroutine(FadeInRoutine(target, duration));
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
                    inRect.localScale = Vector3.one * 0.92f;
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutBack(Mathf.Clamp01(elapsed / duration), 1.2f);
                var alpha = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));

                if (inGroup != null)
                {
                    inGroup.alpha = alpha;
                }

                if (inRect != null)
                {
                    inRect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.92f, Vector3.one, t);
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
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / duration));

                if (outGroup != null)
                {
                    outGroup.alpha = 1f - t;
                }

                if (inGroup != null)
                {
                    inGroup.alpha = t;
                }

                yield return null;
            }

            FinalizeTransition(outgoing, outGroup, null, incoming, inGroup, null);
            _activeTransition = null;
        }

        private IEnumerator FadeInRoutine(GameObject target, float duration)
        {
            var group = EnsureCanvasGroup(target);
            if (target != null)
            {
                target.SetActive(true);
                group.alpha = 0f;
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

                yield return null;
            }

            if (group != null)
            {
                group.alpha = 1f;
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
