using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Assets
{
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

        public void LoadOrDownloadAssetBundle(string assetBundleName, UnityAction<AssetBundle> onGet)
        {
            AssetBundleLoader assetBundleLoader = new AssetBundleLoader(this);
            assetBundleLoader.LoadOrDownloadAssetBundle(assetBundleName, onGet);
        }

        public void LoadAsset<T>(string assetName, string assetBundleName, UnityAction<T> onLoaded) where T : Object
        {

            //#if UNITY_EDITOR && LOCAL_RES
            //string path = AssetBundleConstant.ASSETDATABASE_ASSETBUNDLE_RESOURCES_PATH + ;
            //UnityEditor.AssetDatabase.FindAssets();
            //#else
            //#endif
            LoadOrDownloadAssetBundle(assetBundleName, (AssetBundle assetBundle) =>
            {
                if (assetBundle != null)
                {
                    onLoaded(assetBundle.LoadAsset<T>(assetName));
                }
            });
        }

        public AssetBundle LoadAssetBundleFromLocal(string assetBundleName){
            AssetBundleLoader assetBundleLoader = new AssetBundleLoader(this);
            return assetBundleLoader.LoadAssetBundleFromLocal(assetBundleName);
        }

        public T LoadAssetFromLoad<T>(string assetBundleName , string assetName) where T : Object{
            AssetBundle assetBundle = LoadAssetBundleFromLocal(assetBundleName);
            return assetBundle.LoadAsset<T>(assetName);
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