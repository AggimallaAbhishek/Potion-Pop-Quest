using UnityEngine;

namespace PotionPopQuest.Unity
{
    internal static class UiLayoutMetrics
    {
        public const float ScreenMaxWidth = 920f;
        public const float HudWidth = 920f;
        public const float HudHeight = 92f;
        public const float StarProgressWidth = 880f;
        public const float StarProgressHeight = 0f; // Was 54f
        public const float BoardSize = 800f;
        public const float MessageWidth = 860f;
        public const float MessageHeight = 34f;
        public const float TutorialWidth = 880f;
        public const float TutorialHeight = 64f;
        public const float ActionsWidth = 880f;
        public const float ActionsHeight = 62f;
        public const float TouchHeight = 56f;
        public const float ModalWidth = 740f;
        public const float TileCornerRadius = 10f;
        public const float ButtonCornerRadius = 50f;
        public const float PanelCornerRadius = 20f;

        public static float GameHudHeight()
        {
            return IsShortWideScreen() ? 126f : HudHeight;
        }

        public static float GameStarProgressHeight()
        {
            return IsShortWideScreen() ? 46f : StarProgressHeight;
        }

        public static float GameBoardSize()
        {
            return IsShortWideScreen() ? 630f : BoardSize;
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
            return IsShortWideScreen() ? 8 : 14;
        }

        public static RectOffset GameScreenPadding()
        {
            return IsShortWideScreen()
                ? new RectOffset(24, 24, 18, 18)
                : new RectOffset(34, 34, 48, 42);
        }

        public static int LevelSelectColumnCount()
        {
            return IsShortWideScreen() ? 5 : 4;
        }

        public static float MenuContentWidth()
        {
            return IsShortWideScreen() ? 760f : 820f;
        }

        public static RectOffset ScreenPadding()
        {
            return IsShortWideScreen()
                ? new RectOffset(34, 34, 86, 40)
                : new RectOffset(40, 40, 132, 70);
        }

        private static bool IsShortWideScreen()
        {
            return Screen.width > Screen.height && Screen.height > 0;
        }
    }
}
