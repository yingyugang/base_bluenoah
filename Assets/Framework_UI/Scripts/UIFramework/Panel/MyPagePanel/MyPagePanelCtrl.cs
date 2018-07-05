using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	public class MyPagePanelCtrl : BasePanelCtrl
	{
		MyPagePanelView mMyPagePanelView;
		
		protected override void Awake ()
		{
			base.Awake ();
			mMyPagePanelView = (MyPagePanelView)this.mBasePanelView;
		}


		private void Update()
		{
            if(Input.GetKeyDown(KeyCode.H)){
                UIManager.Instance.uiPanelManager.OpenPanel<TestPanelCtrl>();
            }
		}
	}
}
