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
        protected const string ASSETBUNDLE_PATH = "/Framework_AssetManagement/AssetBundleBuilds/";

        protected const string CONFIG_FILE = "assetbundle_config.json";

        protected AssetConfig mAssetBundleConfig;

        protected Dictionary<string, AssetBundleWindowItem> mAssetbundleItemDic;

        protected List<AssetBundleWindowItem> mAssetBundleItemList;

        protected static string ASSETBUNDLE_PLATFORM_PATH
        {
            get
            {
                return Application.dataPath + ASSETBUNDLE_PATH + DownloadConstant.ASSET_PLATFORM + "/";
            }
        }

        protected static string ASSETDATABASE_PLATFORM_PATH{
            get{
                return "Assets" + ASSETBUNDLE_PATH + DownloadConstant.ASSET_PLATFORM + "/";
            }
        }

        protected static string ASSETBUNDLE_PLATFORM_CONFIG_FILE
        {
            get
            {
                return ASSETBUNDLE_PLATFORM_PATH + CONFIG_FILE;
            }
        }

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
            InitAssetBundleWindowItemsFromABConfigFile();
        }

        void InitAssetBundleWindowItemFromEditorABSetting(string assetbundleName)
        {
            AssetBundleWindowItem assetBundleWindowItem = new AssetBundleWindowItem();
            assetBundleWindowItem.assetBundleName = assetbundleName;
            string path = ASSETBUNDLE_PLATFORM_PATH + assetbundleName;
            if (FileManager.Exists(path))
            {
                assetBundleWindowItem.assetBundle = LoadAssetBunleObject(assetbundleName);
                assetBundleWindowItem.assetBundleHash = FileManager.GetFileHash(ASSETBUNDLE_PLATFORM_PATH + assetbundleName);
                assetBundleWindowItem.assetBundleLength = new FileInfo(ASSETBUNDLE_PLATFORM_PATH + assetbundleName).Length;
            }
            assetBundleWindowItem.displayLength = FileLengthToStr(assetBundleWindowItem.assetBundleLength);
            AddAssetBundleItem(assetbundleName, assetBundleWindowItem);
        }

        void InitAssetBundleWindowItemsFromABConfigFile()
        {
            for (int i = 0; i < mAssetBundleConfig.items.Count; i++)
            {
                InitAssetBundleWindowItemFromABConfigFile(mAssetBundleConfig.items[i]);
            }
        }

        void InitAssetBundleWindowItemFromABConfigFile(AssetConfigItem configItem)
        {
            if (mAssetbundleItemDic.ContainsKey(configItem.assetName))
            {
                mAssetbundleItemDic[configItem.assetName].assetBundleName = configItem.assetName;
                mAssetbundleItemDic[configItem.assetName].assetBundleHash = configItem.hashCode;
                mAssetbundleItemDic[configItem.assetName].resourcesFolder = GetAssetBundleMainFolder(configItem.assetName);
            }
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

            string path = ASSETDATABASE_PLATFORM_PATH + assetbundleName;
            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }

        AssetConfig LoadAssetBundleConfig()
        {
            if (!FileManager.Exists(ASSETBUNDLE_PLATFORM_CONFIG_FILE))
            {
                InitConfigFile();
            }
            string configString = FileManager.ReadString(ASSETBUNDLE_PLATFORM_CONFIG_FILE);
            return JsonUtility.FromJson<AssetConfig>(configString);
        }

        void InitConfigFile()
        {
            FileManager.WriteString(ASSETBUNDLE_PLATFORM_CONFIG_FILE, JsonUtility.ToJson(CreateAssetBundleConfig()));
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