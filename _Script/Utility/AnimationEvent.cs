using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class AnimationEvent : MonoBehaviour
{
    public void PlayFootStepSoftSound()
    {
        EventHandler.CallSoundEffectEvent(SoundName.FootStepSoft, transform.position);
    }

    public void PlayFootStepHardSound()
    {
        EventHandler.CallSoundEffectEvent(SoundName.FootStepHard, transform.position);
    }
}
