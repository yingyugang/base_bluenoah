using BlueNoah.Assets;
using BlueNoah.Utility;
using UnityEngine;
using UnityEngine.Events;

public class AssetLoadManager : SimpleSingleMonoBehaviour<AssetLoadManager>
{

    AssetBundleLoadManager mAssetBundleLoadManager;

    protected override void Awake()
    {
        base.Awake();
        mAssetBundleLoadManager = gameObject.GetOrAddComponent<AssetBundleLoadManager>();
        mAssetBundleLoadManager.Init();
    }

    public void LoadAsset<T>(UnityAction<T> onLoaded, string assetName, string assetBundleName) where T : Object
    {
        mAssetBundleLoadManager.LoadOrDownloadAssetbundle(assetBundleName, (AssetBundle assetBundle) =>
        {
            if (assetBundle != null)
            {
                onLoaded(assetBundle.LoadAsset<T>(assetName));
            }
        });
    }

    public void LoadAssets<T>(UnityAction<T[]> onLoaded, string assetBundleName) where T : Object
    {
        mAssetBundleLoadManager.LoadOrDownloadAssetbundle(assetBundleName, (AssetBundle assetBundle) =>
        {
            if (assetBundle != null)
            {
                onLoaded(assetBundle.LoadAllAssets<T>());
            }
        });
    }
}
