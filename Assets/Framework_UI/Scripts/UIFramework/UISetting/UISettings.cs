using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "UISettings", menuName = "CreateSetting/UISettings", order = 1)]
public class UISettings : ScriptableObject
{

	public TextAsset PANEL_CONFIG_FILE;
	public TextAsset DIALOG_CONFIG_FILE;

}
