using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleBuildWindowGUI
    {
        
        AssetBundleBuildWindow mAssetBundleBuildWindow;

        Vector2 mScrollPos;

        public AssetBundleBuildWindowGUI(AssetBundleBuildWindow assetBundleBuildWindow)
        {
            mAssetBundleBuildWindow = assetBundleBuildWindow;
        }

        public void DrawAssetBundlePattern(List<AssetBundleWindowItem> allAssetBundleEntitys)
        {
            mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, GUILayout.Height(400));
            DrawAssetBundleList(allAssetBundleEntitys);
            EditorGUILayout.EndScrollView();
        }

        void DrawAssetBundleList(List<AssetBundleWindowItem> allAssetBundleEntitys)
        {
            for (int i = 0; i < allAssetBundleEntitys.Count; i++)
            {
                DrawAssetbundleItem(allAssetBundleEntitys[i]);
            }
        }

        void DrawAssetbundleItem(AssetBundleWindowItem abBuildEntity)
        {
            EditorGUILayout.BeginHorizontal();
            DrawAssetbundleItemSelect(abBuildEntity);
            DrawAssetbundleItemData(abBuildEntity);
            DrawSelectAssetButton(abBuildEntity);
            EditorGUILayout.EndHorizontal();
        }

        void DrawAssetbundleItemSelect(AssetBundleWindowItem abBuildEntity)
        {
            GUI.color = abBuildEntity.isSelected ? Color.green : Color.white;
            abBuildEntity.isSelected = GUILayout.Toggle(abBuildEntity.isSelected, "", GUILayout.Width(30));
        }

        void DrawAssetbundleItemData(AssetBundleWindowItem assetBundleWindowItem)
        {
            GUILayout.Label(assetBundleWindowItem.assetBundleName);
            EditorGUILayout.ObjectField(assetBundleWindowItem.assetBundle,typeof(Object), false, GUILayout.Width(200));
            GUILayout.TextField(assetBundleWindowItem.assetBundleHash, GUILayout.Width(300));
            GUI.color = CheckColor(assetBundleWindowItem.assetBundleLength);
            GUILayout.Label(assetBundleWindowItem.displayLength, GUILayout.Width(90));
            GUI.color = Color.white;
        }

        void DrawSelectAssetButton(AssetBundleWindowItem abBuildEntity)
        {
            if (GUILayout.Button("SelectDP", GUILayout.Width(60)))
            {
                mAssetBundleBuildWindow.SelectDependencies(abBuildEntity.assetBundleName);
            }
        }

        Color CheckColor(long fileLength)
        {
            return fileLength > 1024 * 1024 ? Color.red : CheckMiddleColor(fileLength);
        }

        Color CheckMiddleColor(long fileLength)
        {
            return fileLength > 1024 ? Color.yellow : Color.white;
        }

        public void DrawHashCodeFile(string serverHash)
        {
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.TextField(serverHash, GUILayout.Width(400));
            if (GUILayout.Button("ConfigHash", GUILayout.Width(150)))
            {
                mAssetBundleBuildWindow.ReadConfigHash();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void DrawBottomButtonsPattern()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("BuildAll", GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("BuildAll", "It will build all the assetbundles with dependencies.Go on ?", "OK","Cancel"))
                {
                    mAssetBundleBuildWindow.BuildAllAssetBundlesWithDependencies();
                }
            }
            if (GUILayout.Button("BuildSelect", GUILayout.Width(100)))
            {
                if(EditorUtility.DisplayDialog("BuildSelect", "It will build single assetbundle with out dependencies.Go on ?", "OK", "Cancel")){
                    mAssetBundleBuildWindow.BuildSelectAssetBundlesWithOutDependencies();
                }
            }
            if (GUILayout.Button("CopyToServer", GUILayout.Width(120)))
            {
                mAssetBundleBuildWindow.CopySelectAssetBundleToServer();
            }
            if (GUILayout.Button("SelectAll", GUILayout.Width(120)))
            {
                mAssetBundleBuildWindow.SelectAll();
            }
            if (GUILayout.Button("UnSelectAll", GUILayout.Width(120)))
            {
                mAssetBundleBuildWindow.UnSelectAll();
            }
            if (GUILayout.Button("RemoveUnusedABName", GUILayout.Width(150)))
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();
            }
            if (GUILayout.Button("SetAssetBundleNames", GUILayout.Width(150)))
            {
                mAssetBundleBuildWindow.SetAssetBundleNames();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}