using UnityEngine;

namespace BlueNoah.Download
{
    public class Download : MonoBehaviour
    {
		private void Start()
		{
            DownloadManager.Instance.StartDownload();
		}

        public void OnDownloadComplete(){
            Debug.Log("OnDownloadComplete");
        }

        public void OnDownloadProgress(float progress){
            Debug.Log("OnDownloadProgress");
        }

        public void OnDownloadError(string key ,string value)
        {
            Debug.Log("OnDownloadError:" + key + "||" + value);
        }

	}
}
