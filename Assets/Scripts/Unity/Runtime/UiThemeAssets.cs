using UnityEngine;
using UnityEngine.UI;

namespace PotionPopQuest.Unity
{
    public sealed class UiThemeAssets
    {
        private const string DisplayFontPath = "Fonts/PPQ_Display";
        private Font _font;

        public Font Font
        {
            get
            {
                if (_font != null)
                {
                    return _font;
                }

                _font = Resources.Load<Font>(DisplayFontPath);
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                }

                return _font;
            }
        }

        public void AddHighValueTextShadow(Text text)
        {
            if (text == null || text.GetComponent<Shadow>() != null)
            {
                return;
            }

            var shadow = text.gameObject.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(2f, -2f);
            shadow.useGraphicAlpha = true;
        }
    }
}
