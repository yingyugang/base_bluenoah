using System.Collections.Generic;
using System.IO;
using BlueNoah.IO;
using UnityEditor;
using UnityEngine;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public abstract class AssetBundleWindow : EditorWindow
    {
        protected const string ASSETBUNDLE_PATH = "/Framework_AM/AssetBundleBuilds/";

        protected const string CONFIG_FILE = "assetbundle_config.json";

        protected AssetBundleConfig mAssetBundleConfig;

        protected Dictionary<string, AssetBundleWindowItem> mAssetbundleItemDic;

        protected List<AssetBundleWindowItem> mAssetBundleItemList;

        protected static string ASSETBUNDLE_PLATFORM_PATH
        {
            get
            {
                return Application.dataPath + ASSETBUNDLE_PATH + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            }
        }

        protected static string ASSETDATABASE_PLATFORM_PATH{
            get{
                return "Assets" + ASSETBUNDLE_PATH + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
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

        void InitAssetBundleWindowItemFromABConfigFile(AssetBundleConfigItem configItem)
        {
            if (mAssetbundleItemDic.ContainsKey(configItem.name))
            {
                mAssetbundleItemDic[configItem.name].assetBundleName = configItem.name;
                mAssetbundleItemDic[configItem.name].assetBundleHash = configItem.hash;
                mAssetbundleItemDic[configItem.name].resourcesFolder = GetAssetBundleMainFolder(configItem.name);
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

        AssetBundleConfig LoadAssetBundleConfig()
        {
            if (!FileManager.Exists(ASSETBUNDLE_PLATFORM_CONFIG_FILE))
            {
                InitConfigFile();
            }
            string configString = FileManager.ReadString(ASSETBUNDLE_PLATFORM_CONFIG_FILE);
            return JsonUtility.FromJson<AssetBundleConfig>(configString);
        }

        void InitConfigFile()
        {
            FileManager.WriteString(ASSETBUNDLE_PLATFORM_CONFIG_FILE, JsonUtility.ToJson(CreateAssetBundleConfig()));
        }

        AssetBundleConfig CreateAssetBundleConfig()
        {
            AssetBundleConfig config = new AssetBundleConfig();
            config.items = new List<AssetBundleConfigItem>();
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

    [System.Serializable]
    public class AssetBundleConfig
    {
        public List<AssetBundleConfigItem> items;
    }

    [System.Serializable]
    public class AssetBundleConfigItem
    {
        public string name;
        public string hash;
        public long length;
    }
}