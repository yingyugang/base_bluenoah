using UnityEngine.UI;

namespace BlueNoah.UI
{
	public class Sample2Ctrl : BasePanelCtrl
	{
		Sample2View mSample2View;
		
		protected override void Awake ()
		{
			base.Awake ();
			mSample2View = (Sample2View)this.mBasePanelView;
            Button[] buttons = mSample2View.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onClick.AddListener(() => {
                    UIManager.Instance.uiPanelManager.OpenPanel<Sample1Ctrl>();
                });
            }
		}
	}
}
