using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlueNoah.UI
{
	//basic separate the views and controlls.
	public class BasePanelView : MonoBehaviour
	{

		#if UNITY_EDITOR
		[SerializeField]
        protected bool loadUI;
		[SerializeField]
		protected bool loadPath;
		#endif

		//for load ui component at editor
		protected virtual void Update ()
		{
			#if UNITY_EDITOR
			if (loadUI) {
				loadUI = false;
				LoadUIs ();
			}
			if (loadPath) {
				loadPath = false;
				LoadPaths ();
			}
			#endif
		}

		//no matter wether used
		public virtual void LoadUIs ()
		{

		}

		//no matter wether used
		public virtual void LoadPaths ()
		{
	
		}

		public void GetGameObjectPath (Component comp, ref string resultPath)
		{
			if (comp == null)
				return;
			string path = "/" + comp.gameObject.name;
			Transform trans = this.transform;
			GameObject obj = comp.gameObject;
			while (obj.transform.parent != null) {
				if (trans == obj.transform.parent) {
					break;
				}
				obj = obj.transform.parent.gameObject;
				path = "/" + obj.name + path;
			}
			path = path.Remove (0, 1);
			resultPath = path;
		}
	}
}