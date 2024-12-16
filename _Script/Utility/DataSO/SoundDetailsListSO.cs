using Farm.Dialogue;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[CreateAssetMenu(menuName = "DataSO/SoundDetailsListSO")]
public class SoundDetailsListSO : ScriptableObject
{
    public bool clearSoundNameStringBtn = false;
    public bool refreshBtn = false;
    public List<SoundDetails> soundDetailsList = new List<SoundDetails>();

    public SoundDetails GetSoundDetails(SoundName name)
    {
        return soundDetailsList.Find(s => s.soundName == name);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        ClearSoundNameString();
        Refresh();
    }
    private void ClearSoundNameString()
    {
        if (!clearSoundNameStringBtn) return;
        foreach(SoundDetails soundDetails in soundDetailsList)
        {
            soundDetails.soundNameString = string.Empty;
        }
        clearSoundNameStringBtn = false;
    }
    private void Refresh()
    {
        if (!refreshBtn) return;
        foreach (SoundDetails soundDetails in soundDetailsList)
        {
            SoundName correspond = SoundName.None;
            bool found = false;
            foreach (SoundName tmpSoundName in Enum.GetValues(typeof(SoundName)))
            {
                if (tmpSoundName.ToString() == soundDetails.soundNameString)
                {
                    correspond = tmpSoundName;
                    found = true;
                    break;
                }
            }

            if (found)
            {
                soundDetails.soundName = correspond;
            }
            else
            {
                soundDetails.soundNameString = soundDetails.soundName.ToString();
            }

        }

        refreshBtn = false;
    }
#endif

}
[System.Serializable]
public class SoundDetails
{
    public string soundNameString;
    public SoundName soundName;
    public AudioClip soundClip;
    [Range(0.1f, 1.5f)] 
    public float soundPitchMin = 0.8f;
    [Range(0.1f, 1.5f)]
    public float soundPitchMax = 1.2f;
    [Range(0.1f, 1.5f)]
    public float soundVolume = 0.2f;
}