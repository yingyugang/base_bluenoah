using System.Collections.Generic;
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

        protected static string ASSETBUNDLE_PLATFORM_PATH{
            get{
                return Application.dataPath + ASSETBUNDLE_PATH + EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
            }
        }

        protected static string ASSETBUNDLE_PLATFORM_CONFIG_FILE {
            get{
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
            //TODO icon
            //guiContent.image = mUIEditorSettings.WINDOW_ICON_PATH;
            guiContent.tooltip = tooltip;
            this.titleContent = guiContent;
        }

        void LoadAssetBundleInfos()
        {
            mAssetBundleConfig = LoadAssetBundleConfig();
            mAssetbundleItemDic = new Dictionary<string, AssetBundleWindowItem>();
            mAssetBundleItemList = new List<AssetBundleWindowItem>();
            CreateAssetBundleWindowItems();
            SetConfigItemToWindowItems();
        }

        void CreateAssetBundleWindowItems()
        {
            string[] assetbundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetbundleNames.Length; i++)
            {
                CreateAssetBundleWindowItem(assetbundleNames[i]);
            }
        }

        void CreateAssetBundleWindowItem(string assetbundleName)
        {
            AddAssetBundleItem(assetbundleName, new AssetBundleWindowItem());
        }

        void AddAssetBundleItem(string assetbundleName,AssetBundleWindowItem assetBundleWindowItem){
            mAssetbundleItemDic.Add(assetbundleName, assetBundleWindowItem);
            mAssetBundleItemList.Add(assetBundleWindowItem);
        }

        void SetConfigItemToWindowItems()
        {
            for (int i = 0; i < mAssetBundleConfig.items.Count; i++)
            {
                SetConfigItemToWindowItem(mAssetBundleConfig.items[i]);
            }
        }

        void SetConfigItemToWindowItem(AssetBundleConfigItem configItem){
            if(mAssetbundleItemDic.ContainsKey(configItem.name)){
                mAssetbundleItemDic[configItem.name].assetBundleConfigItem = configItem;
                mAssetbundleItemDic[configItem.name].displayLength = FileLengthToStr(configItem.length);
                mAssetbundleItemDic[configItem.name].assetBundle = LoadAssetBunleObject(configItem.name);
            }
        }

        UnityEngine.AssetBundle LoadAssetBunleObject(string assetbundleName){
            string path = ASSETBUNDLE_PLATFORM_PATH + assetbundleName;
            UnityEngine.AssetBundle assetBundle = UnityEngine.AssetBundle.LoadFromFile(path);
            return assetBundle;
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

        void InitConfigFile(){
            FileManager.WriteString(ASSETBUNDLE_PLATFORM_CONFIG_FILE,JsonUtility.ToJson(CreateAssetBundleConfig()));
        }

        AssetBundleConfig CreateAssetBundleConfig(){
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
        public AssetBundleConfigItem assetBundleConfigItem;
        public UnityEngine.AssetBundle assetBundle;
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