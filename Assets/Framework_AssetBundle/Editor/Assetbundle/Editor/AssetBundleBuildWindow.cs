using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BlueNoah.IO;
using System.IO;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleBuildWindow : EditorWindow
    {

        static AssetBundleBuildWindow mAssetBundleBuildWindow;

        AssetBundleBuildWindowGUI mAssetBundleWindowGUI;

        AssetBundleSettingService mAssetBundleSettingService;

        AssetBundleBuildService mAssetBundleBuildService;

        protected AssetConfig mAssetBundleConfig;

        protected Dictionary<string, AssetBundleWindowItem> mAssetBundleWindowItemDic;

        protected List<AssetBundleWindowItem> mAssetBundleWindowItemList;

        protected ulong mTotalAssetBundleSize;

        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        [MenuItem(AssetBundleEditorConstant.ASSETBUNDLE_BUILD_WINDOW_MENUITEM + MenuItemShortcutKeyConstant.SHORTCUT_KEY_ASSETBUNDLE_BUILD)]
        static void Init()
        {
            mAssetBundleBuildWindow = GetWindow<AssetBundleBuildWindow>(true, "AB Build Manager", true);
            mAssetBundleBuildWindow.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
            mAssetBundleBuildWindow.Show();
            mAssetBundleBuildWindow.Focus();
        }

        void OnEnable()
        {
            LoadAssetBundleInfos();
            InitContent("AssetBundleBuild", "Build and manage assetbundles.");
            mAssetBundleWindowGUI = new AssetBundleBuildWindowGUI(this);
            mAssetBundleSettingService = new AssetBundleSettingService();
            mAssetBundleBuildService = new AssetBundleBuildService();
        }

        protected void InitContent(string title, string tooltip)
        {
            GUIContent guiContent = new GUIContent();
            guiContent.text = title;
            guiContent.tooltip = tooltip;
            titleContent = guiContent;
        }

        void LoadAssetBundleInfos()
        {
            mAssetBundleConfig = mAssetBundleBuildService.LoadAssetBundleConfig();
            mAssetBundleWindowItemList = mAssetBundleBuildService.InitAssetBundleWindowItemsFromAssetBundleSetting();
            mAssetBundleWindowItemDic = mAssetBundleBuildService.GetAssetBundleWindowItemDic(mAssetBundleWindowItemList);
            mTotalAssetBundleSize = mAssetBundleBuildService.GetTotalAssetBundleSize(mAssetBundleWindowItemList);
        }

        void OnGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("It will can't be display when playing.");
                return;
            }
            mAssetBundleWindowGUI.DrawAssetBundlePattern(mAssetBundleWindowItemList);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Totle Size : " + mAssetBundleBuildService.FileLengthToStr(mTotalAssetBundleSize));
            EditorGUILayout.EndHorizontal();
            mAssetBundleWindowGUI.DrawBottomButtonsPattern();
        }

        void SaveConfig()
        {
            LoadAssetBundleInfos();
            AssetConfig assetBundleConfig = ConvertAssetBundleWindowItemsToAssetBundleConfig();
            string assetBundleConfigStr = JsonUtility.ToJson(assetBundleConfig, true);
            FileManager.WriteString(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE, assetBundleConfigStr);
        }

        AssetConfig ConvertAssetBundleWindowItemsToAssetBundleConfig()
        {
            AssetConfig assetBundleConfig = new AssetConfig();
            assetBundleConfig.items = new List<AssetConfigItem>();
            for (int i = 0; i < mAssetBundleWindowItemList.Count; i++)
            {
                assetBundleConfig.items.Add(ConvertAssetBundleWindowItemToAssetBundleConfigItem(mAssetBundleWindowItemList[i]));
            }
            return assetBundleConfig;
        }

        AssetConfigItem ConvertAssetBundleWindowItemToAssetBundleConfigItem(AssetBundleWindowItem assetBundleWindowItem)
        {
            AssetConfigItem assetBundleConfigItem = new AssetConfigItem();
            assetBundleConfigItem.assetName = assetBundleWindowItem.assetBundleName;
            assetBundleConfigItem.hashCode = assetBundleWindowItem.assetBundleHash;
            assetBundleConfigItem.size = assetBundleWindowItem.assetBundleLength;
            return assetBundleConfigItem;
        }

        public void SetAssetBundleNames()
        {
            mAssetBundleSettingService.SetAssetBundleNames();
            this.LoadAssetBundleInfos();
        }

        public void SelectAll()
        {
            for (int i = 0; i < mAssetBundleWindowItemList.Count; i++)
            {
                mAssetBundleWindowItemList[i].isSelected = true;
            }
        }

        public void UnSelectAll()
        {
            for (int i = 0; i < mAssetBundleWindowItemList.Count; i++)
            {
                mAssetBundleWindowItemList[i].isSelected = false;
            }
        }

        public void SelectDependencies(string abName)
        {
            List<Object> objs = new List<Object>();
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
            foreach (string p in paths)
            {
                objs.Add(AssetDatabase.LoadAssetAtPath(p, typeof(Object)));
            }
            Selection.objects = objs.ToArray();
        }

        public void CopySelectAssetBundleToServer()
        {
            FileManager.DirectoryCopy(AssetBundleEditorConstant.ASSETBUNDLE_ROOT_PATH, AssetBundleEditorConstant.ASSETBUNDLE_UPLOAD_PATH, true);
            Debug.Log(string.Format("{0} <color=green>==></color> {1}", AssetBundleEditorConstant.ASSETBUNDLE_ROOT_PATH, AssetBundleEditorConstant.ASSETBUNDLE_UPLOAD_PATH));
        }

        public void BuildAllAssetBundlesWithDependencies()
        {
            Debug.Log("BuildAllAssetBundlesWithDependencies!");
            AssetDatabase.RemoveUnusedAssetBundleNames();
            FileManager.CreateDirectoryIfNotExisting(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH);
            BuildPipeline.BuildAssetBundles(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            LoadAssetBundleInfos();
            SaveConfig();
            AssetDatabase.Refresh();
        }

        public void BuildSelectAssetBundlesWithOutDependencies()
        {
            Debug.Log("BuildSelectAssetBundlesWithOutDependencies!");
            AssetDatabase.RemoveUnusedAssetBundleNames();
            List<AssetBundleBuild> assetbundleList = GetSelectedEntities();
            if (assetbundleList.Count > 0)
            {
                BuildPipeline.BuildAssetBundles(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH, assetbundleList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
            AssetDatabase.Refresh();
        }

        public void RemoveUnusedABName()
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            LoadAssetBundleInfos();
        }

        List<AssetBundleBuild> GetSelectedEntities()
        {
            List<AssetBundleBuild> assetbundleList = new List<AssetBundleBuild>();
            for (int i = 0; i < mAssetBundleWindowItemList.Count; i++)
            {
                if (mAssetBundleWindowItemList[i].isSelected)
                {
                    assetbundleList.Add(CreateAssetBundleBuild(mAssetBundleWindowItemList[i]));
                }
            }
            return assetbundleList;
        }

        AssetBundleBuild CreateAssetBundleBuild(AssetBundleWindowItem entity)
        {
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = entity.assetBundleName.Split('.')[0];
            abb.assetBundleVariant = entity.assetBundleName.Split('.')[1];
            abb.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(entity.assetBundleName);
            return abb;
        }
    }



    public class AssetBundleWindowItem
    {
        public string assetBundleName;
        public string assetBundleHash;
        public ulong assetBundleLength;
        public Object assetBundle;
        public Object resourcesFolder;
        public string displayLength;
        public bool isSelected;
    }


    [System.Serializable]
    public class AssetConfig
    {
        public List<AssetConfigItem> items;
    }

    [System.Serializable]
    public class AssetConfigItem
    {
        public int index;
        public string assetName;
        public int assetType;
        public ulong size;
        public string hashCode;
    }
}

