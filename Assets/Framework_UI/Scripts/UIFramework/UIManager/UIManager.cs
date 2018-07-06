/*********************************************************
 * 1.Init the layers;                                    *
 * 2.Init the page configs;                              *
 * 3.Add the page controller to the layers;              *
 * 4.Add the dialogs;                                    *
 * 5.Use session to send datas in all page and dialog.   *
 * 6.Use Init(param) the send data when new page opened. *
 *********************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using BlueNoah.Utility;

namespace BlueNoah.UI
{
    public delegate GameObject UnitEventHandler(string path);

    public class UIManager : SimpleSingleMonoBehaviour<UIManager>
    {
        public const int CANVAS_WIDTH = 1920;

        public const int CANVAS_HEIGHT = 1080;

        const string UI_SETTING = "Settings/UISettings";

        private Dictionary<UILayerNames, Transform> mUILayers = new Dictionary<UILayerNames, Transform>();

        public UIDialogManager uiDialogManager;

        public UIPanelManager uiPanelManager;

        [HideInInspector]
        public UISettings uiSettings;

        private Dictionary<string, System.Object> mSession;


        public enum UILayerNames
        {
            UILayer_Bottom,
            UILayer_Common,
            UILayer_Popup,
            UILayer_Mask
        }

        protected override void Awake()
        {
            base.Awake();
            Init();
        }

        void Init()
        {
            InitSettings();
            InitSession();
            InitManagers();
            InitLayers();
        }

        void InitSettings()
        {
            uiSettings = Resources.Load<UISettings>(UI_SETTING);
        }

        void InitSession()
        {
            mSession = new Dictionary<string, System.Object>();
        }

        void InitManagers()
        {
            uiDialogManager = new UIDialogManager(this);
            uiPanelManager = new UIPanelManager(this);
        }

        void CheckInputBack()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!uiDialogManager.OnBack())
                {
                    uiPanelManager.OnBack();
                }
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                uiDialogManager.OnReturn();
            }
        }

        private void InitLayers()
        {
            AddLayer(UILayerNames.UILayer_Bottom);
            AddLayer(UILayerNames.UILayer_Common);
            AddLayer(UILayerNames.UILayer_Popup);
            AddLayer(UILayerNames.UILayer_Mask);
        }

        private void AddLayer(UILayerNames layerName, bool mouseEventable = true)
        {
            Transform layerTrans = AddLayer(layerName.ToString(), mouseEventable);
            mUILayers.Add(layerName, layerTrans);
        }

        private Transform AddLayer(string layerName, bool mouseEventable = true)
        {
            GameObject rectLayer = CreateLayer(layerName);
            ApendCanvasGroup(rectLayer, mouseEventable);
            return rectLayer.transform;
        }

        private GameObject CreateLayer(string layerName)
        {
            GameObject rectLayer = new GameObject(layerName);
            rectLayer.layer = LayerMask.NameToLayer("UI");
            ApendRectTransform(rectLayer);
            return rectLayer;
        }

        private void ApendRectTransform(GameObject rectLayer)
        {
            RectTransform rect = rectLayer.GetOrAddComponent<RectTransform>();
            rectLayer.transform.SetParent(transform);
            rectLayer.transform.localPosition = Vector3.zero;
            rectLayer.transform.localScale = Vector3.one;
            rect.sizeDelta = new Vector2(CANVAS_WIDTH, CANVAS_HEIGHT);
        }

        private void ApendCanvasGroup(GameObject go, bool mouseEventable)
        {
            CanvasGroup gc = go.AddComponent<CanvasGroup>();
            gc.blocksRaycasts = mouseEventable;
            gc.interactable = mouseEventable;
        }

        public void AddToLayer(GameObject go, UILayerNames targetLayerName)
        {
            Transform trans = mUILayers[targetLayerName];
            go.transform.SetParent(trans);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
        }

        public void ShowMaskOnLayer(UILayerNames targetLayerName)
        {
            Transform layerTrans = this.mUILayers[targetLayerName];
            Image img_mask = layerTrans.GetOrAddComponent<Image>();
            img_mask.color = new Color(0, 0, 0, 0);
            img_mask.enabled = true;
            img_mask.DOFade(0.4f, 0.3f).SetEase(Ease.InSine);
        }

        public void HideMaskOnLayer(UILayerNames targetLayerName)
        {
            Transform layerTrans = this.mUILayers[targetLayerName];
            Image img_mask = layerTrans.GetOrAddComponent<Image>();
            img_mask.DOFade(0, 0.3f).SetEase(Ease.InSine).OnComplete(() => {
                img_mask.enabled = false;
            });
        }

        public System.Object GetSession(string param)
        {
            if (mSession.ContainsKey(param))
            {
                return mSession[param];
            }
            return null;
        }

        public void SetSession(string param, System.Object obj)
        {
            if (mSession.ContainsKey(param))
            {
                mSession[param] = obj;
            }
            else
            {
                mSession.Add(param, obj);
            }
        }

        void Update()
        {
            CheckInputBack();
        }
    }

    [System.Serializable]
    public class PanelConfig
    {
        public List<PanelConfigItem> items;
    }

    [System.Serializable]
    public class PanelConfigItem
    {
        public int index;
        public string ctrlClassName;
        public string viewClassName;
        public string prefabPath;
    }
}