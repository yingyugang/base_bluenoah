using UnityEngine;
using UnityEditor;
using BlueNoah.IO;
using System.IO;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleBuildServiceSetting
    {
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
            Debug.Log(abName);
        }

        string GetABNameByPath(string path)
        {
            return path.Substring(path.IndexOf(AssetBundleEditorConstant.ASSETBUNDLE_RESOURCES_PATH, System.StringComparison.CurrentCulture) + AssetBundleEditorConstant.ASSETBUNDLE_RESOURCES_PATH.Length);
        }

        string GetAssetBundleSubFolder(string path)
        {
            return path.Substring(path.IndexOf("/Assets/", System.StringComparison.CurrentCulture) + 1);
        }

        string[] GetAssetBundleTypeFolderPaths()
        {
            return FileManager.GetDirectories(AssetBundleEditorConstant.SYSTEM_ASSETBUNDLE_RESOURCES_PATH, "*", SearchOption.TopDirectoryOnly);
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
