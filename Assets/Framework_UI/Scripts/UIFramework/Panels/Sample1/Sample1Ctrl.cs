using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

namespace BlueNoah.UI
{
	public class Sample1Ctrl : BasePanelCtrl
	{
		Sample1View mSample1View;
		
		protected override void Awake ()
		{
			base.Awake ();
			mSample1View = (Sample1View)this.mBasePanelView;
            Init();
		}

        void Init(){
            SetListeners();
            DisplayDatas();
            InitTips();
        }

        void SetListeners(){
            Button[] buttons = mSample1View.GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].onClick.AddListener(() => {
                    UIManager.Instance.uiPanelManager.OpenPanel<Sample2Ctrl>();
                });
            }
            mSample1View.btnSound.onClick.RemoveAllListeners();
			mSample1View.btnSound.onClick.AddListener((UnityEngine.Events.UnityAction)(()=>{
				UIManager.Instance.uiDialogManager.OpenDialog<CommonDialog>();
            }));

            mSample1View.btnSetting.onClick.RemoveAllListeners();
            mSample1View.btnSetting.onClick.AddListener((UnityEngine.Events.UnityAction)(() => {
                UIManager.Instance.uiDialogManager.OpenDialogWithoutMask<CommonDialog>();
            }));

            mSample1View.btnBattle.onClick.RemoveAllListeners();
            mSample1View.btnBattle.onClick.AddListener(() => {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Sample2");
            });
        }

        void DisplayDatas(){
            Sample1Proxy proxy = UIManager.Instance.uIModelManager.Get<Sample1Proxy>();
            mSample1View.txtUserName.text = proxy.userName;
            mSample1View.txtUserLevel.text = proxy.userLevel.ToString();
            mSample1View.txtUserGold.text = proxy.userCoin.ToString();
            mSample1View.txtUserGem.text = proxy.userGem.ToString();
            mSample1View.imgUserExp.fillAmount = proxy.userExp;
        }

        void InitTips(){
            mSample1View.container_arrow.DOFade(0, 0.1f).SetLoops(-1, LoopType.Yoyo);
        }
	}
}
