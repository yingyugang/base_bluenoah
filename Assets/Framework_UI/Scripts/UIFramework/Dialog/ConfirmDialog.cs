using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BlueNoah.UI
{
	public class ConfirmDialog : CommonDialog
	{
		public Button btn_cancel;

		public UnityAction onCancel;

		protected override void Awake ()
		{
			base.Awake ();
			if (btn_cancel != null) {
				btn_cancel.onClick.AddListener (() => {
					if(this.onCancel!=null){
						this.onCancel();
					}
					Close();
				});
			}
		}

		public void Show(string msg,UnityAction onOk,UnityAction onCancel,bool isShowClose){
			this.txt_msg.text = msg;
			this.onOk = onOk;
			this.onCancel = onCancel;
			this.btn_close.gameObject.SetActive (isShowClose);
		}

		protected override bool CheckBackable ()
		{
			return base.CheckBackable () || IsButtonActive (btn_cancel) ;
		} 

	}
}
