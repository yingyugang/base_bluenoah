using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using BlueNoah.UI;
using BlueNoah.IO;
using BlueNoah.Editor.IO;

namespace BlueNoah.Editor
{
    public class UIPanelConfigWindow : UIConfigWindow
    {
        static List<Type> m_views;
        static UIPanelConfigWindow mPanelConfigWindow;
        PanelConfig mPanelConfig;
        GameObject mCurrentTemplate;
        List<PanelConfigWindowItem> mPanelConfigWindowItems;
        bool mIsDisable = false;
        string mCreatePanelName = "";

        [MenuItem(UIEditorConstant.UI_PANEL_CONFIG_WINDOW_MENUITEM)]
        static void OnOpen()
        {
            mPanelConfigWindow = GetWindow<UIPanelConfigWindow>();
            mPanelConfigWindow.Show();
            mPanelConfigWindow.Focus();
        }

        void OnEnable()
        {
            LoadSettings();
            InitContent("PanleConfig", "config the panels");
            LoadConfig();
        }

        void OnGUI()
        {
            if (mPanelConfigWindowItems == null)
                LoadConfig();
            UIPanelConfigWindowGUI.DrawTemplatePattern(mUIEditorSettings.PREFAB_TEMPLATES, mCurrentTemplate, SelectTemplate);

            UIPanelConfigWindowGUI.DrawPanelPattern(mPanelConfigWindowItems, RemovePanel, ref mCreatePanelName, CreatePanel);

            UIPanelConfigWindowGUI.DrawConfigPattern(mConfigText, mUIEditorSettings, mUISettings);
        }

        void LoadConfig()
        {
            mPanelConfigWindowItems = new List<PanelConfigWindowItem>();
            mConfigText = mUISettings.PANEL_CONFIG_FILE;
            mPanelConfig = LoadConfigFile(mUISettings.PANEL_CONFIG_FILE);
            AddConfigItems(mPanelConfig.items);
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

        void AddConfigItems(List<PanelConfigItem> configItems){
            for (int i = 0; i < configItems.Count; i++)
            {
                AddConfigItem(configItems[i]);
            }
        }

        void AddConfigItem(PanelConfigItem configItem)
        {
            PanelConfigWindowItem item = CreateConfigItem(configItem);
            mPanelConfigWindowItems.Add(item);
        }

        PanelConfigWindowItem CreateConfigItem(PanelConfigItem configItem)
        {
            PanelConfigWindowItem item = new PanelConfigWindowItem();
            item.id = configItem.index;
            item.ctrlScript = EditorFileManager.FindMono(configItem.ctrlClassName);
            item.viewScript = EditorFileManager.FindMono(configItem.viewClassName);
            item.panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(configItem.prefabPath);
            return item;
        }

        void CreatePanel(string panelName)
        {
            if (CheckCreateable(panelName))
            {
                if (EditorUtility.DisplayDialog("CreatePanel", string.Format("Is create {0} ?", panelName), "OK", "Canel"))
                {
                    OnCreatePanelConfirm(panelName);
                }
            }
        }

        void OnCreatePanelConfirm(string panelName){
            string prefabPath = CreatePrefab(panelName);
            CreateViewClass(panelName);
            CreateCtrlClass(panelName);
            SaveNewPanelConfig(panelName, prefabPath);
        }

        void CreateScriptPath(string panelScriptPath){
            if (!FileManager.DirectoryExists(panelScriptPath))
            {
                FileManager.CreateDirectoryName(panelScriptPath);
            }
        }

        string GetScriptPath(string panelName)
        {
            string path = UIEditorConstant.GetPanelPath(panelName, AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_CLASS_PATH));
            CreateScriptPath(path);
            return path;
        }

        string GetViewClassName(string panelName){
            return UIEditorConstant.GetViewClassName(panelName, mUIEditorSettings.VIEW_CLASS_SUFFIX);
        }

        string GetCtrlClassName(string panelName){
            return UIEditorConstant.GetCtrlClassName(panelName, mUIEditorSettings.CTRL_CLASS_SUFFIX);
        }

