using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace BlueNoah.Download
{
    public class DownloadManager : SimpleSingleMonoBehaviour<DownloadManager>
    {
        public AssetConfig assetConfig;

        private const string DOWNLOADING_MANAGER = "DownloadingManager";

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

        public void StartDownload(UnityAction downloadingComplete = null, UnityAction<float> downloadingProgress = null){
           
            StartCoroutine(_StartDownload(downloadingComplete,downloadingProgress));
        }

        IEnumerator _StartDownload(UnityAction downloadingComplete = null, UnityAction<float> downloadingProgress = null){
            UnityWebRequest unityWebRequest = UnityWebRequest.Put("", "");
            yield return null;
        }

    }

    [System.Serializable]
    public class AssetConfig{
        public List<AssetConfigItem> items;
    }

    [System.Serializable]
    public class AssetConfigItem{
        public int index;
        public string assetName;
        public string assetType;
        public int size;
        public string hashCode;
    }

}
