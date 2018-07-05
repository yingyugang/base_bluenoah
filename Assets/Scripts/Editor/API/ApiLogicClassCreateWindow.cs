using System.Collections;
using System.Collections.Generic;
/**************************************************************
* Create the API class by template.                         
* 1.input the api link.
* 2.select the net method.
* 3.add the api parameter.
* 4.click the "Create API Class" to create the api class.
* the class name will be the most same as api link . 
* e.g api = player/test , the class name will be PlayerTest.
**************************************************************/
using UnityEngine;
using UnityEditor;
using System.Text;
using BlueNoah.IO;

namespace BlueNoah.Editor
{
    public class APIWindow : EditorWindow
    {
        static APIWindow mApiLogicClassCreateWindow;
        public enum METHOD
        {
            POST = 0,
            GET = 1
        }
        static string mApiName;
        static METHOD mMethodName;
        static List<string> mParameterNames;

        [MenuItem(APIEditorConstant.API_WINDOW_MENUITEM)]
        static void Open()
        {
            if (mApiLogicClassCreateWindow == null)
                mApiLogicClassCreateWindow = CreateInstance<APIWindow>();
            mApiLogicClassCreateWindow.position = new Rect(new Vector2(0, 0), new Vector2(400, 300));
            mApiLogicClassCreateWindow.ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("API Name:", GUILayout.Width(100));
            mApiName = EditorGUILayout.TextField(mApiName);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Method Name:", GUILayout.Width(100));
            mMethodName = (METHOD)EditorGUILayout.EnumPopup(mMethodName, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            if (mParameterNames == null)
                mParameterNames = new List<string>();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parameters:", GUILayout.Width(100));
            if (GUILayout.Button("Add Parameter", GUILayout.Width(100)))
            {
                mParameterNames.Add("");
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < mParameterNames.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(60));
                mParameterNames[i] = EditorGUILayout.TextField(mParameterNames[i]);
                if (GUILayout.Button("Remove Parameter", GUILayout.Width(140)))
                {
                    mParameterNames.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Create API Class"))
            {
                CreateApiLogicFromTemplate(mApiName, mMethodName, mParameterNames);
            }
        }

        //Create the apilogic class base on the template.
        static void CreateApiLogicFromTemplate(string apiName, METHOD methodName, List<string> parameterNames)
        {
            string className = StringUtility.GetClassName(apiName, APIEditorConstant.API_CLASS_SUFFIX);
            string templateText = GetApiLogicTemplate();
            StringBuilder paramString = new StringBuilder();
            for (int i = 0; i < parameterNames.Count; i++)
            {
                if (!string.IsNullOrEmpty(parameterNames[i].Trim()))
                    paramString.AppendFormat("\r\nparameterNames.Add(\"{0}\");", parameterNames[i].Trim());
            }
            //why string.Format can not run.
            string resultText = templateText.Replace("{0}", className.Trim());
            resultText = resultText.Replace("{1}", AddApiConstant(apiName.Trim()));
            resultText = resultText.Replace("{2}", methodName.ToString());
            resultText = resultText.Replace("{3}", paramString.ToString().Trim());
            string apiFilePath = GetApiLogicFolder().Remove(0, "Assets".Length);
            string assetPath = "Assets" + apiFilePath + "/" + className + ".cs";
            Debug.Log("assetPath:" + assetPath);
            apiFilePath = Application.dataPath + apiFilePath + "/" + className + ".cs";
            FileManager.WriteString(apiFilePath, resultText);
            AssetDatabase.Refresh();
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(apiFilePath);
            Selection.activeObject = textAsset;
        }

        //Get the api logic folder to store the new class.
        //Search under the hole project. don't need the absolute path.
        static string GetApiLogicFolder()
        {
            string path = EditorFileManager.FindAsset(APIEditorConstant.API_LOGIC_NAME, "cs");
            if (path == null)
            {
                return null;
            }
            else
            {
                path = path.Remove(path.LastIndexOf(APIEditorConstant.API_LOGIC_NAME));
                return path;
            }
        }

        //Get the template file.
        //Search under the hole project. don't need the absolute path.
        static string GetApiLogicTemplate()
        {
            string path = EditorFileManager.FindAsset(APIEditorConstant.API_LOGIC_TEMPLATE_NAME, "txt");
            if (path == null)
            {
                return null;
            }
            else
            {
                return AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
            }
        }

        //Add the api param into ApiConstant class.
        static string AddApiConstant(string api)
        {
            string path = EditorFileManager.FindAsset(APIEditorConstant.API_CONSTANT_FILE_NAME, "cs");
            string apiConstantText = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
            path = Application.dataPath + path.Remove(0, "Assets".Length);
            int index = apiConstantText.IndexOf("{", apiConstantText.IndexOf(APIEditorConstant.API_CONSTANT_FILE_NAME)) + 1;
            string apiName = api.Replace("/", "_");
            apiConstantText = apiConstantText.Insert(index, string.Format("\r\npublic const string {0} = \"{1}\";", apiName, api));//
            FileManager.WriteString(path, apiConstantText);
            return apiName;
        }
    }
}
