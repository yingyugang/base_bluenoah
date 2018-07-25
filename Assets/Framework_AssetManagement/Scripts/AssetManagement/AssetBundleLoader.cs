using BlueNoah.Download;
using BlueNoah.IO;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Assets
{
    public class AssetBundleLoader
    {
        AssetBundleLoadManager mAssetBundleLoadManager;

        public AssetBundleLoader(AssetBundleLoadManager assetBundleLoadManager)
        {
            mAssetBundleLoadManager = assetBundleLoadManager;
        }

        public void LoadOrDownloadAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            GetAllDependencies(assetBundleName, onGet);
        }

        int mDependenciesCount;

        //TODO Add new download when downloading .
        void GetAllDependencies(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            string[] dependencies = mAssetBundleLoadManager.assetBundleServiceManifest.GetAllDependencies(assetBundleName);
            if(dependencies.Length==0){
                if (mDependenciesCount == 0)
                {
                    GetAssetBundle(assetBundleName, onGet);
                }
            }else{
                mDependenciesCount = dependencies.Length;
                for (int i = 0; i < dependencies.Length; i++)
                {
                    GetDependencies(dependencies[i], (AssetBundle assetBundle) =>
                    {
                        mDependenciesCount--;
                        if (mDependenciesCount == 0)
                        {
                            GetAssetBundle(assetBundleName, onGet);
                        }
                    });
                }
            }
        }

        void GetDependencies(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            GetAssetBundle(assetBundleName, onGet);
        }

        void GetAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            if (AssetBundleLoadManager.Instance.assetBundleServiceCache.Contains(assetBundleName))
            {
                if (onGet != null)
                {
                    onGet(AssetBundleLoadManager.Instance.assetBundleServiceCache.GetCached(assetBundleName));
                }
            }
            else if (FileManager.Exists(DownloadConstant.GetDownloadAssetBundlePath(assetBundleName)))
            {
                if (onGet != null)
                {
                    onGet(LoadAndCacheAssetBundle(assetBundleName));
                }
            }
            else
            {
                GetAssetBundleFromRemote(assetBundleName, onGet);
            }
        }

        void GetAssetBundleFromRemote(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            DownloadManager.Instance.DownloadAssetBundle(assetBundleName, () =>
            {
                if (FileManager.Exists(DownloadConstant.GetDownloadAssetBundlePath(assetBundleName)))
                {
                    if (onGet != null)
                    {
                        onGet(LoadAndCacheAssetBundle(assetBundleName));
                    }
                }
            });
        }

        AssetBundle LoadAndCacheAssetBundle(string assetBundleName)
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(DownloadConstant.GetDownloadAssetBundlePath(assetBundleName));
            mAssetBundleLoadManager.assetBundleServiceCache.Cache(assetBundleName, assetBundle);
            return assetBundle;
        }
    }
}
