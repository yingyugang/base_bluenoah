using UnityEngine;

public class UIEditorConstant
{
    //TODO need to search auto and absolute path. need the two of functions.
    //absolute,if not then search auto.
	//TODO to check the paths ,and display the error messages.
	//TODO little complex.
    const string M_PANEL_CONFIG_PATH = "Assets/Resources/Panels/PanelConfig.json";
    const string M_PANEL_PREFAB_PATH = "Assets/Resources/Panels";
    public const string DEFAULT_TEMPLATE_PATH = "Assets/Templates/UITemplates/Prefabs/HomePanel_Landscape.prefab";
	const string M_TEMPLATE_SCRIPT_PATH = "Assets/Templates/UITemplates/Scripts";
	public const string WINDOW_ICON_PATH = "Assets/Templates/UITemplates/Icons/emoji-nerd-glasses.png";
	public const string TEMPLATE_PANEL_CTRL = "PanelCtrlTemplate";
	public const string TEMPLATE_PANEL_VIEW = "PanelViewTemplate";
	const  string M_TEMPLATE_PATTERN = ".txt";
    public const string PANEL_CLASS_PATH = "Scripts/Game/UI/Panel/Panel";
    public const string PANEL_CLASS_NAMESPACE = "BlueNoah.UI";
	const string CTRL_CLASS_SUFFIX = "Ctrl";
	const string VIEW_CLASS_SUFFIX = "View";
	public const string UI_PANEL_CONFIG_WINDOW_MENUITEM = "GameEngine/Panel/PanelConfig";

	public static string PANEL_CONFIG_PATH {
		get { 
			//TODO 
            if (M_PANEL_CONFIG_PATH.IndexOf ("Resources/",System.StringComparison.CurrentCulture) == -1) {
				Debug.LogError (string.Format ("PANEL_CONFIG_PATH should be put in Resources Folder. Current Path is {0}", M_PANEL_CONFIG_PATH));
			}
			return M_PANEL_CONFIG_PATH;
		}
	}

	public static string PANEL_PREFAB_PATH {
		get { 
			//TODO 
            if (M_PANEL_PREFAB_PATH.IndexOf ("Resources/", System.StringComparison.CurrentCulture) == -1) {
				Debug.LogError (string.Format ("PANEL_PREFAB_PATH should be put in Resources Folder. Current Path is {0}", M_PANEL_PREFAB_PATH));
			}
			return M_PANEL_PREFAB_PATH;
		}
	}

	public static string TEMPLATE_PANEL_CTRL_PATH {
		get { 
			return System.IO.Path.Combine (M_TEMPLATE_SCRIPT_PATH, TEMPLATE_PANEL_CTRL + M_TEMPLATE_PATTERN);
		}
	}

	public static string TEMPLATE_PANEL_VIEW_PATH {
		get { 
			return System.IO.Path.Combine (M_TEMPLATE_SCRIPT_PATH, TEMPLATE_PANEL_VIEW + M_TEMPLATE_PATTERN);
		}
	}

	public static string GetPanelPath (string panelName)
	{
		return System.IO.Path.Combine (Application.dataPath,System.IO.Path.Combine (PANEL_CLASS_PATH, panelName));
	}

	public static string GetViewClassName(string panelName){
		return string.Format ("{0}{1}",panelName,VIEW_CLASS_SUFFIX);
	}

	public static string GetCtrlClassName(string panelName){
		return string.Format ("{0}{1}",panelName,CTRL_CLASS_SUFFIX);
	}

	public static string GetFullViewClassName(string panelName){
        return string.Format ("{0}{1}{2}{3}",PANEL_CLASS_NAMESPACE,".",panelName,VIEW_CLASS_SUFFIX);
	}

	public static string GetFullCtrlClassName(string panelName){
        return string.Format ("{0}{1}{2}{3}",PANEL_CLASS_NAMESPACE,".",panelName,CTRL_CLASS_SUFFIX);
	}
}
