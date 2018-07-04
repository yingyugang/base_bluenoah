/*************************************************************
 * Config the panel class by script.                         *
 * 1.Create the View class.                                  *
 * 2.Create the Ctrl class.                                  *
 * 3.Create the prefab gameObject.                           *               
 *************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace BlueNoah.Editor
{
    //TODO format.
    public class UIPanelConfigWindow : EditorWindow
    {
        static List<Type> m_views;
        static UIPanelConfigWindow mPanelConfigWindow;
        PanelConfig mPanelConfig;
        List<PanelConfigWindowItem> mPanelConfigWindowItems;
        TextAsset mConfigText;
        bool mIsDisable = false;

        [MenuItem("Tools/Panel/PanelConfig")]
        static void OnOpen()
        {
            mPanelConfigWindow = EditorWindow.GetWindow<UIPanelConfigWindow>();
            GUIContent guiContent = new GUIContent();
            guiContent.text = "PanleConfig";
            guiContent.image = AssetDatabase.LoadAssetAtPath<Texture>(UIEditorConstant.WINDOW_ICON_PATH);
            guiContent.tooltip = "config the panels";
            mPanelConfigWindow.titleContent = guiContent;
            mPanelConfigWindow.position = new Rect(new Vector2(0, 0), new Vector2(400, 300));
            mPanelConfigWindow.Show();
            mPanelConfigWindow.LoadPanelConfig();
            mPanelConfigWindow.Focus();
        }

        string mCreatePanelName = "";

        void OnGUI()
        {
            if (mPanelConfigWindowItems == null)
                LoadPanelConfig();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Panel list:");
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < mPanelConfigWindowItems.Count; i++)
            {
                PanelConfigWindowItem item = mPanelConfigWindowItems[i];
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(i.ToString());
                EditorGUI.BeginDisabledGroup(mIsDisable);
                EditorGUILayout.ObjectField(item.ctrlScript, typeof(MonoScript), false);
                EditorGUILayout.ObjectField(item.viewScript, typeof(MonoScript), false);
                if (item.viewScript != null && item.panelPrefab!=null)
                    item.panelPrefab.GetOrAddComponent(item.viewScript.GetClass());
                EditorGUILayout.ObjectField(item.panelPrefab, typeof(GameObject), false);
                EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("Remove"))
                {
                    RemovePanel(mPanelConfigWindowItems[i]);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Panel Name : ", GUILayout.Width(70));
            mCreatePanelName = GUILayout.TextField(mCreatePanelName, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                CreatePanel(mCreatePanelName);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Config File : ", GUILayout.Width(70));
            EditorGUI.BeginDisabledGroup(mIsDisable);
            EditorGUILayout.ObjectField(mConfigText, typeof(TextAsset), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {

        }

        private void LoadPanelConfig()
        {
            mPanelConfigWindowItems = new List<PanelConfigWindowItem>();
            mConfigText = AssetDatabase.LoadAssetAtPath<TextAsset>(UIEditorConstant.PANEL_CONFIG_PATH);
            EditorFileManager.ResourcesPathToFilePath(UIEditorConstant.PANEL_CONFIG_PATH);
            string configTxt = FileManager.ReadString(EditorFileManager.AssetDatabasePathToFilePath(UIEditorConstant.PANEL_CONFIG_PATH));
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
            if (EditorUtility.DisplayDialog("CreatePanel", string.Format("Is create {0} ?", panelName), "OK", "Canel"))
            {
                string ctrlClassName = UIEditorConstant.GetCtrlClassName(panelName);
                string viewClassName = UIEditorConstant.GetViewClassName(panelName);
                string panelScriptPath = UIEditorConstant.GetPanelPath(panelName);
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

        static GameObject GetPrefabTemplate()
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(UIEditorConstant.DEFAULT_TEMPLATE_PATH);
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
            string config = JsonUtility.ToJson(ToConfig());
            Debug.Log(config);
            FileManager.WriteString(UIEditorConstant.PANEL_CONFIG_PATH, config);
        }

        void CreateViewClass(string viewClassName, string panelScriptPath)
        {
            string templateText = GetClassTemplate(UIEditorConstant.TEMPLATE_PANEL_VIEW);
            string resultText = templateText.Replace("{0}", UIEditorConstant.PANEL_CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", viewClassName.Trim());
            string filePath = System.IO.Path.Combine(panelScriptPath, viewClassName + ".cs");
            FileManager.WriteString(filePath, resultText);
        }

        void CreateCtrlClass(string ctrlClassName, string viewClassName, string panelScriptPath)
        {
            string templateText = GetClassTemplate(UIEditorConstant.TEMPLATE_PANEL_CTRL);
            string resultText = templateText.Replace("{0}", UIEditorConstant.PANEL_CLASS_NAMESPACE);
            resultText = resultText.Replace("{1}", ctrlClassName.Trim());
            resultText = resultText.Replace("{2}", viewClassName.Trim());
            string filePath = System.IO.Path.Combine(panelScriptPath, ctrlClassName + ".cs");
            FileManager.WriteString(filePath, resultText);
        }

        string CreatePrefab(string panelName)
        {
            GameObject prefab = GetPrefabTemplate();
            //TODO the template should can be changed.
            string prefabPath = UIEditorConstant.PANEL_PREFAB_PATH + "/" + panelName + ".prefab";
            PrefabUtility.CreatePrefab(prefabPath, prefab);
            AssetDatabase.SaveAssets();
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject gameObject = GameObject.Instantiate(prefab, Selection.activeTransform);
            gameObject.name = prefab.name;
            UICreateUtility.PlaceUIElementRoot(gameObject);
            PrefabUtility.ConnectGameObjectToPrefab(gameObject, prefab);
            return prefabPath;
        }

        void SaveNewPanelConfig(string panelName, string prefabPath)
        {
            PanelConfig panelConfig = ToConfig();
            PanelConfigItem panelConfigItem = new PanelConfigItem();
            panelConfigItem.ctrlClassName = UIEditorConstant.GetFullCtrlClassName(panelName);
            panelConfigItem.viewClassName = UIEditorConstant.GetFullViewClassName(panelName);
            panelConfigItem.prefabPath = prefabPath;
            panelConfig.items.Add(panelConfigItem);
            FileManager.WriteString(UIEditorConstant.PANEL_CONFIG_PATH, JsonUtility.ToJson(panelConfig));
        }

        void RemovePanel(PanelConfigWindowItem item)
        {
            if (EditorUtility.DisplayDialog("RemovePanel", "Is remove this panel ?", "OK", "Canel"))
            {
                mPanelConfigWindowItems.Remove(item);
                Save();
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.panelPrefab));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.ctrlScript));
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item.viewScript));
            }
        }

        void FindConfigFile()
        {
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<TextAsset>(UIEditorConstant.PANEL_CONFIG_PATH);
        }

        class PanelConfigWindowItem
        {
            public int id;
            public MonoScript viewScript;
            public MonoScript ctrlScript;
            public GameObject panelPrefab;
        }
    }
}