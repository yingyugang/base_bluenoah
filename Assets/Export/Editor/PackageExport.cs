using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.Editor
{
    public class PackageExport
    {
        const string CONFIG_PATH = "Assets/Export/Editor/UIExportSettings.asset";

        [MenuItem("Tools/ExportPackage/UIFramework")]
        public static void ExportUIFramework()
        {
            ExportSettings exportSettings = AssetDatabase.LoadAssetAtPath<ExportSettings>(CONFIG_PATH);//  FileManager.ReadString(EditorFileManager.AssetDatabasePathToFilePath(CONFIG_PATH));
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
            AssetDatabase.ExportPackage(configStr.ToArray(), "Assets/UIFramework.unitypackage", ExportPackageOptions.Recurse);
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