using BlueNoah.IO;
using BlueNoah.UI;
using UnityEngine;

namespace BlueNoah
{
    public class Sample1Scene : MonoBehaviour
    {
		private void Awake()
		{
            UIPanelManager.LoadPrefab += ResourcesLoader.LoadPanelPrefab;
            UIDialogManager.LoadPrefab += ResourcesLoader.LoadDialogPrefab;
		}

		void Start()
        {
            UIManager.Instance.uiPanelManager.OpenPanel<Sample1Ctrl>();
        }

		private void OnDestroy()
		{
            UIPanelManager.LoadPrefab -= ResourcesLoader.LoadPanelPrefab;
            UIDialogManager.LoadPrefab -= ResourcesLoader.LoadDialogPrefab;
		}
	}
}
