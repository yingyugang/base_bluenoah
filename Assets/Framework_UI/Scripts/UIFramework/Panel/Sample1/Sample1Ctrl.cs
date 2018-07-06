using UnityEngine.UI;
using DG.Tweening;

namespace BlueNoah.UI
{
	public class Sample1Ctrl : BasePanelCtrl
	{
		Sample1View mSample1View;
		
		protected override void Awake ()
		{
			base.Awake ();
			mSample1View = (Sample1View)this.mBasePanelView;
            Button[] buttons = mSample1View.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onClick.AddListener(() => {
                    UIManager.Instance.uiPanelManager.OpenPanel<Sample2Ctrl>();
                });
            }
            mSample1View.btn_battle.onClick.RemoveAllListeners();
            mSample1View.btn_battle.onClick.AddListener(()=>{
                UnityEngine.SceneManagement.SceneManager.LoadScene("Sample2");
            });
            mSample1View.container_arrow.DOFade(0, 0.1f).SetLoops(-1, LoopType.Yoyo);
		}
	}
}
