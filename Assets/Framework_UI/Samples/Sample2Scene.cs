using BlueNoah.IO;
using UnityEngine;

namespace BlueNoah.UI
{
    public class Sample2Scene : MonoBehaviour
    {
		private void Awake()
		{
            UIPanelManager.LoadPrefab += ResourcesLoader.LoadPanelPrefab;
            UIDialogManager.LoadPrefab += ResourcesLoader.LoadDialogPrefab;
		}

		void Start()
        {
            UIManager.Instance.uiPanelManager.OpenPanel<Sample3Ctrl>();
        }

        private void OnDestroy()
        {
            UIPanelManager.LoadPrefab -= ResourcesLoader.LoadPanelPrefab;
            UIDialogManager.LoadPrefab -= ResourcesLoader.LoadDialogPrefab;
        }
    }
}
