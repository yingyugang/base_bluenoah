using System.Collections.Generic;
using BlueNoah.Utility;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace BlueNoah.Editor
{
    public static class UIPanelConfigWindowGUI
    {
        static void DrawTextRow(string text, int width = 160)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(text, GUILayout.Width(width));
            EditorGUILayout.EndHorizontal();
        }

        public static void DrawTemplatePattern(List<GameObject> templates, GameObject currentTemplate, UnityAction<GameObject> onSelect)
        {
            DrawTemplates(templates, onSelect);
            DrawCurrentTemplate(currentTemplate);
        }

        public static void DrawPanelPattern(List<PanelConfigWindowItem> panelConfigWindowItems, UnityAction<PanelConfigWindowItem> onRemove,ref string panelName, UnityAction<string> onCreatePanel){
            DrawPanels(panelConfigWindowItems,onRemove);
            DrawCreatePanel(ref panelName,onCreatePanel);
        }

        public static void DrawConfigPattern(TextAsset configText, ScriptableObject uiEditorSettingScriptObject,ScriptableObject uiSettingScriptableObject){
            DrawConfigFile(configText);
            DrawUIEditorSettingScriptableObjectFile(uiEditorSettingScriptObject);
            DrawUISettingScriptableObjectFile(uiSettingScriptableObject);
        }

        static void DrawPanels(List<PanelConfigWindowItem> panelConfigWindowItems, UnityAction<PanelConfigWindowItem> onRemove){
            DrawContantTitle("Panel List : ");
            DrawConfigItems(panelConfigWindowItems,onRemove);
        }

        static void DrawContantTitle(string title)
        {
            DrawTextRow(title);
        }

        static void DrawConfigItems(List<PanelConfigWindowItem> panelConfigWindowItems, UnityAction<PanelConfigWindowItem> onRemove)
        {
            for (int i = 0; i < panelConfigWindowItems.Count; i++)
            {
                DrawConfigItem(panelConfigWindowItems[i], onRemove);
            }
        }

        static void DrawConfigItem(PanelConfigWindowItem item, UnityAction<PanelConfigWindowItem> onRemove)
        {
            EditorGUILayout.BeginHorizontal();
            DrawConfigItemIndex(item.id);
            DrawConfigItemWithDisable(item);
            DrawConfigItemAction(item, onRemove);
            EditorGUILayout.EndHorizontal();
        }

        static void DrawConfigItemIndex(int index)
        {
            GUILayout.Label(index.ToString());
        }

        static void DrawConfigItemWithDisable(PanelConfigWindowItem item)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(item.ctrlScript, typeof(MonoScript), false);
            EditorGUILayout.ObjectField(item.viewScript, typeof(MonoScript), false);
            //TODO to append the script when object awake when load the prefab in game.
            if (item.viewScript != null && item.panelPrefab != null)
                item.panelPrefab.GetOrAddComponent(item.viewScript.GetClass());
            EditorGUILayout.ObjectField(item.panelPrefab, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

        static void DrawConfigItemAction(PanelConfigWindowItem item, UnityAction<PanelConfigWindowItem> onRemove)
        {
            if (GUILayout.Button("Remove"))
            {
                if (onRemove != null)
                    onRemove(item);
            }
        }

        static void DrawTemplates(List<GameObject> templates, UnityAction<GameObject> onSelect)
        {
            DrawTextRow("Template Name : ", 160);
            for (int i = 0; i < templates.Count; i++)
            {
                DrawTemplate(templates[i], onSelect);
            }
        }

        static void DrawTemplate(GameObject template, UnityAction<GameObject> onSelect)
        {
            EditorGUILayout.BeginHorizontal();
            DrawTemplateWithDisable(template);
            DrawTemplateAction(template, onSelect);
            EditorGUILayout.EndHorizontal();
        }

        static void DrawTemplateWithDisable(GameObject template)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(template, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

        static void DrawTemplateAction(GameObject template, UnityAction<GameObject> onSelect)
        {
            if (GUILayout.Button("Select", GUILayout.Width(70)))
            {
                if (onSelect != null)
                    onSelect(template);
            }
        }

        static void DrawCurrentTemplate(GameObject currentTemplate)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Selected Template : ");
            DrawCurrentTempateWithDisable(currentTemplate);
            EditorGUILayout.EndHorizontal();
        }

        static void DrawCurrentTempateWithDisable(GameObject currentTemplate)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(currentTemplate, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

        static void DrawCreatePanel(ref string panelName, UnityAction<string> onCreatePanel)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Panel Name : ", GUILayout.Width(160));
            panelName = GUILayout.TextField(panelName, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            DrawCreateAction(panelName, onCreatePanel);
            EditorGUILayout.EndHorizontal();
        }

        static void DrawCreateAction(string panelName, UnityAction<string> onCreatePanel)
        {
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                if (onCreatePanel != null)
                    onCreatePanel(panelName);
            }
        }

        static void DrawConfigFile(TextAsset configText){
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Config File : ", GUILayout.Width(160));
            DrawConfigFileWithDisable(configText);
            EditorGUILayout.EndHorizontal();
        }

        static void DrawConfigFileWithDisable(TextAsset configText)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(configText, typeof(TextAsset), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
        }

        static void DrawUIEditorSettingScriptableObjectFile(ScriptableObject scriptableObject)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI Editor ScriptableObject : ", GUILayout.Width(160));
            DrawConfigFileWithDisable(scriptableObject);
            EditorGUILayout.EndHorizontal();
        }

         static void DrawUISettingScriptableObjectFile(ScriptableObject scriptableObject)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI ScriptableObject : ", GUILayout.Width(160));
            DrawConfigFileWithDisable(scriptableObject);
            EditorGUILayout.EndHorizontal();
        }

        static void DrawConfigFileWithDisable(ScriptableObject scriptableObject)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(scriptableObject, typeof(TextAsset), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
        }

    }
}
