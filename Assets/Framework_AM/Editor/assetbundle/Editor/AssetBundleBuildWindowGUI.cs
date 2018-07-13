using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Editor.AssetBundle.Management
{
    public class AssetBundleBuildWindowGUI : AssetBundleWindowGUI
    {
        AssetBundleWindowEvents mAssetBundleWindowEvents;

        Vector2 mScrollPos;

        public AssetBundleBuildWindowGUI(AssetBundleWindowEvents assetBundleWindowEvents)
        {
            mAssetBundleWindowEvents = assetBundleWindowEvents;
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

        void DrawAssetbundleItemData(AssetBundleWindowItem abBuildEntity)
        {
            GUILayout.Label(abBuildEntity.assetBundleConfigItem.name, GUILayout.Width(180));
            GUILayout.TextField(abBuildEntity.assetBundleConfigItem.hash, GUILayout.Width(300));
            GUI.color = CheckColor(abBuildEntity.assetBundleConfigItem.length);
            GUILayout.Label(abBuildEntity.displayLength, GUILayout.Width(90));
            GUI.color = Color.white;
        }

        void DrawSelectAssetButton(AssetBundleWindowItem abBuildEntity)
        {
            if (GUILayout.Button("SelectDP", GUILayout.Width(100)))
            {
                mAssetBundleWindowEvents.onSelectAssets(abBuildEntity.assetBundleConfigItem.name);
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
            if (GUILayout.Button("GetServerCSVHash", GUILayout.Width(150)))
            {
                this.mAssetBundleWindowEvents.onGetFileHash();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void DrawBottomButtonsPattern()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("BuildAll", GUILayout.Width(100)))
            {
                mAssetBundleWindowEvents.onBuildAll();
            }
            if (GUILayout.Button("BuildSelect", GUILayout.Width(100)))
            {
                mAssetBundleWindowEvents.onBuildAll();
            }
            if (GUILayout.Button("CopyToTemp", GUILayout.Width(120)))
            {
                mAssetBundleWindowEvents.onCopyToTemp();
            }
            if (GUILayout.Button("CopyToLocalServer", GUILayout.Width(120)))
            {
                mAssetBundleWindowEvents.onCopyToLocalServer();
            }
            if (GUILayout.Button("CopyToRemoveServer", GUILayout.Width(120)))
            {
                mAssetBundleWindowEvents.onCopyToRemoveServer();
            }
            if (GUILayout.Button("SelectAll", GUILayout.Width(120)))
            {
                mAssetBundleWindowEvents.onSelectAll();
            }
            if (GUILayout.Button("UnSelectAll", GUILayout.Width(120)))
            {
                mAssetBundleWindowEvents.onUnSelectAll();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    public class AssetBundleWindowEvents
    {
        public UnityAction onBuildAll;

        public UnityAction onBuildSelected;

        public UnityAction onCopyToTemp;

        public UnityAction onCopyToLocalServer;

        public UnityAction onCopyToRemoveServer;

        public UnityAction onSelectAll;

        public UnityAction onUnSelectAll;

        public UnityAction onGetFileHash;

        public UnityAction<string> onSelectAssets;
    }


}