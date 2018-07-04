using UnityEditor;
using UnityEngine;

namespace BlueNoah.Editor
{
    public class PackageExport
    {
        const string CONFIG_PATH = "Assets/Scripts/Editor/Export/PackageExportConfig.json";

        [MenuItem("Tools/ExportPackage/UIFramework")]
        public static void ExportUIFramework()
        {
            string context = FileManager.ReadString(EditorFileManager.AssetDatabasePathToFilePath(CONFIG_PATH));
            PackageExportConfig config = JsonUtility.FromJson<PackageExportConfig>(context);
            AssetDatabase.ExportPackage(config.uiFramework, "Assets/UIFramework.unitypackage", ExportPackageOptions.Recurse);
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