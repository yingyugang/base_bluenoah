using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Assets
{
    //TODO Restart when Manifest and json not existing.
    public class AssetBundleLoadManager : SimpleSingleMonoBehaviour<AssetBundleLoadManager>
    {
        public AssetBundleServiceManifest assetBundleServiceManifest;

        public AssetBundleServiceCache assetBundleServiceCache;

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        void OnDestroy()
        {
            AssetBundle.UnloadAllAssetBundles(false);
        }

        public void Init()
        {
            assetBundleServiceManifest = new AssetBundleServiceManifest();
            assetBundleServiceManifest.LoadManifest();
            assetBundleServiceCache = new AssetBundleServiceCache();
        }

        public void Reload()
        {
            Init();
        }

        public void LoadOrDownloadAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet){
            AssetBundleLoader assetBundleLoader = new AssetBundleLoader(this);
            assetBundleLoader.LoadOrDownloadAssetBundle(assetBundleName,onGet);
        }

        public void LoadAsset<T>(UnityAction<T> onLoaded, string assetName, string assetBundleName) where T : Object
        {
            LoadOrDownloadAssetBundle(assetBundleName, (AssetBundle assetBundle) =>
            {
                if (assetBundle != null)
                {
                    onLoaded(assetBundle.LoadAsset<T>(assetName));
                }
            });
        }

        public void LoadAssets<T>(UnityAction<T[]> onLoaded, string assetBundleName) where T : Object
        {
            LoadOrDownloadAssetBundle(assetBundleName, (AssetBundle assetBundle) =>
            {
                if (assetBundle != null)
                {
                    onLoaded(assetBundle.LoadAllAssets<T>());
                }
            });
        }
    }
}