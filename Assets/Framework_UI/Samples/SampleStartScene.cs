using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
    public class SampleStartScene : MonoBehaviour
    {

        void Awake()
        {
#if UNITY_EDITOR
            CheckSampleScenes();
#endif
        }

        void Start()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Sample1");
        }

#if UNITY_EDITOR
        void CheckSampleScenes()
        {
            HashSet<string> editorSceneSet = new HashSet<string>();
            UnityEditor.EditorBuildSettingsScene[] editorSences = UnityEditor.EditorBuildSettings.scenes;
            List<UnityEditor.EditorBuildSettingsScene> editorSceneList = new List<UnityEditor.EditorBuildSettingsScene>();
            for (int i = 0; i < editorSences.Length; i++)
            {
                UnityEditor.EditorBuildSettingsScene editorBuildSettingsScene = editorSences[i];
                editorSceneSet.Add(editorBuildSettingsScene.path);
                editorSceneList.Add(editorBuildSettingsScene);
            }
            string sample1 = "Assets/Framework_UI/Scenes/Sample1.unity";
            string sample2 = "Assets/Framework_UI/Scenes/Sample2.unity";
            string sample3 = "Assets/Framework_UI/Scenes/SampleStartScene.unity";
            if (!editorSceneSet.Contains(sample1))
            {
                Debug.Log(string.Format("{0} add to editor settings.", sample1));
                editorSceneList.Add(new UnityEditor.EditorBuildSettingsScene(sample1, true));
            }
            if (!editorSceneSet.Contains(sample2))
            {
                Debug.Log(string.Format("{0} add to editor settings.", sample2));
                editorSceneList.Add(new UnityEditor.EditorBuildSettingsScene(sample2, true));
            }
            if (!editorSceneSet.Contains(sample3))
            {
                Debug.Log(string.Format("{0} add to editor settings.", sample3));
                editorSceneList.Add(new UnityEditor.EditorBuildSettingsScene(sample3, true));
            }
            UnityEditor.EditorBuildSettings.scenes = editorSceneList.ToArray();
        }
#endif
    }
}
