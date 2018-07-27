using UnityEngine;
using BlueNoah.Assets;
using BlueNoah.Utility;
using UnityEngine.UI;
using System.Collections.Generic;

namespace BlueNoah.Download
{
    public class DownloadSample : MonoBehaviour
    {

        public Slider slider_download;

        public GameObject container_downloads;

        Slider[] mSliders;

		void Start()
		{
            mSliders = container_downloads.GetComponentsInChildren<Slider>(true);
            gameObject.GetOrAddComponent<AssetBundleLoadManager>();
            DownloadManager.Instance.StartAutoDownload (()=>{
                slider_download.gameObject.SetActive(true);
            },()=>{
				Debug.Log("Download Done");
				gameObject.GetOrAddComponent<AssetBundleLoadManager>().Reload();
            },(float progress)=>{
                Debug.Log(progress);
                slider_download.value = progress;
            },(List<DownloadProgress> details)=>{
                for (int i = 0; i < details.Count;i++){
                    if(details[i].progress>0){
                        mSliders[i].value = details[i].progress;
                        mSliders[i].gameObject.SetActive(true);
                    }else{
                        mSliders[i].gameObject.SetActive(false);
                    }
                }
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
