using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using BlueNoah.UI;
using BlueNoah.IO;
using BlueNoah.Editor.IO;

namespace BlueNoah.Editor
{
    public class UIPanelConfigWindow : UIConfigWindow
    {
        static UIPanelConfigWindow mPanelConfigWindow;
        UIPanelConfigWindowGUI mUIPanelConfigWindowGUI;
        PanelConfig mPanelConfig;
        List<UIPanelConfigWindowItem> mPanelConfigWindowItems;

        [MenuItem(UIEditorConstant.UI_PANEL_CONFIG_WINDOW_MENUITEM)]
        static void OnOpen()
        {
            mPanelConfigWindow = GetWindow<UIPanelConfigWindow>();
            mPanelConfigWindow.Show();
            mPanelConfigWindow.Focus();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            InitContent("PanleConfig", "config the panels");
        }

        void OnGUI()
        {
            if (mPanelConfigWindowItems == null)
                LoadConfig();

            mUIPanelConfigWindowGUI.DrawTemplatePattern(mUIEditorSettings.PREFAB_TEMPLATES, mCurrentTemplate, SelectTemplate);

            mUIPanelConfigWindowGUI.DrawConfigItemsPattern<UIPanelConfigWindowItem>(mPanelConfigWindowItems, Remove, ref mName, Create);

            mUIPanelConfigWindowGUI.DrawConfigPattern(mConfigText, mUIEditorSettings, mUISettings);
        }

        protected override void LoadConfig()
        {
            mUIPanelConfigWindowGUI = new UIPanelConfigWindowGUI();
            mPanelConfigWindowItems = new List<UIPanelConfigWindowItem>();
            mConfigText = mUISettings.PANEL_CONFIG_FILE;
            mPanelConfig = LoadConfigFile(mUISettings.PANEL_CONFIG_FILE);
            InitConfigItems(mPanelConfig.items);
        }

        PanelConfig LoadConfigFile(TextAsset config)
        {
            string path = GetConfigFilePath(config);
            string configTxt = FileManager.ReadString(path);
            PanelConfig panelConfig = JsonUtility.FromJson<PanelConfig>(configTxt);
            return panelConfig;
        }

        string GetConfigFilePath(TextAsset config)
        {
            string path = AssetDatabase.GetAssetPath(config);
            return EditorFileManager.AssetDatabasePathToFilePath(path);
        }

        void InitConfigItems(List<PanelConfigItem> configItems){
            for (int i = 0; i < configItems.Count; i++)
            {
                mPanelConfigWindowItems.Add(CreateConfigItem(configItems[i]));
            }
        }

        UIPanelConfigWindowItem CreateConfigItem(PanelConfigItem configItem)
        {
            UIPanelConfigWindowItem item = new UIPanelConfigWindowItem();
            item.id = configItem.index;
            item.ctrlScript = EditorFileManager.FindMono(configItem.ctrlClassName);
            item.viewScript = EditorFileManager.FindMono(configItem.viewClassName);
            item.panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(configItem.prefabPath);
            return item;
        }

        string GetScriptPath(string panelName)
        {
            string path = UIEditorConstant.GetClassPath(panelName, AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_CLASS_PATH));
            CreateScriptPath(path);
            return path;
        }

        string GetViewClassName(string panelName){
            return UIEditorConstant.GetClassName(panelName, mUIEditorSettings.VIEW_CLASS_SUFFIX);
        }

        string GetCtrlClassName(string panelName){
            return UIEditorConstant.GetClassName(panelName, mUIEditorSettings.CTRL_CLASS_SUFFIX);
        }

        void SelectTemplate(GameObject template)
        {
            this.mCurrentTemplate = template;
        }

        PanelConfig ConfigWindowItemsToConfig()
        {
            PanelConfig panelConfig = new PanelConfig();
            panelConfig.items = CreatePanelConfigItems(mPanelConfigWindowItems);
            return panelConfig;
        }

        List<PanelConfigItem> CreatePanelConfigItems(List<UIPanelConfigWindowItem> panelConfigWindowItems){
            List<PanelConfigItem> panelConfigItems = new List<PanelConfigItem>();
            for (int i = 0; i < panelConfigWindowItems.Count; i++)
            {
                panelConfigItems.Add(CreatePanelConfigItem(panelConfigWindowItems[i]));
            }
            return panelConfigItems;
        }

        PanelConfigItem CreatePanelConfigItem(UIPanelConfigWindowItem item){
            PanelConfigItem configItem = new PanelConfigItem();
            configItem.index = item.id;
            if (item.ctrlScript != null)
                configItem.ctrlClassName = item.ctrlScript.GetClass().ToString();
            if (item.ctrlScript != null)
                configItem.viewClassName = item.viewScript.GetClass().ToString();
            if (item.panelPrefab != null)
                configItem.prefabPath = AssetDatabase.GetAssetPath(item.panelPrefab);
            return configItem;
        }

