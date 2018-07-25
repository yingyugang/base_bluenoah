using UnityEngine;
using System.Collections;
using UnityEditor;
using BlueNoah.IO;
using System.IO;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleBuildServiceSetting
    {
        const string ASSETBUNDLE_RESOURCES_PATH = "/Framework_AssetManagement/AssetBundleResources/";
        AssetBundleBuildWindow mAssetBundleBuildWindow;

        public AssetBundleBuildServiceSetting(AssetBundleBuildWindow assetBundleBuildWindow)
        {
            mAssetBundleBuildWindow = assetBundleBuildWindow;
        }

        protected static string SYSTEM_ASSETBUNDLE_RESOURCES_PATH
        {
            get
            {
                return Application.dataPath + ASSETBUNDLE_RESOURCES_PATH + "/";
            }
        }

        protected static string ASSETDATABASE_ASSETBUNDLE_RESOURCES_PATH
        {
            get
            {
                return "Assets/" + ASSETBUNDLE_RESOURCES_PATH + "/";
            }
        }

        public void SetAssetBundleNames()
        {
            string[] assetBundleTypeFolderPaths = GetAssetBundleTypeFolderPaths();
            for (int i = 0; i < assetBundleTypeFolderPaths.Length; i++)
            {
                SetABNameWithFolderNames(assetBundleTypeFolderPaths[i]);
            }
        }

        void SetABNameWithFolderNames(string path)
        {
            string[] assetBundleFolders = GetAssetBundleFolderPaths(path);
            for (int i = 0; i < assetBundleFolders.Length; i++)
            {
                SetABNameWithFolderName(assetBundleFolders[i]);
            }
        }

        void SetABNameWithFolderName(string path)
        {
            string abName = GetABNameByPath(path);
            AssetImporter assetImporter = GetAssetImporter(path);
            assetImporter.SetAssetBundleNameAndVariant(abName, "ab");
        }

        string GetABNameByPath(string path)
        {
            return path.Substring(path.IndexOf(ASSETBUNDLE_RESOURCES_PATH, System.StringComparison.CurrentCulture) + 1);
        }

        string GetAssetBundleSubFolder(string path)
        {
            return path.Substring(path.IndexOf("/Assets/", System.StringComparison.CurrentCulture) + 1);
        }

        string[] GetAssetBundleTypeFolderPaths()
        {
            return FileManager.GetDirectories(SYSTEM_ASSETBUNDLE_RESOURCES_PATH, "*", SearchOption.TopDirectoryOnly);
        }

        string[] GetAssetBundleFolderPaths(string path)
        {
            return FileManager.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        AssetImporter GetAssetImporter(string path)
        {
            string assetPath = FileManager.SystemPathToAssetDataPath(path);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            return assetImporter;
        }
    }
}
