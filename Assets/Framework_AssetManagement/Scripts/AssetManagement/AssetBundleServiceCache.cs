using UnityEngine;
using System.Collections.Generic;

namespace BlueNoah.Assets
{
    public class AssetBundleServiceCache
    {
        Dictionary<string, AssetBundle> mCachedAssetBundles;

        public AssetBundleServiceCache()
        {
            mCachedAssetBundles = new Dictionary<string, AssetBundle>();
        }

        public bool Contains(string assetBundleName)
        {
            return mCachedAssetBundles.ContainsKey(assetBundleName);
        }

        public AssetBundle GetCached(string assetBundleName)
        {
            if (mCachedAssetBundles.ContainsKey(assetBundleName))
            {
                return mCachedAssetBundles[assetBundleName];
            }
            return null;
        }

        public void Cache(string assetBundleName, AssetBundle assetBundle)
        {
            if (!mCachedAssetBundles.ContainsKey(assetBundleName))
            {
                mCachedAssetBundles.Add(assetBundleName, assetBundle);
            }
        }

        public void Unload(string assetBundleName, bool isUnloadAll = false)
        {
            if (mCachedAssetBundles.ContainsKey(assetBundleName))
            {
                AssetBundle assetBundle = mCachedAssetBundles[assetBundleName];
                if (assetBundle != null)
                    assetBundle.Unload(isUnloadAll);
                mCachedAssetBundles.Remove(assetBundleName);
            }
        }

        public void UnloadAll(bool isUnloadAll = false)
        {
            foreach (string assetBundleName in mCachedAssetBundles.Keys)
            {
                AssetBundle assetBundle = mCachedAssetBundles[assetBundleName];
                if (assetBundle != null)
                    assetBundle.Unload(isUnloadAll);
            }
            mCachedAssetBundles.Clear();
        }
    }
}
