using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	public class TestPanelCtrl : BasePanelCtrl
	{
		TestPanelView mTestPanelView;
		
		protected override void Awake ()
		{
			base.Awake ();
			mTestPanelView = (TestPanelView)this.mBasePanelView;
		}
	}
}
