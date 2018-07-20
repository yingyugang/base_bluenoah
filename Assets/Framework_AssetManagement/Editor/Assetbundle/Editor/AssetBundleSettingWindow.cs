using UnityEngine;
using UnityEditor;
using System.IO;
using BlueNoah.IO;
using System.Collections.Generic;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleSettingWindow : AssetBundleWindow
    {

        const string ASSETBUNDLE_RESOURCES_PATH = "/Framework_AssetManagement/AssetBundleResources/";

        static AssetBundleSettingWindow mAssetBundleSettingWindow;
        AssetBundleSettingWindowGUI mAssetBundleWindowGUI;
        List<AssetBundleWindowItem> mAllAssetBundleEntities;

        [MenuItem(AssetBundleEditorConstant.ASSETBUNDLE_SETTING_WINDOW_MENUITEM + MenuItemShortcutKeyConstant.SHORTCUT_KEY_ASSETBUNDLE_SETTING)]
        static void Init()
        {
            mAssetBundleSettingWindow = GetWindow<AssetBundleSettingWindow>(true, "AB Settings", true);
            mAssetBundleSettingWindow.Show();
            mAssetBundleSettingWindow.Focus();
        }

        protected static string SYSTEM_ASSETBUNDLE_RESOURCES_PATH
        {
            get
            {
                return Application.dataPath + ASSETBUNDLE_RESOURCES_PATH  + "/";
            }
        }

        protected static string ASSETDATABASE_ASSETBUNDLE_RESOURCES_PATH
        {
            get
            {
                return "Assets/" + ASSETBUNDLE_RESOURCES_PATH + "/";
            }
        }


        protected override void OnEnable()
        {
            base.OnEnable();
        }

		private void OnGUI()
		{
            if(GUILayout.Button("Set AB Names")){
                SetABNames();
            }
		}

		void SetABNames()
        {
            string[] assetBundleTypeFolderPaths = GetAssetBundleTypeFolderPaths();
            for (int i = 0; i < assetBundleTypeFolderPaths.Length; i++)
            {
                SetABNameWithFolderNames(assetBundleTypeFolderPaths[i]);
            }
        }

        void SetABNameWithFolderNames(string path){
            string[] assetBundleFolders = GetAssetBundleFolderPaths(path);
            for (int i = 0; i < assetBundleFolders.Length; i++)
            {
                SetABNameWithFolderName(assetBundleFolders[i]);
            }
        }

        void SetABNameWithFolderName(string path){
            string abName = GetABNameByPath(path);
            AssetImporter assetImporter = GetAssetImporter(path);
            assetImporter.SetAssetBundleNameAndVariant(abName, "ab");
        }

        string GetABNameByPath(string path){
            return path.Substring(path.IndexOf(ASSETBUNDLE_RESOURCES_PATH, System.StringComparison.CurrentCulture) + 1);
        }

        string GetAssetBundleSubFolder(string path){
            return path.Substring(path.IndexOf("/Assets/", System.StringComparison.CurrentCulture) + 1);
        }

        string[] GetAssetBundleTypeFolderPaths(){
            return FileManager.GetDirectories(SYSTEM_ASSETBUNDLE_RESOURCES_PATH, "*", SearchOption.TopDirectoryOnly);
        }

        string[] GetAssetBundleFolderPaths(string path){
            return  FileManager.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
        }

        AssetImporter GetAssetImporter(string path){
            string assetPath = FileManager.SystemPathToAssetDataPath(path);
            AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
            return assetImporter;
        }
    }
}
