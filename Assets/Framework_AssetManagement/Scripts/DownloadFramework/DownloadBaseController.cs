using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace BlueNoah.Download
{
    public abstract class DownloadBaseController
    {
        
        protected DownloadManager mDownloadManager;

        protected UnityWebRequest CreateUnityWebRequest(string path)
        {
            UnityWebRequest www = UnityWebRequest.Get(path + "?" + mDownloadManager.GetUTCTime());
            www.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
            www.SetRequestHeader("Pragma", "no-cache");
            return www;
        }

    }
}
