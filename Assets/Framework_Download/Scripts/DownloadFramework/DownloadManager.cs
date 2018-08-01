using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadManager : SimpleSingleMonoBehaviour<DownloadManager>
    {

        const string DOWNLOADING_MANAGER = "DownloadingManager";

        DownloadControllerConfig mConfigDownloadManager;

        DownloadControllerAsset mAssetDownloadManager;

        DownloadControllerManifest mDownloadControllerManifest;

        UnityAction onStartAssetDownload;

        UnityAction onDownloadComplete;

        UnityAction<float> onDownloadProgress;

        UnityAction<List<DownloadProgress>> onDownloadProgressDetail;

        protected override void Awake()
        {
            mConfigDownloadManager = new DownloadControllerConfig(this);
            mDownloadControllerManifest = new DownloadControllerManifest(this);
            mAssetDownloadManager = new DownloadControllerAsset(this);
        }

        //check the assets hash and download the assets what hash is different.
        public void StartAutoDownload(UnityAction onStartAssetDownload, UnityAction onDownloadComplete, UnityAction<float> onDownloadProgress, UnityAction<List<DownloadProgress>> onDownloadProgressDetail = null)
        {
            this.onStartAssetDownload = onStartAssetDownload;
            this.onDownloadComplete = onDownloadComplete;
            this.onDownloadProgress = onDownloadProgress;
            this.onDownloadProgressDetail = onDownloadProgressDetail;
            DownloadManifest();
        }

        public void CheckDownload(UnityAction<bool> onCheckDown)
        {
            if (onCheckDown != null)
            {
                mConfigDownloadManager.DownloadRemoteConfig((List<AssetConfigItem> items) =>
                {
                    if (items != null && items.Count > 0)
                    {
                        onCheckDown(true);
                    }
                    else
                    {
                        onCheckDown(false);
                    }
                });
            }
        }

        void DownloadManifest()
        {
            Debug.Log("<color=yellow>1.DownloadManifest</color>");
            mDownloadControllerManifest.DownloadManifest(DownloadConfig);
        }

        void DownloadConfig()
        {
            Debug.Log("<color=yellow>2.DownloadConfig</color>");
            mConfigDownloadManager.DownloadRemoteConfig(DownloadAssets);
        }

        void DownloadAssets(List<AssetConfigItem> items)
        {
            Debug.Log("<color=yellow>3.DownloadAssets</color>");
            if (items.Count > 0)
            {
                if (onStartAssetDownload != null)
                    onStartAssetDownload();
                mAssetDownloadManager.StartDownloads(items, onDownloadComplete, onDownloadProgress, onDownloadProgressDetail);
            }
            else
            {
                Debug.Log("<color=green>There is no item need to download.</color>");
                if (onDownloadComplete != null)
                    onDownloadComplete();
            }
        }

        public void DownloadAssetBundle(string assetBundleName, UnityAction onDownloadComplete)
        {
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            AssetConfigItem item = new AssetConfigItem();
            item.assetName = assetBundleName;
            items.Add(item);
            Download(items, onDownloadComplete);
        }

        public void Download(List<AssetConfigItem> items, UnityAction onDownloadComplete)
        {
            this.onDownloadComplete = onDownloadComplete;
            mAssetDownloadManager.StartDownloads(items, onDownloadComplete, onDownloadProgress, onDownloadProgressDetail);
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
        public ulong size;
        public string hashCode;
    }

}
