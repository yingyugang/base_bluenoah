using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
namespace BlueNoah.Editor
{
    public abstract class UIConfigWindowGUI
    {

        public void DrawTemplatePattern(List<GameObject> templates, GameObject currentTemplate, UnityAction<GameObject> onSelect)
        {
            DrawTemplateTitle();
            DrawTemplateItems(templates, onSelect);
            DrawCurrentTemplate(currentTemplate);
        }

        void DrawTemplateTitle()
        {
            DrawTextRow("Template Name : ", 160);
        }

        void DrawTemplateItems(List<GameObject> templates, UnityAction<GameObject> onSelect)
        {
            for (int i = 0; i < templates.Count; i++)
            {
                DrawTemplate(templates[i], onSelect);
            }
        }

        void DrawTemplate(GameObject template, UnityAction<GameObject> onSelect)
        {
            EditorGUILayout.BeginHorizontal();
            DrawTemplateWithDisable(template);
            DrawTemplateAction(template, onSelect);
            EditorGUILayout.EndHorizontal();
        }

        void DrawTemplateWithDisable(GameObject template)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(template, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

        void DrawTemplateAction(GameObject template, UnityAction<GameObject> onSelect)
        {
            if (GUILayout.Button("Select", GUILayout.Width(70)))
            {
                if (onSelect != null)
                    onSelect(template);
            }
        }

        void DrawCurrentTemplate(GameObject currentTemplate)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Selected Template : ");
            DrawCurrentTempateWithDisable(currentTemplate);
            EditorGUILayout.EndHorizontal();
        }

        void DrawCurrentTempateWithDisable(GameObject currentTemplate)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(currentTemplate, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

        public void DrawConfigItemsPattern<T>(List<T> configWindowItems, UnityAction<T> onRemove, ref string componentName, UnityAction<string> onCreate) where T : UIConfigWindowItem
        {
            DrawPatternTitle("Panel List : ");
            DrawConfigItems<T>(configWindowItems, onRemove);
            DrawCreateItem(ref componentName, onCreate);
        }

        void DrawPatternTitle(string title)
        {
            DrawTextRow(title);
        }

        void DrawConfigItems<T>(List<T> configWindowItems, UnityAction<T> onRemove) where T : UIConfigWindowItem
        {
            for (int i = 0; i < configWindowItems.Count; i++)
            {
                DrawConfigItem<T>(configWindowItems[i], onRemove);
            }
        }

        void DrawConfigItem<T>(T item, UnityAction<T> onRemove) where T : UIConfigWindowItem
        {
            EditorGUILayout.BeginHorizontal();
            DrawConfigItemIndex(item.id);
            DrawConfigItemWithDisable(item);
            DrawConfigItemAction<T>(item, onRemove);
            EditorGUILayout.EndHorizontal();
        }

        void DrawConfigItemIndex(int index)
        {
            GUILayout.Label(index.ToString());
        }

        void DrawConfigItemAction<T>(T item, UnityAction<T> onRemove) where T : UIConfigWindowItem
        {
            if (GUILayout.Button("Remove"))
            {
                if (onRemove != null)
                    onRemove(item);
            }
        }

        void DrawCreateItem(ref string componentName, UnityAction<string> onCreate)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Create Name : ", GUILayout.Width(160));
            componentName = GUILayout.TextField(componentName, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            DrawCreateAction(componentName, onCreate);
            EditorGUILayout.EndHorizontal();
        }

        void DrawCreateAction(string componentName, UnityAction<string> onCreate)
        {
            if (GUILayout.Button("Create", GUILayout.Width(100)))
            {
                if (onCreate != null)
                    onCreate(componentName);
            }
        }

        protected abstract void DrawConfigItemWithDisable(UIConfigWindowItem item);

        public void DrawConfigPattern(TextAsset configText, ScriptableObject uiEditorSettingScriptObject, ScriptableObject uiSettingScriptableObject)
        {
            DrawConfigFile(configText);
            DrawUIEditorSettingScriptableObjectFile(uiEditorSettingScriptObject);
            DrawUISettingScriptableObjectFile(uiSettingScriptableObject);
        }

        void DrawConfigFile(TextAsset configText)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Config File : ", GUILayout.Width(160));
            DrawConfigFileWithDisable(configText);
            EditorGUILayout.EndHorizontal();
        }

        void DrawConfigFileWithDisable(TextAsset configText)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(configText, typeof(TextAsset), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
        }

        void DrawUIEditorSettingScriptableObjectFile(ScriptableObject scriptableObject)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI Editor ScriptableObject : ", GUILayout.Width(160));
            DrawConfigFileWithDisable(scriptableObject);
            EditorGUILayout.EndHorizontal();
        }

        void DrawUISettingScriptableObjectFile(ScriptableObject scriptableObject)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("UI ScriptableObject : ", GUILayout.Width(160));
            DrawConfigFileWithDisable(scriptableObject);
            EditorGUILayout.EndHorizontal();
        }

        void DrawConfigFileWithDisable(ScriptableObject scriptableObject)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(scriptableObject, typeof(TextAsset), false, GUILayout.MinWidth(130), GUILayout.MaxWidth(200));
            EditorGUI.EndDisabledGroup();
        }

        protected void DrawTextRow(string text, int width = 160)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(text, GUILayout.Width(width));
            EditorGUILayout.EndHorizontal();
        }
    }
}