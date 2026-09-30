#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Turret_Rush.Editor
{
    [InitializeOnLoad]
    public static class AutoSceneSaver
    {
        private const double SaveIntervalSeconds = 30.0;

        private static double _nextSaveTime;

        static AutoSceneSaver()
        {
            _nextSaveTime =
                EditorApplication.timeSinceStartup +
                SaveIntervalSeconds;

            EditorApplication.update += Update;

            EditorApplication.playModeStateChanged +=
                OnPlayModeStateChanged;

            EditorApplication.quitting += Save;
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup <
                _nextSaveTime)
            {
                return;
            }

            _nextSaveTime =
                EditorApplication.timeSinceStartup +
                SaveIntervalSeconds;

            Save();
        }

        private static void OnPlayModeStateChanged(
            PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                Save();
        }

        private static void Save()
        {
            if (EditorApplication.isPlaying ||
                EditorApplication.isPlayingOrWillChangePlaymode ||
                EditorApplication.isCompiling)
            {
                return;
            }

            bool savedAnything = false;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene =
                    SceneManager.GetSceneAt(i);

                if (!scene.isLoaded ||
                    !scene.isDirty ||
                    string.IsNullOrEmpty(scene.path))
                {
                    continue;
                }

                EditorSceneManager.SaveScene(scene);
                savedAnything = true;
            }

            if (!savedAnything)
                return;

            AssetDatabase.SaveAssets();

            Debug.Log(
                "[AutoSave] Scene changes saved."
            );
        }
    }
}

#endif