using System.Collections.Generic;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadManager : SimpleSingleMonoBehaviour<DownloadManager>
    {

        const string DOWNLOADING_MANAGER = "DownloadingManager";

        DownloadControllerConfig mConfigDownloadManager;

        DownloadControllerAsset mAssetDownloadManager;

		DownloadControllerManifest mDownloadControllerManifest;

        UnityAction onDownloadComplete;

        protected override void Awake()
        {
            mConfigDownloadManager = new DownloadControllerConfig(this);
			mDownloadControllerManifest = new DownloadControllerManifest (this);
            mAssetDownloadManager = new DownloadControllerAsset(this);
        }

		public void StartDownload(UnityAction onDownloadComplete){
			this.onDownloadComplete = onDownloadComplete;
			DownloadManifest ();
		}

		void DownloadManifest(){
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

        public void DownloadAssetBundle(string assetBundleName,UnityAction onDownloadComplete){
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            AssetConfigItem item = new AssetConfigItem();
            item.assetName = assetBundleName;
            items.Add(item);
            Download(items,onDownloadComplete);
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

        public float GetProgress(){
            return mAssetDownloadManager.GetProgress();
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
