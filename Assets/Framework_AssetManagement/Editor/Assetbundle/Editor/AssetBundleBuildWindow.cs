﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using BlueNoah.IO;
using BlueNoah.Download;

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
            mAssetBundleBuildWindow = GetWindow<AssetBundleBuildWindow>(true, "AB Build Manager", true);
            mAssetBundleBuildWindow.minSize = new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT);
            mAssetBundleBuildWindow.Show();
            mAssetBundleBuildWindow.Focus();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InitContent("AssetBundleBuild", "Build and manage assetbundles.");
            AssetBundleWindowEvents assetBundleWindowEvents = InitAssetBundleWindowEvents();
            mAssetBundleWindowGUI = new AssetBundleBuildWindowGUI(assetBundleWindowEvents);
        }

        void OnGUI()
        {
            mAssetBundleWindowGUI.DrawAssetBundlePattern(mAssetBundleItemList);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("RemoveUnusedABName", GUILayout.Width(150)))
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
            GUILayout.Label("Totle Size : " + FileLengthToStr(mTotalAssetBundleSize));
            EditorGUILayout.EndHorizontal();
            mAssetBundleWindowGUI.DrawBottomButtonsPattern();
            mAssetBundleWindowGUI.DrawHashCodeFile(serverHash);
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

        void SaveConfig()
        {
            Debug.Log("SaveConfig");
            LoadAssetBundleInfos();
            AssetConfig assetBundleConfig = ConvertAssetBundleWindowItemsToAssetBundleConfig();
            string assetBundleConfigStr = JsonUtility.ToJson(assetBundleConfig,true);
            FileManager.WriteString(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_CONFIG_FILE,assetBundleConfigStr);
        }

        AssetConfig ConvertAssetBundleWindowItemsToAssetBundleConfig(){
            AssetConfig assetBundleConfig = new AssetConfig();
            assetBundleConfig.items = new List<AssetConfigItem>();
            for (int i = 0; i < mAssetBundleItemList.Count; i++)
            {
                assetBundleConfig.items.Add(ConvertAssetBundleWindowItemToAssetBundleConfigItem(mAssetBundleItemList[i]));
            }
            return assetBundleConfig;
        }

        AssetConfigItem ConvertAssetBundleWindowItemToAssetBundleConfigItem(AssetBundleWindowItem assetBundleWindowItem){
            AssetConfigItem assetBundleConfigItem = new AssetConfigItem();
            assetBundleConfigItem.assetName = assetBundleWindowItem.assetBundleName;
            assetBundleConfigItem.hashCode = assetBundleWindowItem.assetBundleHash;
            assetBundleConfigItem.size = assetBundleWindowItem.assetBundleLength;
            return assetBundleConfigItem;
        }

        static string serverHash = "";
        Vector2 srollPos;

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
            FileManager.CreateDirectoryIfNotExisting(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH);
            BuildPipeline.BuildAssetBundles(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }

        void BuildSelectAssetBundlesWithOutDependencies()
        {
            Debug.Log("BuildSelectAssetBundlesWithOutDependencies");
            AssetDatabase.RemoveUnusedAssetBundleNames();
            List<AssetBundleBuild> assetbundleList = GetSelectedEntities();
            if (assetbundleList.Count > 0)
            {
                BuildPipeline.BuildAssetBundles(AssetBundleEditorConstant.ASSETBUNDLE_PLATFORM_PATH, assetbundleList.ToArray(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
            AssetDatabase.Refresh();
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
    }
}

