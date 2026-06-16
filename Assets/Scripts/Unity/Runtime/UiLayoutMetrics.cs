using UnityEngine;

namespace PotionPopQuest.Unity
{
    internal static class UiLayoutMetrics
    {
        public const float ScreenMaxWidth = 920f;
        public const float HudWidth = 920f;
        public const float HudHeight = 96f;
        public const float StarProgressWidth = 880f;
        public const float StarProgressHeight = 0f; // Was 54f
        public const float BoardSize = 780f;
        public const float MessageWidth = 860f;
        public const float MessageHeight = 42f;
        public const float TutorialWidth = 880f;
        public const float TutorialHeight = 64f;
        public const float ActionsWidth = 880f;
        public const float ActionsHeight = 66f;
        public const float TouchHeight = 58f;
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
                : new RectOffset(34, 34, 58, 48);
        }

        public static int LevelSelectColumnCount()
        {
            return 3;
        }

        private static bool IsShortWideScreen()
        {
            return Screen.width > Screen.height && Screen.height > 0;
        }
    }
}
