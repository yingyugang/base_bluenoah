using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "UIEditorSettings", menuName = "CreateSetting/UIEditorSettings", order = 1)]
public class UIEditorSettings : ScriptableObject
{

	public Object PANEL_PREFAB_FOLDER;
	public Object DIALOG_PREFAB_FOLDER_PATH;
	public Object SCRIPT_TEMPLATE_PATH;
	public TextAsset SCRIPT_TEMPLATE_PANEL_CTRL;
	public TextAsset SCRIPT_TEMPLATE_PANEL_VIEW;
	public Object PANEL_CLASS_PATH;
	public GameObject DEFAULT_PREFAB_TEMPLATE;
    public List<GameObject> PREFAB_TEMPLATES;
	public Texture WINDOW_ICON_PATH;
	public string PANEL_CLASS_NAMESPACE = "BlueNoah.UI";
	public string CTRL_CLASS_SUFFIX = "Ctrl";
	public string VIEW_CLASS_SUFFIX = "View";

}
