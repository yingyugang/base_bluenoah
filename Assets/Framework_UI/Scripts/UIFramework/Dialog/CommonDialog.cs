using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class CommonDialog : BaseDialog {

	public Button btn_ok;

	public UnityAction onOk;

	public Text txt_msg;

	protected override void Awake(){
		base.Awake ();
		if(btn_ok!=null){
			btn_ok.onClick.AddListener (()=>{
				if(onOk!=null){
					onOk ();
				}
			});
		}
	}

}
