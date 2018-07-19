using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BlueNoah.IO;
using BlueNoah.Editor;

namespace BlueNoah.Editor.AssetBundle.Management
{
    //Auto record the ABs what hashcode has been changed.
    public class AssetBundleBuildWindow : AssetBundleWindow
    {

        static AssetBundleBuildWindow mAssetBundleBuildWindow;
        AssetBundleBuildWindowGUI mAssetBundleWindowGUI;

        const int WINDOW_WIDTH = 800;
        const int WINDOW_HEIGHT = 600;

        [MenuItem(AssetBundleEditorConstant.ASSETBUNDLE_BUILD_WINDOW_MENUITEM + MenuItemShortcutKeyConstant.SHORTCUT_KEY_ASSETBUNDLE_BUILD)]
        static void Init()
        {
            InitPath();
            mAssetBundleBuildWindow = GetWindow<AssetBundleBuildWindow>(true, "AB Build Manager", true);
            mAssetBundleBuildWindow.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
            mAssetBundleBuildWindow.Show();
            mAssetBundleBuildWindow.Focus();
        }

        static GUIStyle styleRed;
        long mTotalSize;
        static string tempResourceAssetPath;
        static string streamPath;
        static string fullTmpVersionOutputPath;
        static string configPath;
        const string localABServerPath = "/Applications/XAMPP/xamppfiles/htdocs/mmorpg/";
        const string serverCSV = "server_resource.csv";

        static void InitPath()
        {
            tempResourceAssetPath = Application.dataPath + "/Assetbundles/temp/";
            streamPath = Application.streamingAssetsPath + EditorUserBuildSettings.activeBuildTarget.ToString();
            configPath = Application.dataPath + "/Assetbundles/" + serverCSV;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Reload();
            InitContent("AssetBundleBuild", "Build and manage assetbundles.");
            AssetBundleWindowEvents assetBundleWindowEvents = InitAssetBundleWindowEvents();
            mAssetBundleWindowGUI = new AssetBundleBuildWindowGUI(assetBundleWindowEvents);
        }

        AssetBundleWindowEvents InitAssetBundleWindowEvents()
        {
            AssetBundleWindowEvents assetBundleWindowEvents = new AssetBundleWindowEvents();
            assetBundleWindowEvents.onSelectAssets = SelectDependencies;
            assetBundleWindowEvents.onBuildAll = BuildAllAssetBundlesWithDependencies;
            assetBundleWindowEvents.onBuildSelected = BuildSelectAssetBundlesWithOutDependencies;
            assetBundleWindowEvents.onSaveConfig = SaveConfig;
            return assetBundleWindowEvents;
        }

        void OnGUI()
        {
            mAssetBundleWindowGUI.DrawAssetBundlePattern(mAssetBundleItemList);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("RemoveUnusedABName", GUILayout.Width(150)))
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
            GUILayout.Label("Totle Size : " + FileLengthToStr(mTotalSize));
            EditorGUILayout.EndHorizontal();
            mAssetBundleWindowGUI.DrawBottomButtonsPattern();
            mAssetBundleWindowGUI.DrawHashCodeFile(serverHash);
        }

        void Reload()
        {
            InitPath();
            //InitBuild();
            InitStyle();
        }

        void SaveConfig()
        {
            Debug.Log("SaveConfig");
            LoadAssetBundleInfos();
            AssetBundleConfig assetBundleConfig = ConvertAssetBundleWindowItemsToAssetBundleConfig();
            string assetBundleConfigStr = JsonUtility.ToJson(assetBundleConfig,true);
            FileManager.WriteString(ASSETBUNDLE_PLATFORM_CONFIG_FILE,assetBundleConfigStr);
        }

