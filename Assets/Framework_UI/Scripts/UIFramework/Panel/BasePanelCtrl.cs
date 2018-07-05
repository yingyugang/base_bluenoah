/**********************************************
 *
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	public class BasePanelCtrl : MonoBehaviour
	{
		protected BasePanelView mBasePanelView;

		public UIPanelManager uiPanelManager;

		protected virtual void Awake ()
		{
			mBasePanelView = GetComponent<BasePanelView> ();
		}

		//this function will be run after awake.
		//TODO better run before awake.
		public virtual void InitData (Hashtable parameters)
		{
			
		}

		protected System.Object GetSession(string param){
			return uiPanelManager.GetSession (param);
		}

		protected void SetSession(string param,System.Object obj){
			uiPanelManager.SetSession (param,obj);
		}
	}
}
