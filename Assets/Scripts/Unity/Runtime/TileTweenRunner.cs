using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public enum TileTweenEase
    {
        SmoothStep,
        EaseInOutCubic,
        EaseOutQuart,
        EaseOutCubic,
        EaseOutBack,
        EaseOutBounce,
        EaseOutElastic
    }

    public readonly struct TileTweenMotion
    {
        public TileTweenMotion(RectTransform rect, Vector2 start, Vector2 end, bool squashOnLand = false)
        {
            Rect = rect;
            Start = start;
            End = end;
            SquashOnLand = squashOnLand;
        }

        public RectTransform Rect { get; }
        public Vector2 Start { get; }
        public Vector2 End { get; }
        public bool SquashOnLand { get; }
    }

    public static class TileTweenRunner
    {
        public static IEnumerator MovePair(
            RectTransform first,
            Vector2 firstStart,
            Vector2 firstEnd,
            RectTransform second,
            Vector2 secondStart,
            Vector2 secondEnd,
            float duration,
            TileTweenEase ease,
            float pulseScale)
        {
            var firstScale = first != null ? first.localScale : Vector3.one;
            var secondScale = second != null ? second.localScale : Vector3.one;
            var elapsed = 0f;

            while (elapsed < duration && first != null && second != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration));
                var t = Evaluate(ease, normalized);
                var pulse = pulseScale > 0f ? Mathf.Lerp(1f, pulseScale, Mathf.Sin(normalized * Mathf.PI)) : 1f;
                first.anchoredPosition = Vector2.LerpUnclamped(firstStart, firstEnd, t);
                second.anchoredPosition = Vector2.LerpUnclamped(secondStart, secondEnd, t);
                first.localScale = firstScale * pulse;
                second.localScale = secondScale * pulse;
                yield return null;
            }

            if (first != null)
            {
                first.anchoredPosition = firstEnd;
                first.localScale = firstScale;
            }

            if (second != null)
            {
                second.anchoredPosition = secondEnd;
                second.localScale = secondScale;
            }
        }

        public static IEnumerator MoveMany(IReadOnlyList<TileTweenMotion> motions, float duration, TileTweenEase ease)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var normalized = Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration));
                var t = Evaluate(ease, normalized);
                foreach (var motion in motions)
                {
                    if (motion.Rect == null)
                    {
                        continue;
                    }

                    motion.Rect.anchoredPosition = Vector2.LerpUnclamped(motion.Start, motion.End, t);
                    if (motion.SquashOnLand)
                    {
                        var settle = Mathf.Clamp01((normalized - 0.68f) / 0.32f);
                        var squash = Mathf.Sin(settle * Mathf.PI);
                        motion.Rect.localScale = new Vector3(
                            Mathf.Lerp(1f, 1.055f, squash),
                            Mathf.Lerp(1f, 0.925f, squash),
                            1f);
                    }
                }

                yield return null;
            }

            foreach (var motion in motions)
            {
                if (motion.Rect == null)
                {
                    continue;
                }

                motion.Rect.anchoredPosition = motion.End;
                motion.Rect.localScale = Vector3.one;
            }
        }

        public static IEnumerator Scale(RectTransform target, Vector3 start, Vector3 end, float duration, TileTweenEase ease)
        {
            if (target == null)
            {
                yield break;
            }

            var elapsed = 0f;
            target.localScale = start;
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Evaluate(ease, Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration)));
                target.localScale = Vector3.LerpUnclamped(start, end, t);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = end;
            }
        }

        public static IEnumerator ScaleFade(
            IReadOnlyList<RectTransform> targets,
            Vector3 startScale,
            Vector3 endScale,
            float startAlpha,
            float endAlpha,
            float duration,
            TileTweenEase ease)
        {
            var groups = new Dictionary<RectTransform, CanvasGroup>();
            foreach (var target in targets)
            {
                if (target != null)
                {
                    var group = target.GetComponent<CanvasGroup>();
                    groups[target] = group != null ? group : target.gameObject.AddComponent<CanvasGroup>();
                }
            }

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Evaluate(ease, Mathf.Clamp01(elapsed / Mathf.Max(0.001f, duration)));
                foreach (var target in targets)
                {
                    if (target == null)
                    {
                        continue;
                    }

                    target.localScale = Vector3.LerpUnclamped(startScale, endScale, t);
                    if (groups.TryGetValue(target, out var group) && group != null)
                    {
                        group.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                    }
                }

                yield return null;
            }
        }

        public static float Evaluate(TileTweenEase ease, float t)
        {
            t = Mathf.Clamp01(t);
            switch (ease)
            {
                case TileTweenEase.EaseInOutCubic:
                    return EasingFunctions.EaseInOutCubic(t);
                case TileTweenEase.EaseOutQuart:
                    return EasingFunctions.EaseOutQuart(t);
                case TileTweenEase.EaseOutCubic:
                    return EasingFunctions.EaseOutCubic(t);
                case TileTweenEase.EaseOutBack:
                    return EasingFunctions.EaseOutBack(t, 1.15f);
                case TileTweenEase.EaseOutBounce:
                    return EasingFunctions.EaseOutBounce(t);
                case TileTweenEase.EaseOutElastic:
                    return EasingFunctions.EaseOutElastic(t);
                default:
                    return EasingFunctions.SmoothStep(t);
            }
        }
    }
}
