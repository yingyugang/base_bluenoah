using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BlueNoah.UI
{
	public class CommonDialog : BaseDialog
	{
		public Button btn_ok;

		public UnityAction onOk;

		public Text txt_msg;

		protected override void Awake ()
		{
			base.Awake ();
			if (btn_ok != null) {
				btn_ok.onClick.AddListener (() => {
					OnOkClick();
				});
			}
		}

		public void Show(string title,string msg,UnityAction ok,bool isShowClose){
			this.txt_title.text = title;
			this.txt_msg.text = msg;
			this.onOk = ok;
			this.btn_close.gameObject.SetActive (isShowClose);
		}

		protected override bool CheckReturnable ()
		{
			return IsButtonActive (btn_ok);
		}

		public override bool OnReturn(){
			if(CheckReturnable()){
				OnOkClick ();
				return true;
			}
			return false;
		}

		void OnOkClick(){
			if (onOk != null) {
				onOk ();
			}
			Close();
		}

	}
}
