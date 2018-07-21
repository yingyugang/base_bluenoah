using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using BlueNoah.IO;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadConfigController : DownloadBaseController
    {
		
        AssetConfig mRemoteAssetConfig;

        public DownloadConfigController(DownloadManager downloadManager)
        {
            mDownloadManager = downloadManager;
        }

        public void DownloadRemoteConfigAndFilterDownloadItems(UnityAction<List<AssetConfigItem>> onComplete)
        {
			Debug.Log ("Download Config Start.");
            mDownloadManager.StartCoroutine(_DownloadRemoteConfigAndFilterDownloadItems(onComplete));
        }

        List<AssetConfigItem> CheckDownloadList(AssetConfig remoteAssetConfig)
        {
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            for (int i = 0;i < remoteAssetConfig.items.Count;i++){
                if(FileManager.GetFileHash(DownloadConstant.DOWNLOAD_ASSET_PATH(remoteAssetConfig.items[i].assetName)) != remoteAssetConfig.items[i].hashCode){
                    items.Add(remoteAssetConfig.items[i]);
                }
            }
            return items;
        }

        IEnumerator _DownloadRemoteConfigAndFilterDownloadItems(UnityAction<List<AssetConfigItem>> onComplet)
        {
            UnityWebRequest www = CreateUnityWebRequest(DownloadConstant.REMOTE_ASSET_CONFIG_PATH);
            yield return www.Send();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.url);
                Debug.Log(www.downloadHandler.text);
                OnDownloadConfigDone(www.downloadHandler.text, onComplet);
            }
            else
            {
				Debug.LogError("Download asset config error. ||" + www.url + "||" + www.error);
            }
        }

        void OnDownloadConfigDone(string downloadText, UnityAction<List<AssetConfigItem>> onComplet)
        {
            mRemoteAssetConfig = JsonUtility.FromJson<AssetConfig>(downloadText);
            List<AssetConfigItem> items = CheckDownloadList( mRemoteAssetConfig);
            if (onComplet != null)
            {
                onComplet(items);
            }
			Debug.Log("Download Config Finish.");
        }

        public void ConvertRemoteAssetConfigToLocalAssetConfig()
        {
            FileManager.WriteString(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH, JsonUtility.ToJson(mRemoteAssetConfig, true));
        }
    }
}