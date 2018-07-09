using UnityEngine;

namespace BlueNoah.IO
{
    public static class ResourcesLoader
    {

        public static GameObject LoadPanelPrefab(string path)
        {
            path = FileManager.AssetDataPathToResourcesPath(path);
            Debug.Log(path);
            return Resources.Load<GameObject>(path);
        }

        public static GameObject LoadDialogPrefab(string path)
        {
            path = FileManager.AssetDataPathToResourcesPath(path);
            return Resources.Load<GameObject>(path);
        }

    }
}