        bool CheckCreateable(string panelName)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                EditorUtility.DisplayDialog("CreatePanel", "Create fail , panel name is empty.", "OK");
                return false;
            }
            else if (mCurrentTemplate == null)
            {
                EditorUtility.DisplayDialog("CreatePanel", "Create fail , template is empty.", "OK");
                return false;
            }
            return true;
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

        List<PanelConfigItem> CreatePanelConfigItems(List<PanelConfigWindowItem> panelConfigWindowItems){
            List<PanelConfigItem> panelConfigItems = new List<PanelConfigItem>();
            for (int i = 0; i < panelConfigWindowItems.Count; i++)
            {
                PanelConfigItem configItem = CreatePanelConfigItem(panelConfigWindowItems[i]);
                //TODO not need ?
                configItem.index = i;
                panelConfigItems.Add(configItem);
            }
            return panelConfigItems;
        }

        PanelConfigItem CreatePanelConfigItem(PanelConfigWindowItem item){
            string ctrlScript = "";
            if (item.ctrlScript != null)
                ctrlScript = item.ctrlScript.GetClass().ToString();
            string viewScript = "";
            if (item.ctrlScript != null)
                viewScript = item.viewScript.GetClass().ToString();
            string prefabPath = "";
            if (item.panelPrefab != null)
                prefabPath = AssetDatabase.GetAssetPath(item.panelPrefab);
            PanelConfigItem configItem = new PanelConfigItem();
            configItem.ctrlClassName = ctrlScript;
            configItem.viewClassName = viewScript;
            configItem.prefabPath = prefabPath;
            return configItem;
        }

        void CreateViewClass(string panelName)
        {
            string viewClassName = GetViewClassName(panelName);
            string panelScriptPath = GetScriptPath(panelName);
            CreateViewClass(viewClassName,panelScriptPath);
        }

        void CreateViewClass(string viewClassName,string panelScriptPath){
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_VIEW.text;
            string resultText = FormatViewClass(templateText,viewClassName);
            SaveViewClass(viewClassName,panelScriptPath,resultText);
        }

