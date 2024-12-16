using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[CreateAssetMenu(menuName = "DataSO/GameSceneSO")]
public class GameSceneSO : ScriptableObject
{
    [SceneName]public string sceneName;
    public AssetReference sceneReference;
    public SceneType sceneType = SceneType.Location;
    public bool isOverrideCamera = false;
    public bool isPlyerDisabled = false;
    public bool isWorldUIDisabled = false;
    public bool isInputDisabled = false;
}
