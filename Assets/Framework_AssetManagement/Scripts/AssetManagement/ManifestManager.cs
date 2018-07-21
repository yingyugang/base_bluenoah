using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlueNoah.Download;

namespace BlueNoah.Assets
{
	public class ManifestManager
	{
		AssetBundleManifest mManifest;

		public void LoadManifest ()
		{
			AssetBundle.UnloadAllAssetBundles (false);
			AssetBundle assetBundle = AssetBundle.LoadFromFile (DownloadConstant.DOWNLOAD_ASSET_PATH_MANIFEST);
			mManifest = assetBundle.LoadAsset<AssetBundleManifest> ("AssetBundleManifest");
		}
	}
}
