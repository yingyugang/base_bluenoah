using System;
using BlueNoah.Download;
using BlueNoah.Utility;
using UnityEngine;

namespace BlueNoah.Assets
{
	public class AssetBundleManager:SimpleSingleMonoBehaviour<AssetBundleManager>
	{

		ManifestManager mManifestManager;

		DownloadManager mDownloadManager;

		protected override void Awake ()
		{
			base.Awake ();
			Init ();
		}

		void Init(){
			mManifestManager = new ManifestManager ();
			mManifestManager.LoadManifest ();
		}

		public void Reload(){
			Init();
		}

	}
}

