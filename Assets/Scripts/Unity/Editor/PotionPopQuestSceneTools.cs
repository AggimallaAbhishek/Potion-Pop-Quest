using System.IO;
using PotionPopQuest.Core;
using PotionPopQuest.Unity;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PotionPopQuest.Editor
{
    public static class PotionPopQuestSceneTools
    {
        private const string ScenePath = "Assets/Scenes/GameScene.unity";

        [MenuItem("Potion Pop Quest/Create MVP Scene")]
        public static void CreateMvpScene()
        {
            Directory.CreateDirectory("Assets/Scenes");

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var cameraObject = new GameObject("Main Camera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.07f, 0.09f, 0.12f);
            camera.orthographic = true;
            cameraObject.tag = "MainCamera";

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();

            var runtime = new GameObject("Potion Pop Quest");
            runtime.AddComponent<GameController>();

            EditorSceneManager.SaveScene(scene, ScenePath);
            RegisterSceneInBuildSettings();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[PotionPopQuest][Editor] Created MVP scene and registered it in build settings.");
        }

        [MenuItem("Potion Pop Quest/Configure Build Settings")]
        public static void ConfigureBuildSettings()
        {
            RegisterSceneInBuildSettings();
            PlayerSettings.companyName = "Potion Pop Quest";
            PlayerSettings.productName = "Potion Pop Quest";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.potionpopquest.game");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.WebGL.nameFilesAsHashes = true;
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;
            AssetDatabase.SaveAssets();
            Debug.Log("[PotionPopQuest][Editor] Configured Android/WebGL build settings.");
        }

        [MenuItem("Potion Pop Quest/QA/Unlock All MVP Levels")]
        public static void UnlockAllMvpLevels()
        {
            var levels = MvpLevelCatalog.CreateLevels();
            var saveData = new SaveData
            {
                highestUnlockedLevel = levels.Count,
                musicEnabled = true,
                sfxEnabled = true
            };

            foreach (var level in levels)
            {
                var score = level.StarThresholds.ThreeStars;
                SaveProgressService.ApplyLevelCompleted(
                    saveData,
                    level.LevelNumber,
                    score,
                    3,
                    hasNextLevel: level.LevelNumber < levels.Count);
            }

            new PlayerPrefsSaveRepository(new NullGameLogger()).Save(saveData);
            Debug.Log("[PotionPopQuest][Editor] Unlocked all MVP levels for local QA.");
        }

        [MenuItem("Potion Pop Quest/QA/Reset Local Progress")]
        public static void ResetLocalProgress()
        {
            new PlayerPrefsSaveRepository(new NullGameLogger()).Reset();
            Debug.Log("[PotionPopQuest][Editor] Reset local Potion Pop Quest progress.");
        }

        private static void RegisterSceneInBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
        }
    }
}
