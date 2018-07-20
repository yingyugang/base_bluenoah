using System.IO;
using UnityEngine;

namespace BlueNoah.Download
{
    public static class DownloadConstant
    {

        public const string SERVER_ASSET_URL = "127.0.0.1/BlueNoah/Assets";

        public const string CONFIG_FILE = "assetbundle_config.json";

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


        public const string CLIENT_SOUNDS_PATH = "";

        public const string SERVER_SOUNDS_PATH = "";

        public const string CLIENT_IMAGES_PATH = "";

        public const string INFORMATION_IMAGE_PATH = "";

        public const string SERVER_VERSION_PATH = "";

        public const string SERVER_ASSETBUNDLES_PATH = "";

        public const string CLIENT_VERSION_PATH = "";

        public const string CLIENT_ASSETBUNDLES_PATH = "";

        public static string REMOTE_ASSET_CONFIG_PATH
        {
            get
            {
                return REMOTE_ASSET_BASE_PATH + CONFIG_FILE;
            }
        }

        public static string REMOTE_ASSET_PATH(string path)
        {
            return REMOTE_ASSET_BASE_PATH + path;
        }

        public static string REMOTE_ASSET_BASE_PATH
        {
            get
            {
                return
#if DEVELOP
                    "http://127.0.0.1/ultrasoul/" + ASSET_PLATFORM + "/";
#elif STG
                    "https://s3-ap-northeast-1.amazonaws.com/stg-resource.koekatsu.d2cr.jp/" + ASSET_PLATFORM + "/";
#elif PRODUCT
                    "https://s3-ap-northeast-1.amazonaws.com/resource.koekatsu.d2cr.jp/" + ASSET_PLATFORM + "/";
#else
                    "http://127.0.0.1/ultrasoul/BuildAssets/" + ASSET_PLATFORM + "/";
#endif
            }
        }

        public static string DOWNLOAD_ASSET_BASE_PATH
        {
            get
            {
                return Application.dataPath + "/Framework_AssetManagement/DownloadAssets/" + ASSET_PLATFORM + "/";
            }
        }

        public static string DOWNLOAD_ASSET_CONFIG_PATH
        {
            get
            {
                return DOWNLOAD_ASSET_BASE_PATH + CONFIG_FILE;
            }
        }

        public static string DOWNLOAD_ASSET_PATH(string path)
        {
            return DOWNLOAD_ASSET_BASE_PATH + path;
        }

        public const string CLIENT_SERVER_RESOURCE_VERSION_CSV = "";

        public const string CLIENT_CLIENT_RESOURCE_VERSION_CSV = "";


        public static string ASSET_PLATFORM
        {
            get
            {
 #if UNITY_EDITOR
                switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
                {
                    case UnityEditor.BuildTarget.Android:
                        return "Android";
                    case UnityEditor.BuildTarget.iOS:
                        return "iOS";
                    case UnityEditor.BuildTarget.WebGL:
                        return "WebGL";
                    default:
                        return "Standard";
                }
#else
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        return "Android";
                    case RuntimePlatform.IPhonePlayer:
                        return "iOS";
                    case RuntimePlatform.WebGLPlayer:
                        return "WebGL";
                    default:
                        return "Standard";
                }
#endif
            }

        }

    }
}
