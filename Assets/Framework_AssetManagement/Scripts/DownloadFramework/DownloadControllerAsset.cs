using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Events;
using BlueNoah.IO;

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

        bool mIsDownloading = false;

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
            mPreDownloadList.AddRange(items);
            mOnDownloadComplete = onDownloadComplete;
            mDownloadManager.StartCoroutine(_StartDownloads());
        }

        IEnumerator _StartDownloads()
        {
            Debug.Log("_StartDownloads Begin");
            while (mDownloadingList.Count > 0 || mPreDownloadList.Count > 0)
            {
                if (mDownloadingAssets < M_MAX_DOWNLOAD_COUNT)
                {
                    AssetConfigItem item = mPreDownloadList[0];
                    mPreDownloadList.RemoveAt(0);
                    mDownloadingList.Add(item);
                    mDownloadingAssets++;
                    mDownloadManager.StartCoroutine(_DownloadAsset(item));
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

        IEnumerator _DownloadAsset(AssetConfigItem item)
        {
            UnityWebRequest www = CreateUnityWebRequest(DownloadConstant.REMOTE_ASSET_PATH(item.assetName));
            yield return www.Send();
            Debug.Log(www.url + " : complete.");
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                mDownloadingAssets--;
                OnDownloadDone(item, www);
                mDownloadingList.Remove(item);
            }
            else
            {
                Debug.LogError("Remove asset failure. ReDownload. ");
                yield return new WaitForSeconds(0.1f);
                mDownloadManager.StartCoroutine(_DownloadAsset(item));
            }
        }

        void OnDownloadDone(AssetConfigItem item, UnityWebRequest www)
        {
            FileManager.WriteAllBytes(DownloadConstant.GetDownloadAssetBundlePath(item.assetName), www.downloadHandler.data);
        }

    }
}