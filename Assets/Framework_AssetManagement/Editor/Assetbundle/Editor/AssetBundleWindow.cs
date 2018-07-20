using System.Collections.Generic;
using System.IO;
using BlueNoah.Download;
using BlueNoah.IO;
using UnityEditor;
using UnityEngine;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public abstract class AssetBundleWindow : EditorWindow
    {
        
        protected AssetConfig mAssetBundleConfig;

        protected Dictionary<string, AssetBundleWindowItem> mAssetbundleItemDic;

        protected List<AssetBundleWindowItem> mAssetBundleItemList;

        protected virtual void OnEnable()
        {
            LoadAssetBundleInfos();
        }

        protected void InitContent(string title, string tooltip)
        {
            GUIContent guiContent = new GUIContent();
            guiContent.text = title;
            guiContent.tooltip = tooltip;
            this.titleContent = guiContent;
        }

        protected void LoadAssetBundleInfos()
        {
            mAssetBundleConfig = LoadAssetBundleConfig();
            mAssetbundleItemDic = new Dictionary<string, AssetBundleWindowItem>();
            mAssetBundleItemList = new List<AssetBundleWindowItem>();
            InitAssetBundleWindowItemsFromEditorABSetting();
        }

        void InitAssetBundleWindowItemsFromEditorABSetting()
        {
            string[] assetbundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetbundleNames.Length; i++)
            {
                InitAssetBundleWindowItemFromEditorABSetting(assetbundleNames[i]);
            }
        }

        void InitAssetBundleWindowItemFromEditorABSetting(string assetbundleName)
        {
            AssetBundleWindowItem assetBundleWindowItem = new AssetBundleWindowItem();
            assetBundleWindowItem.assetBundleName = assetbundleName;
            string path = AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH + assetbundleName;
            if (FileManager.Exists(path))
            {
                assetBundleWindowItem.assetBundle = LoadAssetBunleObject(assetbundleName);
                assetBundleWindowItem.assetBundleHash = FileManager.GetFileHash(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH + assetbundleName);
                assetBundleWindowItem.assetBundleLength = new FileInfo(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH + assetbundleName).Length;
            }
            assetBundleWindowItem.displayLength = FileLengthToStr(assetBundleWindowItem.assetBundleLength);
            AddAssetBundleItem(assetbundleName, assetBundleWindowItem);
        }

        //TODO Get AssetBundle Main Folder.
        Object GetAssetBundleMainFolder(string assetBundleName)
        {
            return null;
        }

        void AddAssetBundleItem(string assetbundleName, AssetBundleWindowItem assetBundleWindowItem)
        {
            mAssetbundleItemDic.Add(assetbundleName, assetBundleWindowItem);
            mAssetBundleItemList.Add(assetBundleWindowItem);
        }

        Object LoadAssetBunleObject(string assetbundleName)
        {

            string path = AssetBundleEditorConstant.ASSETDATABASE_PLATFORM_PATH + assetbundleName;
            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }

        AssetConfig LoadAssetBundleConfig()
        {
            if (!FileManager.Exists(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE))
            {
                InitConfigFile();
            }
            string configString = FileManager.ReadString(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE);
            return JsonUtility.FromJson<AssetConfig>(configString);
        }

        void InitConfigFile()
        {
            FileManager.WriteString(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE, JsonUtility.ToJson(CreateAssetBundleConfig()));
        }

        AssetConfig CreateAssetBundleConfig()
        {
            AssetConfig config = new AssetConfig();
            config.items = new List<AssetConfigItem>();
            return config;
        }

        protected string FileLengthToStr(long length)
        {
            string key = " B";
            float f = length;
            if (f >= 1024)
            {
                f = f / 1024f;
                key = " K";
            }
            if (f >= 1024)
            {
                f = f / 1024f;
                key = " M";
            }
            int v = Mathf.RoundToInt(f * 100);
            return v / 100f + key;
        }

    }

    public class AssetBundleWindowItem
    {
        public string assetBundleName;
        public string assetBundleHash;
        public long assetBundleLength;
        public Object assetBundle;
        public Object resourcesFolder;
        public string displayLength;
        public bool isSelected;
    }
}