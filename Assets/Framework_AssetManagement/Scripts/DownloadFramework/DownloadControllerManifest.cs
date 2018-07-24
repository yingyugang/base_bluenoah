using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using BlueNoah.IO;
using UnityEngine.Events;

namespace BlueNoah.Download
{
    public class DownloadControllerManifest : DownloadControllerBase
    {
		
        AssetConfig mRemoteAssetConfig;

		public DownloadControllerManifest(DownloadManager downloadManager)
        {
            mDownloadManager = downloadManager;
        }

        public void DownloadManifest(UnityAction onComplete)
        {
			Debug.Log ("Download Manifest.");
			mDownloadManager.StartCoroutine(_DownloadManifest(onComplete));
        }

		IEnumerator _DownloadManifest(UnityAction onComplet)
        {
			UnityWebRequest www = CreateUnityWebRequest(DownloadConstant.REMOTE_ASSET_PATH_MANIFEST);
            yield return www.SendWebRequest();
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.url);
				FileManager.WriteAllBytes (DownloadConstant.DOWNLOAD_ASSET_PATH_MANIFEST,www.downloadHandler.data);
				if(onComplet!=null){
					onComplet ();
				}
            }
            else
            {
				Debug.LogError("Download asset manifest error. ||" + www.url + "||" + www.error);
            }
        }
    }
}