using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basic separate the views and controlls. 
public class BasePanelView : MonoBehaviour {

	#if UNITY_EDITOR
	[SerializeField]
	protected bool load;
	[SerializeField]
	protected bool loadPath;
	#endif

	//for load ui component at editor
	protected virtual void Update(){
		#if UNITY_EDITOR
		if(load){
			load = false;
			LoadUIs ();
		}
		if(loadPath){
			loadPath = false;
			LoadPaths ();
		}
		#endif
	}

	//不用也没关系
	//no matter wether used
	public virtual void LoadUIs(){

	}

	//不用也没关系
	//no matter wether used
	public virtual void LoadPaths(){
	
	}

	public void GetGameObjectPath(Component comp,ref string resultPath)
	{
		if (comp == null)
			return;
		string path = "/" + comp.gameObject.name;
		Transform trans = this.transform;
		GameObject obj = comp.gameObject;
		while (obj.transform.parent != null)
		{
			if (trans == obj.transform.parent) {
				break;
			}
			obj = obj.transform.parent.gameObject;
			path = "/" + obj.name + path;
		}
		path = path.Remove(0,1);
		resultPath = path;
	}
}
