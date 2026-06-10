using UnityEngine;

namespace PotionPopQuest.Unity
{
    public static class PotionPopQuestBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateRuntime()
        {
            if (Object.FindFirstObjectByType<GameController>() != null)
            {
                return;
            }

            var root = new GameObject("Potion Pop Quest Runtime");
            Object.DontDestroyOnLoad(root);
            root.AddComponent<GameController>();
        }
    }
}

