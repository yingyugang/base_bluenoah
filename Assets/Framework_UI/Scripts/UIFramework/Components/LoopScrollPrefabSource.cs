using UnityEngine;
using System.Collections;
using BlueNoah.UI;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public string prefabName;
        public int poolSize = 5;

        private bool inited = false;
        public virtual GameObject GetObject()
        {
            if(!inited)
            {
                ResourceManager.Instance.InitPool(prefabName, poolSize);
                inited = true;
            }
            return ResourceManager.Instance.GetObjectFromPool(prefabName);
        }

        public virtual void ReturnObject(Transform go)
        {
            go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
           ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }
    }
}
