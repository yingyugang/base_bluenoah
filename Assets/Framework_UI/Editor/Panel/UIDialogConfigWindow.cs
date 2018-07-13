using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BlueNoah.UI;
using BlueNoah.Editor.IO;
using BlueNoah.IO;
using System.IO;

namespace BlueNoah.Editor.UI
{
	public class UIDialogConfigWindow : UIConfigWindow
	{
		static UIDialogConfigWindow mUIDialogConfigWindow;
		List<UIDialogConfigWindowItem> mDialogConfigWindowItems;
        UIDialogConfigWindowGUI mUIDialogConfigWindowGUI;

        [MenuItem (UIEditorConstant.UI_DIALOG_CONFIG_WINDOW_MENUITEM + MenuItemShortcutKeyConstant.SHORTCUT_KEY_UI_DIALOG_CONFIG)]
		static void OnOpen ()
		{
			mUIDialogConfigWindow = GetWindow<UIDialogConfigWindow> ();
			mUIDialogConfigWindow.Show ();
			mUIDialogConfigWindow.Focus ();
		}

        protected override void OnEnable ()
		{
            base.OnEnable();
			InitContent ("DialogConfig","config the dialogs");
		}

        protected override void LoadConfig ()
		{
            mUIDialogConfigWindowGUI = new UIDialogConfigWindowGUI();
			mDialogConfigWindowItems = new List<UIDialogConfigWindowItem> ();
            mConfigText = mUISettings.DIALOG_CONFIG_FILE;
            InitConfigItems(JsonUtility.FromJson<DialogConfig>(mUISettings.DIALOG_CONFIG_FILE.text).items);
		}

        void InitConfigItems(List<DialogConfigItem> items){
            for (int i = 0; i < items.Count; i++)
            {
                mDialogConfigWindowItems.Add(CreateConfigItem(items[i]));
            }
        }

        UIDialogConfigWindowItem CreateConfigItem(DialogConfigItem configItem){
            UIDialogConfigWindowItem item = new UIDialogConfigWindowItem();
            item.id = configItem.index;
            item.classScript = EditorFileManager.FindMono(configItem.className);
            item.dialogPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(configItem.prefabPath);
            return item;
        }

        void OnGUI ()
		{
			if (mDialogConfigWindowItems == null)
				LoadConfig ();

            mUIDialogConfigWindowGUI.DrawTemplatePattern(mUIEditorSettings.PREFAB_DIALOG_TEMPLATES, mCurrentTemplate, SelectTemplate);

            mUIDialogConfigWindowGUI.DrawConfigItemsPattern<UIDialogConfigWindowItem>(mDialogConfigWindowItems, Remove, ref mName, Create);

            mUIDialogConfigWindowGUI.DrawConfigPattern(mConfigText, mUIEditorSettings, mUISettings);

		}

        void SelectTemplate(GameObject template){
            this.mCurrentTemplate = template;
        }

        void SaveNewDialogConfig(string dialogName, string prefabPath)
        {
            DialogConfig dialogConfig = ConfigWindowItemsToConfig();
            DialogConfigItem dialogConfigItem = CreateNewDialogConfig(dialogName, prefabPath);
            dialogConfig.items.Add(dialogConfigItem);
            SaveConfig(JsonUtility.ToJson(dialogConfig,true),mUISettings.DIALOG_CONFIG_FILE);
            AssetDatabase.Refresh();
        }

        DialogConfig ConfigWindowItemsToConfig()
        {
            DialogConfig dialogConfig = new DialogConfig();
            dialogConfig.items = CreateDialogConfigItems(mDialogConfigWindowItems);
            return dialogConfig;
        }

        List<DialogConfigItem> CreateDialogConfigItems(List<UIDialogConfigWindowItem> dialogConfigWindowItems)
        {
            List<DialogConfigItem> dialogConfigItems = new List<DialogConfigItem>();
            for (int i = 0; i < dialogConfigWindowItems.Count; i++)
            {
                DialogConfigItem configItem = CreateDialogConfigItem(dialogConfigWindowItems[i]);
                configItem.index = i;
                dialogConfigItems.Add(configItem);
            }
            return dialogConfigItems;
        }

