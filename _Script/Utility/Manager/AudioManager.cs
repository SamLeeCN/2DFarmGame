using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 

public class AudioManager : Singleton<AudioManager>
{
    public SoundDetailsListSO soundDetailsList;
    public SceneSoundListSO sceneSoundList;

    public AudioSource ambientAudioSource;
    public AudioSource bgmAudioSource;
    public SoundName currentBgmName;

    private Coroutine playSoundCoroutine;

    public AudioMixer audioMixer;
    [Header("SnapShots")]
    public AudioMixerSnapshot normalSnapshot;
    public AudioMixerSnapshot noBGMSnapshot;
    public AudioMixerSnapshot muteSnapshot;

    
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.SoundEffectEvent += OnSoundEffectEnumEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
        EventHandler.SoundEffectEvent -= OnSoundEffectEnumEvent;
    }

    private void OnSoundEffectEnumEvent(SoundName soundName, Vector3 pos)
    {
        SoundDetails soundDetails = soundDetailsList.GetSoundDetails(soundName);
        if (soundDetails != null)
            PoolManager.Instance.InitSoundEffect(soundDetails, pos);
    }

    private void OnAfterSceneLoadEvent(bool arg1, bool arg2)
    {
        
        SceneSoundItem sceneSoundItem = sceneSoundList.GetSceneSoundItem(SceneLoadManager.Instance.currentScene.sceneName);
        if (sceneSoundItem == null) return;

        SoundDetails ambient = soundDetailsList.GetSoundDetails(sceneSoundItem.ambient);
        SoundDetails bgm = soundDetailsList.GetSoundDetails(sceneSoundItem.music);
        float bgmDelayTime = sceneSoundItem.bgmDelayTime;
        if (playSoundCoroutine != null) 
            StopCoroutine(playSoundCoroutine);
        playSoundCoroutine = StartCoroutine(PlaySoundCoroutine(bgm, ambient,bgmDelayTime));
    }


    private void PlayBGMClip(SoundDetails soundDetails, float transitionTime, bool changeBgm = true)
    {
        
        if (soundDetails == null) return;
        audioMixer.SetFloat("BGMMasterVolume", ConvertSoundVolume(soundDetails.soundVolume));
        if (!changeBgm) return;
        bgmAudioSource.clip = soundDetails.soundClip;
        currentBgmName = soundDetails.soundName;
        if (bgmAudioSource.isActiveAndEnabled) 
            bgmAudioSource.Play();
        normalSnapshot.TransitionTo(transitionTime);
        
    }

    private void PlayAmbientClip(SoundDetails soundDetails, float transitionTime, bool changeBGM  = true)
    {
        if (soundDetails == null) return;
        audioMixer.SetFloat("AmbientMasterVolume", ConvertSoundVolume(soundDetails.soundVolume));
        ambientAudioSource.clip = soundDetails.soundClip;
        if (ambientAudioSource.isActiveAndEnabled)
            ambientAudioSource.Play();
        if (changeBGM)
            noBGMSnapshot.TransitionTo(transitionTime);
    }

    private IEnumerator PlaySoundCoroutine(SoundDetails bgm, SoundDetails ambient, float bgmDelayTime)
    {
        bool changeBgm = bgm != null && currentBgmName != bgm.soundName;
        PlayAmbientClip(ambient, 0.3f, changeBgm);
        yield return new WaitForSeconds(bgmDelayTime);
        PlayBGMClip(bgm, 0f, changeBgm);
    }

    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }
}
