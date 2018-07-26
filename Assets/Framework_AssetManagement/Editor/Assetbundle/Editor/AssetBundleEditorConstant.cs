using BlueNoah.Download;
using UnityEngine;

public static class AssetBundleEditorConstant
{
    public const string ASSETBUNDLE_BUILD_WINDOW_MENUITEM = "Tools/AssetBundle/AB Build Manager";
    public const string ASSETBUNDLE_SETTING_WINDOW_MENUITEM = "Tools/AssetBundle/AB Settings";
    public const string ASSETBUNDLE_PATH = "/Framework_AssetManagement/AssetBundleBuilds/";
    public const string ASSETBUNDLE_RESOURCES_PATH = "/Framework_AssetManagement/AssetBundleResources/";
    public const string ASSETBUNDLE_UPLOAD_PATH = "/Applications/XAMPP/xamppfiles/htdocs/DownloadSample/";

    public static string ASSETBUNDLE_ROOT_PATH
    {
        get
        {
            return Application.dataPath + ASSETBUNDLE_PATH;
        }
    }

    public static string ASSETBUNDLE_PLATFORM_PATH
    {
        get
        {
            return ASSETBUNDLE_ROOT_PATH + DownloadConstant.PLATFORM + "/";
        }
    }

    public static string ASSETDATABASE_PLATFORM_PATH
    {
        get
        {
            return "Assets" + ASSETBUNDLE_PATH + DownloadConstant.PLATFORM + "/";
        }
    }

    public static string ASSETBUNDLE_PLATFORM_CONFIG_FILE
    {
        get
        {
            return ASSETBUNDLE_PLATFORM_PATH + DownloadConstant.CONFIG_FILE;
        }
    }

    public static string SYSTEM_ASSETBUNDLE_RESOURCES_PATH
    {
        get
        {
            return Application.dataPath + ASSETBUNDLE_RESOURCES_PATH;
        }
    }

    public static string ASSETDATABASE_ASSETBUNDLE_RESOURCES_PATH
    {
        get
        {
            return "Assets" + ASSETBUNDLE_RESOURCES_PATH;
        }
    }

}
