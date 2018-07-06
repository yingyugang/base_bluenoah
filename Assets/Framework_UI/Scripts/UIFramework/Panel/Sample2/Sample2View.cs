using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueNoah.UI
{
	[ExecuteInEditMode]
	public class Sample2View : BasePanelView
	{
		Sample2View mSample2View;

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