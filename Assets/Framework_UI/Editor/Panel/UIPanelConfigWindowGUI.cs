using BlueNoah.Utility;
using UnityEditor;
using UnityEngine;

namespace BlueNoah.Editor
{
    public class UIPanelConfigWindowGUI : UIConfigWindowGUI
    {

        protected override void DrawConfigItemWithDisable(UIConfigWindowItem item)
        {
            UIPanelConfigWindowItem panelConfigWindowItem = (UIPanelConfigWindowItem)item;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(panelConfigWindowItem.ctrlScript, typeof(MonoScript), false);
            EditorGUILayout.ObjectField(panelConfigWindowItem.viewScript, typeof(MonoScript), false);
            //TODO to append the script when object awake when load the prefab in game.
            if (panelConfigWindowItem.viewScript != null && panelConfigWindowItem.panelPrefab != null)
                panelConfigWindowItem.panelPrefab.GetOrAddComponent(panelConfigWindowItem.viewScript.GetClass());
            EditorGUILayout.ObjectField(panelConfigWindowItem.panelPrefab, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
        }

    }
}
