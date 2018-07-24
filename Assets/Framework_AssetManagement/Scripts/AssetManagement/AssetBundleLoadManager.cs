using BlueNoah.Download;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Assets
{
    public class AssetBundleLoadManager : SimpleSingleMonoBehaviour<AssetBundleLoadManager>
    {

        public ManifestLoadManager manifestManager;

        public DownloadManager downloadManager;

        public AssetBundleCacheManager assetBundleCacheManager;

        protected override void Awake()
        {
            base.Awake();
        }

        void OnDestroy()
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }

        public void Init()
        {
            manifestManager = new ManifestLoadManager();
            manifestManager.LoadManifest();
            downloadManager = DownloadManager.Instance;
            assetBundleCacheManager = new AssetBundleCacheManager();
        }

        public void Reload()
        {
            Init();
        }

        public void LoadOrDownloadAssetbundle(string assetBundleName, UnityAction<AssetBundle> onGet){
            AssetBundleLoader assetBundleLoader = new AssetBundleLoader(this);
            assetBundleLoader.LoadOrDownloadAssetbundle(assetBundleName,onGet);
        }
    }
}