using BlueNoah.Download;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Assets
{
    public class AssetBundleLoadManager : SimpleSingleMonoBehaviour<AssetBundleLoadManager>
    {
        public ManifestLoadService manifestManager;

        public DownloadManager downloadManager;

        public AssetBundleCacheService assetBundleCacheManager;

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
            manifestManager = new ManifestLoadService();
            manifestManager.LoadManifest();
            downloadManager = DownloadManager.Instance;
            assetBundleCacheManager = new AssetBundleCacheService();
        }

        public void Reload()
        {
            Init();
        }

        public void LoadOrDownloadAssetbundle(string assetBundleName, UnityAction<AssetBundle> onGet){
            AssetBundleLoader assetBundleLoader = new AssetBundleLoader(this);
            assetBundleLoader.LoadOrDownloadAssetbundle(assetBundleName,onGet);
        }

        public void LoadAsset<T>(UnityAction<T> onLoaded, string assetName, string assetBundleName) where T : Object
        {
            LoadOrDownloadAssetbundle(assetBundleName, (AssetBundle assetBundle) =>
            {
                if (assetBundle != null)
                {
                    onLoaded(assetBundle.LoadAsset<T>(assetName));
                }
            });
        }

        public void LoadAssets<T>(UnityAction<T[]> onLoaded, string assetBundleName) where T : Object
        {
            LoadOrDownloadAssetbundle(assetBundleName, (AssetBundle assetBundle) =>
            {
                if (assetBundle != null)
                {
                    onLoaded(assetBundle.LoadAllAssets<T>());
                }
            });
        }

    }
}