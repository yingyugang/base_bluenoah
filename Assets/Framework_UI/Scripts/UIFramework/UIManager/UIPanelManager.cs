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

        const float WHITE_FLASH_DURATION = 2f;

        Dictionary<string, PanelConfigItem> mConfigItemDic = new Dictionary<string, PanelConfigItem>();

        Dictionary<string, GameObject> mPanelPrefabs = new Dictionary<string, GameObject>();

        List<System.Type> mPageQueue = new List<System.Type>();

        GameObject mCurrentPanel;

        System.Type mCurrentPanelType;

        bool mIsOpening;

        public static event UnitEventHandler LoadPrefab;

        public UIPanelManager(UIManager uiManager)
        {
            this.uiManager = uiManager;
            LoadConfig(uiManager.uiSettings.PANEL_CONFIG_FILE);
        }

        void LoadConfig(TextAsset config)
        {
            if (config != null)
            {
                LoadConfigToDic(JsonUtility.FromJson<PanelConfig>(config.text));
            }
        }

        void LoadConfigToDic(PanelConfig panelConfig)
        {
            for (int i = 0; i < panelConfig.items.Count; i++)
            {
                AddConfigItem(panelConfig.items[i]);
            }
        }

        void AddConfigItem(PanelConfigItem item){
            mConfigItemDic.Add(item.ctrlClassName, item);
        }

        GameObject GetPrefab(string classType)
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

        GameObject GetNewPrefab(string classType)
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

        bool CheckOpenable(System.Type type)
        {
            if (mCurrentPanelType == type)
            {
                return false;
            }
            return !mIsOpening;
        }

        void EnableOpen()
        {
            mIsOpening = false;
        }

        void DisableOpen()
        {
            mIsOpening = true;
        }

        public void OpenPanel<T>(Hashtable param = null, UnityAction<GameObject> onShow = null) where T : BasePanelCtrl
        {
            if (CheckOpenable(typeof(T)))
            {
                OpenPageWithFlash(typeof(T), true, param, onShow);
            }
        }

        GameObject ShowPanel(System.Type type, bool isNew, Hashtable param, UnityAction<GameObject> onShow)
        {
            GameObject go = CreatePanel(type);
            uiManager.AddToLayer(go, UIManager.UILayerNames.UILayer_Common);
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

        GameObject CreatePanel(System.Type type)
        {
            GameObject prefab = GetPrefab(type.ToString());
            Debug.Log(string.Format("CreatePanel {0}",type));
            //to let the components call the awake after Init datas.
            prefab.SetActive(false);
            GameObject go = Object.Instantiate(prefab);
            return go;
        }

        void SetNewCurrentPage(GameObject go, System.Type type)
        {
            if (mCurrentPanel != null)
            {
                Object.Destroy(mCurrentPanel);
            }
            mCurrentPanel = go;
            mCurrentPanelType = type;
        }

        void SetDeltaSize(GameObject go)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(UIManager.CANVAS_WIDTH, UIManager.CANVAS_HEIGHT);
        }

        void ApendController(GameObject go, System.Type type, Hashtable param)
        {
            go.GetOrAddComponent(type);
            go.GetComponent<BasePanelCtrl>().uiPanelManager = this;
            go.GetComponent<BasePanelCtrl>().Transmit(param);
        }

        public void OnBack()
        {
            if (mPageQueue.Count > 1)
            {
                System.Type type = mPageQueue[mPageQueue.Count - 1];
                OpenPageWithFlash(type, false);
                if (CheckOpenable(type)){
                    //remove current page from queue.
                    mPageQueue.RemoveAt(mPageQueue.Count - 1);
                }
            }
        }

        void OpenPageWithFlash(System.Type type, bool isNew = true, Hashtable param = null, UnityAction<GameObject> onShow = null)
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