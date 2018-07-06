using BlueNoah.Assets;
using UnityEngine;

namespace BlueNoah.UI
{
    public class Sample2Scene : MonoBehaviour
    {
		private void Awake()
		{
            UIPanelManager.LoadPrefab += AssetsManager.LoadPanelPrefab;
            UIDialogManager.LoadPrefab += AssetsManager.LoadDialogPrefab;
		}

		void Start()
        {
            UIManager.Instance.uiPanelManager.OpenPanel<Sample3Ctrl>();
        }

        private void OnDestroy()
        {
            UIPanelManager.LoadPrefab -= AssetsManager.LoadPanelPrefab;
            UIDialogManager.LoadPrefab -= AssetsManager.LoadDialogPrefab;
        }
    }
}
