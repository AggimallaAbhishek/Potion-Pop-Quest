using UnityEngine;

namespace PotionPopQuest.Unity
{
    internal static class UiLayoutMetrics
    {
        public const float ScreenMaxWidth = 960f;
        public const float HudWidth = 960f;
        public const float HudHeight = 110f; // Was 174f
        public const float StarProgressWidth = 920f;
        public const float StarProgressHeight = 0f; // Was 54f
        public const float BoardSize = 780f;
        public const float MessageWidth = 900f;
        public const float MessageHeight = 56f;
        public const float TutorialWidth = 920f;
        public const float TutorialHeight = 82f;
        public const float ActionsWidth = 920f;
        public const float ActionsHeight = 78f;
        public const float TouchHeight = 70f;
        public const float ModalWidth = 740f;
        public const float TileCornerRadius = 14f;
        public const float ButtonCornerRadius = 16f;
        public const float PanelCornerRadius = 12f;

        public static float GameHudHeight()
        {
            return IsShortWideScreen() ? 134f : HudHeight;
        }

        public static float GameStarProgressHeight()
        {
            return IsShortWideScreen() ? 46f : StarProgressHeight;
        }

        public static float GameBoardSize()
        {
            return IsShortWideScreen() ? 640f : BoardSize;
        }

        public static float GameMessageHeight()
        {
            return IsShortWideScreen() ? 44f : MessageHeight;
        }

        public static float GameTutorialHeight()
        {
            return IsShortWideScreen() ? 66f : TutorialHeight;
        }

        public static float GameActionsHeight()
        {
            return IsShortWideScreen() ? 62f : ActionsHeight;
        }

        public static float GameTouchHeight()
        {
            return IsShortWideScreen() ? 58f : TouchHeight;
        }

        public static int GameScreenSpacing()
        {
            return IsShortWideScreen() ? 10 : 20;
        }

        public static RectOffset GameScreenPadding()
        {
            return IsShortWideScreen()
                ? new RectOffset(24, 24, 20, 20)
                : new RectOffset(36, 36, 72, 72);
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
