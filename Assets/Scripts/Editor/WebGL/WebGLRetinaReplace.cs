/// <summary>
/// Description：
/// Auto set the webgl release to adapt the Retina mac browser. 
/// Author：應　彧剛（yingyugang@gmail.com）
/// Create time：2018-05-30 17:26:56
/// </summary>
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build;
using System.Xml.Linq;
using BlueNoah.IO;
using UnityEditor.Build.Reporting;

public class WebGLRetinaReplace : EditorWindow, IPreprocessBuildWithReport
{
    public static string RootPath = "";
    public static string FolderName = "";

    public static string FolderPath { get { return RootPath + FolderName + "/"; } }
#if UNITY_2018
    public static string TARGET_PATH { get { return FolderPath + "Build/" + FolderName + ".wasm.framework.unityweb"; } }
#else
    public static string TARGET_PATH { get { return FolderPath + "Build/" + FolderName + ".asm.framework.unityweb"; } }
#endif
    public static string INDEX_PATH { get { return FolderPath + "index.html"; } }

	public static int WIDTH = 0;
	public static int HEIGHT = 0;

	[MenuItem("Tools/BlueNoah/WebGL/WebGL-Retina")]
	static void Open ()
	{
		EditorWindow.GetWindow<WebGLRetinaReplace> ("WebGL-Retina");
	}

	void OnGUI ()
	{
		GUILayout.Space (30);
#if !UNITY_WEBGL
		GUILayout.Label("Not Build Settings WebGL");
		GUILayout.Space(10);
#endif
		if (GUILayout.Button ("Set Folder Path")) {
			string path = EditorUtility.OpenFolderPanel ("Build WebGLFolder", "", "");
			if (SetPaths (path))
				SetDefaultScreenSize ();
			else
				return;
		}

		if (string.IsNullOrEmpty (RootPath))
			return;

		GUILayout.Label ("Root Path : " + RootPath);
		GUILayout.Label ("FolderName : " + FolderName);

		GUILayout.Space (10);
		GUILayout.Label ("WindowSize");
		WIDTH = EditorGUILayout.IntField ("Width : ", WIDTH);
		HEIGHT = EditorGUILayout.IntField ("Height : ", HEIGHT);
		
		if (WIDTH < 1 || HEIGHT < 1)
			return;
		
		if (!FileManager.Exists (TARGET_PATH)) {
			GUILayout.Label ("is not existing! for Target WebGL Folder.");
			return;
		}

		GUILayout.Space (10);
		if (GUILayout.Button ("Retina")) {
			ReplaceForRetina ();
		}
	}

	static bool SetPaths (string path)
	{
		List<string> paths = path.Split ('/').ToList ();
		if (paths.Count < 2) {
			Debug.LogError ("Web GL Builds Not Founds Files.  Path:" + path);
			return false;
		}

		FolderName = paths.Last ();

		paths.RemoveAt (paths.Count - 1);
		RootPath = string.Join ("/", paths.ToArray ()) + "/";
		return true;
	}

	static void SetDefaultScreenSize ()
	{
		if (WIDTH < 1 || HEIGHT < 1) {
			WIDTH = UnityEditor.PlayerSettings.defaultWebScreenWidth;
			HEIGHT = UnityEditor.PlayerSettings.defaultWebScreenHeight;
		}
	}

	public int callbackOrder { get { return 0; } }

	[PostProcessBuild (100)]
	public static void OnPostProcessBuild (BuildTarget target, string path)
	{
		Debug.Log ("BuildPath : " + path);
		if (target == BuildTarget.WebGL) {
			PlayerSettings.SetApiCompatibilityLevel (BuildTargetGroup.WebGL, ApiCompatibilityLevel.NET_2_0);
			if (SetPaths (path)) {
				SetDefaultScreenSize ();
				ReplaceForRetina ();
			}
		}
	}

	public static void ReplaceForRetina ()
	{
		ReplaceKeyWords ();
		ReplaceIndex ();
	}

