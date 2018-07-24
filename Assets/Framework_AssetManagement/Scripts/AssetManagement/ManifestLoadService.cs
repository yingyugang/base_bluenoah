using UnityEngine;
using BlueNoah.Download;

namespace BlueNoah.Assets
{
	public class ManifestLoadService
	{
		AssetBundleManifest mManifest;

		public void LoadManifest ()
		{
			AssetBundle.UnloadAllAssetBundles (false);
			AssetBundle assetBundle = AssetBundle.LoadFromFile (DownloadConstant.DOWNLOAD_ASSET_PATH_MANIFEST);
			mManifest = assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		}

        public string[] GetAllDependencies(string assetBundleName){
           return mManifest.GetAllDependencies(assetBundleName);
        }

	}
}