        AssetBundleConfig ConvertAssetBundleWindowItemsToAssetBundleConfig(){
            AssetBundleConfig assetBundleConfig = new AssetBundleConfig();
            assetBundleConfig.items = new List<AssetBundleConfigItem>();
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                assetBundleConfig.items.Add(ConvertAssetBundleWindowItemToAssetBundleConfigItem(mAssetBundleItemList[i]));
            }
            return assetBundleConfig;
        }

        AssetBundleConfigItem ConvertAssetBundleWindowItemToAssetBundleConfigItem(AssetBundleWindowItem assetBundleWindowItem){
            AssetBundleConfigItem assetBundleConfigItem = new AssetBundleConfigItem();
            assetBundleConfigItem.name = assetBundleWindowItem.assetBundleName;
            assetBundleConfigItem.hash = assetBundleWindowItem.assetBundleHash;
            assetBundleConfigItem.length = assetBundleWindowItem.assetBundleLength;
            return assetBundleConfigItem;
        }


        void InitStyle()
        {
            styleRed = new GUIStyle();
            styleRed.active.textColor = Color.red;
        }

        static string serverHash = "";
        Vector2 srollPos;

        void OnGetFileHashCode()
        {
            serverHash = FileManager.GetFileHash(configPath);
        }

        void SelectAll(List<AssetBundleWindowItem> allAssetBundleEntitys)
        {
            for (int i = 0; i < allAssetBundleEntitys.Count; i++)
            {
                allAssetBundleEntitys[i].isSelected = true;
            }
        }

        void UnSelectAll(List<AssetBundleWindowItem> allAssetBundleEntitys)
        {
            for (int i = 0; i < allAssetBundleEntitys.Count; i++)
            {
                allAssetBundleEntitys[i].isSelected = false;
            }
        }

        void SelectDependencies(string abName)
        {
            List<Object> objs = new List<Object>();
            string[] paths = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
            foreach (string p in paths)
            {
                objs.Add(AssetDatabase.LoadAssetAtPath(p, typeof(Object)));
            }
            Selection.objects = objs.ToArray();
        }

        void BuildAllAssetBundlesWithDependencies()
        {
            Debug.Log("BuildAllAssetBundlesWithDependencies!");
            AssetDatabase.RemoveUnusedAssetBundleNames();
            FileManager.CreateDirectoryIfNotExisting(ASSETBUNDLE_PLATFORM_PATH);
            BuildPipeline.BuildAssetBundles(ASSETBUNDLE_PLATFORM_PATH, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }

        void BuildSelectAssetBundlesWithOutDependencies()
        {
            Debug.Log("BuildSelectAssetBundlesWithOutDependencies");
            AssetDatabase.RemoveUnusedAssetBundleNames();
            List<AssetBundleBuild> assetbundleList = GetSelectedEntities();
            if (assetbundleList.Count > 0)
            {
                BuildPipeline.BuildAssetBundles(ASSETBUNDLE_PLATFORM_PATH, assetbundleList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
            AssetDatabase.Refresh();
        }

        //TODO
        void RemoveABNotInABList()
        {
            HashSet<string> setStrs = new HashSet<string>();
            foreach (AssetBundleWindowItem entity in mAssetBundleItemList)
            {
                //if(entity.isSelected)
                setStrs.Add(entity.assetBundleName);
            }
            string[] paths = Directory.GetFiles(ASSETBUNDLE_PLATFORM_PATH);
            for (int i = 0; i < paths.Length; i++)
            {
                string fileName = paths[i].Substring(paths[i].LastIndexOf("/", System.StringComparison.CurrentCulture) + 1);
                if (!setStrs.Contains(fileName) && fileName != serverCSV)
                    FileManager.DeleteFile(paths[i]);
            }
        }

        void GetHash()
        {
            mTotalSize = 0;
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                mAssetBundleItemList[i].assetBundleHash = FileManager.GetFileHash(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName);
                if (mAssetBundleItemList[i].assetBundleHash != null && mAssetBundleItemList[i].assetBundleHash.Trim() != "")
                {
                    mAssetBundleItemList[i].assetBundleLength = new FileInfo(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName).Length;
                    mAssetBundleItemList[i].displayLength = FileLengthToStr(mAssetBundleItemList[i].assetBundleLength);
                    mTotalSize += mAssetBundleItemList[i].assetBundleLength;
                }
            }
        }

        List<AssetBundleBuild> GetSelectedEntities()
        {
            List<AssetBundleBuild> assetbundleList = new List<AssetBundleBuild>();
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                if (mAssetBundleItemList[i].isSelected)
                {
                    assetbundleList.Add(CreateAssetBundleBuild(mAssetBundleItemList[i]));
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

        void CopySelectToTemp()
        {
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                if (mAssetBundleItemList[i].isSelected)
                {
                    if (FileManager.Exists(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName))
                        FileManager.CopyFile(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName, tempResourceAssetPath + mAssetBundleItemList[i].assetBundleName);
                }
            }
            AssetDatabase.Refresh();
            Debug.Log("Done");
        }

        void CopySelectToLocalServer()
        {
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                if (mAssetBundleItemList[i].isSelected)
                {
                    if (FileManager.Exists(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName))
                        FileManager.CopyFile(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName, localABServerPath + ASSETBUNDLE_PLATFORM_PATH.Replace(Application.dataPath, "") + mAssetBundleItemList[i].assetBundleName);
                }
            }
            AssetDatabase.Refresh();
            Debug.Log("Done");
        }

        void CopyToStream()
        {
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                if (mAssetBundleItemList[i].isSelected)
                {
                    FileManager.CopyFile(ASSETBUNDLE_PLATFORM_PATH + mAssetBundleItemList[i].assetBundleName, streamPath + mAssetBundleItemList[i].assetBundleName);
                }
            }
            AssetDatabase.Refresh();
        }

        void CreateVersion()
        {
            Caching.ClearCache();
            StringBuilder result = new StringBuilder();
            string title = "FileName,FileSize,IsCSV,HashCode";
            result.AppendLine(title);
            foreach (AssetBundleWindowItem abEntity in mAssetBundleItemList)
            {
                //			if (abEntity.isSelected) {
                if (abEntity.assetBundleHash != null && abEntity.assetBundleHash.Trim() != "")
                {
                    result.AppendLine(CreateLine(abEntity.assetBundleName, new FileInfo(ASSETBUNDLE_PLATFORM_PATH + abEntity.assetBundleName).Length, abEntity.assetBundleName == "csv.assetbundle" ? 1 : 0, abEntity.assetBundleHash).ToString());
                }
                //			}
            }
            //		result.Append (GetMovieHash ());
            FileManager.WriteString(configPath, result.ToString());
            serverHash = FileManager.GetFileHash(configPath);
            AssetDatabase.Refresh();
            Debug.Log("CreateVersion successfully");
        }

        const string comma = ",";

        StringBuilder CreateLine(string name, long size, int isCsv, string hashCode)
        {
            StringBuilder line = new StringBuilder();
            line.Append(name);
            line.Append(comma);
            line.Append(size);
            line.Append(comma);
            line.Append(isCsv);
            line.Append(comma);
            line.Append(hashCode);
            return line;
        }
    }
}

