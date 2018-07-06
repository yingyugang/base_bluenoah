using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExportSettings", menuName = "CreateSetting/ExportSettings", order = 1)]
public class ExportSettings : ScriptableObject{

    public List<Object> includeObjects;
	
}