using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PotionPopQuest.Unity
{
    public sealed class ButtonPressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        private RectTransform _rect;
        private Coroutine _scaleRoutine;

        private void Awake()
        {
            _rect = transform as RectTransform;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ScaleTo(0.96f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ScaleTo(1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ScaleTo(1f);
        }

        private void ScaleTo(float targetScale)
        {
            if (_rect == null)
            {
                return;
            }

            if (_scaleRoutine != null)
            {
                StopCoroutine(_scaleRoutine);
            }

            _scaleRoutine = StartCoroutine(ScaleRoutine(targetScale));
        }

        private IEnumerator ScaleRoutine(float targetScale)
        {
            var start = _rect.localScale;
            var end = Vector3.one * targetScale;
            const float duration = 0.07f;
            var elapsed = 0f;

            while (elapsed < duration && _rect != null)
            {
                elapsed += Time.unscaledDeltaTime;
                _rect.localScale = Vector3.Lerp(start, end, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            if (_rect != null)
            {
                _rect.localScale = end;
            }
        }
    }
}
