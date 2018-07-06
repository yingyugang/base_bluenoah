using BlueNoah.Assets;
using BlueNoah.UI;
using UnityEngine;

namespace BlueNoah
{
    public class GameEntrance : MonoBehaviour
    {

		private void Awake()
		{
            Init();
            //UIManager.Instance.uiPanelManager.OpenPanel<HomeTestCtrl>();
		}

        void Init()
        {
            UIPanelManager.LoadPrefab += AssetsManager.LoadPanelPrefab;
            UIDialogManager.LoadPrefab += AssetsManager.LoadDialogPrefab;
        }

        void OnDestroy()
        {
            UIPanelManager.LoadPrefab -= AssetsManager.LoadPanelPrefab;
            UIDialogManager.LoadPrefab -= AssetsManager.LoadDialogPrefab;
        }
	}
}