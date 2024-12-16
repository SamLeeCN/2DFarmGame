using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[CreateAssetMenu(menuName = "DataSO/SceneSoundListSO")]
public class SceneSoundListSO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundList = new List<SceneSoundItem>();

    public SceneSoundItem GetSceneSoundItem(string sceneName)
    {
        return sceneSoundList.Find(s => s.sceneName == sceneName);
    }

}
[System.Serializable]
public class SceneSoundItem
{
    public string sceneName;
    public SoundName ambient;
    public SoundName music;
    public float bgmDelayTime = 3f;
}
