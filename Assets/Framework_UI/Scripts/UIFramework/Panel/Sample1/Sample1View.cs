using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlueNoah.UI
{
	[ExecuteInEditMode]
	public class Sample1View : BasePanelView
	{
		Sample1View mSample1View;

        public Button btn_battle;

        public CanvasGroup container_arrow;
		
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