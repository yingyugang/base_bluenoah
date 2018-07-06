using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	public class HomeTestCtrl : BasePanelCtrl
	{
		HomeTestView mHomeTestView;
		
		protected override void Awake ()
		{
			base.Awake ();
			mHomeTestView = (HomeTestView)this.mBasePanelView;
		}
	}
}
