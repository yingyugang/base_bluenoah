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
            return LoadUIPrefab(path);
        }

        //TODO
        public static GameObject LoadDialogPrefab(string path)
        {
            return LoadUIPrefab(path);
        }

        static GameObject LoadUIPrefab(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
#else
            path = FileManager.AssetDataPathToResourcesPath(path);
            Debug.Log(path);
            return Resources.Load<GameObject>(path);
#endif
        }



    }
}
