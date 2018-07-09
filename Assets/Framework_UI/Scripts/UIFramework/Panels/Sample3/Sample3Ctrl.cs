using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	public class Sample3Ctrl : BasePanelCtrl
	{
		Sample3View mSample3View;
		
		protected override void Awake ()
		{
			base.Awake ();
			mSample3View = (Sample3View)this.mBasePanelView;
            mSample3View.btn_back.onClick.AddListener(()=>{
                UnityEngine.SceneManagement.SceneManager.LoadScene("Sample1");
            });
		}

        public override void Transmit(Hashtable parameters)
        {
            base.Transmit(parameters);
        }
	}
}
