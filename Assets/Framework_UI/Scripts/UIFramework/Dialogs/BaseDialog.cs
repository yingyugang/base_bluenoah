using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using BlueNoah.Utility;

namespace BlueNoah.UI
{
	public class BaseDialog : MonoBehaviour
	{
        public GameObject containerTitleBar;

		public UnityAction onClose;

		public Button btnClose;

		public Text txtTitle;

		public UIDialogManager uiDialogManager;

        public UnityAction onReturn;

        public UnityAction onBack;

		protected virtual void Awake ()
		{
			if (btnClose != null) {
				btnClose.onClick.AddListener (() => {
					Close();
				});
			}
		}

		public virtual void Transmit(Hashtable param){
		
		}

        public virtual void SetSize(Vector2 size)
        {
            RectTransform rect = GetComponent<RectTransform>();
            rect.sizeDelta = size;
        }

		public void Close ()
		{
			if (onClose != null) {
				onClose ();
			}
			CanvasGroup cg = gameObject.GetOrAddComponent<CanvasGroup> ();
			cg.DOFade (0.0f,0.3f).OnComplete(()=>{
				UIManager.Instance.uiDialogManager.CloseDialog (this);
			});
		}

		bool mIsBackable = true;

		public bool isBackable{
			set{
				mIsBackable = value;
			}
		}
        // return able when ESC click.
		protected virtual bool CheckReturnable(){
			return false;
		}
        // back able when Return click.
		protected virtual bool CheckBackable(){
			return IsButtonActive (btnClose) && mIsBackable;
		}
		//will be called by pc's back button.
		//will be called by android's back button.
		public bool OnBack(){
			if(CheckBackable()){
                if (onBack != null)
                    onBack();
				Close ();
				return true;
			}
			return false;
		}

		public bool OnReturn(){
			if(CheckReturnable()){
                if (onReturn != null)
                    onReturn();
				Close ();
				return true;
			}
			return false;
		}

		protected bool IsButtonActive(Button btn){
			return btn != null && btn.IsActive () && btn.interactable && btn.targetGraphic.raycastTarget;
		}

		public System.Object GetSession(string param){
			return uiDialogManager.GetSession (param);
		}

		public void SetSession(string param,System.Object obj){
			uiDialogManager.SetSession (param,obj);
		}
	}
}
