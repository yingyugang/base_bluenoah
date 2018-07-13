using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//TODO
namespace BlueNoah.Download
{
    public class DownloadManager : SimpleSingleMonoBehaviour<DownloadManager>
    {
        public AssetConfig assetConfig;

        const string ASSET_CONFIG_PATH = "AssetConfig";

        AssetConfig mLocalAssetConfig;

        AssetConfig mRemoteAssetConfig;

        void LoadAssetConfigs(){
            mLocalAssetConfig = GetLocalConfig();
            mRemoteAssetConfig = GetRemoveConfig();
        }
        //TODO
        AssetConfig GetLocalConfig(){
            string configStr = Resources.Load<TextAsset>(ASSET_CONFIG_PATH).text;
            assetConfig = JsonUtility.FromJson<AssetConfig>(configStr);
            return assetConfig;
        }
        //TODO
        AssetConfig GetRemoveConfig(){
            string configStr = Resources.Load<TextAsset>(ASSET_CONFIG_PATH).text;
            assetConfig = JsonUtility.FromJson<AssetConfig>(configStr);
            return assetConfig;
        }

        List<AssetConfigItem> GetDownloadFiles(){
            List<AssetConfigItem> items = new List<AssetConfigItem>();
            for (int i = 0; i < assetConfig.items.Count;i++)
            {
                items.Add(assetConfig.items[i]);
            }
            return items;
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
