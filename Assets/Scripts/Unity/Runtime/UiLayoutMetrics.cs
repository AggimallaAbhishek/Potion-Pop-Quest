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

        public static float GameHudHeight()
        {
            return IsShortWideScreen() ? 128f : HudHeight;
        }

        public static float GameStarProgressHeight()
        {
            return IsShortWideScreen() ? 44f : StarProgressHeight;
        }

        public static float GameBoardSize()
        {
            return IsShortWideScreen() ? 620f : BoardSize;
        }

        public static float GameMessageHeight()
        {
            return IsShortWideScreen() ? 42f : MessageHeight;
        }

        public static float GameTutorialHeight()
        {
            return IsShortWideScreen() ? 64f : TutorialHeight;
        }

        public static float GameActionsHeight()
        {
            return IsShortWideScreen() ? 60f : ActionsHeight;
        }

        public static float GameTouchHeight()
        {
            return IsShortWideScreen() ? 56f : TouchHeight;
        }

        public static int GameScreenSpacing()
        {
            return IsShortWideScreen() ? 12 : 24;
        }

        public static RectOffset GameScreenPadding()
        {
            return IsShortWideScreen()
                ? new RectOffset(28, 28, 24, 24)
                : new RectOffset(40, 40, 80, 80);
        }

        public static int LevelSelectColumnCount()
        {
            var width = Mathf.Min(Screen.width, Screen.height);
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

        private static bool IsShortWideScreen()
        {
            return Screen.width > Screen.height && Screen.height > 0;
        }
    }
}