        string FormatViewClass(string templateText,string viewClassName){
            string resultText = templateText.Replace("{0}", mUIEditorSettings.PANEL_CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", viewClassName.Trim());
            return resultText.Replace("{2}", StringUtility.NameSpaceToPathFormat(mUIEditorSettings.PANEL_CLASS_NAMESPACE));
        }

        void SaveViewClass(string viewClassName, string panelScriptPath,string context){
            string filePath = Path.Combine(panelScriptPath, viewClassName + ".cs");
            FileManager.WriteString(filePath, context);
        }

        void CreateCtrlClass(string panelName)
        {
            string ctrlClassName = GetCtrlClassName(panelName);
            string viewClassName = GetViewClassName(panelName);
            string panelScriptPath = GetScriptPath(panelName);
            CreateCtrlClass(viewClassName,ctrlClassName,panelScriptPath);
        }

        void CreateCtrlClass(string viewClassName,string ctrlClassName,string panelScriptPath){
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_CTRL.text;
            string resultText = FormatCtrlClass(viewClassName,ctrlClassName,templateText);
            SaveCtrlClass(ctrlClassName,panelScriptPath,resultText);
        }

        string FormatCtrlClass(string viewClassName, string ctrlClassName,string templateText){
            string resultText = templateText.Replace("{0}", mUIEditorSettings.PANEL_CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", ctrlClassName.Trim());
            resultText = resultText.Replace("{2}", viewClassName.Trim());
            return resultText.Replace("{3}", StringUtility.NameSpaceToPathFormat(mUIEditorSettings.PANEL_CLASS_NAMESPACE));
        }

        void SaveCtrlClass(string ctrlClassName,string panelScriptPath,string resultText){
            string filePath = Path.Combine(panelScriptPath, ctrlClassName + ".cs");
            FileManager.WriteString(filePath, resultText);
        }

        string CreatePrefab(string panelName)
        {
            string prefabPath = GetPrefabPath(panelName);
            CreatePrefab(prefabPath, mCurrentTemplate);
            PlaceIntoScene(prefabPath);
            return prefabPath;
        }

        void CreatePrefab(string prefabPath ,GameObject template){
            PrefabUtility.CreatePrefab(prefabPath, template);
            AssetDatabase.SaveAssets();
        }

        string GetPrefabPath(string panelName){
            return AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_PREFAB_FOLDER) + "/" + panelName + ".prefab";
        }

        void PlaceIntoScene(string prefabPath){
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject gameObject = Instantiate(prefab, Selection.activeTransform);
            gameObject.name = prefab.name;
            UICreateUtility.PlaceUIElementRoot(gameObject);
            PrefabUtility.ConnectGameObjectToPrefab(gameObject, prefab);
        }

        void SaveNewPanelConfig(string panelName, string prefabPath)
        {
            PanelConfig panelConfig = ConfigWindowItemsToConfig();
            PanelConfigItem panelConfigItem = CreateNewPanelConfig(panelName,prefabPath);
            panelConfig.items.Add(panelConfigItem);
            SavePanelConfig(panelConfig);
            AssetDatabase.Refresh();
        }

        PanelConfigItem CreateNewPanelConfig(string panelName, string prefabPath){
            PanelConfigItem panelConfigItem = new PanelConfigItem();
            panelConfigItem.ctrlClassName = UIEditorConstant.GetFullCtrlClassName(mUIEditorSettings.PANEL_CLASS_NAMESPACE, panelName, mUIEditorSettings.CTRL_CLASS_SUFFIX);
            panelConfigItem.viewClassName = UIEditorConstant.GetFullViewClassName(mUIEditorSettings.PANEL_CLASS_NAMESPACE, panelName, mUIEditorSettings.VIEW_CLASS_SUFFIX);
            panelConfigItem.prefabPath = prefabPath;
            return panelConfigItem;
        }

        void SavePanelConfig(PanelConfig panelConfig){
            string path = AssetDatabase.GetAssetPath(mUISettings.PANEL_CONFIG_FILE);
            path = EditorFileManager.AssetDatabasePathToFilePath(path);
            FileManager.WriteString(path, JsonUtility.ToJson(panelConfig, true));
            AssetDatabase.Refresh();
        }

        void RemovePanel(PanelConfigWindowItem item)
        {
            if (EditorUtility.DisplayDialog("RemovePanel", "Is remove this panel ?", "OK", "Canel"))
            {
                OnRemovePanelConfirm(item);
            }
        }

        void OnRemovePanelConfirm(PanelConfigWindowItem item){
            RemovePanelConfigItem(item);
            string scriptPath = GetScriptPath(item.ctrlScript);
            RemovePanelFiles(item);
            RemoveEmptyFolder(scriptPath);
            AssetDatabase.Refresh();
        }

        string GetScriptPath(MonoScript monoScript){
            string scriptPath = AssetDatabase.GetAssetPath(monoScript);
            return EditorFileManager.AssetDatabasePathToFilePath(scriptPath);
        }

        void RemovePanelConfigItem(PanelConfigWindowItem item){
            mPanelConfigWindowItems.Remove(item);
            SavePanelConfig(ConfigWindowItemsToConfig());
        }

        void RemovePanelFiles(PanelConfigWindowItem item){
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.panelPrefab));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.ctrlScript));
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.viewScript));
        }

        string GetScriptFolder(string scriptPath){
            return scriptPath.Substring(0, scriptPath.LastIndexOf("/", StringComparison.CurrentCulture));
        }

        void RemoveEmptyFolder(string scriptPath){
            scriptPath = GetScriptFolder(scriptPath);
            if (Directory.GetFiles(scriptPath, "*.*", SearchOption.AllDirectories).Length == 0)
            {
                FileManager.DeleteDirectory(scriptPath);
            }
        }

        void FindConfigFile()
        {
            Selection.activeObject = mUISettings.PANEL_CONFIG_FILE;
        }

    }

    public class PanelConfigWindowItem
    {
        public int id;
        public MonoScript viewScript;
        public MonoScript ctrlScript;
        public GameObject panelPrefab;
    }
}