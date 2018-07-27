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
        public const int CANVAS_WIDTH = 2000;

        public const int CANVAS_HEIGHT = 1125;

        const string UI_SETTING = "Settings/UISettings";

        Dictionary<UILayerNames, Transform> mUILayers = new Dictionary<UILayerNames, Transform>();
        //attack to uiSystem.
        public UIDialogManager uiDialogManager;
        //ctrl and view.ctrl and view will not be destroy when close.
        public UIPanelManager uiPanelManager;
        //store model datas.model will be not destroy when close.
        public UIModelManager uIModelManager;

        [HideInInspector]
        public UISettings uiSettings;

        Dictionary<string, System.Object> mSession;

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
            uIModelManager = new UIModelManager();
            uiDialogManager = new UIDialogManager(this);
            uiPanelManager = new UIPanelManager(this);
        }

        void CheckInputBack()
        {
            CheckInputEscape();
            CheckInputReturn();
        }

        void CheckInputEscape()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!uiDialogManager.OnBack())
                {
                    uiPanelManager.OnBack();
                }
            }
        }

        void CheckInputReturn()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                uiDialogManager.OnReturn();
            }
        }

        void InitLayers()
        {
            AddLayer(UILayerNames.UILayer_Bottom);
            AddLayer(UILayerNames.UILayer_Common);
            AddLayer(UILayerNames.UILayer_Popup);
            AddLayer(UILayerNames.UILayer_Mask);
        }

        void AddLayer(UILayerNames layerName, bool mouseEventable = true)
        {
            Transform layerTrans = AddLayer(layerName.ToString(), mouseEventable);
            mUILayers.Add(layerName, layerTrans);
        }

        Transform AddLayer(string layerName, bool mouseEventable = true)
        {
            GameObject rectLayer = CreateLayer(layerName);
            ApendCanvasGroup(rectLayer, mouseEventable);
            return rectLayer.transform;
        }

        GameObject CreateLayer(string layerName)
        {
            GameObject rectLayer = new GameObject(layerName);
            SetNameOfLayer(rectLayer, "UI");
            ApendRectTransform(rectLayer);
            return rectLayer;
        }

        void SetNameOfLayer(GameObject rectLayer, string layerName)
        {
            rectLayer.layer = LayerMask.NameToLayer(layerName);
        }

        void ApendRectTransform(GameObject rectLayer)
        {
            RectTransform rect = rectLayer.GetOrAddComponent<RectTransform>();
            rectLayer.transform.SetParent(transform);
            rectLayer.transform.ResetLocal();
            rect.sizeDelta = new Vector2(CANVAS_WIDTH, CANVAS_HEIGHT);
            GetComponent<CanvasScaler>().referenceResolution = new Vector2(CANVAS_WIDTH,CANVAS_HEIGHT) ;
        }

        void ApendCanvasGroup(GameObject go, bool mouseEventable)
        {
            CanvasGroup gc = go.AddComponent<CanvasGroup>();
            gc.blocksRaycasts = mouseEventable;
            gc.interactable = mouseEventable;
        }

        public void AddToLayer(GameObject go, UILayerNames targetLayerName)
        {
            Transform trans = mUILayers[targetLayerName];
            go.transform.SetParent(trans);
            go.transform.ResetLocal();
        }

        public void ShowMaskOnLayer(UILayerNames targetLayerName)
        {
            Transform layerTrans = this.mUILayers[targetLayerName];
            Image img_mask = layerTrans.GetOrAddComponent<Image>();
            ResetMasK(img_mask);
        }

        void ResetMasK(Image img_mask)
        {
            img_mask.color = new Color(0, 0, 0, 0);
            img_mask.enabled = true;
            MaskEaseIn(img_mask);
        }

        void MaskEaseIn(Image image)
        {
            image.DOFade(0.4f, 0.3f).SetEase(Ease.InSine);
        }

        public void HideMaskOnLayer(UILayerNames targetLayerName)
        {
            Transform layerTrans = this.mUILayers[targetLayerName];
            Image img_mask = layerTrans.GetComponent<Image>();
            if(img_mask!=null && img_mask.enabled)
                MaseEaseOut(img_mask);
        }

        void MaseEaseOut(Image image)
        {
            image.DOFade(0, 0.3f).SetEase(Ease.InSine).OnComplete(() =>
            {
                image.enabled = false;
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