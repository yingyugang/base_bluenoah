using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BlueNoah.UI
{
	public class ConfirmDialog : CommonDialog
	{
        public Button btnCancel;

		public UnityAction onCancel;

		protected override void Awake ()
		{
			base.Awake ();
			if (btnCancel != null) {
				btnCancel.onClick.AddListener (() => {
					if(this.onCancel!=null){
						this.onCancel();
					}
					Close();
				});
			}
		}

		public void Show(string msg,UnityAction onOk,UnityAction onCancel,bool isShowClose){
			this.txtMsg.text = msg;
			this.onOk = onOk;
			this.onCancel = onCancel;
			this.btnClose.gameObject.SetActive (isShowClose);
		}

		protected override bool CheckBackable ()
		{
			return base.CheckBackable () || IsButtonActive (btnCancel) ;
		} 

	}
}
