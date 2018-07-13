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
        UIManager mUIManager;

        Dictionary<string, DialogConfigItem> mDialogInfos = new Dictionary<string, DialogConfigItem>();

        List<BaseDialog> mActivedDialogs = new List<BaseDialog>();

        public delegate GameObject UnitEventHandler(string path);

        public static event UnitEventHandler LoadPrefab;

        public UIDialogManager(UIManager uiManager)
        {
            mUIManager = uiManager;
            LoadConfig(mUIManager.uiSettings.DIALOG_CONFIG_FILE);
        }

        void LoadConfig(TextAsset config)
        {
            if (config != null)
            {
                DialogConfig dialogConfig = JsonUtility.FromJson<DialogConfig>(config.text);
                LoadConfigItems(dialogConfig.items);
            }
        }

        void LoadConfigItems(List<DialogConfigItem> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                LoadConfigInfo(items[i]);
            }
        }

        void LoadConfigInfo(DialogConfigItem configItem)
        {
            mDialogInfos.Add(configItem.className.Trim(), configItem);
        }

        GameObject GetNewPrefab(string dialogName)
        {
#if UNITY_EDITOR
            if (LoadPrefab == null)
            {
                Debug.LogError("can't load prefab , please set the LoadPrefab to UIDialogManager!");
            }
#endif
            if (mDialogInfos.ContainsKey(dialogName) && LoadPrefab != null)
            {
                return LoadPrefab(mDialogInfos[dialogName].prefabPath);
            }
            return null;
        }

        public bool OnBack()
        {
            if (mActivedDialogs != null && mActivedDialogs.Count > 0)
            {
                mActivedDialogs[mActivedDialogs.Count - 1].OnBack();
                return true;
            }
            return false;
        }

        public bool OnReturn()
        {
            if (mActivedDialogs != null && mActivedDialogs.Count > 0)
            {
                mActivedDialogs[mActivedDialogs.Count - 1].OnReturn();
                return true;
            }
            return false;
        }

        public T OpenDialog<T>(Hashtable param = null) where T : BaseDialog
        {
            T t = OpenDialogWithoutMask<T>(param);
            ShowMask();
            return t;
        }

        public T OpenDialogWithoutMask<T>(Hashtable param = null) where T : BaseDialog
        {
            T t = CreateDialog<T>();
            AddToHistory(t);
            t.Transmit(param);
            return t;
        }

        void AddToHistory(BaseDialog baseDialog)
        {
            mActivedDialogs.Add(baseDialog);
        }

        T CreateDialog<T>() where T : BaseDialog
        {
            GameObject prefab = GetNewPrefab(typeof(T).ToString());
            GameObject go = UnityEngine.Object.Instantiate(prefab);
            return InitDialog<T>(go);
        }

        T InitDialog<T>(GameObject go) where T : BaseDialog
        {
            AddToLayer(go);
            T t = go.GetComponent<T>();
            t.uiDialogManager = this;
            return t;
        }

        void AddToLayer(GameObject go)
        {
            mUIManager.AddToLayer(go, UIManager.UILayerNames.UILayer_Popup);
        }

        public void ShowCommonDialog(string title, string msg, UnityAction onOk, bool showClose = false)
        {
            CommonDialog commonDialog = OpenDialog<CommonDialog>();
            commonDialog.Show(title, msg, onOk, showClose);
        }

        public void ShowConfirmDialog(string msg, UnityAction onOk, UnityAction onCancel, bool showClose = false)
        {
            ConfirmDialog confirmDialog = OpenDialog<ConfirmDialog>();
            confirmDialog.Show(msg, onOk, onCancel, showClose);
        }

        public void CloseDialog(BaseDialog baseDialog)
        {
            mActivedDialogs.Remove(baseDialog);
            if (baseDialog != null)
            {
                UnityEngine.Object.Destroy(baseDialog.gameObject);
            }
            HideMask();
        }

        void ShowMask()
        {
            mUIManager.ShowMaskOnLayer(UIManager.UILayerNames.UILayer_Popup);
        }

        void HideMask()
        {
            if (mActivedDialogs == null || mActivedDialogs.Count == 0)
            {
                mUIManager.HideMaskOnLayer(UIManager.UILayerNames.UILayer_Popup);
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