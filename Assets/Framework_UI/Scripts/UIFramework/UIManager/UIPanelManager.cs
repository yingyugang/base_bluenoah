/**********************************************
 *
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BlueNoah.Utility;

namespace BlueNoah.UI
{
    public class UIPanelManager
    {
        public UIManager uiManager;

        private const float WHITE_FLASH_DURATION = 2f;

        private Dictionary<string, PanelConfigItem> mConfigItemDic = new Dictionary<string, PanelConfigItem>();

        private Dictionary<string, GameObject> mPanelPrefabs = new Dictionary<string, GameObject>();

        private List<System.Type> mPageQueue = new List<System.Type>();

        private GameObject mCurrentPanel;

        private System.Type mCurrentPanelType;

        private bool mIsOpening;

        public static event UnitEventHandler LoadPrefab;

        public UIPanelManager(UIManager uiManager)
        {
            this.uiManager = uiManager;
            LoadConfig(uiManager.uiSettings.PANEL_CONFIG_FILE);
        }

        private void LoadConfig(TextAsset config)
        {
            if (config != null)
            {
                PanelConfig panelConfig = JsonUtility.FromJson<PanelConfig>(config.text);
                LoadConfigToDic(panelConfig);
            }
        }

        private void LoadConfigToDic(PanelConfig panelConfig)
        {
            for (int i = 0; i < panelConfig.items.Count; i++)
            {
                PanelConfigItem item = panelConfig.items[i];
                mConfigItemDic.Add(item.ctrlClassName, item);
            }
        }

        private GameObject GetPrefab(string classType)
        {
            GameObject panelPrefab = null;
            if (mConfigItemDic.ContainsKey(classType))
            {
                if (mPanelPrefabs.ContainsKey(classType))
                {
                    panelPrefab = mPanelPrefabs[classType];
                }
                else
                {
                    panelPrefab = GetNewPrefab(classType);
                }
            }
            return panelPrefab;
        }

        private GameObject GetNewPrefab(string classType)
        {
            PanelConfigItem item = mConfigItemDic[classType];
            GameObject panelPrefab = null;
            if (LoadPrefab != null)
            {
                panelPrefab = LoadPrefab(item.prefabPath);
                if (panelPrefab != null)
                    mPanelPrefabs.Add(classType, panelPrefab);
            }
            return panelPrefab;
        }

        private bool CheckOpenable()
        {
            return !mIsOpening;
        }

        private void EnableOpen()
        {
            mIsOpening = false;
        }

        private void DisableOpen()
        {
            mIsOpening = true;
        }

        public void OpenPanel<T>(Hashtable param = null, UnityAction<GameObject> onShow = null) where T : BasePanelCtrl
        {
            if (!CheckOpenable())
            {
                return;
            }
            if (mCurrentPanelType == typeof(T))
            {
                return;
            }
            OpenPageWithFlash(typeof(T), true, param, onShow);
        }

        private GameObject ShowPanel(System.Type type, bool isNew, Hashtable param, UnityAction<GameObject> onShow)
        {
            GameObject go = CreatePanel(type);
            uiManager.AddToLayer(go, BlueNoah.UI.UIManager.UILayerNames.UILayer_Common);
            SetNewCurrentPage(go, type);
            SetDeltaSize(go);
            ApendController(go, type, param);
            go.SetActive(true);
            if (isNew)
                mPageQueue.Add(type);
            if (onShow != null)
                onShow(go);
            return go;
        }

        private GameObject CreatePanel(System.Type type)
        {
            GameObject prefab = GetPrefab(type.ToString());
            Debug.Log(string.Format("CreatePanel {0}",type.ToString()));
            //to let the components call the awake after Init datas.
            prefab.SetActive(false);
            GameObject go = GameObject.Instantiate(prefab);
            return go;
        }

        private void SetNewCurrentPage(GameObject go, System.Type type)
        {
            if (mCurrentPanel != null)
            {
                GameObject.Destroy(mCurrentPanel);
            }
            mCurrentPanel = go;
            mCurrentPanelType = type;
        }

        private void SetDeltaSize(GameObject go)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(UIManager.CANVAS_WIDTH, UIManager.CANVAS_HEIGHT);
        }

        private void ApendController(GameObject go, System.Type type, Hashtable param)
        {
            go.GetOrAddComponent(type);
            go.GetComponent<BasePanelCtrl>().uiPanelManager = this;
            go.GetComponent<BasePanelCtrl>().InitData(param);
        }

        public void OnBack()
        {
            //disable when the page changing.
            if (!CheckOpenable())
                return;
            if (mPageQueue.Count > 1)
            {
                //remove current page from queue.
                mPageQueue.RemoveAt(mPageQueue.Count - 1);
                System.Type type = mPageQueue[mPageQueue.Count - 1];
                OpenPageWithFlash(type, false);
            }
        }

        private void OpenPageWithFlash(System.Type type, bool isNew = true, Hashtable param = null, UnityAction<GameObject> onShow = null)
        {
            DisableCurrentPanel();
            DisableOpen();
            UICommonWhiteOutInMask.Instance.DoFlash(WHITE_FLASH_DURATION, () => {
                ShowPanel(type, isNew, param, onShow);
            }, () => {
                EnableOpen();
            });
        }

        void DisableCurrentPanel()
        {
            if (mCurrentPanel != null)
            {
                CanvasGroup cg = mCurrentPanel.GetOrAddComponent<CanvasGroup>();
                cg.blocksRaycasts = false;
            }
        }

        public System.Object GetSession(string param)
        {
            return uiManager.GetSession(param);
        }

        public void SetSession(string param, System.Object obj)
        {
            uiManager.SetSession(param, obj);
        }
    }
}