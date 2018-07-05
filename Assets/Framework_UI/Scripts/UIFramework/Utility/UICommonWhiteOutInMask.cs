using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;
using BlueNoah.Utility;

public class UICommonWhiteOutInMask : SimpleSingleMonoBehaviour<UICommonWhiteOutInMask> {

	public AnimationCurve whiteOutCurve;
	public Image img_flash;
	protected override void Awake(){
		base.Awake ();
        img_flash = GetComponent<Image> ();
	}

	public void DoFlash(float duration,UnityAction onMiddle = null,UnityAction onComplete = null){
		if(img_flash==null)
			img_flash = gameObject.GetOrAddComponent<Image> ();
		img_flash.enabled = true;
		img_flash.DOFade (1, duration/2).SetEase (whiteOutCurve).OnComplete(()=>{
			if(onMiddle!=null)
				onMiddle();
			img_flash.DOFade (0, duration/2).SetEase (whiteOutCurve).OnComplete(()=>{
				img_flash.enabled = false;
				if(onComplete!=null)
					onComplete();
			});
		});
	}

	public void Reset(){
		img_flash.DOKill ();
		img_flash.DOFade (0,0);
	}

}
