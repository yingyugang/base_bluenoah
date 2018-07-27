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

        List<DownloadAssetDownloader> mDownloaders;

        List<DownloadAssetDownloader> mPrepareDownloaders;

        List<DownloadAssetDownloader> mRunningDownloaders;

        UnityAction onDownloadComplete;

        UnityAction<float> onDownloading;

        UnityAction<List<DownloadProgress>> onDownloadProgressDetail;

        ulong mTotalDownloadFileSize;

        ulong mTotalDownloadDoneFileSize;

        bool mIsDownloading;

        public DownloadControllerAsset(DownloadManager downloadManager)
        {
            mDownloadManager = downloadManager;
        }

        public void StartDownloads(List<AssetConfigItem> items, UnityAction onDownloadComplete, UnityAction<float> onDownloading, UnityAction<List<DownloadProgress>> onDownloadProgressDetail)
        {
            if (mPreDownloadList == null)
            {
                mPreDownloadList = new List<AssetConfigItem>();
            }
            if (mDownloadingList == null)
            {
                mDownloadingList = new List<AssetConfigItem>();
            }
            mDownloaders = new List<DownloadAssetDownloader>();
            for (int i = 0; i < DownloadConstant.MAX_DOWNLOAD_COUNT; i++)
            {
                mDownloaders.Add(CreateAssetDownloader());
            }
            mPrepareDownloaders = new List<DownloadAssetDownloader>();
            mPrepareDownloaders.AddRange(mDownloaders);
            mRunningDownloaders = new List<DownloadAssetDownloader>();
            mPreDownloadList.AddRange(items);
            this.onDownloadComplete = onDownloadComplete;
            this.onDownloading = onDownloading;
            this.onDownloadProgressDetail = onDownloadProgressDetail;
            mTotalDownloadFileSize = CalculateTotalDownloadSize(items);
            mTotalDownloadDoneFileSize = 0;
            mDownloadManager.StartCoroutine(_StartDownloads());
        }

        IEnumerator _StartDownloads()
        {
            Debug.Log("_StartDownloads Begin");
            mIsDownloading = true;
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
                if (onDownloading != null)
                    onDownloading(GetProgress());
                if (onDownloadProgressDetail != null)
                    onDownloadProgressDetail(GetDetailProgress());
                yield return null;
            }
            if (onDownloadComplete != null)
            {
                if (onDownloading != null)
                    onDownloading(GetProgress());
                onDownloadComplete();
            }
            Debug.Log("_StartDownloads End");
            mIsDownloading = false;
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        void StartDownload(AssetConfigItem item)
        {
            DownloadAssetDownloader downloader = mPrepareDownloaders[0];
            mPrepareDownloaders.RemoveAt(0);
            mRunningDownloaders.Add(downloader);
            downloader.StartDownload(item, OnPerDownloadDone);
        }

        void OnPerDownloadDone(DownloadAssetDownloader downloader, AssetConfigItem assetConfigItem)
        {
            mDownloadingAssets--;
            mDownloadingList.Remove(assetConfigItem);
            downloader.gameObject.name = "AssetDownloader";
            mPrepareDownloaders.Add(downloader);
            mRunningDownloaders.Remove(downloader);
            if (onDownloadProgressDetail != null)
                onDownloadProgressDetail(GetDetailProgress());
            mTotalDownloadDoneFileSize += (ulong)assetConfigItem.size;
        }

        DownloadAssetDownloader CreateAssetDownloader()
        {
            GameObject gameObject = new GameObject("AssetDownloader");
            return gameObject.AddComponent<DownloadAssetDownloader>();
        }

        List<DownloadProgress> GetDetailProgress()
        {
            List<DownloadProgress> downloading = new List<DownloadProgress>();
            for (int i = 0; i < mDownloaders.Count; i++)
            {
                downloading.Add(new DownloadProgress(mDownloaders[i].GetDownloadAssetName(), mDownloaders[i].GetTotalSize(), mDownloaders[i].GetProgress()));
            }
            return downloading;
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
            for (int i = 0; i < mRunningDownloaders.Count; i++)
            {
                downloadSize += mRunningDownloaders[i].GetDownloadSize();
            }
            return downloadSize;
        }

        ulong GetDownloadDoneSize()
        {
            return mTotalDownloadDoneFileSize;
        }

        float GetProgress()
        {
            if (mIsDownloading)
                return (GetRunningDownloadedSize() + mTotalDownloadDoneFileSize) / (float)mTotalDownloadFileSize;
            else
                return 0;
        }

    }

    public class DownloadProgress
    {

        public string name;
        public ulong size;
        public float progress;

        public DownloadProgress(string name, ulong size, float progress)
        {
            this.name = name;
            this.size = size;
            this.progress = progress;
        }
    }

}