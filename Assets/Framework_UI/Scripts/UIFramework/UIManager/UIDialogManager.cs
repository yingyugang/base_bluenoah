/**********************************************
 *
 *********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace BlueNoah.UI
{
    //one dialog class may correspond servel prefabs, so need use the dialog name to unique the all the dialogs.
    public class UIDialogManager
    {
        public UIManager uiManager;

        Dictionary<string,DialogConfigItem> mDialogInfos = new Dictionary<string,DialogConfigItem> ();

        List<BaseDialog> mActivedDialogs = new List<BaseDialog> ();

        public delegate GameObject UnitEventHandler(string path);

        public static event UnitEventHandler LoadPrefab;

        public UIDialogManager (UIManager uiManager)
        {
            this.uiManager = uiManager;
            LoadConfig (uiManager.uiSettings.DIALOG_CONFIG_FILE);
        }

        void LoadConfig (TextAsset config)
        {
            if (config != null) {
                DialogConfig dialogConfig = JsonUtility.FromJson<DialogConfig> (config.text);
                for (int i = 0; i < dialogConfig.items.Count; i++) {
                    DialogConfigItem configItem = dialogConfig.items [i];
                    mDialogInfos.Add (configItem.className.Trim (), configItem);
                }
            }
        }

        GameObject GetPrefab (string dialogName)
        {
            if (string.IsNullOrEmpty (dialogName)) {
                return null;
            }
            if (mDialogInfos.ContainsKey (dialogName)) {
                string prefabPath = mDialogInfos [dialogName].prefabPath;
                GameObject prefab = null;
                if(LoadPrefab!=null){
                    prefab = LoadPrefab (prefabPath);
                }
                return prefab;
            }
            return null;
        }

        public bool OnBack ()
        {
            if (mActivedDialogs != null && mActivedDialogs.Count > 0) {
                BaseDialog baseDialog = mActivedDialogs [mActivedDialogs.Count - 1];
                baseDialog.OnBack ();
                return true;
            }
            return false;
        }

        public bool OnReturn(){
            if (mActivedDialogs != null && mActivedDialogs.Count > 0) {
                BaseDialog baseDialog = mActivedDialogs [mActivedDialogs.Count - 1];
                baseDialog.OnReturn ();
                return true;
            }
            return false;
        }
        //1.in front of the page(no mask , close when panel close)
        //2.on the mask layer.(mask , can not do anything before close it)
        public T OpenDialog<T> (string dialogName, bool isShowMask = true, Hashtable param = null) where T : BaseDialog
        {
            T t = CreateDialog<T> (dialogName);
            mActivedDialogs.Add (t);
            if (isShowMask) {
                ShowMask ();
            }
            t.Transmit (param);
            return t;
        }

        T CreateDialog<T>(string dialogName) where T : BaseDialog{
            GameObject prefab = GetPrefab (typeof(T).ToString());
            if(!ValidatePrefab<T>(prefab)){
                return null;
            }
            GameObject go = UnityEngine.Object.Instantiate (prefab);
            uiManager.AddToLayer (go, UIManager.UILayerNames.UILayer_Popup);
            T t = go.GetComponent<T> ();
            t.uiDialogManager = this;
            return t;
        }

        bool ValidatePrefab<T>(GameObject prefab){
            if (prefab == null)
            {
                Debug.LogError(string.Format("prefab is not existing!"));
                return false;
            }
            if (prefab.GetComponent<T> () == null) {
                Debug.LogError (string.Format ("{0}'s component {1} is not existing!", prefab.name, typeof(T)));
                return false;
            }
            return true;
        }

        public void ShowCommonDialog (string title, string msg, UnityAction onOk, bool showClose = false)
        {
            CommonDialog commonDialog = OpenDialog<CommonDialog> ("BaseCommonDialog");
            commonDialog.Show (title, msg, onOk, showClose);
        }

        public void ShowConfirmDialog (string msg, UnityAction onOk, UnityAction onCancel, bool showClose = false)
        {
            ConfirmDialog confirmDialog = OpenDialog<ConfirmDialog> ("BaseConfirmDialog");
            confirmDialog.Show (msg, onOk, onCancel, showClose);
        }

        public void CloseDialog (BaseDialog baseDialog)
        {
            mActivedDialogs.Remove (baseDialog);
            if (baseDialog != null) {
                UnityEngine.Object.Destroy (baseDialog.gameObject);
            }
            HideMask ();
        }

        void ShowMask ()
        {
            uiManager.ShowMaskOnLayer (UIManager.UILayerNames.UILayer_Popup);
        }

        void HideMask ()
        {
            if (mActivedDialogs == null || mActivedDialogs.Count == 0) {
                uiManager.HideMaskOnLayer (UIManager.UILayerNames.UILayer_Popup);
            }
        }

        public System.Object GetSession(string param){
            return uiManager.GetSession (param);
        }

        public void SetSession(string param,System.Object obj){
            uiManager.SetSession (param,obj);
        }
    }

    [Serializable]
    public class DialogConfig
    {
        public List<DialogConfigItem> items;
    }

    [Serializable]
    public class DialogConfigItem
    {
        public int index;
        public string className;
        public string prefabPath;
    }
}