        void CreateViewClass(string panelName)
        {
            string viewClassName = GetViewClassName(panelName);
            string viewClassPath = GetClassFullPath(panelName,AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_CLASS_PATH));
            CreateViewClass(viewClassName,viewClassPath);
        }

        void CreateViewClass(string viewClassName,string viewClasstPath){
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_VIEW.text;
            string resultText = FormatViewClass(templateText,viewClassName);
            SaveViewClass(viewClassName,viewClasstPath,resultText);
        }

        string FormatViewClass(string templateText,string viewClassName){
            string resultText = templateText.Replace("{0}", mUIEditorSettings.CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", viewClassName.Trim());
            return resultText.Replace("{2}", StringUtility.NameSpaceToPathFormat(mUIEditorSettings.CLASS_NAMESPACE));
        }

        void SaveViewClass(string viewClassName, string viewClasstPath,string context){
            string filePath = Path.Combine(viewClasstPath, viewClassName + ".cs");
            FileManager.WriteString(filePath, context);
        }

        void CreateCtrlClass(string panelName)
        {
            string ctrlClassName = GetCtrlClassName(panelName);
            string viewClassName = GetViewClassName(panelName);
            string ctrlClassPath = GetClassFullPath(panelName, AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_CLASS_PATH));
            CreateCtrlClass(viewClassName,ctrlClassName,ctrlClassPath);
        }

        void CreateCtrlClass(string viewClassName,string ctrlClassName,string ctrlClassPath){
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_CTRL.text;
            string resultText = FormatCtrlClass(viewClassName,ctrlClassName,templateText);
            SaveCtrlClass(ctrlClassName,ctrlClassPath,resultText);
        }

        string FormatCtrlClass(string viewClassName, string ctrlClassName,string templateText){
            string resultText = templateText.Replace("{0}", mUIEditorSettings.CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", ctrlClassName.Trim());
            resultText = resultText.Replace("{2}", viewClassName.Trim());
            return resultText.Replace("{3}", StringUtility.NameSpaceToPathFormat(mUIEditorSettings.CLASS_NAMESPACE));
        }

        void SaveCtrlClass(string ctrlClassName,string ctrlClassPath,string resultText){
            string filePath = Path.Combine(ctrlClassPath, ctrlClassName + ".cs");
            FileManager.WriteString(filePath, resultText);
        }

        protected override string GetPrefabPath(string componentName){
            return AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_PREFAB_FOLDER) + "/" + componentName + ".prefab";
        }

        void SaveNewPanelConfig(string panelName, string prefabPath)
        {
            PanelConfig panelConfig = ConfigWindowItemsToConfig();
            PanelConfigItem panelConfigItem = CreateNewPanelConfig(panelName,prefabPath);
            panelConfig.items.Add(panelConfigItem);
            SaveConfig(JsonUtility.ToJson(panelConfig,true),mUISettings.PANEL_CONFIG_FILE);
            AssetDatabase.Refresh();
        }

        PanelConfigItem CreateNewPanelConfig(string panelName, string prefabPath){
            PanelConfigItem panelConfigItem = new PanelConfigItem();
            panelConfigItem.ctrlClassName = UIEditorConstant.GetFullClassName(mUIEditorSettings.CLASS_NAMESPACE, panelName, mUIEditorSettings.CTRL_CLASS_SUFFIX);
            panelConfigItem.viewClassName = UIEditorConstant.GetFullClassName(mUIEditorSettings.CLASS_NAMESPACE, panelName, mUIEditorSettings.VIEW_CLASS_SUFFIX);
            panelConfigItem.prefabPath = prefabPath;
            return panelConfigItem;
        }

        void RemovePanelConfigItem(UIPanelConfigWindowItem item){
            mPanelConfigWindowItems.Remove(item);
            SaveConfig(JsonUtility.ToJson(ConfigWindowItemsToConfig(), true), mUISettings.PANEL_CONFIG_FILE);
        }

        void RemovePanelFiles(UIPanelConfigWindowItem item){
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.panelPrefab));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.ctrlScript));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.viewScript));
        }

        void FindConfigFile()
        {
            Selection.activeObject = mUISettings.PANEL_CONFIG_FILE;
        }

        protected override void OnCreate(string componentName)
        {
            string prefabPath = CreatePrefab(componentName, mCurrentTemplate);
            CreateViewClass(componentName);
            CreateCtrlClass(componentName);
            SaveNewPanelConfig(componentName, prefabPath);
        }

        protected override void OnRemoveConfirm(UIConfigWindowItem item)
        {
            UIPanelConfigWindowItem panelConfigWindowItem = (UIPanelConfigWindowItem)item;
            RemovePanelConfigItem(panelConfigWindowItem);
            string scriptPath = GetScriptPath(panelConfigWindowItem.ctrlScript);
            RemovePanelFiles(panelConfigWindowItem);
            RemoveEmptyFolder(scriptPath);
            AssetDatabase.Refresh();
        }
    }

    public class UIPanelConfigWindowItem:UIConfigWindowItem
    {
        public MonoScript viewScript;
        public MonoScript ctrlScript;
        public GameObject panelPrefab;
    }
}