using UnityEditor;
using UnityEngine;
namespace BlueNoah.Editor
{
    public class UIDialogConfigWindowGUI : UIConfigWindowGUI
    {
        protected override void DrawConfigItemWithDisable(UIConfigWindowItem item)
        {
            UIDialogConfigWindowItem uiDialogConfigWindowItem = (UIDialogConfigWindowItem)item;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(uiDialogConfigWindowItem.classScript, typeof(MonoScript), false);
            EditorGUILayout.ObjectField(uiDialogConfigWindowItem.dialogPrefab, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }
    }
}