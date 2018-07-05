using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UIConfigWindow : EditorWindow {

	const string UIEDITORSETTINGS_PATH = "Assets/Editor/Setting/UIEditorSettings.asset";
	const string UISETTINGS_PATH = "Assets/Resources/Settings/UISettings.asset";

	protected UIEditorSettings mUIEditorSettings;
	protected UISettings mUISettings;
	protected TextAsset mConfigText;


	protected void LoadSettings ()
	{
		mUIEditorSettings = AssetDatabase.LoadAssetAtPath<UIEditorSettings> (UIEDITORSETTINGS_PATH);
		mUISettings = AssetDatabase.LoadAssetAtPath<UISettings> (UISETTINGS_PATH);
	}

	protected void InitContent(string title,string tooltip){
		GUIContent guiContent = new GUIContent ();
		guiContent.text = title;
		guiContent.image = mUIEditorSettings.WINDOW_ICON_PATH;
		guiContent.tooltip = tooltip;
		this.titleContent = guiContent;
		position = new Rect (new Vector2 (0, 0), new Vector2 (400, 300));
	}

	protected void DisplaySettings(){
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Config File : ", GUILayout.Width (160));
		EditorGUI.BeginDisabledGroup (true);
		EditorGUILayout.ObjectField (mConfigText, typeof(TextAsset), false, GUILayout.MinWidth (130), GUILayout.MaxWidth (200));
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("UI Editor ScriptableObject : ", GUILayout.Width (160));
		EditorGUI.BeginDisabledGroup (true);
		EditorGUILayout.ObjectField (mUIEditorSettings, typeof(ScriptableObject), false, GUILayout.MinWidth (130), GUILayout.MaxWidth (200));
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("UI ScriptableObject : ", GUILayout.Width (160));
		EditorGUI.BeginDisabledGroup (true);
		EditorGUILayout.ObjectField (mUISettings, typeof(ScriptableObject), false, GUILayout.MinWidth (130), GUILayout.MaxWidth (200));
		EditorGUI.EndDisabledGroup ();
		EditorGUILayout.EndHorizontal ();
	}
}
