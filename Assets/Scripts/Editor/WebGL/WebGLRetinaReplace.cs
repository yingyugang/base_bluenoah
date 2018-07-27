/// <summary>
/// Description：
/// Auto set the webgl release to adapt the Retina mac browser. 
/// Author：應　彧剛（yingyugang@gmail.com）
/// Create time：2018-05-30 17:26:56
/// </summary>
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

namespace BlueNoah.Editor
{
    public class WebGLRetinaReplace : EditorWindow
    {
        public static string RootPath = "";
        public static string FolderName = "";

        public static string FolderPath { get { return RootPath + FolderName + "/"; } }
        public static string TARGET_PATH { get { return FolderPath + "Build/" + FolderName + ".asm.framework.unityweb"; } }
        public static string INDEX_PATH { get { return FolderPath + "index.html"; } }

        public static int WIDTH = 0;
        public static int HEIGHT = 0;

        [MenuItem("Tools/BlueNoah/WebGL-Retina")]
        static void Open()
        {
            EditorWindow.GetWindow<WebGLRetinaReplace>("WebGL-Retina");
        }

        void OnGUI()
        {
            GUILayout.Space(30);
#if !UNITY_WEBGL
        GUILayout.Label("Not Build Settings WebGL");
        GUILayout.Space(10);
#endif
            if (GUILayout.Button("Set Folder Path"))
            {
                string path = EditorUtility.OpenFolderPanel("Build WebGLFolder", "", "");
                if (SetPaths(path))
                    SetDefaultScreenSize();
                else
                    return;
            }

            if (string.IsNullOrEmpty(RootPath))
                return;

            GUILayout.Label("Root Path : " + RootPath);
            GUILayout.Label("FolderName : " + FolderName);

            GUILayout.Space(10);
            GUILayout.Label("WindowSize");
            WIDTH = EditorGUILayout.IntField("Width : ", WIDTH);
            HEIGHT = EditorGUILayout.IntField("Height : ", HEIGHT);

            if (WIDTH < 1 || HEIGHT < 1)
                return;

            if (!File.Exists(TARGET_PATH))
            {
                GUILayout.Label("is not existing! for Target WebGL Folder.");
                return;
            }

            GUILayout.Space(10);
            if (GUILayout.Button("Retina"))
            {
                ReplaceForRetina();
            }
        }

        private static bool SetPaths(string path)
        {
            List<string> paths = path.Split('/').ToList();
            if (paths.Count < 2)
            {
                Debug.LogError("Web GL Builds Not Founds Files.  Path:" + path);
                return false;
            }

            FolderName = paths.Last();

            paths.RemoveAt(paths.Count - 1);
            RootPath = string.Join("/", paths.ToArray()) + "/";
            return true;
        }

        private static void SetDefaultScreenSize()
        {
            if (WIDTH < 1 || HEIGHT < 1)
            {
                WIDTH = PlayerSettings.defaultWebScreenWidth;
                HEIGHT = PlayerSettings.defaultWebScreenHeight;
            }
        }

        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            Debug.Log("BuildPath : " + path);

            if (target != BuildTarget.WebGL)
                return;

            if (!SetPaths(path))
                return;

            SetDefaultScreenSize();
            ReplaceForRetina();
        }

        public static void ReplaceForRetina()
        {
            ReplaceKeyWords();
            ReplaceIndex();
        }

        static void ReplaceKeyWords()
        {
            if (!File.Exists(TARGET_PATH))
            {
                Debug.LogError(TARGET_PATH + " is not existing!");
                return;
            }
            string text = File.ReadAllText(TARGET_PATH);
            text = text.Replace("HEAP32[eventStruct+8>>2]=e.screenX;", "HEAP32[eventStruct+8>>2]=e.screenX*devicePixelRatio;");
            text = text.Replace("e.screenX-JSEvents.previousScreenX;", "e.screenX*devicePixelRatio-JSEvents.previousScreenX;");
            text = text.Replace("JSEvents.previousScreenX=e.screenX;", "JSEvents.previousScreenX=e.screenX*devicePixelRatio;");

            text = text.Replace("HEAP32[eventStruct+12>>2]=e.screenY;", "HEAP32[eventStruct+12>>2]=e.screenY*devicePixelRatio;");
            text = text.Replace("e.screenY-JSEvents.previousScreenY;", "e.screenY*devicePixelRatio-JSEvents.previousScreenY;");
            text = text.Replace("JSEvents.previousScreenY=e.screenY}", "JSEvents.previousScreenY=e.screenY*devicePixelRatio}");

            text = text.Replace("HEAP32[eventStruct+60>>2]=e.clientX-rect.left;", "HEAP32[eventStruct+60>>2]=(e.clientX-rect.left)*devicePixelRatio;");
            text = text.Replace("HEAP32[eventStruct+52>>2]=e.clientX-rect.left;", "HEAP32[eventStruct+52>>2]=(e.clientX-rect.left)*devicePixelRatio;");

            text = text.Replace("HEAP32[eventStruct+64>>2]=e.clientY-rect.top}", "HEAP32[eventStruct+64>>2]=(e.clientY-rect.top)*devicePixelRatio}");
            text = text.Replace("HEAP32[eventStruct+56>>2]=e.clientY-rect.top}", "HEAP32[eventStruct+56>>2]=(e.clientY-rect.top)*devicePixelRatio}");
            File.WriteAllText(TARGET_PATH, text);
            Debug.Log(string.Format("{0} Completed!", TARGET_PATH));
        }

        static void ReplaceIndex()
        {
            if (!File.Exists(INDEX_PATH))
            {
                Debug.LogError(INDEX_PATH + " is not existing!");
                return;
            }
            string text = File.ReadAllText(INDEX_PATH);
            string msg = "<style>canvas{width:" + WIDTH + "px;height:" + HEIGHT + "px;position:;transform:scale(0.5);transform-origin: 0 0;}</style>";
            //remove the old canvas style.
            if (text.IndexOf("<style>canvas") != -1)
            {
                int startIndex = text.IndexOf("<style>canvas");
                int endIndex = text.IndexOf("</style>", startIndex) + "</style>".Length;
                text = text.Remove(startIndex, endIndex - startIndex);
            }
            //write the canvas style;
            string matchText = "</title>";
            int insertIndex = text.IndexOf(matchText) + matchText.Length;
            text = text.Insert(insertIndex, msg);
            //replace the game container 
            if (text.IndexOf("<div id=\"gameContainer\"") != -1)
            {
                int startIndex = text.IndexOf("<div id=\"gameContainer\"");
                int endIndex = text.IndexOf("></div>") + "></div>".Length;
                text = text.Remove(startIndex, endIndex - startIndex);
                string gameContainerDiv = "<div id=\"gameContainer\" style=\"width: " + WIDTH / 2 + "px; height: " + HEIGHT / 2 + "px\"></div>";
                text = text.Insert(startIndex, gameContainerDiv);
            }
            File.WriteAllText(INDEX_PATH, text);
            Debug.Log(string.Format("{0} Completed!", INDEX_PATH));
        }
    }
}
