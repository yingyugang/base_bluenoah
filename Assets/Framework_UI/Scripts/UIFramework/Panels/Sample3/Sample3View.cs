using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueNoah.UI
{
	[ExecuteInEditMode]
	public class Sample3View : BasePanelView
	{
		Sample3View mSample3View;

        public Button btn_back;
		
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