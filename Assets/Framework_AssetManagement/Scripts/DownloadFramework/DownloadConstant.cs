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


        public const string CLIENT_SOUNDS_PATH = "";

        public const string SERVER_SOUNDS_PATH = "";

        public const string CLIENT_IMAGES_PATH = "";

        public const string INFORMATION_IMAGE_PATH = "";

        public const string SERVER_VERSION_PATH = "";

        public const string SERVER_ASSETBUNDLES_PATH = "";

        public const string CLIENT_VERSION_PATH = "";

        public const string CLIENT_ASSETBUNDLES_PATH = "";

        public static string LOCAL_VERSION_CONFIG_PATH {
            get{
                return "";
            }
        }

        public static string REMOTE_VERSION_CONFIG_PATH{
            get{
                return "";
            }
        }

        public const string CLIENT_SERVER_RESOURCE_VERSION_CSV = "";

        public const string CLIENT_CLIENT_RESOURCE_VERSION_CSV = "";

    }
}
