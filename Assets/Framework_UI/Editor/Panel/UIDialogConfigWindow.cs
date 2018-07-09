using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BlueNoah.UI;
using BlueNoah.Editor.IO;

namespace BlueNoah.Editor
{
	public class UIDialogConfigWindow : UIConfigWindow
	{
		static UIDialogConfigWindow mUIDialogConfigWindow;

		List<DialogConfigWindowItem> mDialogConfigWindowItems;

        bool mIsDisable = true;

		string mDialogName;

		[MenuItem (UIEditorConstant.UI_DIALOG_CONFIG_WINDOW_MENUITEM)]
		static void OnOpen ()
		{
			mUIDialogConfigWindow = GetWindow<UIDialogConfigWindow> ();
			mUIDialogConfigWindow.Show ();
			mUIDialogConfigWindow.Focus ();
		}

		void OnEnable ()
		{
			LoadSettings ();
			InitContent ("DialogConfig","config the dialogs");
			LoadConfig ();
		}

		private void LoadConfig ()
		{
			mDialogConfigWindowItems = new List<DialogConfigWindowItem> ();
			mConfigText = this.mUISettings.DIALOG_CONFIG_FILE;
			string configTxt = mConfigText.text;
			DialogConfig mDialogConfig = JsonUtility.FromJson<DialogConfig> (configTxt);
			for (int i = 0; i < mDialogConfig.items.Count; i++) {
				DialogConfigWindowItem item = new DialogConfigWindowItem ();
				item.index = i;
                item.classScript = EditorFileManager.FindMono(mDialogConfig.items[i].className);
				item.dialogPrefab = AssetDatabase.LoadAssetAtPath<GameObject> (mDialogConfig.items [i].prefabPath);
				mDialogConfigWindowItems.Add (item);
			}
		}

		void OnGUI ()
		{
			if (mDialogConfigWindowItems == null)
				LoadConfig ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("==============================");
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("please wait for the functions.");
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("==============================");
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Dialog list:");
			EditorGUILayout.EndHorizontal ();
			for (int i = 0; i < mDialogConfigWindowItems.Count; i++) {
				DialogConfigWindowItem item = mDialogConfigWindowItems [i];
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label (i.ToString ());
				EditorGUI.BeginDisabledGroup (mIsDisable);
                EditorGUILayout.ObjectField(item.classScript,typeof(MonoScript),false);
				EditorGUILayout.ObjectField (item.dialogPrefab, typeof(GameObject), false);
				EditorGUI.EndDisabledGroup ();
				EditorGUI.BeginDisabledGroup (true);
				if (GUILayout.Button ("Remove")) {
					RemoveDialog ();
					i--;
				}
				EditorGUI.EndDisabledGroup ();
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Dialog Name : ", GUILayout.Width (160));
			mDialogName = GUILayout.TextField (mDialogName, GUILayout.MinWidth (130), GUILayout.MaxWidth (200));
			EditorGUI.BeginDisabledGroup (true);
			if (GUILayout.Button ("Create", GUILayout.Width (100))) {
				CreateDialog (mDialogName);
			}
			EditorGUI.EndDisabledGroup ();
			EditorGUILayout.EndHorizontal ();
			DisplaySettings ();
		}

		void RemoveDialog(){
			//Remove Config?
			//Remove Object?

		}

		void CreateDialog(string dialogName){
		
		}

        //void CreateViewClass(string viewClassName, string panelScriptPath)
        //{
        //    string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_VIEW.text;
        //    string resultText = templateText.Replace("{0}", mUIEditorSettings.PANEL_CLASS_NAMESPACE);
        //    resultText = resultText.Replace("{1}", viewClassName.Trim());
        //    resultText = resultText.Replace("{2}", NameSpaceToPathFormat(mUIEditorSettings.PANEL_CLASS_NAMESPACE));
        //    string filePath = Path.Combine(panelScriptPath, viewClassName + ".cs");
        //    FileManager.WriteString(filePath, resultText);
        //}

		class DialogConfigWindowItem
		{
			public int index;
			public MonoScript classScript;
			public GameObject dialogPrefab;
		}

	}
}
