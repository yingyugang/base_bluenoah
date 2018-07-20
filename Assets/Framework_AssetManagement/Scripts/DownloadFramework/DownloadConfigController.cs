using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using BlueNoah.IO;
using UnityEngine.Events;
using System.IO;

namespace BlueNoah.Download
{
    public class DownloadConfigController : DownloadBaseController
    {

        AssetConfig mRemoteAssetConfig;

        AssetConfig mLocalAssetConfig;

        public DownloadConfigController(DownloadManager downloadManager)
        {
            mDownloadManager = downloadManager;
        }

        public void DownloadRemoteConfigAndFilterDownloadItems(UnityAction<List<AssetConfigItem>> onComplete)
        {
            mDownloadManager.StartCoroutine(_DownloadRemoteConfigAndFilterDownloadItems(onComplete));
        }

        List<AssetConfigItem> CheckDownloadList(AssetConfig remoteAssetConfig)
        {
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            for (int i = 0;i < remoteAssetConfig.items.Count;i++){
                Debug.Log(FileManager.GetFileHash(DownloadConstant.DOWNLOAD_ASSET_PATH(remoteAssetConfig.items[i].assetName)) + " || " + remoteAssetConfig.items[i].hashCode);
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
                Debug.LogError("Remove asset config is not existing.");
            }
        }

        void OnDownloadConfigDone(string downloadText, UnityAction<List<AssetConfigItem>> onComplet)
        {
            Debug.Log("OnDownloadConfigDone");
            mRemoteAssetConfig = JsonUtility.FromJson<AssetConfig>(downloadText);
            if (FileManager.Exists(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH))
            {
                mLocalAssetConfig = JsonUtility.FromJson<AssetConfig>(FileManager.ReadString(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH));
            }
            List<AssetConfigItem> items = CheckDownloadList( mRemoteAssetConfig);
            if (onComplet != null)
            {
                onComplet(items);
            }
        }

        public void ConvertRemoteAssetConfigToLocalAssetConfig()
        {
            FileManager.WriteString(DownloadConstant.DOWNLOAD_ASSET_CONFIG_PATH, JsonUtility.ToJson(mRemoteAssetConfig, true));
        }


    }
}