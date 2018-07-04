using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasSelfAdapting : MonoBehaviour {

	const float DEFAULT_RATIO = 1.78f;
	CanvasScaler mCanvasScaler;

	void Awake(){
		mCanvasScaler = GetComponent<CanvasScaler> ();
	}

	void Update(){
		if (mCanvasScaler == null)
			return;
		float currentRatio = (float)Screen.width / Screen.height;
		if (currentRatio < DEFAULT_RATIO) {
			mCanvasScaler.matchWidthOrHeight = 0;
		} else {
			mCanvasScaler.matchWidthOrHeight = 1;
		}
	}
}
