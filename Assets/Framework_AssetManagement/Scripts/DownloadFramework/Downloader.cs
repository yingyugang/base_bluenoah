using System.Collections;
using BlueNoah.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace BlueNoah.Download
{
    class Downloader : MonoBehaviour
    {
        internal string downloaderName;
        private UnityWebRequest www;
        internal DownloadingFileData downloadingFileData;
        private const float INCORRECT_PROGRESS = 0.5f;

        internal UnityAction<Downloader, DownloadingFileData, AssetBundle> downloaderComplete;
        internal UnityAction<Downloader, DownloadingFileData, string> downloaderError;
        private float reachedSize;

        internal void StartDownload(DownloadingFileData downloadingFileData)
        {
            reachedSize = 0;
            this.downloadingFileData = downloadingFileData;
            StartCoroutine(Download());
        }

        internal void ReDownload()
        {
            StartCoroutine(Download());
        }

        private IEnumerator Download()
        {
            //TODO Always get new asset.
            www = UnityWebRequest.Get(GetPathFromDownloadingFileType(downloadingFileData.FileType, true) + downloadingFileData.FileName + "?" + Random.Range(0, int.MaxValue));
            yield return www.Send();

            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                FileManager.WriteAllBytes(GetPathFromDownloadingFileType(downloadingFileData.FileType, false) + downloadingFileData.FileName, www.downloadHandler.data);
                downloaderComplete(this, downloadingFileData, downloadingFileData.IsAssetBundle == 1 ? ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle : null);
            }
            else
            {
                Debug.Log(www.error);
            }
        }

        private string GetPathFromDownloadingFileType(DownloadingFileTypeEnum downloadingFileTypeEnum, bool isServer)
        {
            if (isServer)
            {
                if (downloadingFileTypeEnum == DownloadingFileTypeEnum.CSV)
                {
                    return DownloadConstant.SERVER_VERSION_PATH;
                }
                else if (downloadingFileTypeEnum == DownloadingFileTypeEnum.Assets)
                {
                    return DownloadConstant.SERVER_ASSETBUNDLES_PATH;
                }
            }
            else
            {
                if (downloadingFileTypeEnum == DownloadingFileTypeEnum.CSV)
                {
                    return DownloadConstant.CLIENT_VERSION_PATH;
                }
                else if (downloadingFileTypeEnum == DownloadingFileTypeEnum.Assets)
                {
                    return DownloadConstant.CLIENT_ASSETBUNDLES_PATH;
                }
            }

            return null;
        }

        private void Update()
        {
            if (www != null && !string.IsNullOrEmpty(www.error))
            {
                Retry();
            }
        }

        internal float CurrentSize
        {
            get
            {
                return downloadingFileData.FileSize * www.downloadProgress;
            }
        }

        private void OnDestroy()
        {
            www.Dispose();
            www = null;
        }

        private void Retry()
        {
            www.Dispose();
            www = null;
            ReDownload();
        }
    }
}
