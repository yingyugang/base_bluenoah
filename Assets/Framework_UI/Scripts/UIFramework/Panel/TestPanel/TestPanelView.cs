using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	[ExecuteInEditMode]
	public class TestPanelView : BasePanelView
	{
		TestPanelView mTestPanelView;
		
		void Awake ()
		{
			LoadUIs ();
		}

		public override void LoadPaths ()
		{
			base.LoadPaths ();
		}

		public override void LoadUIs ()
		{
			base.LoadUIs ();
		}
	}
}