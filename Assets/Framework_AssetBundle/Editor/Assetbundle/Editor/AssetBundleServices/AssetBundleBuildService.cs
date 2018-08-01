using System.Collections;
using System.Collections.Generic;
using System.IO;
using BlueNoah.IO;
using UnityEditor;
using UnityEngine;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleBuildService
    {

        public ulong GetTotalAssetBundleSize(List<AssetBundleWindowItem> assetBundleWindowItemList)
        {
            ulong totalSize = 0;
            for (int i = 0; i < assetBundleWindowItemList.Count; i++)
            {
                totalSize += assetBundleWindowItemList[i].assetBundleLength;
            }
            return totalSize;
        }

        public List<AssetBundleWindowItem> InitAssetBundleWindowItemsFromAssetBundleSetting()
        {
            List<AssetBundleWindowItem> assetBundleWindowItemList = new List<AssetBundleWindowItem>();
            string[] assetbundleNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < assetbundleNames.Length; i++)
            {
                assetBundleWindowItemList.Add(InitAssetBundleWindowItemFromAssetBundleSetting(assetbundleNames[i]));
            }
            return assetBundleWindowItemList;
        }

        AssetBundleWindowItem InitAssetBundleWindowItemFromAssetBundleSetting(string assetbundleName)
        {
            AssetBundleWindowItem assetBundleWindowItem = new AssetBundleWindowItem();
            assetBundleWindowItem.assetBundleName = assetbundleName;
            string path = AssetBundleConstant.ASSETBUNDLE_PLATFORM_PATH + assetbundleName;
            if (FileManager.Exists(path))
            {
                assetBundleWindowItem.assetBundle = LoadAssetBunleObject(assetbundleName);
                assetBundleWindowItem.assetBundleHash = FileManager.GetFileHash(AssetBundleConstant.ASSETBUNDLE_PLATFORM_PATH + assetbundleName);
                assetBundleWindowItem.assetBundleLength = (ulong)(new FileInfo(AssetBundleConstant.ASSETBUNDLE_PLATFORM_PATH + assetbundleName).Length);
            }
            assetBundleWindowItem.displayLength = FileLengthToStr(assetBundleWindowItem.assetBundleLength);
            return assetBundleWindowItem;
        }

        public Dictionary<string,AssetBundleWindowItem> GetAssetBundleWindowItemDic(List<AssetBundleWindowItem> assetBundleWindowItemList){
            Dictionary<string, AssetBundleWindowItem> assetBundleWindowItemDic = new Dictionary<string, AssetBundleWindowItem>();
            for (int i = 0; i < assetBundleWindowItemList.Count;i++){
                assetBundleWindowItemDic.Add(assetBundleWindowItemList[i].assetBundleName,assetBundleWindowItemList[i]);
            }
            return assetBundleWindowItemDic;
        }

        Object LoadAssetBunleObject(string assetbundleName)
        {
            string path = AssetBundleConstant.ASSETDATABASE_PLATFORM_PATH + assetbundleName;
            return AssetDatabase.LoadAssetAtPath<Object>(path);
        }

        public string FileLengthToStr(ulong length)
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



        public AssetConfig LoadAssetBundleConfig()
        {
            if (!FileManager.Exists(AssetBundleConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE))
            {
                InitConfigFile();
            }
            string configString = FileManager.ReadString(AssetBundleConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE);
            return JsonUtility.FromJson<AssetConfig>(configString);
        }

        void InitConfigFile()
        {
            FileManager.WriteString(AssetBundleConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE, JsonUtility.ToJson(CreateAssetBundleConfig()));
        }

        AssetConfig CreateAssetBundleConfig()
        {
            AssetConfig config = new AssetConfig();
            config.items = new List<AssetConfigItem>();
            return config;
        }
    }
}