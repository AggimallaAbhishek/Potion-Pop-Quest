using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    /// <summary>
    /// Spawns floating ambient particles that drift upward with a gentle horizontal
    /// sine-wave, creating a magical atmosphere behind the game board.
    /// Attach to a Canvas or child RectTransform.
    /// </summary>
    public sealed class AmbientParticleView : MonoBehaviour
    {
        private RectTransform[] _particles;
        private Image[] _images;
        private float[] _speeds;
        private float[] _phases;
        private float[] _sizes;
        private int _count;
        private RectTransform _container;

        public void Initialize(RectTransform container, int count = -1)
        {
            _container = container;
            _count = count > 0 ? count : GameplayPresentationConfig.AmbientParticleCount;
            _particles = new RectTransform[_count];
            _images = new Image[_count];
            _speeds = new float[_count];
            _phases = new float[_count];
            _sizes = new float[_count];

            for (var i = 0; i < _count; i++)
            {
                var particle = new GameObject($"AmbientParticle_{i}", typeof(RectTransform), typeof(Image));
                particle.transform.SetParent(container, false);

                var rect = particle.GetComponent<RectTransform>();
                var size = Random.Range(
                    GameplayPresentationConfig.AmbientParticleMinSize,
                    GameplayPresentationConfig.AmbientParticleMaxSize);
                rect.sizeDelta = new Vector2(size, size);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                // Random starting position across the full screen area
                var startX = Random.Range(-480f, 480f);
                var startY = Random.Range(-960f, 960f);
                rect.anchoredPosition = new Vector2(startX, startY);

                var image = particle.GetComponent<Image>();
                var colorIndex = i % UiColorPalette.ParticleGlow.Length;
                image.color = UiColorPalette.ParticleGlow[colorIndex];
                image.raycastTarget = false;

                // Add layout ignore
                var layout = particle.AddComponent<LayoutElement>();
                layout.ignoreLayout = true;

                _particles[i] = rect;
                _images[i] = image;
                _speeds[i] = Random.Range(0.6f, 1.4f);
                _phases[i] = Random.Range(0f, Mathf.PI * 2f);
                _sizes[i] = size;
            }
        }

        private void Update()
        {
            if (_particles == null || _container == null)
            {
                return;
            }

            var baseSpeed = GameplayPresentationConfig.AmbientParticleSpeed;
            var drift = GameplayPresentationConfig.AmbientParticleDrift;
            var containerHeight = _container.rect.height;
            var halfHeight = containerHeight * 0.5f + 40f;

            for (var i = 0; i < _count; i++)
            {
                if (_particles[i] == null)
                {
                    continue;
                }

                var pos = _particles[i].anchoredPosition;

                // Float upward
                pos.y += baseSpeed * _speeds[i] * Time.unscaledDeltaTime;

                // Gentle horizontal sine drift
                pos.x += Mathf.Sin(Time.unscaledTime * 0.8f + _phases[i]) * drift * Time.unscaledDeltaTime;

                // Wrap around when leaving top
                if (pos.y > halfHeight)
                {
                    pos.y = -halfHeight;
                    pos.x = Random.Range(-480f, 480f);
                }

                _particles[i].anchoredPosition = pos;

                // Gentle alpha pulsing
                if (_images[i] != null)
                {
                    var alpha = _images[i].color.a;
                    var baseAlpha = UiColorPalette.ParticleGlow[i % UiColorPalette.ParticleGlow.Length].a;
                    var pulse = Mathf.Sin(Time.unscaledTime * 1.2f + _phases[i]) * 0.15f;
                    var c = _images[i].color;
                    _images[i].color = new Color(c.r, c.g, c.b, Mathf.Clamp01(baseAlpha + pulse));
                }
            }
        }

        private void OnDestroy()
        {
            if (_particles == null)
            {
                return;
            }

            for (var i = 0; i < _particles.Length; i++)
            {
                if (_particles[i] != null)
                {
                    Destroy(_particles[i].gameObject);
                }
            }

            _particles = null;
        }
    }
}
