using BlueNoah.Assets;
using BlueNoah.UI;
using UnityEngine;

namespace BlueNoah
{
    public class Sample1Scene : MonoBehaviour
    {

		private void Awake()
		{
            UIPanelManager.LoadPrefab += AssetsManager.LoadPanelPrefab;
            UIDialogManager.LoadPrefab += AssetsManager.LoadDialogPrefab;
		}

		void Start()
        {
            UIManager.Instance.uiPanelManager.OpenPanel<Sample1Ctrl>();
        }

		private void OnDestroy()
		{
            UIPanelManager.LoadPrefab -= AssetsManager.LoadPanelPrefab;
            UIDialogManager.LoadPrefab -= AssetsManager.LoadDialogPrefab;
		}
	}
}
