using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.Editor
{
    public class PackageExport
    {
        const string CONFIG_PATH_UI = "Assets/Export/Editor/ExportSettings_Framework_UI.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_UI")]
        public static void ExportUIFramework()
        {
            ExportSettings exportSettings = AssetDatabase.LoadAssetAtPath<ExportSettings>(CONFIG_PATH_UI);
            List<string> configStr = new List<string>();
            for (int i = 0; i < exportSettings.includeObjects.Count; i++)
            {
                Object obj = exportSettings.includeObjects[i];
                if (obj != null){
                    string path = AssetDatabase.GetAssetPath(obj);
                    configStr.Add(path);
                    Debug.Log(path);
                }
            }
            AssetDatabase.ExportPackage(configStr.ToArray(), "Assets/Framework_UI.unitypackage", ExportPackageOptions.Recurse);
            EditorUtility.DisplayDialog("Export", "Complete!", "OK");
            AssetDatabase.Refresh();
        }

        const string CONFIG_PATH_ASSETMANAGEMENT = "Assets/Export/Editor/ExportSettings_Framework_AssetManagement.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_AssetManagement")]
        public static void ExportAssetManagementFramework()
        {
            ExportSettings exportSettings = AssetDatabase.LoadAssetAtPath<ExportSettings>(CONFIG_PATH_ASSETMANAGEMENT);
            List<string> configStr = new List<string>();
            for (int i = 0; i < exportSettings.includeObjects.Count; i++)
            {
                Object obj = exportSettings.includeObjects[i];
                if (obj != null)
                {
                    string path = AssetDatabase.GetAssetPath(obj);
                    configStr.Add(path);
                    Debug.Log(path);
                }
            }
            AssetDatabase.ExportPackage(configStr.ToArray(), "Assets/Framework_AssetManagement.unitypackage", ExportPackageOptions.Recurse);
            EditorUtility.DisplayDialog("Export", "Complete!", "OK");
            AssetDatabase.Refresh();
        }


    }

    [System.Serializable]
    public class PackageExportConfig
    {
        public string[] uiFramework;
    }
}