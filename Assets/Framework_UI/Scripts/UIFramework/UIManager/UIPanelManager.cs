using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BlueNoah.Utility;

namespace BlueNoah.UI
{
    public class UIPanelManager
    {
        UIManager mUIManager;

        const float WHITE_FLASH_DURATION = 2f;

        Dictionary<string, PanelConfigItem> mConfigItemDic = new Dictionary<string, PanelConfigItem>();

        Dictionary<string, GameObject> mPanelPrefabDic = new Dictionary<string, GameObject>();

        List<System.Type> mPageList = new List<System.Type>();

        GameObject mCurrentPanel;

        System.Type mCurrentPanelType;

        bool mIsOpening;

        public static event UnitEventHandler LoadPrefab;

        public UIPanelManager(UIManager uiManager)
        {
            this.mUIManager = uiManager;
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

        void AddConfigItem(PanelConfigItem item)
        {
            mConfigItemDic.Add(item.ctrlClassName, item);
        }

        bool ConfigExisting(string classType)
        {
            return mConfigItemDic.ContainsKey(classType);
        }

        bool IsPrefabCached(string classType)
        {
            return mPanelPrefabDic.ContainsKey(classType);
        }

        GameObject GetPrefab(string classType)
        {
            return ConfigExisting(classType) ? GetOrCreatePrefab(classType) : null;
        }

        GameObject GetOrCreatePrefab(string classType)
        {
            return IsPrefabCached(classType) ? mPanelPrefabDic[classType] : GetNewPrefab(classType);
        }

        //TODO to write the sample load function.
        GameObject GetNewPrefab(string classType)
        {
            #if UNITY_EDITOR
            if (LoadPrefab == null)
            {
                Debug.LogError("can't load prefab , please set the LoadPrefab to UIPanelManager!");
            }
            #endif
            GameObject panelPrefab = null;
            if (LoadPrefab != null)
            {
                panelPrefab = LoadPrefab(mConfigItemDic[classType].prefabPath);
                CachePrefab(classType, panelPrefab);
            }
            return panelPrefab;
        }

        void CachePrefab(string classType, GameObject prefab)
        {
            if (!string.IsNullOrEmpty(classType) && prefab != null)
                mPanelPrefabDic.Add(classType, prefab);
        }

        bool CheckOpenable(System.Type type)
        {
            return mCurrentPanelType == type ? false : !mIsOpening;
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
                if (UICommonWhiteOutInMask.Instance != null)
                    ForwardWithFlash(typeof(T), param, onShow);
                else
                    ShowPanel(typeof(T), param, onShow);
            }
        }

        public T GetCurrentCtrl<T>() where T : BasePanelCtrl
        {
            return mCurrentPanel.GetComponent<T>();
        }

        void ForwardWithFlash(System.Type type, Hashtable param = null, UnityAction<GameObject> onShow = null)
        {
            OnBeforeChangePage();
            UICommonWhiteOutInMask.Instance.DoFlash(WHITE_FLASH_DURATION, () =>
            {
                ShowPanel(type, param, onShow);
            }, () =>
            {
                EnableOpen();
            });
        }

        void BackPage()
        {
            System.Type type = mPageList[mPageList.Count - 1];
            if (CheckOpenable(type))
            {
                if (UICommonWhiteOutInMask.Instance != null)
                    BackwardWithFlash(type);
                else
                    BackwardPanel(type);
            }
        }

        void BackwardWithFlash(System.Type type, Hashtable param = null, UnityAction<GameObject> onShow = null)
        {
            OnBeforeChangePage();
            UICommonWhiteOutInMask.Instance.DoFlash(WHITE_FLASH_DURATION, () =>
            {
                BackwardPanel(type, param, onShow);
            }, () =>
            {
                EnableOpen();
            });
        }

        void OnBeforeChangePage()
        {
            DisableCurrentPanel();
            DisableOpen();
        }

        void BackwardPanel(System.Type type, Hashtable param = null, UnityAction<GameObject> onShow = null)
        {
            ShowPanel(type, param, onShow);
        }

        void ForwardPanel(System.Type type, Hashtable param = null, UnityAction<GameObject> onShow = null)
        {
            ShowPanel(type, param, onShow);
            AddHistory(type);
        }

        GameObject ShowPanel(System.Type type, Hashtable param = null, UnityAction<GameObject> onShow = null)
        {
            GameObject go = CreatePanel(type);
            ResetPanel(type, go);
            InitController(go, type, param);
            OnShowPanel(go, onShow);
            return go;
        }

        GameObject CreatePanel(System.Type type)
        {
            GameObject prefab = GetPrefab(type.ToString());
            //to let the components call the awake after Init datas.
            prefab.SetActive(false);
            GameObject go = Object.Instantiate(prefab);
            return go;
        }

        void ResetPanel(System.Type type, GameObject go)
        {
            PutToLayer(go);
            SetNewCurrentPage(go, type);
            SetDeltaSize(go);
        }

        void AddHistory(System.Type type)
        {
            mPageList.Add(type);
        }

        void RemoveHistory()
        {
            mPageList.RemoveAt(mPageList.Count - 1);
        }

        void OnShowPanel(GameObject go, UnityAction<GameObject> onShow)
        {
            if (onShow != null)
                onShow(go);
        }

        void ActivePanel(GameObject go)
        {
            go.SetActive(true);
        }

        void PutToLayer(GameObject go)
        {
            mUIManager.AddToLayer(go, UIManager.UILayerNames.UILayer_Common);
        }

        void SetNewCurrentPage(GameObject go, System.Type type)
        {
            DestroyCurrentPanel();
            mCurrentPanel = go;
            mCurrentPanelType = type;
        }

        void DestroyCurrentPanel()
        {
            if (mCurrentPanel != null)
            {
                Object.Destroy(mCurrentPanel);
            }
        }

        void SetDeltaSize(GameObject go)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(UIManager.CANVAS_WIDTH, UIManager.CANVAS_HEIGHT);
        }

        void InitController(GameObject go, System.Type type, Hashtable param)
        {
            go.GetOrAddComponent(type);
            go.GetComponent<BasePanelCtrl>().uiPanelManager = this;
            go.GetComponent<BasePanelCtrl>().Transmit(param);
            ActivePanel(go);
        }

        public void OnBack()
        {
            if (mPageList.Count > 1)
            {
                BackPage();
            }
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
            return mUIManager.GetSession(param);
        }

        public void SetSession(string param, System.Object obj)
        {
            mUIManager.SetSession(param, obj);
        }
    }
}