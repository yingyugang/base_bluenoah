using System.IO;
using UnityEngine;
//TODO
namespace BlueNoah.Download
{
    public static class DownloadConstant
    {

        public const string SERVER_ASSET_URL = "127.0.0.1/BlueNoah/Assets";

        public static string APPLICATION_DATA_PATH
        {
            get
            {
#if UNITY_EDITOR
                return Application.dataPath;
#else
                return Application.persistentDataPath;
#endif
            }
        }

        public static string DOWNLOAD_ASSET_PATH
        {
            get
            {
                return Path.Combine(APPLICATION_DATA_PATH, "DownloadAssets");
            }
        }
    }
}
