using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO
public static class ResourcesManager {

	//TODO
	public static GameObject LoadPanelPrefab(string path){
		path = FileManager.AssetDataPathToResourcesPath (path);
		Debug.Log (path);
		return Resources.Load<GameObject>(path);
	}

	//TODO
	public static GameObject LoadDialogPrefab(string path){
		return Resources.Load<GameObject> (path);
	}

}
