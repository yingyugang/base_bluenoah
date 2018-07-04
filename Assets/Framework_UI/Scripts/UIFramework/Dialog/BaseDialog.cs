using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseDialog : MonoBehaviour
{

	public UnityAction onClose;

	public Button btn_close;

	public Text txt_title;

	protected virtual void Awake ()
	{
		if (btn_close != null) {
			btn_close.onClick.AddListener (() => {
				if (onClose != null)
					onClose ();
				UIManager.Instance.uiDialogManager.Close(this);
			});
		}
	}

	public void Close ()
	{
		if (onClose != null) {
			onClose ();
		}
	}

}
