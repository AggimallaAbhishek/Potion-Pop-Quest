using UnityEngine;

namespace PotionPopQuest.Unity
{
    internal static class UiLayoutMetrics
    {
        public const float ScreenMaxWidth = 940f;
        public const float HudWidth = 940f;
        public const float HudHeight = 168f;
        public const float StarProgressWidth = 900f;
        public const float StarProgressHeight = 52f;
        public const float BoardSize = 760f;
        public const float MessageWidth = 880f;
        public const float MessageHeight = 54f;
        public const float TutorialWidth = 900f;
        public const float TutorialHeight = 78f;
        public const float ActionsWidth = 900f;
        public const float ActionsHeight = 74f;
        public const float TouchHeight = 68f;
        public const float ModalWidth = 720f;

        public static int LevelSelectColumnCount()
        {
            var width = Mathf.Max(Screen.width, Screen.height);
            if (width <= 900)
            {
                return 3;
            }

            if (width <= 1300)
            {
                return 4;
            }

            return 5;
        }
    }
}
