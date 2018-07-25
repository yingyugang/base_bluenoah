using System.Collections;
using BlueNoah.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace BlueNoah.Download
{
    public class DownloadAssetDownloader : MonoBehaviour
    {

        UnityWebRequest www;

        AssetConfigItem mItem;

        public void StartDownload(AssetConfigItem item, UnityAction<AssetConfigItem> onComplete){
            mItem = item;
            StartCoroutine(_DownloadAsset(item, onComplete));
        }

        IEnumerator _DownloadAsset(AssetConfigItem item,UnityAction<AssetConfigItem> onComplete)
        {
            Debug.Log(string.Format("Start download : {0}", item.assetName));
            www = DownloadControllerBase.CreateUnityWebRequest(DownloadConstant.REMOTE_ASSET_PATH(item.assetName));
            yield return www.SendWebRequest();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                OnDownloadDone(item, www);
                Debug.Log(string.Format("End download : {0}", item.assetName));
            }
            else
            {
                Debug.LogError(string.Format("Failure to download {0} ReDownload.", item.assetName));
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(_DownloadAsset(item,onComplete));
            }
            Destroy(gameObject);
        }

        void OnDownloadDone(AssetConfigItem item, UnityWebRequest www)
        {
            FileManager.WriteAllBytes(DownloadConstant.GetDownloadAssetBundlePath(item.assetName), www.downloadHandler.data);
        }

        public ulong GetDownloadSize(){
            return (ulong)(mItem.size * www.downloadProgress);
        }

    }
}
