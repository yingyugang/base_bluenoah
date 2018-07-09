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

		//this function will be run before awake.
        //use to send data from outside to this ctrl.
        public virtual void Transmit (Hashtable parameters)
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
