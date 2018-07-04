using System.Collections;
using System.Collections.Generic;
using BlueNoah.UI;
using UnityEngine;
using UnityEngine.Events;

/**********************************************
 *1.Init the layers;
 *2.Init the page configs;
 *3.Add the page controller to the layers;
 *4.Add the dialogs;
 *********************************************/
//TODO
public class UIManager : SingleMonoBehaviour<UIManager>
{
	const int CANVAS_WIDTH = 1920;

	const int CANVAS_HEIGHT = 1080;

	const float WHITE_FLASH_DURATION = 2f;

	private Dictionary<UILayerNames, Transform> mUILayers = new Dictionary<UILayerNames, Transform> ();

	private Dictionary<System.Type,GameObject> mPanelPrefabs = new Dictionary<System.Type, GameObject> ();

	private List<System.Type> mPageQueue = new List<System.Type> ();

	private GameObject mCurrentPage;

	private System.Type mCurrentPageType;

	private bool mIsOpening;

	public UIDialogManager uiDialogManager;

	public UIPanelManager uiPanelManager;

	const string CONFIG_FILE = "Panels/PanelConfig";

	public enum UILayerNames
	{
		UILayer_Bottom,
		UILayer_Common,
		UILayer_Popup,
		UILayer_Mask
	}

	protected override void Awake ()
	{
		base.Awake ();
		uiDialogManager = new UIDialogManager ();
		uiPanelManager = new UIPanelManager ();
		InitLayers ();
		LoadPanelConfig ();
	}

	public void OpenPanel<T> (Hashtable param = null, UnityAction<GameObject> onShow = null) where T : BasePanelCtrl
	{
		if(mIsOpening){
			return;
		}
		if(mCurrentPageType == typeof(T)){
			return;
		}
		mIsOpening = true;
		if(mCurrentPage!=null){
			CanvasGroup cg = mCurrentPage.GetOrAddComponent<CanvasGroup> ();
			cg.blocksRaycasts = false;
		}
		UICommonWhiteOutInMask.Instance.DoFlash (WHITE_FLASH_DURATION,()=>{
			GameObject go = ShowPanel<T>().gameObject;
			go.GetComponent<BasePanelCtrl>().InitData(param);
			go.SetActive(true);
			mPageQueue.Add (typeof(T));
			mIsOpening = false;
			if(onShow!=null)
				onShow(go);
		});
	}

	public void Back ()
	{
		if(mPageQueue.Count>1){
			mPageQueue.RemoveAt (mPageQueue.Count-1);
			System.Type type = mPageQueue[mPageQueue.Count-1];
			if (mCurrentPage != null)
				Destroy (mCurrentPage);
			GameObject go = ShowPanel (type);
			go.SetActive(true);
		}
	}

	private void InitLayers ()
	{
		Debug.Log ("InitLayers");
		AddLayer (UILayerNames.UILayer_Bottom);
		AddLayer (UILayerNames.UILayer_Common);
		AddLayer (UILayerNames.UILayer_Popup);
		AddLayer (UILayerNames.UILayer_Mask);
	}

	private void AddLayer (UILayerNames layerName, bool mouseEventable = true)
	{
		Transform layerTrans = AddLayer (layerName.ToString (), mouseEventable);
		mUILayers.Add (layerName, layerTrans);
	}

	private Transform AddLayer (string layerName, bool mouseEventable = true)
	{
		GameObject rectLayer;
        rectLayer = new GameObject (layerName);
		rectLayer.layer = LayerMask.NameToLayer ("UI");
		RectTransform rect = rectLayer.AddComponent<RectTransform> ();
		rectLayer.transform.SetParent (transform);
		rectLayer.transform.localPosition = Vector3.zero;
		rectLayer.transform.localScale = Vector3.one;
		CanvasGroup gc = rectLayer.AddComponent<CanvasGroup> ();
		gc.blocksRaycasts = mouseEventable;
		gc.interactable = mouseEventable;
		rect.sizeDelta = new Vector2 (CANVAS_WIDTH,CANVAS_HEIGHT);
		return rectLayer.transform;
	}

	//TODO
	private void LoadPanelConfig ()
	{
		TextAsset config = Resources.Load<TextAsset> (CONFIG_FILE );
		if (config != null) {
			PanelConfig panelConfig = JsonUtility.FromJson<PanelConfig> (config.text);
			for (int i = 0; i < panelConfig.items.Count; i++) {
				PanelConfigItem item = panelConfig.items [i];
				GameObject panelPrefab = ResourcesManager.LoadPanelPrefab (item.prefabPath);
				System.Type type = System.Type.GetType (item.ctrlClassName);
				mPanelPrefabs.Add (type, panelPrefab);
			}
		}
	}

	private T ShowPanel<T> () where T : BasePanelCtrl
	{
		if (mPanelPrefabs.ContainsKey (typeof(T))) {
			if (mCurrentPage != null)
				Destroy (mCurrentPage);
			GameObject go = ShowPanel (typeof(T));
			return go.GetOrAddComponent<T> ();
		}
		return null;
	}

	private GameObject ShowPanel (System.Type type) 
	{
		if (mPanelPrefabs.ContainsKey (type)) {
			GameObject prefab = mPanelPrefabs [type];
			prefab.SetActive (false);
			GameObject go = Instantiate (prefab);
			Transform trans = mUILayers [UILayerNames.UILayer_Common];
			go.transform.SetParent (trans);
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.SetActive (true);
			mCurrentPage = go;
			mCurrentPageType = type;
			RectTransform rect = go.GetComponent<RectTransform> ();
			rect.sizeDelta = new Vector2 (CANVAS_WIDTH,CANVAS_HEIGHT);
			return go;
		}
		return null;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.G)) {
			OpenPanel<MyPagePanelCtrl>();
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			Back ();
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			//OpenPanel<StagePanelCtrl>();
		}
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
