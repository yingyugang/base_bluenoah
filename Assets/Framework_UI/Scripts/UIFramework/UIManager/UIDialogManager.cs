using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIDialogManager
{
	//TODO
	//1.in front of the page(no mask , close when panel close)
	//2.on the mask layer.(mask , can not do anything before close it)
	public T OpenDialog<T> (Hashtable param = null, UnityAction<GameObject> onShow = null) where T : BaseDialog
	{
		//Load the Dialog
		//Show the Dialog
		return null;
	}

	public void ShowCommonDialog (string msg, UnityAction onOk, bool showClose)
	{
		
	}

	public void ShowConfirmDialog (string msg, UnityAction onOk, UnityAction onCancel, bool showClose)
	{
	
	}

	public void Close(BaseDialog baseDialog){
		if(baseDialog!=null){
			GameObject.Destroy (baseDialog.gameObject);
		}
	}
}
