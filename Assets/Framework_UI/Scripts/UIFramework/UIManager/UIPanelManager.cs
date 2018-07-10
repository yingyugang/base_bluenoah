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

        Dictionary<string, GameObject> mPanelPrefabDic = new Dictionary<string, GameObject>();

        List<System.Type> mPageList = new List<System.Type>();

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

        bool ConfigExisting(string classType){
            return mConfigItemDic.ContainsKey(classType);
        }

        bool IsPrefabCached(string classType){
            return mPanelPrefabDic.ContainsKey(classType);
        }

        void CachePrefab(string classType,GameObject prefab){
            if (!string.IsNullOrEmpty(classType) &&  prefab != null)
                mPanelPrefabDic.Add(classType, prefab);
        }

        GameObject GetPrefab(string classType)
        {
            GameObject panelPrefab = null;
            if (ConfigExisting(classType))
            {
                panelPrefab = GetOrCreatePrefab(classType);
            }
            return panelPrefab;
        }

        GameObject GetOrCreatePrefab(string classType){
            if (IsPrefabCached(classType))
            {
                return mPanelPrefabDic[classType];
            }
            else
            {
                return GetNewPrefab(classType);
            }
        }

        GameObject GetNewPrefab(string classType)
        {
            GameObject panelPrefab = null;
            if (LoadPrefab != null)
            {
                panelPrefab = LoadPrefab(mConfigItemDic[classType].prefabPath);
                CachePrefab(classType,panelPrefab);
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
            ResetPanel(type, go);
            InitController(go, type, param);
            AddToHistory(type, isNew);
            OnShowPanel(go,onShow);
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

        void ResetPanel(System.Type type,GameObject go){
            PutToLayer(go);
            SetNewCurrentPage(go, type);
            SetDeltaSize(go);
        }

        void AddToHistory(System.Type type,bool isNew){
            if (isNew)
                mPageList.Add(type);
        }

        void OnShowPanel(GameObject go,UnityAction<GameObject> onShow){
            if (onShow != null)
                onShow(go);
        }

        void ActivePanel(GameObject go){
            go.SetActive(true);
        }

        void PutToLayer(GameObject go){
            uiManager.AddToLayer(go, UIManager.UILayerNames.UILayer_Common);
        }

        void SetNewCurrentPage(GameObject go, System.Type type)
        {
            DestroyCurrentPanel();
            mCurrentPanel = go;
            mCurrentPanelType = type;
        }

        void DestroyCurrentPanel(){
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
                System.Type type = mPageList[mPageList.Count - 1];
                OpenPageWithFlash(type, false);
                if (CheckOpenable(type)){
                    //remove current page from queue.
                    mPageList.RemoveAt(mPageList.Count - 1);
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