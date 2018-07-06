/*************************************************************
 * Config the panel class by script.                         *
 * 1.Create the View class.                                  *
 * 2.Create the Ctrl class.                                  *
 * 3.Create the prefab gameObject.                           *
 * 4.Save the config json file.		                         *				 
 *************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using BlueNoah.UI;
using BlueNoah.Utility;
using BlueNoah.IO;
using BlueNoah.Editor.IO;

namespace BlueNoah.Editor
{
    public class UIPanelConfigWindow : UIConfigWindow
    {
        static List<Type> m_views;
        static UIPanelConfigWindow mPanelConfigWindow;
        PanelConfig mPanelConfig;
        GameObject mSelectTemplate;
        List<PanelConfigWindowItem> mPanelConfigWindowItems;
        bool mIsDisable = false;
        string mCreatePanelName = "";

        [MenuItem(UIEditorConstant.UI_PANEL_CONFIG_WINDOW_MENUITEM)]
        static void OnOpen()
        {
            mPanelConfigWindow = EditorWindow.GetWindow<UIPanelConfigWindow>();
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
            GUIContantTitle();
            GUIConfigItems();
            GUITemplates();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Panel Name : ", GUILayout.Width(160));
            mCreatePanelName = GUILayout.TextField(mCreatePanelName, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                CreatePanel(mCreatePanelName);
            }
            EditorGUILayout.EndHorizontal();

            DisplaySettings();

        }

        private void GUIContantTitle()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Panel list:");
            EditorGUILayout.EndHorizontal();
        }

        private void GUIConfigItems()
        {
            for (int i = 0; i < mPanelConfigWindowItems.Count; i++)
            {
                PanelConfigWindowItem item = mPanelConfigWindowItems[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(i.ToString());
                GUIConfigItem(item);
                if (GUILayout.Button("Remove"))
                {
                    RemovePanel(mPanelConfigWindowItems[i]);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void GUIConfigItem(PanelConfigWindowItem item)
        {
            EditorGUI.BeginDisabledGroup(mIsDisable);
            EditorGUILayout.ObjectField(item.ctrlScript, typeof(MonoScript), false);
            EditorGUILayout.ObjectField(item.viewScript, typeof(MonoScript), false);
            if (item.viewScript != null && item.panelPrefab != null)
                item.panelPrefab.GetOrAddComponent(item.viewScript.GetClass());
            EditorGUILayout.ObjectField(item.panelPrefab, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

        private void GUITemplates()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Template Name : ", GUILayout.Width(160));
            EditorGUILayout.EndHorizontal();
            List<GameObject> templates = this.mUIEditorSettings.PREFAB_TEMPLATES;
            for (int i = 0; i < templates.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(templates[i], typeof(GameObject), false);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    mSelectTemplate = templates[i];
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Selected Template : ");
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(mSelectTemplate, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void LoadConfig()
        {
            mPanelConfigWindowItems = new List<PanelConfigWindowItem>();
            string path = AssetDatabase.GetAssetPath(mUISettings.PANEL_CONFIG_FILE);
            path = EditorFileManager.AssetDatabasePathToFilePath(path);
            mConfigText = mUISettings.PANEL_CONFIG_FILE;
            string configTxt = FileManager.ReadString(path);
            mPanelConfig = JsonUtility.FromJson<PanelConfig>(configTxt);
            for (int i = 0; i < mPanelConfig.items.Count; i++)
            {
                PanelConfigWindowItem item = new PanelConfigWindowItem();
                item.id = i;
                item.ctrlScript = EditorFileManager.FindMono(mPanelConfig.items[i].ctrlClassName);
                item.viewScript = EditorFileManager.FindMono(mPanelConfig.items[i].viewClassName);
                item.panelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(mPanelConfig.items[i].prefabPath);
                mPanelConfigWindowItems.Add(item);
            }
        }

        void CreatePanel(string panelName)
        {
            if (string.IsNullOrEmpty(panelName))
            {
                EditorUtility.DisplayDialog("CreatePanel", "Panel name is empty.", "OK");
            }
            else if (EditorUtility.DisplayDialog("CreatePanel", string.Format("Is create {0} ?", panelName), "OK", "Canel"))
            {
                string ctrlClassName = UIEditorConstant.GetCtrlClassName(panelName, mUIEditorSettings.CTRL_CLASS_SUFFIX);
                string viewClassName = UIEditorConstant.GetViewClassName(panelName, mUIEditorSettings.VIEW_CLASS_SUFFIX);
                string panelScriptPath = UIEditorConstant.GetPanelPath(panelName, AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_CLASS_PATH));
                if (!FileManager.DirectoryExists(panelScriptPath))
                {
                    FileManager.CreateDirectoryName(panelScriptPath);
                }
                CreateViewClass(viewClassName, panelScriptPath);
                CreateCtrlClass(ctrlClassName, viewClassName, panelScriptPath);
                string prefabPath = CreatePrefab(panelName);
                SaveNewPanelConfig(panelName, prefabPath);
                AssetDatabase.Refresh();
            }
        }

        static string GetClassTemplate(string templateName)
        {
            string path = EditorFileManager.FindAsset(templateName, "txt");
            if (path == null)
            {
                return null;
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
            }
        }

        GameObject GetPrefabTemplate()
        {
            return mUIEditorSettings.DEFAULT_PREFAB_TEMPLATE;
        }

        PanelConfig ToConfig()
        {
            PanelConfig panelConfig = new PanelConfig();
            panelConfig.items = new List<PanelConfigItem>();
            for (int i = 0; i < mPanelConfigWindowItems.Count; i++)
            {
                PanelConfigWindowItem item = mPanelConfigWindowItems[i];
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
                configItem.index = i;
                configItem.ctrlClassName = ctrlScript;
                configItem.viewClassName = viewScript;
                configItem.prefabPath = prefabPath;
                panelConfig.items.Add(configItem);
            }
            return panelConfig;
        }

        void Save()
        {
            string config = JsonUtility.ToJson(ToConfig(), true);
            string path = AssetDatabase.GetAssetPath(mUISettings.PANEL_CONFIG_FILE);
            path = EditorFileManager.AssetDatabasePathToFilePath(path);
            FileManager.WriteString(path, config);
        }

        void CreateViewClass(string viewClassName, string panelScriptPath)
        {
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_VIEW.text;
            string resultText = templateText.Replace("{0}", mUIEditorSettings.PANEL_CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", viewClassName.Trim());
            string filePath = System.IO.Path.Combine(panelScriptPath, viewClassName + ".cs");
            FileManager.WriteString(filePath, resultText);
        }

        void CreateCtrlClass(string ctrlClassName, string viewClassName, string panelScriptPath)
        {
            string templateText = mUIEditorSettings.SCRIPT_TEMPLATE_PANEL_CTRL.text;// GetClassTemplate (UIEditorConstant.TEMPLATE_PANEL_CTRL);
            string resultText = templateText.Replace("{0}", mUIEditorSettings.PANEL_CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", ctrlClassName.Trim());
            resultText = resultText.Replace("{2}", viewClassName.Trim());
            string filePath = System.IO.Path.Combine(panelScriptPath, ctrlClassName + ".cs");
            FileManager.WriteString(filePath, resultText);
        }

        string CreatePrefab(string panelName)
        {
            //GameObject prefab = GetPrefabTemplate ();
            string prefabPath = AssetDatabase.GetAssetPath(mUIEditorSettings.PANEL_PREFAB_FOLDER) + "/" + panelName + ".prefab";
            PrefabUtility.CreatePrefab(prefabPath, mSelectTemplate);
            AssetDatabase.SaveAssets();
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject gameObject = GameObject.Instantiate(prefab, Selection.activeTransform);
            gameObject.name = prefab.name;
            UICreateUtility.PlaceUIElementRoot(gameObject);
            PrefabUtility.ConnectGameObjectToPrefab(gameObject, prefab);
            //TODO the prefabpath the best is relatively path.
            //e.g. absolute path is Assets/Panels/MyPagePanel/MypagePanel.prefab 
            //the relatively path will be MyPagePanel/MypagePanel.
            return prefabPath;
        }

        void SaveNewPanelConfig(string panelName, string prefabPath)
        {
            PanelConfig panelConfig = ToConfig();
            PanelConfigItem panelConfigItem = new PanelConfigItem();
            panelConfigItem.ctrlClassName = UIEditorConstant.GetFullCtrlClassName(mUIEditorSettings.PANEL_CLASS_NAMESPACE, panelName, mUIEditorSettings.CTRL_CLASS_SUFFIX);
            panelConfigItem.viewClassName = UIEditorConstant.GetFullViewClassName(mUIEditorSettings.PANEL_CLASS_NAMESPACE, panelName, mUIEditorSettings.VIEW_CLASS_SUFFIX);
            panelConfigItem.prefabPath = prefabPath;
            panelConfig.items.Add(panelConfigItem);
            string path = AssetDatabase.GetAssetPath(mUISettings.PANEL_CONFIG_FILE);
            path = EditorFileManager.AssetDatabasePathToFilePath(path);
            FileManager.WriteString(path, JsonUtility.ToJson(panelConfig, true));
            AssetDatabase.Refresh();
        }

        void RemovePanel(PanelConfigWindowItem item)
        {
            if (EditorUtility.DisplayDialog("RemovePanel", "Is remove this panel ?", "OK", "Canel"))
            {
                mPanelConfigWindowItems.Remove(item);
                Save();
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.panelPrefab));
                string scriptPath = AssetDatabase.GetAssetPath(item.ctrlScript);
                scriptPath = EditorFileManager.AssetDatabasePathToFilePath(scriptPath);
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.ctrlScript));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.viewScript));
                scriptPath = scriptPath.Substring(0, scriptPath.LastIndexOf("/", StringComparison.CurrentCulture));
                string[] paths = Directory.GetFiles(scriptPath, "*.*", SearchOption.AllDirectories);
                foreach (string path in paths)
                {
                    Debug.Log(path);
                }
                if (paths.Length == 0)
                {
                    FileManager.DeleteDirectory(scriptPath);
                }
                AssetDatabase.Refresh();
            }
        }

        void FindConfigFile()
        {
            Selection.activeObject = mUISettings.PANEL_CONFIG_FILE;
        }

        class PanelConfigWindowItem
        {
            public int id;
            public MonoScript viewScript;
            public MonoScript ctrlScript;
            public GameObject panelPrefab;
        }

        class TemplateItem
        {
            public bool isSelect;
            public GameObject template;
        }
    }
}