using UnityEngine;
using UnityEditor;
using BlueNoah.IO;
using System.IO;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleSettingService
    {
        public void SetAssetBundleNames()
        {
            if (EditorUtility.DisplayDialog("Auto Set AssetBundle Name", "It will remove all the assetBundle settings even if have been used , and reset it by framework's rule. Go on?", "OK", "Cancel"))
            {
                string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
                for (int i = 0; i < assetBundleNames.Length; i++)
                {
                    AssetDatabase.RemoveAssetBundleName(assetBundleNames[i], true);
                    Debug.Log(string.Format("Remove old assetBundle name : {0}", assetBundleNames[i]));
                }
                string[] assetBundleTypeFolderPaths = GetAssetBundleTypeFolderPaths();
                for (int i = 0; i < assetBundleTypeFolderPaths.Length; i++)
                {
                    SetAssetBundleNameWithFolderNames(assetBundleTypeFolderPaths[i]);
                }
            }
        }

        void SetAssetBundleNameWithFolderNames(string path)
        {
            string[] assetBundleFolders = GetAssetBundleFolderPaths(path);
            for (int i = 0; i < assetBundleFolders.Length; i++)
            {
                SetAssetBundleNameWithFolderName(assetBundleFolders[i]);
            }
        }

        //TODO Need to explain.
        void SetAssetBundleNameWithFolderName(string path)
        {
            string abName = GetAssetBundleNameByPath(path);
            AssetImporter assetImporter = GetAssetImporter(path);
            assetImporter.SetAssetBundleNameAndVariant(abName, "ab");
            Debug.Log(string.Format("Set assetBundle folder : <color='#3EFF00FF'>{0}</color> , name <color='#FFDF29FF'>{1}</color>",path,abName));
        }

        string GetAssetBundleNameByPath(string path)
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
