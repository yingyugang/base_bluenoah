using UnityEngine.Networking;

namespace BlueNoah.Download
{
    public abstract class DownloadControllerBase
    {
        protected DownloadManager mDownloadManager;

        public static UnityWebRequest CreateUnityWebRequest(string path)
        {
            UnityWebRequest www = UnityWebRequest.Get(path + "?" + GetUTCTime());
            www.SetRequestHeader("Cache-Control", "max-age=0, no-cache, no-store");
            www.SetRequestHeader("Pragma", "no-cache");
            return www;
        }

        static string GetUTCTime()
        {
            System.Int32 unixTimestamp = (System.Int32)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp.ToString();
        }

    }
}