        DialogConfigItem CreateDialogConfigItem(UIDialogConfigWindowItem item)
        {
            DialogConfigItem configItem = new DialogConfigItem();
            if (item.classScript != null)
                configItem.className = item.classScript.GetClass().ToString();
            if(item.dialogPrefab!=null)
                configItem.prefabPath = AssetDatabase.GetAssetPath(item.dialogPrefab);
            return configItem;
        }

        DialogConfigItem CreateNewDialogConfig(string dialogName, string prefabPath)
        {
            DialogConfigItem dialogConfigItem = new DialogConfigItem();
            dialogConfigItem.className = UIEditorConstant.GetFullClassName(mUIEditorSettings.CLASS_NAMESPACE, dialogName, mUIEditorSettings.DIALOG_CLASS_SUFFIX);
            dialogConfigItem.prefabPath = prefabPath;
            return dialogConfigItem;
        }

        void CreateDialogClass(string dialogName)
        {
            string dialogClassName = GetDialogClassName(dialogName);
            string dialogClassPath = GetClassFullPath(dialogName, AssetDatabase.GetAssetPath(mUIEditorSettings.DIALOG_CLASS_PATH));
            CreateDialogClass(dialogClassName, dialogClassPath);
        }

        void CreateDialogClass(string dialogClassName, string dialogClassPath)
        {
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_VIEW.text;
            string resultText = FormatDialogClass(templateText, dialogClassName);
            SaveDialogClass(dialogClassName, dialogClassPath, resultText);
        }

        string FormatDialogClass(string templateText, string dialogClassName)
        {
            string resultText = templateText.Replace("{0}", mUIEditorSettings.CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", dialogClassName.Trim());
            return resultText.Replace("{2}", StringUtility.NameSpaceToPathFormat(mUIEditorSettings.CLASS_NAMESPACE));
        }

        void SaveDialogClass(string dialogClassName, string dialogClassPath, string context)
        {
            string filePath = Path.Combine(dialogClassPath, dialogClassName + ".cs");
            FileManager.WriteString(filePath, context);
        }

        string GetDialogClassName(string dialogName)
        {
            return UIEditorConstant.GetClassName(dialogName, mUIEditorSettings.DIALOG_CLASS_SUFFIX);
        }

        protected override string GetPrefabPath(string componentName)
        {
            return AssetDatabase.GetAssetPath(mUIEditorSettings.DIALOG_PREFAB_FOLDER_PATH) + "/" + componentName + ".prefab";
        }

        protected override void OnCreate(string componentName)
        {
            CreateDialogClass(componentName);
            string prefabPath = CreatePrefab(componentName, mCurrentTemplate);
            SaveNewDialogConfig(componentName, prefabPath);
        }

        protected override void OnRemoveConfirm(UIConfigWindowItem item)
        {
            UIDialogConfigWindowItem configWindowItem = (UIDialogConfigWindowItem)item;
            RemoveConfigItem(configWindowItem);
            string scriptPath = GetScriptPath(configWindowItem.classScript);
            RemoveFiles(configWindowItem);
            RemoveEmptyFolder(scriptPath);
            AssetDatabase.Refresh();
        }

        void RemoveFiles(UIDialogConfigWindowItem item)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.classScript));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.dialogPrefab));
        }

        void RemoveConfigItem(UIDialogConfigWindowItem item)
        {
            mDialogConfigWindowItems.Remove(item);
            SaveConfig(JsonUtility.ToJson(ConfigWindowItemsToConfig(), true), mUISettings.DIALOG_CONFIG_FILE);
        }
	}

    public class UIDialogConfigWindowItem: UIConfigWindowItem
    {
        public MonoScript classScript;
        public GameObject dialogPrefab;
    }
}
