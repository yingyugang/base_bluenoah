using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadControllerAsset : DownloadControllerBase
    {

        const int M_MAX_DOWNLOAD_COUNT = 5;

        const int M_MAX_DOWNLOAD_SIZE = 10 * 1024 * 1024;

        int mDownloadingAssets = 0;

        List<AssetConfigItem> mPreDownloadList;

        List<AssetConfigItem> mDownloadingList;

        UnityAction mOnDownloadComplete;

        List<DownloadAssetDownloader> mDownloadAssetDownloaderList;

        public DownloadControllerAsset(DownloadManager downloadManager)
        {
            mDownloadManager = downloadManager;
        }

        public void StartDownloads(List<AssetConfigItem> items, UnityAction onDownloadComplete)
        {
            if (mPreDownloadList == null)
            {
                mPreDownloadList = new List<AssetConfigItem>();
            }
            if (mDownloadingList == null)
            {
                mDownloadingList = new List<AssetConfigItem>();
            }
            mDownloadAssetDownloaderList = new List<DownloadAssetDownloader>();
            mPreDownloadList.AddRange(items);
            mOnDownloadComplete = onDownloadComplete;
            mDownloadManager.StartCoroutine(_StartDownloads());
        }

        IEnumerator _StartDownloads()
        {
            Debug.Log("_StartDownloads Begin");
            while (mDownloadingList.Count > 0 || mPreDownloadList.Count > 0)
            {
                if (mDownloadingAssets < M_MAX_DOWNLOAD_COUNT && mPreDownloadList.Count > 0)
                {
                    AssetConfigItem item = mPreDownloadList[0];
                    mPreDownloadList.RemoveAt(0);
                    mDownloadingList.Add(item);
                    mDownloadingAssets++;
                    StartDownload(item);
                }
                yield return null;
            }
            if (mOnDownloadComplete != null)
            {
                mOnDownloadComplete();
            }
            Debug.Log("_StartDownloads End");
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        void StartDownload(AssetConfigItem item)
        {
            DownloadAssetDownloader downloader = CreateAssetDownloader(item);
            mDownloadAssetDownloaderList.Add(downloader);
            downloader.StartDownload(item, (AssetConfigItem assetConfigItem) =>
            {
                mDownloadingAssets--;
                mDownloadingList.Remove(assetConfigItem);
                mDownloadAssetDownloaderList.Remove(downloader);
            });
        }

        DownloadAssetDownloader CreateAssetDownloader(AssetConfigItem item)
        {
            GameObject gameObject = new GameObject(string.Format("Download : {0}", item.assetName));
            return gameObject.AddComponent<DownloadAssetDownloader>();
        }
    }
}