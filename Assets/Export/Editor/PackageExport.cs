using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.Editor
{
    public class PackageExport
    {

        const string CONFIG_PATH_ASSETBUNDLE = "Assets/Export/Editor/ExportSettings_AssetBundle.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_AssetBundle")]
        public static void ExportFrameworkAssetBundle()
        {
            ExportPackage(CONFIG_PATH_ASSETBUNDLE,"Assets/Framework_AssetBundle.unitypackage");
        }

        const string CONFIG_PATH_API = "Assets/Export/Editor/ExportSettings_API.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_API")]
        public static void ExportFrameworkAPI()
        {
            ExportPackage(CONFIG_PATH_API, "Assets/Framework_API.unitypackage");
        }

        const string CONFIG_PATH_COMMON = "Assets/Export/Editor/ExportSettings_Common.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_Common")]
        public static void ExportFrameworkCommon()
        {
            ExportPackage(CONFIG_PATH_COMMON, "Assets/Framework_Common.unitypackage");
        }

        const string CONFIG_PATH_CSV = "Assets/Export/Editor/ExportSettings_CSV.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_CSV")]
        public static void ExportFrameworkCSV()
        {
            ExportPackage(CONFIG_PATH_CSV, "Assets/Framework_CSV.unitypackage");
        }

        const string CONFIG_PATH_DOWNLOAD = "Assets/Export/Editor/ExportSettings_Download.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_Download")]
        public static void ExportFrameworkDownload()
        {
            ExportPackage(CONFIG_PATH_DOWNLOAD, "Assets/Framework_Download.unitypackage");
        }

        const string CONFIG_PATH_UI = "Assets/Export/Editor/ExportSettings_UI.asset";

        [MenuItem("Tools/BlueNoah/ExportPackage/Framework_UI")]
        public static void ExportFrameworkUI()
        {
            ExportPackage(CONFIG_PATH_UI, "Assets/Framework_UI.unitypackage");
        }

        public static void ExportPackage(string configPath , string packagePath){
            ExportSettings exportSettings = AssetDatabase.LoadAssetAtPath<ExportSettings>(configPath);
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
            AssetDatabase.ExportPackage(configStr.ToArray(), packagePath, ExportPackageOptions.Recurse);
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