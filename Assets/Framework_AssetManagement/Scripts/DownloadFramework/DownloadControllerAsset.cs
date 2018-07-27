using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadControllerAsset : DownloadControllerBase
    {

        int mDownloadingAssets = 0;

        List<AssetConfigItem> mPreDownloadList;

        List<AssetConfigItem> mDownloadingList;

        UnityAction mOnDownloadComplete;

        List<DownloadAssetDownloader> mDownloadAssetDownloaderList;

        ulong mTotalDownloadFileSize;

        ulong mTotalDownloadDoneFileSize;

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
            mTotalDownloadFileSize = CalculateTotalDownloadSize(items);
            mTotalDownloadDoneFileSize = 0;
            mDownloadManager.StartCoroutine(_StartDownloads());
        }

        IEnumerator _StartDownloads()
        {
            Debug.Log("_StartDownloads Begin");
            while (mDownloadingList.Count > 0 || mPreDownloadList.Count > 0)
            {
                if (mDownloadingAssets < DownloadConstant.MAX_DOWNLOAD_COUNT && mPreDownloadList.Count > 0)
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
            downloader.StartDownload(item,OnPerDownloadDone);
        }

        void OnPerDownloadDone(DownloadAssetDownloader downloader,AssetConfigItem assetConfigItem){
            mDownloadingAssets--;
            mDownloadingList.Remove(assetConfigItem);
            mDownloadAssetDownloaderList.Remove(downloader);
            mTotalDownloadDoneFileSize += (ulong)assetConfigItem.size;
        }

        DownloadAssetDownloader CreateAssetDownloader(AssetConfigItem item)
        {
            GameObject gameObject = new GameObject(string.Format("Download : {0}", item.assetName));
            return gameObject.AddComponent<DownloadAssetDownloader>();
        }

        ulong CalculateTotalDownloadSize(List<AssetConfigItem> items)
        {
            ulong totalSize = 0;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                    totalSize += items[i].size;
            }
            return totalSize;
        }

        //get the total downloaded size that in the current downloading progresses/
        ulong GetRunningDownloadedSize()
        {
            ulong downloadSize = 0;
            for (int i = 0; i < mDownloadAssetDownloaderList.Count; i++)
            {
                downloadSize += mDownloadAssetDownloaderList[i].GetDownloadSize();
            }
            return downloadSize;
        }

        ulong GetDownloadDoneSize()
        {
            return mTotalDownloadDoneFileSize;
        }

        public float GetProgress()
        {
            return (GetRunningDownloadedSize() + mTotalDownloadDoneFileSize) / mTotalDownloadFileSize;
        }

    }
}