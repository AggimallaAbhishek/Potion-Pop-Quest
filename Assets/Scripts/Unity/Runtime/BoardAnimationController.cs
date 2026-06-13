using System.Collections;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class BoardAnimationController : MonoBehaviour
    {
        public IEnumerator Play(
            IReadOnlyList<BoardAnimationEvent> events,
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            RectTransform boardRoot)
        {
            if (events == null || events.Count == 0 || boardRoot == null)
            {
                yield break;
            }

            foreach (var animationEvent in events)
            {
                switch (animationEvent.Kind)
                {
                    case BoardAnimationEventKind.Swap:
                        yield return Swap(tileViews, animationEvent.From, animationEvent.To);
                        break;
                    case BoardAnimationEventKind.InvalidSwap:
                        yield return ShakePositions(tileViews, animationEvent.Positions);
                        break;
                    case BoardAnimationEventKind.CascadeStarted:
                        yield return new WaitForSecondsRealtime(0.08f);
                        break;
                    case BoardAnimationEventKind.Clear:
                        yield return PopPositions(tileViews, animationEvent.Positions, new Color(1f, 0.95f, 0.55f, 1f));
                        break;
                    case BoardAnimationEventKind.PotionCreated:
                        yield return PopPositions(tileViews, animationEvent.Positions, PotionColor(animationEvent.Potion));
                        break;
                    case BoardAnimationEventKind.PotionActivated:
                        yield return PotionBurst(boardRoot, animationEvent);
                        break;
                    case BoardAnimationEventKind.ObstacleDamaged:
                        yield return PopPositions(tileViews, animationEvent.Positions, new Color(1f, 0.58f, 0.28f, 1f));
                        break;
                    case BoardAnimationEventKind.ObstacleDestroyed:
                        yield return PopPositions(tileViews, animationEvent.Positions, new Color(0.78f, 0.54f, 1f, 1f));
                        break;
                    case BoardAnimationEventKind.TileDropped:
                        yield return DropTile(tileViews, animationEvent.From, animationEvent.To);
                        break;
                    case BoardAnimationEventKind.TileSpawned:
                        yield return SpawnTile(tileViews, animationEvent.From, animationEvent.To);
                        break;
                    case BoardAnimationEventKind.Win:
                    case BoardAnimationEventKind.Lose:
                        yield return Pulse(boardRoot, 1.035f, 0.16f);
                        break;
                }
            }
        }

        private static IEnumerator Swap(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            GridPosition first,
            GridPosition second)
        {
            if (!tileViews.TryGetValue(first, out var firstRect) || !tileViews.TryGetValue(second, out var secondRect))
            {
                yield break;
            }

            var firstStart = firstRect.localScale;
            var secondStart = secondRect.localScale;
            var firstEndPosition = firstRect.anchoredPosition;
            var secondEndPosition = secondRect.anchoredPosition;
            const float duration = 0.12f;
            var elapsed = 0f;
            firstRect.anchoredPosition = secondEndPosition;
            secondRect.anchoredPosition = firstEndPosition;

            while (elapsed < duration && firstRect != null && secondRect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var progress = Smooth(Mathf.Clamp01(elapsed / duration));
                var pulse = Mathf.Sin(progress * Mathf.PI);
                firstRect.anchoredPosition = Vector2.LerpUnclamped(secondEndPosition, firstEndPosition, progress);
                secondRect.anchoredPosition = Vector2.LerpUnclamped(firstEndPosition, secondEndPosition, progress);
                firstRect.localScale = firstStart * Mathf.Lerp(1f, 1.08f, pulse);
                secondRect.localScale = secondStart * Mathf.Lerp(1f, 1.08f, pulse);
                yield return null;
            }

            if (firstRect != null)
            {
                firstRect.anchoredPosition = firstEndPosition;
                firstRect.localScale = firstStart;
            }

            if (secondRect != null)
            {
                secondRect.anchoredPosition = secondEndPosition;
                secondRect.localScale = secondStart;
            }
        }

        private static IEnumerator ShakePositions(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            IReadOnlyList<GridPosition> positions)
        {
            var targets = ResolveTargets(tileViews, positions);
            var starts = new Dictionary<RectTransform, Vector2>();
            foreach (var target in targets)
            {
                starts[target] = target.anchoredPosition;
            }

            const float duration = 0.18f;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var offset = Mathf.Sin(elapsed * 120f) * Mathf.Lerp(10f, 0f, elapsed / duration);
                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        target.anchoredPosition = starts[target] + new Vector2(offset, 0f);
                    }
                }

                yield return null;
            }

            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.anchoredPosition = starts[target];
                }
            }
        }

        private IEnumerator PopPositions(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            IReadOnlyList<GridPosition> positions,
            Color flashColor)
        {
            var targets = ResolveTargets(tileViews, positions);
            foreach (var target in targets)
            {
                if (target == null)
                {
                    continue;
                }

                StartCoroutine(Flash(target, flashColor));
            }

            yield return new WaitForSecondsRealtime(0.12f);
        }

        private static IEnumerator DropTile(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            GridPosition from,
            GridPosition to)
        {
            if (!tileViews.TryGetValue(to, out var target) || target == null)
            {
                yield break;
            }

            var end = target.anchoredPosition;
            var start = tileViews.TryGetValue(from, out var source) && source != null && source != target
                ? source.anchoredPosition
                : end + new Vector2(0f, CellPitch(target, vertical: true) * Mathf.Max(1, to.Row - from.Row));
            yield return MoveAnchored(target, start, end, 0.12f);
        }

        private static IEnumerator SpawnTile(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            GridPosition from,
            GridPosition to)
        {
            if (!tileViews.TryGetValue(to, out var target) || target == null)
            {
                yield break;
            }

            var end = target.anchoredPosition;
            var rowDistance = Mathf.Max(1, to.Row - from.Row);
            var start = end + new Vector2(0f, CellPitch(target, vertical: true) * rowDistance);
            target.localScale = Vector3.one * 0.72f;
            yield return MoveAnchored(target, start, end, 0.14f);
            yield return Scale(target, Vector3.one * 0.72f, Vector3.one, 0.08f);
        }

        private IEnumerator PotionBurst(RectTransform boardRoot, BoardAnimationEvent animationEvent)
        {
            if (boardRoot == null)
            {
                yield break;
            }

            if (animationEvent.Potion == PotionType.LineHorizontal || animationEvent.Potion == PotionType.LineVertical)
            {
                yield return Beam(boardRoot, animationEvent.Potion == PotionType.LineHorizontal);
                yield break;
            }

            var color = PotionColor(animationEvent.Potion);
            var burst = new GameObject("Potion Burst", typeof(RectTransform), typeof(Image));
            burst.transform.SetParent(boardRoot, false);
            var rect = burst.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.one * (animationEvent.Potion == PotionType.Bomb ? 140f : 220f);
            var image = burst.GetComponent<Image>();
            image.color = new Color(color.r, color.g, color.b, 0.45f);
            image.raycastTarget = false;

            yield return Scale(rect, Vector3.one * 0.35f, Vector3.one * 1.35f, 0.20f);
            Object.Destroy(burst);
        }

        private static IEnumerator Beam(RectTransform boardRoot, bool horizontal)
        {
            var beam = new GameObject("Line Potion Beam", typeof(RectTransform), typeof(Image));
            beam.transform.SetParent(boardRoot, false);
            var rect = beam.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = horizontal ? new Vector2(boardRoot.rect.width, 18f) : new Vector2(18f, boardRoot.rect.height);
            var image = beam.GetComponent<Image>();
            image.color = new Color(0.74f, 0.94f, 1f, 0.72f);
            image.raycastTarget = false;
            yield return Scale(rect, horizontal ? new Vector3(0.08f, 1f, 1f) : new Vector3(1f, 0.08f, 1f), Vector3.one, 0.14f);
            Object.Destroy(beam);
        }

        private static IEnumerator Pulse(RectTransform target, float scale, float duration)
        {
            yield return Scale(target, Vector3.one, Vector3.one * scale, duration * 0.5f);
            yield return Scale(target, Vector3.one * scale, Vector3.one, duration * 0.5f);
        }

        private static IEnumerator Flash(RectTransform target, Color color)
        {
            var image = target.GetComponent<Image>();
            if (image == null)
            {
                yield return Pulse(target, 1.10f, 0.12f);
                yield break;
            }

            var startColor = image.color;
            image.color = color;
            yield return Pulse(target, 1.12f, 0.12f);
            if (image != null)
            {
                image.color = startColor;
            }
        }

        private static IEnumerator MoveAnchored(RectTransform target, Vector2 start, Vector2 end, float duration)
        {
            var elapsed = 0f;
            target.anchoredPosition = start;
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                target.anchoredPosition = Vector2.LerpUnclamped(start, end, t);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = end;
            }
        }

        private static IEnumerator Scale(RectTransform target, Vector3 start, Vector3 end, float duration)
        {
            var elapsed = 0f;
            target.localScale = start;
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                target.localScale = Vector3.LerpUnclamped(start, end, t);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = end;
            }
        }

        private static List<RectTransform> ResolveTargets(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            IReadOnlyList<GridPosition> positions)
        {
            var targets = new List<RectTransform>();
            foreach (var position in positions)
            {
                if (tileViews.TryGetValue(position, out var target) && target != null)
                {
                    targets.Add(target);
                }
            }

            return targets;
        }

        private static Color PotionColor(PotionType potion)
        {
            switch (potion)
            {
                case PotionType.Bomb:
                    return new Color(1f, 0.50f, 0.20f, 1f);
                case PotionType.Lightning:
                    return new Color(0.94f, 0.96f, 1f, 1f);
                case PotionType.Mega:
                    return new Color(1f, 0.82f, 0.34f, 1f);
                default:
                    return new Color(0.62f, 0.86f, 1f, 1f);
            }
        }

        private static float Smooth(float t)
        {
            return t * t * (3f - 2f * t);
        }

        private static float CellPitch(RectTransform target, bool vertical)
        {
            if (target.parent != null && target.parent.TryGetComponent<GridLayoutGroup>(out var layout))
            {
                return vertical ? layout.cellSize.y + layout.spacing.y : layout.cellSize.x + layout.spacing.x;
            }

            return vertical ? Mathf.Max(32f, target.rect.height + 8f) : Mathf.Max(32f, target.rect.width + 8f);
        }
    }
}
