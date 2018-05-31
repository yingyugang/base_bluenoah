/// <summary>
/// Description：
/// Auto set the webgl release to adapt the Retina mac browser. 
/// Author：應　彧剛（yingyugang@gmail.com）
/// Create time：2018-05-30 17:26:56
/// </summary>
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameEngine.IO;

public class WebGLRetinaReplace {

	//change the path as the file path on your computer.(xxxxxx.asm.framework.unityweb)
	//自分のパソコンのこのファイルのパッスで差し替えてくれ。
	const string TARGET_PATH = "/Users/yingyugang/ultrasoul-client/ultrasoul/Build/ultrasoul.asm.framework.unityweb";

	//change the path as the file path on your computer.(index.html)
	//自分のパソコンのこのファイルのパッスで差し替えてくれ。
	const string INDEX_PATH = "/Users/yingyugang/ultrasoul-client/ultrasoul/index.html";

	const int WIDTH = 2436;

	const int HEIGHT = 1125;

	[MenuItem("Tools/WebGL-Retina")]
	public static void ReplaceForRetina(){
		ReplaceKeyWords ();
		ReplaceIndex ();
	}

	static void ReplaceKeyWords(){
		if (!FileManage.Exists (TARGET_PATH)) {
			Debug.LogError ( TARGET_PATH + " is not existing!");
			return;
		}
		string text = FileManage.ReadString (TARGET_PATH);
		text = text.Replace ("HEAP32[eventStruct+8>>2]=e.screenX;","HEAP32[eventStruct+8>>2]=e.screenX*devicePixelRatio;");
		text = text.Replace ("e.screenX-JSEvents.previousScreenX;","e.screenX*devicePixelRatio-JSEvents.previousScreenX;");
		text = text.Replace ("JSEvents.previousScreenX=e.screenX;","JSEvents.previousScreenX=e.screenX*devicePixelRatio;");

		text = text.Replace ("HEAP32[eventStruct+12>>2]=e.screenY;","HEAP32[eventStruct+12>>2]=e.screenY*devicePixelRatio;");
		text = text.Replace ("e.screenY-JSEvents.previousScreenY;","e.screenY*devicePixelRatio-JSEvents.previousScreenY;");
		text = text.Replace ("JSEvents.previousScreenY=e.screenY}","JSEvents.previousScreenY=e.screenY*devicePixelRatio}");

		text = text.Replace ("HEAP32[eventStruct+60>>2]=e.clientX-rect.left;","HEAP32[eventStruct+60>>2]=(e.clientX-rect.left)*devicePixelRatio;");
		text = text.Replace ("HEAP32[eventStruct+52>>2]=e.clientX-rect.left;","HEAP32[eventStruct+52>>2]=(e.clientX-rect.left)*devicePixelRatio;");

		text = text.Replace ("HEAP32[eventStruct+64>>2]=e.clientY-rect.top}","HEAP32[eventStruct+64>>2]=(e.clientY-rect.top)*devicePixelRatio}");
		text = text.Replace ("HEAP32[eventStruct+56>>2]=e.clientY-rect.top}","HEAP32[eventStruct+56>>2]=(e.clientY-rect.top)*devicePixelRatio}");

		FileManage.WriteString (TARGET_PATH,text);
		Debug.Log (string.Format("{0} Completed!",TARGET_PATH));
	}

	static void ReplaceIndex(){
		if (!FileManage.Exists (INDEX_PATH)) {
			Debug.LogError ( INDEX_PATH + " is not existing!");
			return;
		}
		string text = FileManage.ReadString (INDEX_PATH);
		string msg = "<style>canvas{width:" + WIDTH + "px;height:" + HEIGHT + "px;position:;transform:scale(0.5);transform-origin: 0 0;}</style>";
		//remove the old canvas style.
		if(text.IndexOf("<style>canvas")!=-1){
			int startIndex = text.IndexOf ("<style>canvas");
			int endIndex = text.IndexOf ("</style>",startIndex) + "</style>".Length;
			text = text.Remove (startIndex,endIndex-startIndex);
		}
		//write the canvas style;
		string matchText = "</title>";
		int insertIndex = text.IndexOf (matchText) + matchText.Length;
		text = text.Insert (insertIndex,msg);
		//replace the game container 
		if(text.IndexOf("<div id=\"gameContainer\"")!=-1){
			int startIndex = text.IndexOf ("<div id=\"gameContainer\"");
			int endIndex = text.IndexOf ("></div>") + "></div>".Length;
			text = text.Remove (startIndex,endIndex-startIndex);
			string gameContainerDiv = "<div id=\"gameContainer\" style=\"width: " + WIDTH / 2 + "px; height: " + HEIGHT / 2 + "px\"></div>";
			text = text.Insert (startIndex,gameContainerDiv);
		}
		FileManage.WriteString (INDEX_PATH,text);
		Debug.Log (string.Format("{0} Completed!",INDEX_PATH));
	}

}
