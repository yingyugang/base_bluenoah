using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using BlueNoah.IO;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadControllerConfig : DownloadControllerBase
    {
		
        AssetConfig mRemoteAssetConfig;

        public DownloadControllerConfig(DownloadManager downloadManager)
        {
            mDownloadManager = downloadManager;
        }

        public void DownloadRemoteConfig(UnityAction<List<AssetConfigItem>> onComplete)
        {
			Debug.Log ("Download Config.");
            mDownloadManager.StartCoroutine(_DownloadRemoteConfigAndFilterDownloadItems(onComplete));
        }

        List<AssetConfigItem> CheckDownloadList(AssetConfig remoteAssetConfig)
        {
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            for (int i = 0;i < remoteAssetConfig.items.Count;i++){
                if(FileManager.GetFileHash(DownloadConstant.GetDownloadAssetBundlePath(remoteAssetConfig.items[i].assetName)) != remoteAssetConfig.items[i].hashCode){
                    items.Add(remoteAssetConfig.items[i]);
                }
            }
            return items;
        }

        IEnumerator _DownloadRemoteConfigAndFilterDownloadItems(UnityAction<List<AssetConfigItem>> onComplet)
        {
            UnityWebRequest www = CreateUnityWebRequest(DownloadConstant.REMOTE_ASSET_PATH_CONFIG);
            yield return www.SendWebRequest();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.url);
                OnDownloadAssetDone(www.downloadHandler.text, onComplet);
            }
            else
            {
				Debug.LogError("Download asset config error. ||" + www.url + "||" + www.error);
            }
        }

        void OnDownloadAssetDone(string downloadText, UnityAction<List<AssetConfigItem>> onComplet)
        {
            mRemoteAssetConfig = JsonUtility.FromJson<AssetConfig>(downloadText);
            List<AssetConfigItem> items = CheckDownloadList(mRemoteAssetConfig);
            if (onComplet != null)
            {
                onComplet(items);
            }
			Debug.Log("Download assets Finish.");
        }
    }
}