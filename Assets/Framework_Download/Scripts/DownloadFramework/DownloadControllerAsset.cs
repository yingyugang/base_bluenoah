using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadControllerAsset : DownloadControllerBase
    {

        List<AssetConfigItem> mPreDownloadAssetList;

        List<AssetConfigItem> mDownloadingAssetList;

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
            InitDownloadAssets(items);
            InitDownloaders();
            this.onDownloadComplete = onDownloadComplete;
            this.onDownloading = onDownloading;
            this.onDownloadProgressDetail = onDownloadProgressDetail;
            mTotalDownloadFileSize = CalculateTotalDownloadSize(items);
            mTotalDownloadDoneFileSize = 0;
            mDownloadManager.StartCoroutine(_StartDownloads());
        }

        void InitDownloadAssets(List<AssetConfigItem> items){
            if (mPreDownloadAssetList == null)
            {
                mPreDownloadAssetList = new List<AssetConfigItem>();
            }
            if (mDownloadingAssetList == null)
            {
                mDownloadingAssetList = new List<AssetConfigItem>();
            }
            mPreDownloadAssetList.AddRange(items);
        }

        void InitDownloaders(){
            mDownloaders = new List<DownloadAssetDownloader>();
            for (int i = 0; i < DownloadConstant.MAX_DOWNLOAD_COUNT; i++)
            {
                mDownloaders.Add(CreateAssetDownloader());
            }
            mPrepareDownloaders = new List<DownloadAssetDownloader>();
            mPrepareDownloaders.AddRange(mDownloaders);
            mRunningDownloaders = new List<DownloadAssetDownloader>();
        }

        IEnumerator _StartDownloads()
        {
            Debug.Log("_StartDownloads Begin");
            mIsDownloading = true;
            while (mDownloadingAssetList.Count > 0 || mPreDownloadAssetList.Count > 0)
            {
                if (mPrepareDownloaders.Count>0 && mPreDownloadAssetList.Count > 0)
                {
                    StartDownload(mPreDownloadAssetList[0]);
                }
                OnProgress();
                OnDetailProgress();
                yield return null;
            }
            mIsDownloading = false;
//#if UNITY_EDITOR
//            UnityEditor.AssetDatabase.Refresh();
//#endif
            ClearDownload();
            OnDownloadComplete();
            Debug.Log("_StartDownloads End");
        }

        void OnProgress(){
            //catch the exceptions,it can't break out download thread when exception happened.
            try
            {
                if (onDownloading != null)
                    onDownloading(GetProgress());
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        void OnDetailProgress(){
            //catch the exceptions,it can't break out download thread when exception happened.
            try
            {
                if (onDownloadProgressDetail != null)
                    onDownloadProgressDetail(GetDetailProgress());
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        void OnDownloadComplete(){
            //catch the exceptions,it can't break out download thread when exception happened.
            try
            {
                if (onDownloadComplete != null)
                {
                    if (onDownloading != null)
                        onDownloading(GetProgress());
                    onDownloadComplete();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        void ClearDownload(){
            for (int i = 0; i < mDownloaders.Count;i++){
                mDownloaders[i].Dispose();
            }
            mDownloaders.Clear();
        }

        void StartDownload(AssetConfigItem item)
        {
            mPreDownloadAssetList.Remove(item);
            mDownloadingAssetList.Add(item);
            DownloadAssetDownloader downloader = mPrepareDownloaders[0];
            mPrepareDownloaders.RemoveAt(0);
            mRunningDownloaders.Add(downloader);
            downloader.StartDownload(item, OnPerDownloadDone);
        }

        void OnPerDownloadDone(DownloadAssetDownloader downloader, AssetConfigItem assetConfigItem)
        {
            mDownloadingAssetList.Remove(assetConfigItem);
            downloader.gameObject.name = "AssetDownloader";
            mPrepareDownloaders.Add(downloader);
            mRunningDownloaders.Remove(downloader);
            mTotalDownloadDoneFileSize += (ulong)assetConfigItem.size;
            OnDetailProgress();
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
                return 1;
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