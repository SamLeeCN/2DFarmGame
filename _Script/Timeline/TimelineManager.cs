using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class TimelineManager : Singleton<TimelineManager>
{
    public PlayableDirector startDirector;
    public PlayableDirector currentDirector;
    public bool doMuteBGM = true;
    public bool isPause;
    public bool isDone;
    
    private void Awake()
    {
        
    }

    private void Update()
    {
        if (isPause && isDone && InputManager.Instance.TalkWithNPCInput)
        {
            PlayFromPause();
        }
    }

    private void OnEnable()
    {
        /*if (currentDirector!= null)
            SetDirector(currentDirector);*/
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        
    }

    private void OnDisable()
    {
        //RemoveDirector();
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent(bool arg1, bool isFirstLoad)
    {
        if (isFirstLoad && SceneLoadManager.Instance.currentScene == SceneLoadManager.Instance.startScene)
        {
            currentDirector = startDirector;
            doMuteBGM = true;
            //SetDirector(startDirector);
            startDirector.gameObject.SetActive(true);
            startDirector.Play();
        }


    }
/*
    public bool SetDirector(PlayableDirector director)
    {
        if (!gameObject.activeInHierarchy) return false;
        currentDirector = director;
        currentDirector.played += OnCurrentDirectorPlayed; // Play On Awake would not trigger this event!!!
        currentDirector.stopped += OnCurrentDirectorStopped;
        return true;
    }

    public bool RemoveDirector()
    {
        if (currentDirector == null) return false;
        currentDirector.played -= OnCurrentDirectorPlayed; // Play On Awake would not trigger this event!!!
        currentDirector.stopped -= OnCurrentDirectorStopped;
        return true;
    }

    private void OnCurrentDirectorPlayed(PlayableDirector director)
    {
        if (director == null) return;
        EventHandler.CallUpdateGameRunningStateEvent(GameRunningState.Pause);
    }
    private void OnCurrentDirectorStopped(PlayableDirector director)
    {
        if (director == null) return;
        EventHandler.CallUpdateGameRunningStateEvent(GameRunningState.GamePlay);
        currentDirector.gameObject.SetActive(false);
    }*/
    

    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        if (isPause) return;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }

    public void PlayFromPause()
    {
        if (currentDirector == null || !isPause) return;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        isPause = false;
    }
}
