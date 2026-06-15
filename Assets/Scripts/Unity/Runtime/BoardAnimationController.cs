using System.Collections;
using System.Collections.Generic;
using PotionPopQuest.Core;
using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class BoardAnimationController : MonoBehaviour
    {
        private int _cascadeCount;

        public IEnumerator Play(
            IReadOnlyList<BoardAnimationEvent> events,
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            RectTransform boardRoot)
        {
            if (events == null || events.Count == 0 || boardRoot == null)
            {
                yield break;
            }

            _cascadeCount = 0;

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
                        _cascadeCount++;
                        yield return new WaitForSecondsRealtime(GameplayPresentationConfig.CascadeDelay);
                        break;
                    case BoardAnimationEventKind.Clear:
                        yield return PopWithParticles(tileViews, animationEvent.Positions, UiColorPalette.ClearGlow, boardRoot);
                        break;
                    case BoardAnimationEventKind.PotionCreated:
                        yield return PopWithParticles(tileViews, animationEvent.Positions, PotionColor(animationEvent.Potion), boardRoot);
                        break;
                    case BoardAnimationEventKind.PotionActivated:
                        yield return PotionBurst(boardRoot, animationEvent);
                        break;
                    case BoardAnimationEventKind.ObstacleDamaged:
                        yield return PopWithParticles(tileViews, animationEvent.Positions, UiColorPalette.ObstacleDamageFlash, boardRoot);
                        break;
                    case BoardAnimationEventKind.ObstacleDestroyed:
                        yield return PopWithParticles(tileViews, animationEvent.Positions, UiColorPalette.ObstacleDestroyFlash, boardRoot);
                        break;
                    case BoardAnimationEventKind.TileDropped:
                        yield return DropTileWithBounce(tileViews, animationEvent.From, animationEvent.To);
                        break;
                    case BoardAnimationEventKind.TileSpawned:
                        yield return SpawnTile(tileViews, animationEvent.From, animationEvent.To);
                        break;
                    case BoardAnimationEventKind.Win:
                    case BoardAnimationEventKind.Lose:
                        yield return Pulse(boardRoot, 1.04f, GameplayPresentationConfig.BoardPulseDuration);
                        break;
                    case BoardAnimationEventKind.BoardShuffled:
                        yield return ShuffleBoard(tileViews, animationEvent.Positions, boardRoot);
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
            var duration = GameplayPresentationConfig.SwapDuration;
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
                // Slightly larger pulse for juicier swap
                firstRect.localScale = firstStart * Mathf.Lerp(1f, 1.12f, pulse);
                secondRect.localScale = secondStart * Mathf.Lerp(1f, 1.12f, pulse);
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

            var duration = GameplayPresentationConfig.InvalidShakeDuration;
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var decay = 1f - elapsed / duration;
                var offset = Mathf.Sin(elapsed * 120f) * Mathf.Lerp(12f, 0f, 1f - decay * decay);
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

        /// <summary>
        /// Enhanced pop with starburst particle effects on clear.
        /// </summary>
        private IEnumerator PopWithParticles(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            IReadOnlyList<GridPosition> positions,
            Color flashColor,
            RectTransform boardRoot)
        {
            var targets = ResolveTargets(tileViews, positions);
            var origins = new List<Vector2>();

            foreach (var target in targets)
            {
                if (target == null)
                {
                    continue;
                }

                origins.Add(target.anchoredPosition);
                StartCoroutine(FlashAndPop(target, flashColor));
            }

            // Spawn starburst particles at each cleared tile
            if (boardRoot != null && origins.Count > 0)
            {
                StartCoroutine(SpawnParticleBursts(boardRoot, origins, flashColor));
            }

            yield return new WaitForSecondsRealtime(GameplayPresentationConfig.ClearPopDuration);
        }

        /// <summary>
        /// Flash color and elastic pop with scale-down shrink.
        /// </summary>
        private static IEnumerator FlashAndPop(RectTransform target, Color color)
        {
            var image = target.GetComponent<Image>();
            Color? startColor = null;
            if (image != null)
            {
                startColor = image.color;
                image.color = color;
            }

            // Scale up with elastic overshoot
            var elapsed = 0f;
            var phase1 = GameplayPresentationConfig.ClearPopDuration * 0.45f;
            while (elapsed < phase1 && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutBack(Mathf.Clamp01(elapsed / phase1), 2.0f);
                target.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 1.18f, t);
                yield return null;
            }

            // Shrink down quickly
            elapsed = 0f;
            var phase2 = GameplayPresentationConfig.ClearPopDuration * 0.55f;
            while (elapsed < phase2 && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutQuart(Mathf.Clamp01(elapsed / phase2));
                target.localScale = Vector3.Lerp(Vector3.one * 1.18f, Vector3.one, t);

                // Fade out the flash color
                if (image != null && startColor.HasValue)
                {
                    image.color = Color.Lerp(color, startColor.Value, t);
                }

                yield return null;
            }

            if (target != null)
            {
                target.localScale = Vector3.one;
            }

            if (image != null && startColor.HasValue)
            {
                image.color = startColor.Value;
            }
        }

        /// <summary>
        /// Spawns starburst particles that fly outward from each cleared tile position.
        /// </summary>
        private static IEnumerator SpawnParticleBursts(RectTransform boardRoot, IReadOnlyList<Vector2> origins, Color color)
        {
            var particles = new List<RectTransform>();
            var images = new List<Image>();
            var velocities = new List<Vector2>();
            var count = GameplayPresentationConfig.ParticleBurstCount;
            var speed = GameplayPresentationConfig.ParticleBurstSpeed;

            foreach (var origin in origins)
            {
                for (var i = 0; i < count; i++)
                {
                    var angle = (i / (float)count) * Mathf.PI * 2f + Random.Range(-0.2f, 0.2f);
                    var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                    var particle = new GameObject("ClearParticle", typeof(RectTransform), typeof(Image));
                    particle.transform.SetParent(boardRoot, false);
                    var rect = particle.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.5f, 0.5f);
                    rect.anchorMax = new Vector2(0.5f, 0.5f);
                    rect.sizeDelta = Vector2.one * Random.Range(
                        GameplayPresentationConfig.ParticleBurstSize * 0.6f,
                        GameplayPresentationConfig.ParticleBurstSize * 1.4f);
                    rect.anchoredPosition = origin;
                    rect.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

                    var img = particle.GetComponent<Image>();
                    // Randomize particle color slightly
                    var hueShift = Random.Range(-0.06f, 0.06f);
                    var particleColor = new Color(
                        Mathf.Clamp01(color.r + hueShift),
                        Mathf.Clamp01(color.g + hueShift),
                        Mathf.Clamp01(color.b + hueShift), 0.90f);
                    img.color = particleColor;
                    img.raycastTarget = false;

                    particles.Add(rect);
                    images.Add(img);
                    velocities.Add(dir * speed * Random.Range(0.7f, 1.3f));
                }
            }

            var elapsed = 0f;
            var duration = GameplayPresentationConfig.ParticleBurstDuration;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);

                for (var i = 0; i < particles.Count; i++)
                {
                    if (particles[i] == null)
                    {
                        continue;
                    }

                    // Move outward with deceleration
                    particles[i].anchoredPosition += velocities[i] * (1f - t) * Time.unscaledDeltaTime;
                    // Shrink and fade
                    particles[i].localScale = Vector3.one * (1f - t * t);
                    if (images[i] != null)
                    {
                        var c = images[i].color;
                        images[i].color = new Color(c.r, c.g, c.b, 0.90f * (1f - t));
                    }
                }

                yield return null;
            }

            foreach (var p in particles)
            {
                if (p != null)
                {
                    Object.Destroy(p.gameObject);
                }
            }
        }

        /// <summary>
        /// Tile drop with squash-and-stretch bounce on landing.
        /// </summary>
        private static IEnumerator DropTileWithBounce(
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

            // Move phase with EaseOutBounce
            var elapsed = 0f;
            var moveDuration = GameplayPresentationConfig.DropDuration;
            target.anchoredPosition = start;
            while (elapsed < moveDuration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutBounce(Mathf.Clamp01(elapsed / moveDuration));
                target.anchoredPosition = Vector2.LerpUnclamped(start, end, t);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = end;
            }

            // Squash-and-stretch on landing
            elapsed = 0f;
            var ssDuration = GameplayPresentationConfig.SquashStretchDuration;
            while (elapsed < ssDuration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / ssDuration);
                var ss = EasingFunctions.SquashStretch(t);
                // Squash: wider and shorter; Stretch: thinner and taller
                var scaleX = ss < 1f ? Mathf.Lerp(1f, GameplayPresentationConfig.SquashScaleX, (1f - ss)) : 1f;
                var scaleY = ss;
                target.localScale = new Vector3(scaleX, scaleY, 1f);
                yield return null;
            }

            if (target != null)
            {
                target.localScale = Vector3.one;
            }
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
            target.localScale = Vector3.one * 0.65f;

            // Move and scale simultaneously
            var elapsed = 0f;
            var duration = GameplayPresentationConfig.SpawnDropDuration;
            while (elapsed < duration && target != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var moveT = EasingFunctions.EaseOutCubic(t);
                var scaleT = EasingFunctions.EaseOutBack(t, 1.2f);
                target.anchoredPosition = Vector2.LerpUnclamped(start, end, moveT);
                target.localScale = Vector3.LerpUnclamped(Vector3.one * 0.65f, Vector3.one, scaleT);
                yield return null;
            }

            if (target != null)
            {
                target.anchoredPosition = end;
                target.localScale = Vector3.one;
            }
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

            // Shockwave ring
            StartCoroutine(ShockwaveRing(boardRoot, color, animationEvent.Potion == PotionType.Bomb ? 400f : 600f));

            // Color burst
            var burst = new GameObject("Potion Burst", typeof(RectTransform), typeof(Image));
            burst.transform.SetParent(boardRoot, false);
            var rect = burst.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.one * (animationEvent.Potion == PotionType.Bomb ? 160f : 240f);
            var image = burst.GetComponent<Image>();
            image.color = new Color(color.r, color.g, color.b, 0.50f);
            image.raycastTarget = false;

            yield return Scale(rect, Vector3.one * 0.30f, Vector3.one * 1.5f, GameplayPresentationConfig.PotionBurstDuration);
            Object.Destroy(burst);
        }

        /// <summary>
        /// Expanding shockwave ring effect for potion activations.
        /// </summary>
        private static IEnumerator ShockwaveRing(RectTransform parent, Color color, float maxSize)
        {
            var ring = new GameObject("Shockwave", typeof(RectTransform), typeof(Image));
            ring.transform.SetParent(parent, false);
            var rect = ring.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.one * 30f;
            var image = ring.GetComponent<Image>();
            image.color = new Color(color.r, color.g, color.b, 0.45f);
            image.raycastTarget = false;

            var elapsed = 0f;
            var duration = GameplayPresentationConfig.ShockwaveExpandDuration;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = EasingFunctions.EaseOutQuart(t);
                rect.sizeDelta = Vector2.one * Mathf.Lerp(30f, maxSize, eased);
                image.color = new Color(color.r, color.g, color.b, 0.45f * (1f - t));
                yield return null;
            }

            Object.Destroy(ring);
        }

        private static IEnumerator Beam(RectTransform boardRoot, bool horizontal)
        {
            // Create glow trail behind beam
            var glow = new GameObject("Beam Glow", typeof(RectTransform), typeof(Image));
            glow.transform.SetParent(boardRoot, false);
            var glowRect = glow.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(0.5f, 0.5f);
            glowRect.anchorMax = new Vector2(0.5f, 0.5f);
            glowRect.sizeDelta = horizontal ? new Vector2(boardRoot.rect.width, 40f) : new Vector2(40f, boardRoot.rect.height);
            var glowImage = glow.GetComponent<Image>();
            glowImage.color = new Color(0.48f, 0.88f, 1f, 0.22f);
            glowImage.raycastTarget = false;

            // Main beam
            var beam = new GameObject("Line Potion Beam", typeof(RectTransform), typeof(Image));
            beam.transform.SetParent(boardRoot, false);
            var rect = beam.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = horizontal ? new Vector2(boardRoot.rect.width, 20f) : new Vector2(20f, boardRoot.rect.height);
            var image = beam.GetComponent<Image>();
            image.color = new Color(0.74f, 0.94f, 1f, 0.80f);
            image.raycastTarget = false;

            yield return Scale(rect, horizontal ? new Vector3(0.06f, 1f, 1f) : new Vector3(1f, 0.06f, 1f), Vector3.one, GameplayPresentationConfig.BeamDuration);

            // Fade out glow
            var elapsed = 0f;
            while (elapsed < 0.10f)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / 0.10f);
                glowImage.color = new Color(0.48f, 0.88f, 1f, 0.22f * (1f - t));
                image.color = new Color(0.74f, 0.94f, 1f, 0.80f * (1f - t));
                yield return null;
            }

            Object.Destroy(glow);
            Object.Destroy(beam);
        }

        private static IEnumerator ShuffleBoard(
            IReadOnlyDictionary<GridPosition, RectTransform> tileViews,
            IReadOnlyList<GridPosition> positions,
            RectTransform boardRoot)
        {
            yield return Pulse(boardRoot, 1.04f, GameplayPresentationConfig.BoardPulseDuration);
            var targets = ResolveTargets(tileViews, positions);
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.localScale = Vector3.one * 0.85f;
                }
            }

            var elapsed = 0f;
            const float duration = 0.20f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = EasingFunctions.EaseOutBack(Mathf.Clamp01(elapsed / duration), 1.2f);
                foreach (var target in targets)
                {
                    if (target != null)
                    {
                        target.localScale = Vector3.LerpUnclamped(Vector3.one * 0.85f, Vector3.one, t);
                    }
                }

                yield return null;
            }

            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.localScale = Vector3.one;
                }
            }
        }

        private static IEnumerator Pulse(RectTransform target, float scale, float duration)
        {
            yield return Scale(target, Vector3.one, Vector3.one * scale, duration * 0.5f);
            yield return Scale(target, Vector3.one * scale, Vector3.one, duration * 0.5f);
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
            return UiColorPalette.PotionColor(potion);
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
