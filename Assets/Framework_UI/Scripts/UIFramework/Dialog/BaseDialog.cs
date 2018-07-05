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
		public UnityAction onClose;

		public Button btn_close;

		public Text txt_title;

		public UIDialogManager uiDialogManager;

		protected virtual void Awake ()
		{
			if (btn_close != null) {
				btn_close.onClick.AddListener (() => {
					Close();
				});
			}
		}

		public virtual void InitData(Hashtable param){
		
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

		protected virtual bool CheckReturnable(){
			return false;
		}

		protected virtual bool CheckBackable(){
			return IsButtonActive (btn_close) && mIsBackable;
		}

		//will be called by pc's back button.
		//will be called by android's back button.
		public bool OnBack(){
			if(CheckBackable()){
				Close ();
				return true;
			}
			return false;
		}

		public virtual bool OnReturn(){
			if(CheckReturnable()){
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
