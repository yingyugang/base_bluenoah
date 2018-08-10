using BlueNoah.Download;
using BlueNoah.IO;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Assets
{
    public class AssetBundleLoader
    {
        AssetBundleLoadManager mAssetBundleLoadManager;

        int mDependenciesCount;

        public AssetBundleLoader(AssetBundleLoadManager assetBundleLoadManager)
        {
            mAssetBundleLoadManager = assetBundleLoadManager;
        }

        public void LoadOrDownloadAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            GetAllDependencies(assetBundleName, onGet);
        }

        void GetAllDependencies(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            string[] dependencies = mAssetBundleLoadManager.assetBundleServiceManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
            {
                if (mDependenciesCount == 0)
                {
                    GetAssetBundle(assetBundleName, onGet);
                }
            }
            else
            {
                mDependenciesCount = dependencies.Length;
                for (int i = 0; i < dependencies.Length; i++)
                {
                    GetDependenciesAssetBundle(dependencies[i], (AssetBundle assetBundle) =>
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

        void GetDependenciesAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            GetAssetBundle(assetBundleName, onGet);
        }

        public AssetBundle LoadAssetBundleFromLocal(string assetBundleName)
        {
            string[] dependencies = mAssetBundleLoadManager.assetBundleServiceManifest.GetAllDependencies(assetBundleName);
            for (int i = 0; i < dependencies.Length; i++)
            {
                //Load the dependencies into memory.this is have to do.
                GetAssetBundleFromLocal(dependencies[i]);
            }
            return GetAssetBundleFromLocal(assetBundleName);
        }

        AssetBundle GetAssetBundleFromLocal(string assetBundleName)
        {
            if (AssetBundleLoadManager.Instance.assetBundleServiceCache.Contains(assetBundleName))
            {
                return AssetBundleLoadManager.Instance.assetBundleServiceCache.GetCached(assetBundleName);
            }
            else if (FileManager.Exists(DownloadConstant.GetDownloadAssetBundlePath(assetBundleName)))
            {
                return LoadAndCacheAssetBundle(assetBundleName);
            }
            return null;
        }

        void GetAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            AssetBundle assetBundle = GetAssetBundleFromLocal(assetBundleName);
            if (assetBundle != null)
            {
                if (onGet != null)
                {
                    onGet(AssetBundleLoadManager.Instance.assetBundleServiceCache.GetCached(assetBundleName));
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