	static void ReplaceKeyWords ()
	{
		if (!FileManager.Exists (TARGET_PATH)) {
			Debug.LogError (TARGET_PATH + " is not existing!");
			return;
		}
		string text = FileManager.ReadString (TARGET_PATH);
		text = text.Replace ("HEAP32[eventStruct+8>>2]=e.screenX;", "HEAP32[eventStruct+8>>2]=e.screenX*devicePixelRatio;");
		text = text.Replace ("e.screenX-JSEvents.previousScreenX;", "e.screenX*devicePixelRatio-JSEvents.previousScreenX;");
		text = text.Replace ("JSEvents.previousScreenX=e.screenX;", "JSEvents.previousScreenX=e.screenX*devicePixelRatio;");
		text = text.Replace ("HEAP32[eventStruct+12>>2]=e.screenY;", "HEAP32[eventStruct+12>>2]=e.screenY*devicePixelRatio;");
		text = text.Replace ("e.screenY-JSEvents.previousScreenY;", "e.screenY*devicePixelRatio-JSEvents.previousScreenY;");

		text = text.Replace ("JSEvents.previousScreenY=e.screenY}", "JSEvents.previousScreenY=e.screenY*devicePixelRatio}");
		text = text.Replace ("HEAP32[eventStruct+60>>2]=e.clientX-rect.left;", "HEAP32[eventStruct+60>>2]=(e.clientX-rect.left)*devicePixelRatio;");
		text = text.Replace ("HEAP32[eventStruct+52>>2]=e.clientX-rect.left;", "HEAP32[eventStruct+52>>2]=(e.clientX-rect.left)*devicePixelRatio;");
		text = text.Replace ("HEAP32[eventStruct+64>>2]=e.clientY-rect.top}", "HEAP32[eventStruct+64>>2]=(e.clientY-rect.top)*devicePixelRatio}");
		text = text.Replace ("HEAP32[eventStruct+56>>2]=e.clientY-rect.top}", "HEAP32[eventStruct+56>>2]=(e.clientY-rect.top)*devicePixelRatio}");

		FileManager.WriteString (TARGET_PATH, text);
		Debug.Log (string.Format ("{0} Completed!", TARGET_PATH));
	}

	static void ReplaceIndex ()
	{
		if (!FileManager.Exists (INDEX_PATH)) {
			Debug.LogError (INDEX_PATH + " is not existing!");
			return;
		}

		string text = FileManager.ReadString (INDEX_PATH);
		string msg = "<style>canvas{width:" + WIDTH + "px;height:" + HEIGHT + "px;position:absolute;transform:scale(0.5);transform-origin: 0 0;}</style>";
		//remove the old canvas style.
		if (text.IndexOf ("<style>canvas",StringComparison.CurrentCulture) != -1) {
            int startIndex = text.IndexOf ("<style>canvas", StringComparison.CurrentCulture);
            int endIndex = text.IndexOf ("</style>", startIndex, StringComparison.CurrentCulture) + "</style>".Length;
			text = text.Remove (startIndex, endIndex - startIndex);
		}
		//write the canvas style;
		string matchText = "</title>";
        int insertIndex = text.IndexOf (matchText, StringComparison.CurrentCulture) + matchText.Length;
		text = text.Insert (insertIndex, msg);
		//replace the game container 
        if (text.IndexOf ("<div id=\"gameContainer\"", StringComparison.CurrentCulture) != -1) {
            int startIndex = text.IndexOf ("<div id=\"gameContainer\"", StringComparison.CurrentCulture);
            int endIndex = text.IndexOf ("></div>", StringComparison.CurrentCulture) + "></div>".Length;
			text = text.Remove (startIndex, endIndex - startIndex);
			string gameContainerDiv = "<div id=\"gameContainer\" style=\"width: " + WIDTH / 2 + "px; height: " + HEIGHT / 2 + "px\"></div>";
			text = text.Insert (startIndex, gameContainerDiv);
		}
		FileManager.WriteString (INDEX_PATH, text);
		Debug.Log (string.Format ("{0} Completed!", INDEX_PATH));
	}

//	[MenuItem ("GameEngine/Tools/Load-Index.html")]
	public static void LoadIndexHtml(){
//		System.Windows.Forms.HtmlDocument ddd;
		XElement indexFile = XElement.Load("/Users/yingyugang/ultrasoul-client/ultrasoul_normal/index.html"); 
		Debug.Log (indexFile.ToString());
	}

    public void OnPreprocessBuild(BuildReport report)
    {
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
    }

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
    }
}
