using System.Collections;
using BlueNoah.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace BlueNoah.Download
{
    public class DownloadAssetDownloader : MonoBehaviour
    {

        UnityWebRequest mUnityWebRequest;

        AssetConfigItem mItem;

        public void StartDownload(AssetConfigItem item, UnityAction<DownloadAssetDownloader,AssetConfigItem> onComplete){
            mItem = item;
            StartCoroutine(_DownloadAsset(item, onComplete));
        }

        IEnumerator _DownloadAsset(AssetConfigItem item,UnityAction<DownloadAssetDownloader,AssetConfigItem> onComplete)
        {
            Debug.Log(string.Format("Start download : {0}", item.assetName));
            mUnityWebRequest = DownloadControllerBase.CreateUnityWebRequest(DownloadConstant.REMOTE_ASSET_PATH(item.assetName));
            Debug.Log(Time.frameCount +" --- " + item.assetName);
            yield return mUnityWebRequest.SendWebRequest();
            Debug.Log(Time.frameCount + " --- " + item.assetName);
            if (mUnityWebRequest.isDone && string.IsNullOrEmpty(mUnityWebRequest.error))
            {
                Debug.Log(string.Format("End download : {0}", item.assetName));
                OnDownloadDone(item, mUnityWebRequest);
                if(onComplete!=null){
                    onComplete(this,item);
                }
            }
            else
            {
                Debug.LogError(string.Format("Failure to download {0} ReDownload.", item.assetName));
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(_DownloadAsset(item,onComplete));
            }
            Destroy(gameObject);
        }

        void OnDownloadDone(AssetConfigItem item, UnityWebRequest unityWebRequest)
        {
            FileManager.WriteAllBytes(DownloadConstant.GetDownloadAssetBundlePath(item.assetName), unityWebRequest.downloadHandler.data);
        }

        public ulong GetDownloadSize(){
            Debug.Log(string.Format("<color=green>{0}</color>",mUnityWebRequest.downloadProgress));
            return (ulong)(mItem.size * mUnityWebRequest.downloadProgress);
        }

    }
}
