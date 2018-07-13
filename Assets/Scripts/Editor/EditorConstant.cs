using BlueNoah.Editor.IO;

namespace BlueNoah.Editor
{
    public class UIEditorConstant
    {
        public const string UI_PANEL_CONFIG_WINDOW_MENUITEM = "Tools/UI/PanelConfig";
        public const string UI_DIALOG_CONFIG_WINDOW_MENUITEM = "Tools/UI/DialogConfig";
        public const string ASSETBUNDLE_BUILD_WINDOW_MENUITEM = "Tools/AssetBundle/AB Build Manager";
        public const string ASSETBUNDLE_SETTING_WINDOW_MENUITEM = "Tools/AssetBundle/AB Settings";

        public static string GetClassPath(string subFolderName, string parentPath)
        {
            return EditorFileManager.AssetDatabasePathToFilePath(System.IO.Path.Combine(parentPath, subFolderName));
        }

        public static string GetClassName(string componentName, string suffix)
        {
            return string.Format("{0}{1}", componentName, suffix);
        }

        public static string GetFullClassName(string namespaceName, string panelName, string suffix)
        {
            return string.Format("{0}{1}{2}{3}", namespaceName, ".", panelName, suffix);
        }
    }
}
