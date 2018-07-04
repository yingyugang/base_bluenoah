using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanelCtrl : MonoBehaviour {

	protected BasePanelView mBasePanelView;

	protected virtual void Awake(){
		mBasePanelView = GetComponent<BasePanelView> ();
	}

	//this function will be run after awake. 
	//TODO better run before awake.
	public virtual void InitData(Hashtable parameters){
		
	}

}
