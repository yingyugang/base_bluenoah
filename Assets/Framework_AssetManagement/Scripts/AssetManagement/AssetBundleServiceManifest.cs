using UnityEngine;
using BlueNoah.Download;

namespace BlueNoah.Assets
{
	public class AssetBundleServiceManifest
	{
        AssetBundleManifest mAssetBundleManifest;

		public void LoadManifest ()
		{
			AssetBundle.UnloadAllAssetBundles (false);
			AssetBundle assetBundle = AssetBundle.LoadFromFile (DownloadConstant.DOWNLOAD_ASSET_PATH_MANIFEST);
			mAssetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		}

        public string[] GetAllDependencies(string assetBundleName){
           return mAssetBundleManifest.GetAllDependencies(assetBundleName);
        }

	}
}
