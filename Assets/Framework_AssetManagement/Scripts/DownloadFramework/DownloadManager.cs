using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace BlueNoah.Download
{
    public class DownloadManager : SimpleSingleMonoBehaviour<DownloadManager>
    {

        DownloadConfigController mConfigDownloadManager;

        DownloadAssetController mAssetDownloadManager;

        UnityAction onDownloadComplete;

        protected override void Awake()
        {
            mConfigDownloadManager = new DownloadConfigController(this);
            mAssetDownloadManager = new DownloadAssetController(this);
        }

        const string DOWNLOADING_MANAGER = "DownloadingManager";

        public void StartDownload()
        {
            mConfigDownloadManager.DownloadRemoteConfigAndFilterDownloadItems(OnDownloadRemoteConfigAndFilterDownloadItemsDone);
        }

        void SaveConfig()
        {
            mConfigDownloadManager.ConvertRemoteAssetConfigToLocalAssetConfig();
        }

        void OnDownloadRemoteConfigAndFilterDownloadItemsDone(List<AssetConfigItem> items)
        {
            Debug.Log("OnDownloadRemoteConfigAndFilterDownloadItemsDone:" + items.Count);
            mAssetDownloadManager.StartDownloads(items, OnDownloadComplete);
        }

        void OnDownloadComplete()
        {
            SaveConfig();
            if (onDownloadComplete != null)
                onDownloadComplete();
        }

        public void StartDownload(UnityAction downloadingComplete = null, UnityAction<float> downloadingProgress = null, UnityAction<string, string> downloadingError = null)
        {
            GameObject go = new GameObject(DOWNLOADING_MANAGER);
            DownloadingGetter downloadingGetter = go.AddComponent<DownloadingGetter>();
            downloadingGetter.DownloadingComplete = () =>
            {
                if (downloadingComplete != null)
                {
                    downloadingComplete();
                }
                Destroy(go);
            };
            downloadingGetter.DownloadingProgress = f =>
            {
                if (downloadingProgress != null)
                {
                    downloadingProgress(f);
                }
            };
            downloadingGetter.DownloadingError = (name, error) =>
            {
                if (downloadingError != null)
                {
                    downloadingError(name, error);
                }
            };
            downloadingGetter.Download();
        }

        public void StartAssetDownload(List<AssetConfigItem> items)
        {

        }

        public string GetUTCTime()
        {
            System.Int32 unixTimestamp = (System.Int32)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

    }

    [System.Serializable]
    public class AssetConfig
    {
        public List<AssetConfigItem> items;
    }

    [System.Serializable]
    public class AssetConfigItem
    {
        public int index;
        public string assetName;
        public int assetType;
        public long size;
        public string hashCode;
    }

}
