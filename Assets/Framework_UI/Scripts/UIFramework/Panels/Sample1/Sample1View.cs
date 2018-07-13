using UnityEngine;
using UnityEngine.UI;
using BlueNoah.Utility;

namespace BlueNoah.UI
{
	[ExecuteInEditMode]
	public class Sample1View : BasePanelView
	{
		Sample1View mSample1View;

        public Button btnBattle;

        public Button btnSound;

        public Button btnSetting;

        public Image imgUserExp;

        public Text txtUserName;

        public Text txtUserLevel;

        public Text txtUserGold;

        public Text txtUserGem;

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
            btnBattle = transform.Find<Button>("container_right_middle/container_right_middle_btns/btn_battle");
            btnSound = transform.Find<Button>("container_bottom_right/container_bottom_right_btns/btn_sound");
            btnSetting = transform.Find<Button>("container_bottom_right/container_bottom_right_btns/btn_setting");
            txtUserName = transform.Find<Text>("container_left_top/container_left_top_btns/txt_username");
            txtUserLevel = transform.Find<Text>("container_left_top/container_left_top_btns/txt_level");
            imgUserExp = transform.Find<Image>("container_left_top/container_left_top_btns/img_exp");
            txtUserGold = transform.Find<Text>("container_right_top/container_right_top_btns/container_gold/txt_gold");
            txtUserGem = transform.Find<Text>("container_right_top/container_right_top_btns/container_gem/txt_gem");
		}
	}
}