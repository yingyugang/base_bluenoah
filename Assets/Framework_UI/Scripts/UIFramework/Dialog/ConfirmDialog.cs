using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmDialog : CommonDialog {

	public Button btn_cancel;

	protected override void Awake(){
		base.Awake ();
		if(btn_cancel!=null){
			btn_cancel.onClick.AddListener (()=>{
				if(onClose!=null){
					onClose();
				}
			});
		}
	}

}
