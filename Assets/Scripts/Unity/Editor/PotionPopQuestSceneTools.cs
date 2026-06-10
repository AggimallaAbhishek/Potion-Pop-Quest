using System.IO;
using PotionPopQuest.Unity;
using UnityEditor;
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
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[PotionPopQuest][Editor] Created MVP scene and registered it in build settings.");
        }
    }
}

