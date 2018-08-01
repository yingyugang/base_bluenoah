using UnityEngine;
using BlueNoah.Download;
using BlueNoah.IO;

namespace BlueNoah.Assets
{
	public class AssetBundleServiceManifest
	{
        AssetBundleManifest mAssetBundleManifest;

		public void LoadManifest ()
		{
            AssetBundle.UnloadAllAssetBundles(false);
            if (FileManager.Exists(DownloadConstant.DOWNLOAD_ASSET_PATH_MANIFEST))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(DownloadConstant.DOWNLOAD_ASSET_PATH_MANIFEST);
                mAssetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }
		}

        public string[] GetAllDependencies(string assetBundleName){
            if(mAssetBundleManifest==null){
                LoadManifest();
            }
           return mAssetBundleManifest.GetAllDependencies(assetBundleName);
        }

	}
}
