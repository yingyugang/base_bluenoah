using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace BlueNoah.Download
{
    public class DownloadManager : SimpleSingleMonoBehaviour<DownloadManager>
    {

        DownloadControllerConfig mConfigDownloadManager;

        DownloadControllerAsset mAssetDownloadManager;

		DownloadControllerManifest mDownloadControllerManifest;

        UnityAction onDownloadComplete;

        protected override void Awake()
        {
            mConfigDownloadManager = new DownloadControllerConfig(this);
            mAssetDownloadManager = new DownloadControllerAsset(this);
			mDownloadControllerManifest = new DownloadControllerManifest (this);
        }

        const string DOWNLOADING_MANAGER = "DownloadingManager";

		public void StartDownload(UnityAction onDownloadComplete){
			this.onDownloadComplete = onDownloadComplete;
			DownloadManifest ();
		}

		public void DownloadManifest(){
			mDownloadControllerManifest.DownloadManifest (DownloadConfig);
		}

        void DownloadConfig()
        {
            mConfigDownloadManager.DownloadRemoteConfig(DownloadAssets);
        }

		void DownloadAssets(List<AssetConfigItem> items)
		{
			mAssetDownloadManager.StartDownloads(items, OnDownloadComplete);
		}

		public void Download(List<AssetConfigItem> items,UnityAction onDownloadComplete){
			this.onDownloadComplete = onDownloadComplete;
			mAssetDownloadManager.StartDownloads(items, OnDownloadComplete);
		}

        void OnDownloadComplete()
        {
            if (onDownloadComplete != null)
                onDownloadComplete();
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
