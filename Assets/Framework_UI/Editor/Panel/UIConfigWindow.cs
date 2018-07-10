using UnityEngine;
using UnityEditor;
using BlueNoah.IO;
using BlueNoah.Editor.IO;
using System.IO;

namespace BlueNoah.Editor
{
    public abstract class UIConfigWindow : EditorWindow
    {
        const string UIEDITORSETTINGS_PATH = "Assets/Framework_UI/Editor/Setting/UIEditorSettings.asset";
        const string UISETTINGS_PATH = "Assets/Framework_UI/Resources/Settings/UISettings.asset";

        protected UIEditorSettings mUIEditorSettings;
        protected UISettings mUISettings;
        protected TextAsset mConfigText;
        protected GameObject mCurrentTemplate;
        protected string mName;

        protected virtual void OnEnable()
        {
            LoadSettings();
            LoadConfig();
        }

        protected abstract void LoadConfig();

        protected void LoadSettings()
        {
            mUIEditorSettings = AssetDatabase.LoadAssetAtPath<UIEditorSettings>(UIEDITORSETTINGS_PATH);
            if (mUIEditorSettings == null)
            {
                Debug.Log(string.Format("The UIEditorSettings is not at {0}", UIEDITORSETTINGS_PATH));
                mUIEditorSettings = SearchSettingFile<UIEditorSettings>(UIEDITORSETTINGS_PATH);
            }
            mUISettings = AssetDatabase.LoadAssetAtPath<UISettings>(UISETTINGS_PATH);
            if (mUISettings == null)
            {
                Debug.Log(string.Format("The UISETTINGS_PATH is not at {0}", UISETTINGS_PATH));
                mUISettings = SearchSettingFile<UISettings>(UISETTINGS_PATH);
            }
        }

        private T SearchSettingFile<T>(string path) where T : ScriptableObject
        {
            string fileName = FileManager.GetFileNameFromPath(path);
            string fileMain = FileManager.GetFileMain(fileName);
            string filePattern = FileManager.GetFilePattern(fileName);
            string assetFilePath = EditorFileManager.FindAsset(fileMain,filePattern);
            T t = AssetDatabase.LoadAssetAtPath<T>(assetFilePath);
            if(t!=null){
                Debug.Log(string.Format("{0} is searched.",assetFilePath));
            }else{
                Debug.Log(string.Format("{0} is not existing.", typeof(T)));
            }
            return t;
        }

        protected void InitContent(string title, string tooltip)
        {
            GUIContent guiContent = new GUIContent();
            guiContent.text = title;
            guiContent.image = mUIEditorSettings.WINDOW_ICON_PATH;
            guiContent.tooltip = tooltip;
            this.titleContent = guiContent;
        }

        protected string GetClassFullPath(string subFolderName,string parentPath)
        {
            string path = UIEditorConstant.GetClassPath(subFolderName, parentPath);
            CreateScriptPath(path);
            return path;
        }

        protected void CreateScriptPath(string panelScriptPath)
        {
            if (!FileManager.DirectoryExists(panelScriptPath))
            {
                FileManager.CreateDirectoryName(panelScriptPath);
            }
        }

        protected string CreatePrefab(string panelName,GameObject template)
        {
            string prefabPath = GetPrefabPath(panelName);
            CreatePrefabByTemplate(prefabPath, template);
            PlaceIntoScene(prefabPath);
            return prefabPath;
        }

        void CreatePrefabByTemplate(string prefabPath, GameObject template)
        {
            PrefabUtility.CreatePrefab(prefabPath, template);
            AssetDatabase.SaveAssets();
        }

        protected abstract string GetPrefabPath(string componentName);

        protected void PlaceIntoScene(string prefabPath)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject gameObject = Instantiate(prefab, Selection.activeTransform);
            gameObject.name = prefab.name;
            UICreateUtility.PlaceUIElementRoot(gameObject);
            PrefabUtility.ConnectGameObjectToPrefab(gameObject, prefab);
        }

        protected bool CheckCreateable(string currentName,GameObject currentTemplate)
        {
            if (string.IsNullOrEmpty(currentName))
            {
                EditorUtility.DisplayDialog("CreatePanel", "Create fail , panel name is empty.", "OK");
                return false;
            }
            else if (currentTemplate == null)
            {
                EditorUtility.DisplayDialog("CreatePanel", "Create fail , template is empty.", "OK");
                return false;
            }
            return true;
        }

        protected void Create(string dialogName)
        {
            if (CheckCreateable(dialogName, mCurrentTemplate))
            {
                if (EditorUtility.DisplayDialog("Create", string.Format("Is create {0} ?", dialogName), "OK", "Canel"))
                {
                    OnCreate(dialogName);
                }
            }
        }

        protected abstract void OnCreate(string componentName);

        protected void Remove(UIConfigWindowItem item)
        {
            if (EditorUtility.DisplayDialog("Remove", "Is remove this ?", "OK", "Canel"))
            {
                OnRemoveConfirm(item);
            }
        }

        protected abstract void OnRemoveConfirm(UIConfigWindowItem item);

        protected void SaveConfig(string panelConfig,TextAsset config)
        {
            string path = AssetDatabase.GetAssetPath(config);
            path = EditorFileManager.AssetDatabasePathToFilePath(path);
            FileManager.WriteString(path, panelConfig);
            AssetDatabase.Refresh();
        }

        protected string GetScriptPath(MonoScript monoScript)
        {
            string scriptPath = AssetDatabase.GetAssetPath(monoScript);
            return EditorFileManager.AssetDatabasePathToFilePath(scriptPath);
        }

        protected void RemoveEmptyFolder(string scriptPath)
        {
            scriptPath = GetScriptFolder(scriptPath);
            if (Directory.GetFiles(scriptPath, "*.*", SearchOption.AllDirectories).Length == 0)
            {
                FileManager.DeleteDirectory(scriptPath);
            }
        }

        string GetScriptFolder(string scriptPath)
        {
            return scriptPath.Substring(0, scriptPath.LastIndexOf("/", System.StringComparison.CurrentCulture));
        }
    }

    public class UIConfigWindowItem
    {
        public int id;
    }
}
