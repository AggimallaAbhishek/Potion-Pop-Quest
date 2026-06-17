using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class ButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private RectTransform _rect;
        private Image _image;
        private Color _originalColor;
        private Coroutine _scaleRoutine;
        private bool _colorCaptured;

        private void Awake()
        {
            _rect = transform as RectTransform;
            _image = GetComponent<Image>();
            if (_image != null)
            {
                _originalColor = _image.color;
                _colorCaptured = true;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ScaleTo(new Vector3(0.85f, 0.85f, 1f), useEaseIn: true);
            if (_image != null && _colorCaptured)
            {
                _image.color = Brighten(_originalColor, 0.12f);
            }

            SpawnRipple(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScaleTo(Vector3.one, useEaseIn: false);
            RestoreColor();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ScaleTo(Vector3.one, useEaseIn: false);
            RestoreColor();
        }

        private void RestoreColor()
        {
            if (_image != null && _colorCaptured)
            {
                _image.color = _originalColor;
            }
        }

        private void ScaleTo(Vector3 targetScale, bool useEaseIn)
        {
            if (_rect == null)
            {
                return;
            }

            if (_scaleRoutine != null)
            {
                StopCoroutine(_scaleRoutine);
            }

            _scaleRoutine = StartCoroutine(ScaleRoutine(targetScale, useEaseIn));
        }

        private IEnumerator ScaleRoutine(Vector3 end, bool useEaseIn)
        {
            var start = _rect.localScale;
            var duration = useEaseIn ? 0.06f : GameplayPresentationConfig.ButtonBounceBackDuration;
            var elapsed = 0f;

            while (elapsed < duration && _rect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = useEaseIn ? t * t : EasingFunctions.EaseOutElasticGentle(t);
                _rect.localScale = Vector3.LerpUnclamped(start, end, eased);
                yield return null;
            }

            if (_rect != null)
            {
                _rect.localScale = end;
            }
        }

        private void SpawnRipple(PointerEventData eventData)
        {
            if (_rect == null)
            {
                return;
            }

            var rippleObject = new GameObject("Ripple", typeof(RectTransform), typeof(Image));
            rippleObject.transform.SetParent(transform, false);
            var rippleRect = rippleObject.GetComponent<RectTransform>();
            rippleRect.anchorMin = new Vector2(0.5f, 0.5f);
            rippleRect.anchorMax = new Vector2(0.5f, 0.5f);
            rippleRect.sizeDelta = new Vector2(24f, 24f);

            // Position ripple at press point
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _rect, eventData.position, eventData.pressEventCamera, out var localPoint))
            {
                rippleRect.anchoredPosition = localPoint;
            }
            else
            {
                rippleRect.anchoredPosition = Vector2.zero;
            }

            var rippleImage = rippleObject.GetComponent<Image>();
            rippleImage.color = new Color(1f, 1f, 1f, 0.25f);
            rippleImage.raycastTarget = false;

            StartCoroutine(RippleRoutine(rippleRect, rippleImage));
        }

        private static IEnumerator RippleRoutine(RectTransform rect, Image image)
        {
            const float duration = 0.32f;
            var elapsed = 0f;
            var startSize = rect.sizeDelta;
            var endSize = startSize * 6f;

            while (elapsed < duration && rect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = EasingFunctions.EaseOutQuart(t);
                rect.sizeDelta = Vector2.Lerp(startSize, endSize, eased);
                if (image != null)
                {
                    image.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.25f, 0f, eased));
                }

                yield return null;
            }

            if (rect != null)
            {
                Object.Destroy(rect.gameObject);
            }
        }

        private static Color Brighten(Color color, float amount)
        {
            return new Color(
                Mathf.Clamp01(color.r + amount),
                Mathf.Clamp01(color.g + amount),
                Mathf.Clamp01(color.b + amount),
                color.a);
        }
    }
}
