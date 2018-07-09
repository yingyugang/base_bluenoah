using System.Collections.Generic;

namespace BlueNoah.UI
{
    public class UIModelManager
    {
        public UIModelManager uiModelManager;

        Dictionary<string, BaseProxy> mModels = new Dictionary<string, BaseProxy>();

        public T Get<T>() where T : BaseProxy, new()
        {
            string typeStr = typeof(T).ToString();
            T t = null;
            if (!this.mModels.ContainsKey(typeStr))
            {
                t = Add<T>();
            }
            else
            {
                t = (T)mModels[typeStr];
            }
            return t;
        }

        public void Clear()
        {
            mModels.Clear();
        }

        public bool Remove<T>() where T : BaseProxy, new()
        {
            string typeStr = typeof(T).ToString();
            return Remove(typeStr);
        }

        bool Remove(string typeStr){
            return mModels.Remove(typeStr);
        }

        T Add<T>() where T : BaseProxy, new()
        {
            string typeStr = typeof(T).ToString();
            return Add<T>(typeStr);
        }

        T Add<T>(string typeStr) where T : BaseProxy, new()
        {
            T bm = new T();
            mModels.Add(typeStr, new T());
            return bm;
        }

        bool IsExisting(string typeStr) 
        {
            if(mModels!=null && mModels.ContainsKey(typeStr)){
                return true;
            }
            return false;
        }
    }
}
