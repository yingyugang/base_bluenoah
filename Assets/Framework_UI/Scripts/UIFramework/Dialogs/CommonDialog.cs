using UnityEngine.UI;
using UnityEngine.Events;

namespace BlueNoah.UI
{
	public class CommonDialog : BaseDialog
	{
        public Button btnOk;

		public UnityAction onOk;

		public Text txtMsg;

		protected override void Awake ()
		{
			base.Awake ();
			if (btnOk != null) {
				btnOk.onClick.AddListener (() => {
					OnOkClick();
				});
			}
            this.onReturn = () => {
                if (onOk != null)
                {
                    onOk();
                }
            };
		}

		public void Show(string title,string msg,UnityAction ok,bool isShowClose){
			txtTitle.text = title;
			txtMsg.text = msg;
			onOk = ok;
			btnClose.gameObject.SetActive (isShowClose);
		}

		protected override bool CheckReturnable ()
		{
			return IsButtonActive (btnOk);
		}

		void OnOkClick(){
			if (onOk != null) {
				onOk ();
			}
			Close();
		}

	}
}
