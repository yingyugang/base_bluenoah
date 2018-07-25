using UnityEngine;
using BlueNoah.Assets;
using BlueNoah.Utility;

namespace BlueNoah.Download
{
    public class DownloadSample : MonoBehaviour
    {
		void Start()
		{
            gameObject.GetOrAddComponent<AssetBundleLoadManager>();
			DownloadManager.Instance.StartDownload(()=>{
				Debug.Log("Download Done");
				gameObject.GetOrAddComponent<AssetBundleLoadManager>().Reload();
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
