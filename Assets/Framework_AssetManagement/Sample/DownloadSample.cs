using UnityEngine;
using BlueNoah.Assets;
using BlueNoah.Utility;
using UnityEngine.UI;

namespace BlueNoah.Download
{
    public class DownloadSample : MonoBehaviour
    {

        public Slider slider_download;

		void Start()
		{
            gameObject.GetOrAddComponent<AssetBundleLoadManager>();
			DownloadManager.Instance.StartDownload(()=>{
				Debug.Log("Download Done");
				gameObject.GetOrAddComponent<AssetBundleLoadManager>().Reload();
            },(float progress)=>{
                Debug.Log(progress);
                slider_download.value = progress;
            });
		}

		void Update()
		{
            if(Input.GetKeyDown(KeyCode.X)){
                Debug.Log("KeyCode.X");
                GetComponent<AssetBundleLoadManager>().LoadOrDownloadAssetBundle("images/units.ab",(AssetBundle assetBundle) =>{
                    Debug.Log(assetBundle);
                });
            }
		}


	}
}
