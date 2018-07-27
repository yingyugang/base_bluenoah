using System.IO;
using UnityEngine;

namespace BlueNoah.Download
{
	public static class DownloadConstant
	{

		public const string CONFIG_FILE = "assetbundle_config.json";

        public const int MAX_DOWNLOAD_COUNT = 1;

        public const int MAX_DOWNLOAD_SIZE = 10 * 1024 * 1024;

		public static string MANIFEST_FILE{
			get{
				return PLATFORM;
			}
		}

		public static string APPLICATION_DATA_PATH {
			get {
#if UNITY_EDITOR
				return Application.dataPath;
#else
                return Application.persistentDataPath;
#endif
			}
		}

		public static string REMOTE_ASSET_PATH_MANIFEST {
			get {
				return REMOTE_ASSET_PATH_BASE + MANIFEST_FILE;
			}
		}

		public static string REMOTE_ASSET_PATH_CONFIG {
			get {
				return REMOTE_ASSET_PATH_BASE + CONFIG_FILE;
			}
		}

		public static string REMOTE_ASSET_PATH (string path)
		{
			return REMOTE_ASSET_PATH_BASE + path;
		}

		public static string REMOTE_ASSET_PATH_BASE {
			get {
				return
#if DEVELOP
                    "http://127.0.0.1/DownloadSample/" + PLATFORM + "/";
#elif STG
                    "http://127.0.0.1/DownloadSample/" + PLATFORM + "/";
#elif PRODUCT
                    "http://127.0.0.1/DownloadSample/" + PLATFORM + "/";
#else

                    "http://ultrasoul.ifkzgph84v.ap-northeast-1.elasticbeanstalk.com/AssetBundleBuilds/" + PLATFORM + "/";
                    //"http://127.0.0.1/DownloadSample/" + PLATFORM + "/";
#endif
			}
		}

		public static string DOWNLOAD_ASSET_PATH_BASE_ROOT {
			get {
				return "/Framework_AssetManagement/DownloadAssets/" + PLATFORM + "/";
			}
		}

		public static string DOWNLOAD_ASSET_PATH_BASE {
			get {
				#if UNITY_EDITOR
				return Application.dataPath + DOWNLOAD_ASSET_PATH_BASE_ROOT;
				#else
                return Application.persistentDataPath + DOWNLOAD_ASSET_PATH_BASE_ROOT;
				#endif
			}
		}

		public static string DOWNLOAD_ASSET_PATH_MANIFEST {
			get {
				return DOWNLOAD_ASSET_PATH_BASE + MANIFEST_FILE;
			}
		}

		public static string GetDownloadAssetBundlePath (string path)
		{
			return DOWNLOAD_ASSET_PATH_BASE + path;
		}

		public static string PLATFORM {
			get {
				#if UNITY_EDITOR
				return EDITOR_PLATFORM;
				#else
				return DEPLOY_PLATFORM;
				#endif
			}

		}

		#if UNITY_EDITOR
		static string EDITOR_PLATFORM {
			get { 
				switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget) {
				case UnityEditor.BuildTarget.Android:
					return "Android";
				case UnityEditor.BuildTarget.iOS:
					return "iOS";
				case UnityEditor.BuildTarget.WebGL:
					return "WebGL";
            #if UNITY_2018_1_OR_NEWER
                case UnityEditor.BuildTarget.StandaloneOSX:
            #else
                case UnityEditor.BuildTarget.StandaloneOSXUniversal:
            #endif
					return "StandardOSX";
				case UnityEditor.BuildTarget.StandaloneWindows:
					return "StandardWindows";
				default:
					return "StandardOSX";
				}
			}
		}
		#endif

		static string DEPLOY_PLATFORM {
			get { 
				switch (Application.platform) {
				case RuntimePlatform.Android:
					return "Android";
				case RuntimePlatform.IPhonePlayer:
					return "iOS";
				case RuntimePlatform.WebGLPlayer:
					return "WebGL";
				case RuntimePlatform.OSXPlayer:
					return "StandardOSX";
				case RuntimePlatform.WindowsPlayer:
					return "StandardWindows";
				default:
					return "StandardOSX";
				}
			}
		}
	}
}
