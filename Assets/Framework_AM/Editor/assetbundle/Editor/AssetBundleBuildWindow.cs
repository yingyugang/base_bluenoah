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

        [MenuItem(AssetBundleEditorConstant.ASSETBUNDLE_BUILD_WINDOW_MENUITEM + MenuItemShortcutKeyConstant.SHORTCUT_KEY_ASSETBUNDLE_BUILD)]
        static void Init()
        {
            InitPath();
            mAssetBundleBuildWindow = GetWindow<AssetBundleBuildWindow>(true, "AB Build Manager", true);
            mAssetBundleBuildWindow.Show();
            mAssetBundleBuildWindow.Focus();
        }

        static GUIStyle styleRed;
        long mTotalSize;
        static string[] allAssetBundleNames;
        static string[] allStreamAssetBundleNames;
        static string[] allStreamAssetPath;
        static string fullResourceAssetPath;
        static string tempResourceAssetPath;
        static string fullStreamPath;
        static string fullTmpOutputPath;
        static string fullTmpVersionOutputPath;
        static string fullServerCSVPath;
        const string localABServerPath = "/Applications/XAMPP/xamppfiles/htdocs/mmorpg/";
        const string serverCSV = "server_resource.csv";

        static void InitPath()
        {
            fullResourceAssetPath = Application.dataPath + "/DownloadResources/assets/";
            tempResourceAssetPath = Application.dataPath + "/Assetbundles/temp/";
            fullStreamPath = Application.streamingAssetsPath + Application.platform.ToString();
            fullTmpOutputPath = Application.dataPath + "/Assetbundles/" + Application.platform.ToString();
            fullServerCSVPath = Application.dataPath + "/Assetbundles/" + serverCSV;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Reload();
            InitContent("AssetBundleBuild","Build and manage assetbundles.");
            AssetBundleWindowEvents assetBundleWindowEvents = InitAssetBundleWindowEvents();
            mAssetBundleWindowGUI = new AssetBundleBuildWindowGUI(assetBundleWindowEvents);
        }

        AssetBundleWindowEvents InitAssetBundleWindowEvents()
        {
            AssetBundleWindowEvents assetBundleWindowEvents = new AssetBundleWindowEvents();
            assetBundleWindowEvents.onSelectAssets = SelectDependencies;
            assetBundleWindowEvents.onBuildAll = BuildAllAssetBundlesWithDependencies;
            assetBundleWindowEvents.onBuildSelected = BuildSelectAssetBundlesWithOutDependencies;
            return assetBundleWindowEvents;
        }

        void OnGUI()
        {
            OnBuildGUI();
        }

        void Reload()
        {
            InitPath();
            //InitBuild();
            InitStyle();
        }

        void InitStyle()
        {
            styleRed = new GUIStyle();
            styleRed.active.textColor = Color.red;
        }

        static string serverHash = "";
        Vector2 srollPos;

        void OnBuildGUI()
        {
            mAssetBundleWindowGUI.DrawAssetBundlePattern(mAssetBundleItemList);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("RemoveUnusedABName", GUILayout.Width(150)))
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();
               // InitBuild();
            }
            GUILayout.Label("Totle Size : " + FileLengthToStr(mTotalSize));
            EditorGUILayout.EndHorizontal();
            mAssetBundleWindowGUI.DrawBottomButtonsPattern();
            mAssetBundleWindowGUI.DrawHashCodeFile(serverHash);
        }

        void OnGetFileHashCode()
        {
            serverHash = FileManager.GetFileHash(fullServerCSVPath);
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

        void InitBuild()
        {
            allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            mAssetBundleItemList = new List<AssetBundleWindowItem>();
            for (int i = 0; i < allAssetBundleNames.Length; i++)
            {
                AssetBundleWindowItem ab = new AssetBundleWindowItem();
                ab.assetBundleConfigItem.name = allAssetBundleNames[i];
                mAssetBundleItemList.Add(ab);
            }
            GetHash();
            serverHash = FileManager.GetFileHash(fullServerCSVPath);
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

        void BuildAllAssetBundlesWithDependencies(){
            AssetDatabase.RemoveUnusedAssetBundleNames();
            BuildPipeline.BuildAssetBundles(fullTmpOutputPath,BuildAssetBundleOptions.None,EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }

        void BuildSelectAssetBundlesWithOutDependencies(){
            AssetDatabase.RemoveUnusedAssetBundleNames();
            List<AssetBundleBuild> assetbundleList = GetSelectedEntities();
            if (assetbundleList.Count > 0)
            {
                BuildPipeline.BuildAssetBundles(fullTmpOutputPath, assetbundleList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
            AssetDatabase.Refresh();
        }
        //TODO
        void RemoveABNotInABList(){
            HashSet<string> setStrs = new HashSet<string>();
            foreach (AssetBundleWindowItem entity in mAssetBundleItemList)
            {
                //if(entity.isSelected)
                setStrs.Add(entity.assetBundleConfigItem.name);
            }
            string[] paths = Directory.GetFiles(fullTmpOutputPath);
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
                mAssetBundleItemList[i].assetBundleConfigItem.hash = FileManager.GetFileHash(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name);
                if (mAssetBundleItemList[i].assetBundleConfigItem.hash != null && mAssetBundleItemList[i].assetBundleConfigItem.hash.Trim() != "")
                {
                    mAssetBundleItemList[i].assetBundleConfigItem.length = new FileInfo(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name).Length;
                    mAssetBundleItemList[i].displayLength = FileLengthToStr(mAssetBundleItemList[i].assetBundleConfigItem.length);
                    mTotalSize += mAssetBundleItemList[i].assetBundleConfigItem.length;
                }
            }
        }

        List<AssetBundleBuild> GetSelectedEntities(){
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

        AssetBundleBuild CreateAssetBundleBuild(AssetBundleWindowItem entity){
            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = entity.assetBundleConfigItem.name.Split('.')[0];
            abb.assetBundleVariant = entity.assetBundleConfigItem.name.Split('.')[1];
            abb.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(entity.assetBundleConfigItem.name);
            return abb;
        }

        void CopySelectToTemp()
        {
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                if (mAssetBundleItemList[i].isSelected)
                {
                    if (FileManager.Exists(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name))
                        FileManager.CopyFile(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name, tempResourceAssetPath + mAssetBundleItemList[i].assetBundleConfigItem.name);
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
                    if (FileManager.Exists(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name))
                        FileManager.CopyFile(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name, localABServerPath + fullTmpOutputPath.Replace(Application.dataPath, "") + mAssetBundleItemList[i].assetBundleConfigItem.name);
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
                    FileManager.CopyFile(fullTmpOutputPath + mAssetBundleItemList[i].assetBundleConfigItem.name, fullStreamPath + mAssetBundleItemList[i].assetBundleConfigItem.name);
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
                if (abEntity.assetBundleConfigItem.hash != null && abEntity.assetBundleConfigItem.hash.Trim() != "")
                {
                    result.AppendLine(CreateLine(abEntity.assetBundleConfigItem.name, new FileInfo(fullTmpOutputPath + abEntity.assetBundleConfigItem.name).Length, abEntity.assetBundleConfigItem.name == "csv.assetbundle" ? 1 : 0, abEntity.assetBundleConfigItem.hash).ToString());
                }
                //			}
            }
            //		result.Append (GetMovieHash ());
            FileManager.WriteString(fullServerCSVPath, result.ToString());
            serverHash = FileManager.GetFileHash(fullServerCSVPath);
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

