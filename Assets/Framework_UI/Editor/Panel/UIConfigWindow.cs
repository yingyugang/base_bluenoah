using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BlueNoah.IO;
using BlueNoah.Editor.IO;

namespace BlueNoah.Editor
{
    public class UIConfigWindow : EditorWindow
    {

        const string UIEDITORSETTINGS_PATH = "Assets/Editor/Setting/UIEditorSettings.asset";
        const string UISETTINGS_PATH = "Assets/Resources/Settings/UISettings.asset";

        protected UIEditorSettings mUIEditorSettings;
        protected UISettings mUISettings;
        protected TextAsset mConfigText;

        protected void LoadSettings()
        {
            mUIEditorSettings = AssetDatabase.LoadAssetAtPath<UIEditorSettings>(UIEDITORSETTINGS_PATH);
            if (mUIEditorSettings == null)
            {
                Debug.Log(string.Format("The UIEditorSettings is not at {0}", UIEDITORSETTINGS_PATH));
                mUIEditorSettings = SearchSettingFile<UIEditorSettings>(UIEDITORSETTINGS_PATH);
            }
            mUISettings = AssetDatabase.LoadAssetAtPath<UISettings>(UISETTINGS_PATH);
            if (mUISettings == null)
            {
                Debug.Log(string.Format("The UISETTINGS_PATH is not at {0}", UISETTINGS_PATH));
                mUISettings = SearchSettingFile<UISettings>(UISETTINGS_PATH);
            }
        }

        private T SearchSettingFile<T>(string path) where T : ScriptableObject
        {
            string fileName = FileManager.GetFileNameFromPath(path);
            string fileMain = FileManager.GetFileMain(fileName);
            string filePattern = FileManager.GetFilePattern(fileName);
            string assetFilePath = EditorFileManager.FindAsset(fileMain,filePattern);
            T t = AssetDatabase.LoadAssetAtPath<T>(assetFilePath);
            if(t!=null){
                Debug.Log(string.Format("{0} is searched.",assetFilePath));
            }else{
                Debug.Log(string.Format("{0} is not existing.", typeof(T).ToString()));
            }
            return t;
        }

        protected void InitContent(string title, string tooltip)
        {
            GUIContent guiContent = new GUIContent();
            guiContent.text = title;
            guiContent.image = mUIEditorSettings.WINDOW_ICON_PATH;
            guiContent.tooltip = tooltip;
            this.titleContent = guiContent;
            //this.position = new Rect(new Vector2(0, 0), new Vector2(400, 300));
        }

        protected void DisplaySettings()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Config File : ", GUILayout.Width(160));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(mConfigText, typeof(TextAsset), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI Editor ScriptableObject : ", GUILayout.Width(160));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(mUIEditorSettings, typeof(ScriptableObject), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI ScriptableObject : ", GUILayout.Width(160));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(mUISettings, typeof(ScriptableObject), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }
    }
}
