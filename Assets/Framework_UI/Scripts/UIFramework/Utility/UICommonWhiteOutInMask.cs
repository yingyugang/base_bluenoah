using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class UICommonWhiteOutInMask : SingleMonoBehaviour<UICommonWhiteOutInMask> {

	public AnimationCurve whiteOutCurve;
	public Image img_flash;
	protected override void Awake(){
		base.Awake ();
        img_flash = GetComponent<Image> ();
	}

	public void DoFlash(float duration,UnityAction onComplete = null){
		if(img_flash==null)
            img_flash = gameObject.GetOrAddComponent<Image> ();
		img_flash.enabled = true;
		img_flash.DOFade (1, duration/2).SetEase (whiteOutCurve).OnComplete(()=>{
			if(onComplete!=null)
				onComplete();
			img_flash.DOFade (0, duration/2).SetEase (whiteOutCurve).OnComplete(()=>{
				img_flash.enabled = false;
			});
		});
	}

	public void Reset(){
		img_flash.DOKill ();
		img_flash.DOFade (0,0);
	}

}
