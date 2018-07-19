using BlueNoah.IO;
using UnityEngine;

namespace BlueNoah.Assets
{
    //TODO
    public static class AssetsManager
    {
        //TODO
        public static GameObject LoadPanelPrefab(string path)
        {
            path = FileManager.AssetDataPathToResourcesPath(path);
            Debug.Log(path);
            return Resources.Load<GameObject>(path);
        }

        //TODO
        public static GameObject LoadDialogPrefab(string path)
        {
            path = FileManager.AssetDataPathToResourcesPath(path);
            return Resources.Load<GameObject>(path);
        }

    }
}
